using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TheAchEcom.Models;
using Repository.DomainModels;
using Repository.BusinessModels.ShopList;
using Repository.BusinessModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using SqlClientExtention.Extentions;

namespace TheAchEcom.Controllers
{
    public class ApplicationController : Controller
    {
        protected const string _cartCookieName = "_cartCookieName";

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
