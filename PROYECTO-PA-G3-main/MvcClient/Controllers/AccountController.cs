using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Uam.LabHelpDesk.MvcClient.Services.Auth;
using Uam.LabHelpDesk.MvcClient.Models.Auth;

namespace Uam.LabHelpDesk.MvcClient.Controllers
{
    public class AccountController(IAuthService authService, IStringLocalizer<AccountController> localizer) : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await authService.InitiateLoginAsync(model);

            if (result == null || !result.Success || result.Result == null)
            {
                ModelState.AddModelError("", result?.Message ?? localizer["ConnectionError"].Value);
                return View(model);
            }

            // Almacenar el SessionToken en la sesión del servidor
            HttpContext.Session.SetString("SessionToken", result.Result.SessionToken);

            return RedirectToAction(nameof(VerifyOtp));
        }

        [HttpGet]
        public IActionResult VerifyOtp()
        {
            var sessionToken = HttpContext.Session.GetString("SessionToken");
            if (string.IsNullOrEmpty(sessionToken))
            {
                TempData["ErrorMessage"] = localizer["SessionExpired"].Value;
                return RedirectToAction(nameof(Login));
            }

            return View(new VerifyOtpViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOtp(VerifyOtpViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var sessionToken = HttpContext.Session.GetString("SessionToken");
            if (string.IsNullOrEmpty(sessionToken))
            {
                ModelState.AddModelError("", localizer["SessionExpired"].Value);
                return View(model);
            }

            var result = await authService.VerifyOtpAsync(sessionToken, model.Code);

            if (result == null || !result.Success || result.Result == null)
            {
                ModelState.AddModelError("", result?.Message ?? localizer["ConnectionError"].Value);
                return View(model);
            }

            // Limpiar el token de sesión después de verificar exitosamente
            HttpContext.Session.Remove("SessionToken");

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await authService.LogoutAsync();
            return RedirectToAction(nameof(Login));
        }
    }
}