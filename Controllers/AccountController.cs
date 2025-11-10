using Microsoft.AspNetCore.Mvc;
using stockhub;
using WarehouseApp.Models;
using WarehouseApp.Services;

namespace WarehouseApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _authService.AuthenticateAsync(model.Email, model.Password);
            if (user == null)
            {
                ModelState.AddModelError("", "Неверный email или пароль");
                return View(model);
            }

            // Сохраняем пользователя в сессии
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserRole", user.Role.ToString());

            return RedirectToAction("Dashboard", "Home");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _authService.RegisterAsync(model);
            if (user == null)
            {
                ModelState.AddModelError("", "Пользователь с таким email уже существует");
                return View(model);
            }

            // Автоматический вход после регистрации
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserRole", user.Role.ToString());

            return RedirectToAction("Dashboard", "Home");
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _authService.SendPasswordResetCodeAsync(model.Email);
            if (result)
            {
                TempData["Message"] = "Код для сброса пароля отправлен на вашу почту";
                return RedirectToAction("ResetPassword", new { email = model.Email });
            }

            ModelState.AddModelError("", "Пользователь с таким email не найден");
            return View(model);
        }

        public IActionResult ResetPassword(string email)
        {
            var model = new ResetPasswordViewModel { Email = email };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var isValid = await _authService.ValidateResetCodeAsync(model.Email, model.Code);
            if (!isValid)
            {
                ModelState.AddModelError("", "Неверный код");
                return View(model);
            }

            await _authService.ResetPasswordAsync(model.Email, model.NewPassword);

            TempData["Message"] = "Пароль успешно изменен";
            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
