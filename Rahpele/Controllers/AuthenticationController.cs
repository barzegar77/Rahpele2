using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Rahpele.Services.Interfaces;
using Rahpele.ViewModels.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Rahpele.Controllers
{
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly Rahpele.Services.Interfaces.IAuthenticationService _authenticationService;
        private readonly IUserManager _userManager;
        private readonly IPasswordHasher _passwordHasher;

        public AuthenticationController(Rahpele.Services.Interfaces.IAuthenticationService authenticationService,
            IUserManager userManager,
            IPasswordHasher passwordHasher)
        {
            _authenticationService = authenticationService;
            _userManager = userManager;
            _passwordHasher = passwordHasher;
        }




        //[HttpGet]
        //public IActionResult Login(string? returnUrl)
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        //        {
        //            return Redirect(returnUrl);
        //        }
        //        else
        //        {
        //            return RedirectToAction("Index", "Home");
        //        }
        //    }
        //    ViewBag.ReturnUrl = returnUrl;
        //    return View();
        //}



        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Login(LoginViewModel model, string? returnUrl)
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }
        //    if (!ModelState.IsValid)
        //    {
        //        string validationErrors = string.Join(',',
        //            ModelState.Values.Where(
        //                x => x.Errors.Count() > 0)
        //            .SelectMany(x => x.Errors)
        //            .Select(x => x.ErrorMessage)
        //            .ToArray()
        //            );
        //        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        //        {
        //            ViewBag.ReturnUrl = returnUrl;
        //        }
        //        TempData["error"] = validationErrors;
        //        return View(model);
        //    }
        //    var existingUser = _userManager.GetUserByPhoneNumber(model.PhoneNumber).Result;
        //    if (existingUser == null)
        //    {
        //        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        //        {
        //            ViewBag.ReturnUrl = returnUrl;
        //        }
        //        TempData["error"] = "حساب کاربری یافت نشد";
        //        return View(model);
        //    }

        //    if (existingUser.IsPhoneNumberConfirmed == false)
        //    {
        //        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        //        {
        //            ViewBag.ReturnUrl = returnUrl;
        //        }
        //        TempData["error"] = "حساب کاربری شما غیرفعال است";
        //        return View(model);
        //    }

        //    bool isPasswordCorrect = _passwordHasher.VerifyPassword(model.Password, existingUser.HashedPassword);
        //    if (!isPasswordCorrect)
        //    {
        //        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        //        {
        //            ViewBag.ReturnUrl = returnUrl;
        //        }
        //        TempData["error"] = "رمز عبور اشتباه است";
        //        return View(model);
        //    }

        //    var claims = new List<Claim>()
        //        {
        //            new Claim(ClaimTypes.Name, existingUser.PhoneNumber),
        //            new Claim(ClaimTypes.NameIdentifier, existingUser.Id.ToString()),
        //        };
        //    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        //    var principal = new ClaimsPrincipal(identity);

        //    var properties = new AuthenticationProperties
        //    {
        //        IsPersistent = true,
        //        AllowRefresh = true
        //    };
        //    HttpContext.SignInAsync(principal, properties);

        //    TempData["success"] = "شما با موفقیت وارد حساب کاربری خود شدید";
        //    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        //    {
        //        return Redirect(returnUrl);
        //    }
        //    else
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }
        //}




        [HttpPost]
        [Route("api/Authentication/Login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                string validationErrors = string.Join(',',
                    ModelState.Values.Where(
                        x => x.Errors.Count() > 0)
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage)
                .ToArray()
                    );
                return new JsonResult(new { status = false, message = validationErrors });
            }
            var existingUser = await _userManager.GetUserByPhoneNumber(model.PhoneNumber);
            if (existingUser == null)
            {
                return new JsonResult(new { status = false, message = "حساب کاربری یافت نشد" });
            }

            if (existingUser.IsPhoneNumberConfirmed == false)
            {
                return new JsonResult(new { status = false, message = "حساب کاربری شما غیرفعال است" });
            }

            bool isPasswordCorrect = _passwordHasher.VerifyPassword(model.Password, existingUser.HashedPassword);
            if (!isPasswordCorrect)
            {
                return new JsonResult(new { status = false, message = "پسوورد اشتباه است" });
            }

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("AbolfazlBarzegar"));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var tokenOptions = new JwtSecurityToken(
                issuer: "https://rahpele.com",
                claims: new List<Claim>
                {
                    new Claim(ClaimTypes.Name, existingUser.PhoneNumber.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, existingUser.Id.ToString()),
                },
                expires: DateTime.Now.AddDays(14),
                signingCredentials: signinCredentials
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return new JsonResult(new { status = true, message = "ورود با موفقیت انجام شد", token = tokenString });
        }


        //[HttpGet]
        //public IActionResult Register()
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Register(RegisterViewModel model)
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }
        //    if (!ModelState.IsValid)
        //    {
        //        string validationErrors = string.Join(',',
        //            ModelState.Values.Where(
        //                x => x.Errors.Count() > 0)
        //            .SelectMany(x => x.Errors)
        //            .Select(x => x.ErrorMessage)
        //            .ToArray()
        //            );

        //        TempData["error"] = validationErrors;
        //        return View(model);
        //    }
        //    var result = _authenticationService.RegisterAsync(model).Result;
        //    if (result.Result)
        //    {
        //        TempData["success"] = result.Message;
        //        return RedirectToAction("ConfirmationPhoneNumber", new {id = model.PhoneNumber });
        //    }
        //    TempData["error"] = result.Message;
        //    return View(model);
        //}


        //[HttpGet]
        //public IActionResult ConfirmationPhoneNumber(string? id)
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }
        //    ViewBag.PhoneNumber = id;
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ConfirmationPhoneNumber(PhoneNumberConfirmationViewModel model)
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }
        //    if (!ModelState.IsValid)
        //    {
        //        string validationErrors = string.Join(',',
        //            ModelState.Values.Where(
        //                x => x.Errors.Count() > 0)
        //            .SelectMany(x => x.Errors)
        //            .Select(x => x.ErrorMessage)
        //            .ToArray()
        //            );
        //        TempData["error"] = validationErrors;
        //        return View(model);
        //    }
        //    var result = _authenticationService.ConfrimPhoneNumber(model);

        //    if (result.Result)
        //    {
        //        Guid userId = _userManager.GetUserIdByPhoneNumber(model.PhoneNumber);

        //        var claims = new List<Claim>()
        //        {
        //            new Claim(ClaimTypes.Name, model.PhoneNumber),
        //            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        //        };
        //        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        //        var principal = new ClaimsPrincipal(identity);

        //        var properties = new AuthenticationProperties
        //        {
        //            IsPersistent = false,
        //            AllowRefresh = true
        //        };
        //        await HttpContext.SignInAsync(principal, properties);

        //        TempData["success"] = result.Message;
        //        return RedirectToAction("Index", "Home");
        //    }
        //    else
        //    {
        //        TempData["error"] = result.Message;
        //        return View(model);
        //    }
        //}

        //[HttpPost]
        //public async Task<IActionResult> ResendPhoneNumberVerificationCode(ResendPhoneNumberVerificationCodeViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        string validationErrors = string.Join(',',
        //            ModelState.Values.Where(
        //                x => x.Errors.Count() > 0)
        //            .SelectMany(x => x.Errors)
        //            .Select(x => x.ErrorMessage)
        //            .ToArray()
        //            );
        //        return new JsonResult(new { status = false, message = validationErrors });
        //    }
        //    var result = _authenticationService.ResendPhoneNumberVerificationCode(model);
        //    if (result.Result)
        //    {
        //        return new JsonResult(new { status = true, message = result.Message });
        //    }
        //    else
        //    {
        //        return new JsonResult(new { status = false, message = result.Message });
        //    }
        //}


        //[HttpGet]
        //public IActionResult ForgotPassword()
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }
        //    if (!ModelState.IsValid)
        //    {
        //        string validationErrors = string.Join(',',
        //            ModelState.Values.Where(
        //                x => x.Errors.Count() > 0)
        //            .SelectMany(x => x.Errors)
        //            .Select(x => x.ErrorMessage)
        //            .ToArray()
        //            );
        //        TempData["error"] = validationErrors;
        //        return View(model);
        //    }
        //    var result = _authenticationService.ForgotPassword(model);
        //    if (result.Result)
        //    {
        //        TempData["success"] = result.Message;
        //        return RedirectToAction("ResetPassword", new { id = model.PhoneNumber });
        //    }
        //    else
        //    {
        //        TempData["error"] = result.Message;
        //        return View(model);
        //    }
        //}

        //[HttpGet]
        //public IActionResult ResetPassword(string? id)
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }
        //    ViewBag.PhoneNumber = id;
        //    return View();
        //}


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }
        //    if (!ModelState.IsValid)
        //    {
        //        string validationErrors = string.Join(',',
        //            ModelState.Values.Where(
        //                x => x.Errors.Count() > 0)
        //            .SelectMany(x => x.Errors)
        //            .Select(x => x.ErrorMessage)
        //            .ToArray()
        //            );
        //        ViewBag.PhoneNumber = model.PhoneNumber;
        //        TempData["error"] = validationErrors;
        //        return View(model);
        //    }
        //    var result = _authenticationService.ResetPassword(model);
        //    if (result.Result)
        //    {
        //        Guid userId = _userManager.GetUserIdByPhoneNumber(model.PhoneNumber);
        //        var claims = new List<Claim>()
        //        {
        //            new Claim(ClaimTypes.Name, model.PhoneNumber),
        //            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        //        };
        //        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        //        var principal = new ClaimsPrincipal(identity);

        //        var properties = new AuthenticationProperties
        //        {
        //            IsPersistent = true,
        //            AllowRefresh = true
        //        };
        //        HttpContext.SignInAsync(principal, properties);

        //        TempData["success"] = result.Message;
        //        return RedirectToAction("Index", "Home");
        //    }
        //    else
        //    {
        //        ViewBag.PhoneNumber = model.PhoneNumber;
        //        TempData["error"] = result.Message;
        //        return View(model);
        //    }
        //}


        //[HttpPost]
        //public async Task<IActionResult> ResendForgotPasswordVerificationCode(ResendPhoneNumberVerificationCodeViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        string validationErrors = string.Join(',',
        //            ModelState.Values.Where(
        //                x => x.Errors.Count() > 0)
        //            .SelectMany(x => x.Errors)
        //            .Select(x => x.ErrorMessage)
        //            .ToArray()
        //            );
        //        return new JsonResult(new { status = false, message = validationErrors });
        //    }
        //    var result = _authenticationService.ResendForgotPasswordVerificationCode(model);
        //    if (result.Result)
        //    {
        //        return new JsonResult(new { status = true, message = result.Message });
        //    }
        //    else
        //    {
        //        return new JsonResult(new { status = false, message = result.Message });
        //    }
        //}


        //[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        //public IActionResult SignOut()
        //{
        //    HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        //    TempData["warning"] = "شما از حساب کاربری خود خارج شدید";
        //    return RedirectToAction("Index", "Home");
        //}

    }
}
