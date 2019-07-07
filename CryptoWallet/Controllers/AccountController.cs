using CryptoWallet.Models;
using CryptoWalletDB;
using CryptoWalletDB.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CryptoWallet.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel viewModel)
        {
           /* 
            * Echivalente cu [Required] din LoginViewModel
            * if(viewModel.Password == null)
            {
                ModelState.AddModelError("Password", "Password is required");
            }

            if (viewModel.Username == null)
            {
                ModelState.AddModelError("Username", "Username is required");
            }*/

            if (ModelState.IsValid)
            {
                //Verifica baza de date si valideaza utilizator
                using (CryptoWalletDbContext ctx = new CryptoWalletDbContext())
                {
                    User user = ctx.Users.FirstOrDefault(u => u.Email == viewModel.Username
                    && u.Password == viewModel.Password);
                    if (user != null)
                    {
                        //Daca ma loghez ma duce la bankaccounts
                        FormsAuthentication.SetAuthCookie(viewModel.Username, true);
                        return RedirectToAction("Index", "BankAccounts");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid credentials");
                        return View(viewModel);
                    }
                }
            }
            else
            {
                return View(viewModel);
            }
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                //Verifica baza de date si valideaza utilizator
                //Daca utilizatorul se afla deja in baza de date, afiseaza un mesaj
                using (CryptoWalletDbContext ctx = new CryptoWalletDbContext())
                {
                    //User pentru verificarea email-ului in baza de date, daca este gasit => deja exista acel mail in baza de date si nu se face nimic
                    User testUser = ctx.Users.FirstOrDefault(u => u.Email == viewModel.Email || u.Name == viewModel.Name);

                    if (testUser != null && viewModel.Password != null)
                    {
                        ModelState.AddModelError("", "The name or email is already being used.");
                        return View(viewModel);
                    }
                    else
                    {
                        //Adauga in baza de date
                        User userToAdd = new User
                        {
                            Email = viewModel.Email,
                            Name = viewModel.Name,
                            Password = viewModel.Password
                        };

                        ctx.Users.Add(userToAdd);
                        ctx.SaveChanges();
                        return View();
                    }
                }
            }
            else
            {
                return View(viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}