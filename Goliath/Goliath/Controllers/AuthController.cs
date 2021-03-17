using Goliath.Models;
using Goliath.Repository;
using Goliath.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Goliath.Controllers
{
    /// <summary>
    /// Manages the Views for Authentication.
    /// <br />
    /// ButtonID indicates the radio button to be checked.
    ///
    /// </summary>

    public sealed class AuthController : Controller
    {
        /// <summary>
        /// For interfacing with the application user.
        /// </summary>
        private readonly IAccountRepository _accountRepository;
        /// <summary>
        /// Manage if the user is signed in as well as authentication.
        /// </summary>
        private readonly SignInManager<ApplicationUser> _signInManager;
        /// <summary>
        /// Send emails.
        /// </summary>
        private readonly IEmailService _emailService;

        public AuthController(IAccountRepository accountRepository, SignInManager<ApplicationUser> signInManager, IEmailService emailService)
        {
            _accountRepository = accountRepository;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        // Refereed to as "Login" as well.
        public IActionResult Index()
        {
            // If the user is signed in redirect them to the user panel.
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "UserPanel");
            }
            ViewData["ButtonID"] = "login";
            return View("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Index(SignInModel signInModel)
        {

            ViewData["ButtonID"] = "login";
            // If the user has signed in with valid data.
            if (ModelState.IsValid)
            {
                var result = await _accountRepository.PasswordSignInAsync(signInModel);
                // If the user name and password match.
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "UserPanel");
                    // Else we send the specified errors to the user.
                }
                else if (result.IsLockedOut)
                {
                    ModelState.AddModelError("", "Please try again later.");
                }
                else if (result.IsNotAllowed)
                {
                    ModelState.AddModelError("", "You must verify your email!");
                }
                else if (result.RequiresTwoFactor)
                {
                    ModelState.AddModelError("", "You must login through 2FA.");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid Credentials.");
                }
            }
            return View("Login", signInModel);
        }

        [Route("register/goliath")]
        public IActionResult Register()
        {
            ViewData["ButtonID"] = "register";
            return View();
        }

        [Route("register/goliath")]
        [HttpPost]
        public async Task<IActionResult> Register(SignUpUserModel model)
        {
            ViewData["ButtonID"] = "register";
            if (ModelState.IsValid)
            {
                // Pass in the information for the confirmation email.
                var result = await _accountRepository.CreateUserAsync(model, new string[] { Request.Headers["User-Agent"].ToString(), HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString() });
                if (!result.Succeeded)
                {
                    // For every error that is created during registration we add that to the eventual bootstrap modal.
                    foreach (var errorMessage in result.Errors)
                    {
                        ModelState.AddModelError("", errorMessage.Description);
                    }

                    return View(model);
                }

                // Registration is Valid.
                ModelState.Clear();
                // Send the user to the index with register tempdata.
                TempData["Redirect"] = "register";
                return RedirectToAction("Index");
            }
            return View();
        }

        [Route("register/method")]
        public IActionResult RegisterMethod()
        {
            ViewData["ButtonID"] = "register";
            return View();
        }

        [Route("forgotpassword")]
        public IActionResult ForgotPassword()
        {
            ViewData["ButtonID"] = "forgot-password";
            return View();
        }

        [Route("forgotusername")]
        public IActionResult ForgotUsername()
        {
            ViewData["ButtonID"] = "forgot-username";
            return View();
        }

        [Route("verify")]
        public IActionResult VerifyEmail()
        {
            ViewData["ButtonID"] = "verify-email";
            return View();
        }

        [Route("verify")]
        [HttpPost]
        public async Task<IActionResult> VerifyEmail(EmailConfirmModel model)
        {
            ViewData["ButtonID"] = "verify-email";
            // Verify that the user exists with the specified email.
            var user = await _accountRepository.FindByEmailAsync(model.Email);
            if (user != null)
            {
                // If the email is already confirmed.
                if (user.EmailConfirmed)
                {
                    model.IsConfirmed = true;
                    ModelState.AddModelError("", "Account already verified.");
                    return View(model);
                }
                // Generate a token as well as a user agent.
                await _accountRepository.GenerateEmailConfirmationToken(user, new string[] { Request.Headers["User-Agent"].ToString(), HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString() });
                // Indicate to the View that the email was sent.
                model.IsEmailSent = true;
                // Clear all fields.
                ModelState.Clear();

                return View(model);
            }
            else
            {
                ModelState.AddModelError("", "We could not find user: " + model.Email);
            }

            return View();
        }

        /// <summary>
        /// Verifies an email given a UID and TOKEN.<br />
        /// Does not contain a view and redirects to index view.<br />
        /// The result of the method call is returned by the Bootstrap modal.
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail(string uid, string token)
        {
            // If the unique ID as well as the user exist.
            if (!string.IsNullOrWhiteSpace(uid) && !string.IsNullOrWhiteSpace(token))
            {
                token = token.Replace(" ", "+");
                // Check to make sure the token has not expired or is invalid.
                var result = await _accountRepository.ConfirmEmailAsync(uid, token);
                if (result.Succeeded)
                {
                    // If the email confirmation is a success then we can pass that info into the view.
                    TempData["Redirect"] = "verified";
                    return RedirectToAction("Index");
                }
            }
            // Alert to the view that the verification failed.
            TempData["Redirect"] = "verified-failure";
            return RedirectToAction("Index");
        }
    }
}