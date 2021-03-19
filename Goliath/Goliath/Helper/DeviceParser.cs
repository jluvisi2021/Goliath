﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Goliath.Helper
{
    /// <summary>
    /// Returns ordered data depending on the clients browser information that is available.
    /// </summary>
    public class DeviceParser
    {
        private string _userAgent { get; set; }
        private string _ipAddress { get; set; }

        public string OSName { get; private set; } = "Unknown";
        public string BrowserName { get; private set; } = "Unknown";
        public string DeviceType { get; private set; } = "Unknown";
        public string IPv4 { get; private set; } = "Unknown";

        public DeviceParser(string userAgent, string ipAddress)
        {
            _userAgent = userAgent;
            _ipAddress = ipAddress;
            // Parse all information from the given strings.
            OSName = ParseOSName();
            BrowserName = ParseBrowserName();
            DeviceType = ParseDeviceType();
            IPv4 = _ipAddress;
        }

        private string ParseOSName()
        {
            var OSNames = new Dictionary<string, string>
            {
                { "Windows NT 5.0", "Windows 2000" },
                { "Windows NT 5.01", "Windows 2000 (SP1)" },
                { "Windows 2000", "Windows 2000" },
                { "Windows NT 5.1", "Windows XP" },
                { "Windows XP", "Windows XP" },
                { "Windows NT 5.2", "Windows Server 2003" },
                { "Windows NT 6.0", "Windows Vista" },
                { "Windows NT 6.1", "Windows 7" },
                { "Windows NT 6.2", "Windows 8" },
                { "Windows NT 10.0", "Windows 10" },
                { "Windows NT 4.0", "Windows NT 4.0" },
                { "Windows ME", "Windows ME Edition" },
                { "Macintosh", "Mac OS" },
                { "OpenBSD", "Open BSD" },
                { "SunOS", "Sun OS" },
                { "CrOS", "Chrome OS" },
                { "Ubuntu", "Ubuntu Linux" },
                { "iPhone", "iOS" },
                { "iPod", "iOS (iPod)" },
                { "iPad", "iPad OS" },
                { "Android", "Android" },
                { "Linux", "Generic Linux" },
                { "X11", "Generic Linux" },
                { "QNX", "QNX" },
                { "BeOS", "BeOS" },
                { "OS/2", "OS/2" },
            };
            foreach(string key in OSNames.Keys)
            {
                if(_userAgent.Contains(key))
                {
                    return OSNames[key];
                }
            }
            return "Unknown";
        }

        private string ParseBrowserName()
        {
            var browserNames = new Dictionary<string, string>
            {
                {"Chrome", "Chrome/Chromium"},
                {"Firefox", "Firefox/Tor" },
                {"Safari", "Safari" },
                {"Edge", "Microsoft Edge (Non-Chromium)" },
                {"Edg", "Microsoft Edge" },
                {"MSIE", "Internet Explorer"},
                {"IEMobile", "Internet Explorer"},
                {"OPR", "Opera"},
                {"SamsungBrowser", "Samsung Browser"},
                {"webOS", "Generic Mobile"},
                {"WebOS", "Generic Mobile"}
            };
            foreach (string key in browserNames.Keys)
            {
                if (_userAgent.Contains(key))
                {
                    return browserNames[key];
                }
            }
            return "Unknown";
        }

        private string ParseDeviceType()
        {
            if(_userAgent.Contains("Mobile"))
            {
                return "Mobile";
            }
            return "Desktop";

        }



        public string ToSimpleString()
        {
            StringBuilder str = new();
            str.AppendLine("<br />");
            str.Append($" - Operating System: {OSName}");
            str.AppendLine("<br />");
            str.Append($" - Browser Name: {BrowserName}");
            str.AppendLine("<br />");
            str.Append($" - Device Type: {DeviceType}");
            return str.ToString();
        }

        public string ToComplexString()
        {
            return _userAgent;
        }
        

        

    }
}
