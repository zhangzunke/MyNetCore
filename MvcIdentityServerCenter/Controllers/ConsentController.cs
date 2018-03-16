using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MvcIdentityServerCenter.Models;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityServer4.Services;
using MvcIdentityServerCenter.Services;

namespace MvcIdentityServerCenter.Controllers
{
    public class ConsentController : Controller
    {
        private readonly ConsentService _consentService;

        public ConsentController(ConsentService consentService)
        {
            _consentService = consentService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string returnUrl)
        {
            var model = await _consentService.BuildConsentViewModel(returnUrl);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(InputConsentViewModel model)
        {
            var result = await _consentService.ProcessConsent(model);
            if (result.IsRedirct)
            {
                return Redirect(result.RedirctUrl);
            }
            if (!string.IsNullOrEmpty(result.ValidationError))
            {
                ModelState.AddModelError("", result.ValidationError);
            }
            return View(model);
        }
    }
}