using Goliath.Attributes;
using Goliath.Enums;
using Goliath.Helper;
using Goliath.Models.GoliathAes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goliath.Controllers
{
    /// <summary>
    /// The controller for managing AES actions. Currently used to decrypt user data.
    /// </summary>
    [Route("aes")]
    public class GoliathAesController : Controller
    {
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("decrypt")]
        public IActionResult DecryptUserData()
        {
            return View();
        }

        [Route("decrypt")]
        [HttpGet]
        public IActionResult DecryptUserData(string m)
        {
            if (string.IsNullOrWhiteSpace(m))
            {
                return View();
            }
            DecryptionResultModel model;
            try
            {
                model = JsonConvert.DeserializeObject<DecryptionResultModel>(Encoding.UTF8.GetString(Convert.FromBase64String(m)));
            }
            catch (Exception)
            {
                // Serialized Model passed is invalid.
                model = new();
            }

            TempData[TempDataKeys.Model] = model;
            return View();
        }

        private const string uploadFormat = ".txt";

        [Route("decrypt")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [PreventDuplicateRequest]
        public async Task<IActionResult> DecryptUserData(FileDecryptionModel model)
        {
            DecryptionResultModel resultModel = new();
            // make sure file is .txt file.
            string fileExtension = Path.GetExtension(model.DataFile?.FileName);
            if (!string.IsNullOrWhiteSpace(fileExtension) && !fileExtension.Equals(uploadFormat))
            {
                ModelState.AddModelError(string.Empty, $"Invalid format. File must be a {uploadFormat} file.");
            }
            // Check if model state is good.
            if (!ModelState.IsValid)
            {
                // Get model errors.
                List<string> allErrors = new();
                try
                {
                    allErrors = ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage).ToList();
                }
                catch (Exception)
                {
                    allErrors.Add("Internal error processing request. Try again.");
                }

                resultModel.IsSuccess = false;
                resultModel.ModelErrors = allErrors;
                // Encode and Serialize
                return RedirectToAction(nameof(DecryptUserData), new { m = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(resultModel))) });
            }

            string fileData = await ReadAsStringAsync(model.DataFile);
            string decryptedData = AesHelper.DecryptText(fileData, model.SecretKey, model.SaltValue);
            // Check if encryption failed.
            if (decryptedData.Equals("Error"))
            {
                ModelState.AddModelError(string.Empty, "Invalid Key or Salt.");
                List<string> allErrors = new();
                try
                {
                    allErrors = ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage).ToList();
                }
                catch (Exception)
                {
                    allErrors.Add("Internal error processing request. Try again.");
                }
                resultModel.IsSuccess = false;
                resultModel.ModelErrors = allErrors;
                return RedirectToAction(nameof(DecryptUserData), new { m = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(resultModel))) });
            }
            // Save the decrypted data in TempData
            TempData[TempDataKeys.HtmlMessage] = decryptedData;
            resultModel.IsSuccess = true;
            resultModel.FileBytes = $"{Encoding.UTF8.GetByteCount(fileData)}";
            resultModel.FileName = model.DataFile.FileName;
            return RedirectToAction(nameof(DecryptUserData), new { m = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(resultModel))) });
        }

        /// <summary>
        /// Reads a IFormFile and prints it to a string.
        /// </summary>
        /// <param name="file"> </param>
        /// <returns> </returns>
        private static async Task<string> ReadAsStringAsync(IFormFile file)
        {
            StringBuilder result = new StringBuilder();
            using (StreamReader reader = new StreamReader(file.OpenReadStream()))
            {
                while (reader.Peek() >= 0)
                    result.AppendLine(await reader.ReadLineAsync());
            }
            return result.ToString();
        }
    }
}

/*
 *
 *
resultModel.IsSuccess = true;
                ModelState.Clear();
                return RedirectToAction(nameof(DecryptUserData), new { m = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(resultModel))) });
 *
 */