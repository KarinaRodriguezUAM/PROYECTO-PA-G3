using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Uam.LabHelpDesk.MvcClient.Models.Auth;
using Uam.LabHelpDesk.MvcClient.Services.Auth;

namespace Uam.LabHelpDesk.MvcClient.Controllers
{
    public class AccountController(
        IAuthService authService,
        IStringLocalizer<AccountController> localizer) : Controller
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
                ModelState.AddModelError(
                    "",
                    result?.Message ?? localizer["ConnectionError"].Value);

                return View(model);
            }

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
                ModelState.AddModelError(
                    "",
                    result?.Message ?? localizer["ConnectionError"].Value);

                return View(model);
            }

            HttpContext.Session.Remove("SessionToken");

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await authService.LogoutAsync();
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await authService.ForgotPasswordAsync(model);

            if (result == null || !result.Success)
            {
                ModelState.AddModelError("", result?.Message ?? "Error inesperado");
                return View(model);
            }

            HttpContext.Session.SetString("ResetSessionToken", result.Result ?? "");

            return RedirectToAction(nameof(ResetPassword));
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            var token = HttpContext.Session.GetString("ResetSessionToken");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction(nameof(ForgotPassword));

            return View(new ResetPasswordViewModel
            {
                SessionToken = token
            });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await authService.ResetPasswordAsync(model);

            if (result == null || !result.Success)
            {
                ModelState.AddModelError(
                    "",
                    result?.Message ?? "Código inválido o expirado"
                );

                return View(model);
            }

            TempData["Message"] = "Contraseña restablecida correctamente";

            return RedirectToAction(nameof(Login));
        }

        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.RefreshToken = Request.Cookies["RefreshToken"] ?? "";

            var result = await authService.ChangePasswordAsync(model);

            if (!result)
            {
                ModelState.AddModelError("", localizer["PasswordChangeError"].Value);
                return View(model);
            }

            TempData["Message"] = localizer["PasswordChangedSuccess"].Value;

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MySessions()
        {
            var sessions = await authService.GetMySessionsAsync();
            return View(sessions);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RevokeSession(int id)
        {
            var isCurrentSession = await authService.RevokeSessionAsync(id);

            if (isCurrentSession)
            {
                await authService.LogoutAsync();
                return RedirectToAction(nameof(Login));
            }

            return RedirectToAction(nameof(MySessions));
        }
        

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RevokeAllSessions()
        {
            await authService.RevokeAllSessionsAsync();
            return RedirectToAction(nameof(MySessions));
        }
    }
}