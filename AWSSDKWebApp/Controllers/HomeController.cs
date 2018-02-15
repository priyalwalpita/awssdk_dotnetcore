using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AWSSDKWebApp.Models;
using AWSSDKWebApp.Util;

namespace AWSSDKWebApp.Controllers
{
    public class HomeController : Controller
    {
        private ISecureEnclave _secureEnclave;

        public HomeController( ISecureEnclave secureEnclave)
        {
            _secureEnclave = secureEnclave;
        }
        
        public IActionResult Index()
        {
            _secureEnclave.WriteValue("MyKey123", "This is the data to secure");
            return View();
        }

        public async Task<IActionResult> About()
        {
            string securedData = await _secureEnclave.ReadValue("MyKey123");
            ViewData["Message"] = securedData;

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}