using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Lib.Net.ViewModels;
using FirebaseAdmin.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Lib.Net.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVeiwModel loginVm)
        {
            if (ModelState.IsValid)
            {
                UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(loginVm.Email);
                
                

                if (userRecord != null)
                {
                    var auth = "FxZ2TEdYgbloyOzVGCnG2Amu1sUyuBQsDkbu98fm";
                    var firebaseClient = new FirebaseClient("https://kotlinlibrary-5e1b7.firebaseio.com/",
                                                               new FirebaseOptions
                                                               {
                                                                   AuthTokenAsyncFactory = () => Task.FromResult(auth)
                                                               });
                    
                    var user = await firebaseClient
                        .Child($"user/{userRecord.Uid}")
                        .OnceSingleAsync<UserViewModel>();

                    await Authenticate(user);
                    return RedirectToAction("Index", "Home");

                }
            }

            return View(loginVm);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel register)
        {
            if (ModelState.IsValid)
            {
                bool error = false;
                try
                {
                    UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(register.Email);
                }
                catch
                {
                    error = true;
                };

                if (error)
                {
                    UserRecordArgs args = new UserRecordArgs()
                    {
                        Email = register.Email,
                        EmailVerified = true,
                        Password = register.Password
                    };

                    UserRecord newUser = await FirebaseAuth.DefaultInstance.CreateUserAsync(args);

                    var auth = "FxZ2TEdYgbloyOzVGCnG2Amu1sUyuBQsDkbu98fm";
                    var firebaseClient = new FirebaseClient("https://kotlinlibrary-5e1b7.firebaseio.com/",
                                                               new FirebaseOptions
                                                               {
                                                                   AuthTokenAsyncFactory = () => Task.FromResult(auth)
                                                               });

                    UserViewModel newUserVm = new UserViewModel
                    {
                        Email = register.Email,
                        Group = register.Group,
                        Name = register.Name,
                        Role = "User",
                        Status = "Подтвержден"
                    };

                    var user = await firebaseClient
                        .Child("user")
                        .PostAsync(newUserVm);

                    await Authenticate(newUserVm);

                    return RedirectToAction("Index", "Home");


                }
                else
                    ModelState.AddModelError("", "Error");
            }

            return View(register);
        }



        private async Task Authenticate(UserViewModel user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Name),
                new Claim("Email", user.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role)
            };

            ClaimsIdentity identity = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        
    }
}