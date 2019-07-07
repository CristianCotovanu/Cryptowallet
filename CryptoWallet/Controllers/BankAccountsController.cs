using CryptoWallet.Models;
using CryptoWalletDB;
using CryptoWalletDB.Domain;
using CryptoWalletExchange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CryptoWallet.Controllers
{
    [Authorize]
    public class BankAccountsController : Controller
    {
        private List<CurrencyRate> GetRates(string getRateFor)
        {
            /*
             * Currency enum Legend
            0 - EUR
            1 - USD
            2 - GBP 
            3 - BTC
            4 - XRP
            */
            Currency Curr = (Currency)Enum.Parse(typeof(Currency), getRateFor, true);
            Currency[] allCurrencies = { 0, (Currency)1, (Currency)2, (Currency)3, (Currency)4 };

            int toDelete = 0;

            for (int i = 0; i < allCurrencies.Count(); i++)
            {
                if (allCurrencies[i] == Curr)
                {
                    toDelete = i;
                }
            }

            allCurrencies = allCurrencies.Where((source, index) => index != toDelete).ToArray();

            ExchangeService exchangeService = new ExchangeService();
            List<CurrencyRate> rates = exchangeService.GetConversionRate(Curr, allCurrencies);

            return rates;
        }
        private CurrencyRate GetRates(string fromCurrency, string toCurrency)
        {
            Currency from = (Currency)Enum.Parse(typeof(Currency), fromCurrency, true);
            Currency[] to = { (Currency)Enum.Parse(typeof(Currency), toCurrency, true) };

            ExchangeService exchangeService = new ExchangeService();
            List<CurrencyRate> rates = exchangeService.GetConversionRate(from, to);

            CurrencyRate rate = rates[0];

            return rate;
        }

        [HttpGet]
        public ActionResult Index()
        {
            using (CryptoWalletDbContext ctx = new CryptoWalletDbContext())
            {
                User currentUser = ctx.Users.AsNoTracking().FirstOrDefault(u => u.Email == User.Identity.Name);

                List<UserBankAccount> currentUserAccounts = ctx.UsersBankAccounts.Where(b => b.UserID == currentUser.UserID).ToList();

                List<CurrencyRate> eurRates = GetRates("EUR");

                decimal totalEur = 0;

                for (int j = 0; j < currentUserAccounts.Count(); j++)
                {
                    for (int i = 0; i < eurRates.Count(); i++)
                    {
                        if (currentUserAccounts[j].Currency == "EUR")
                        {
                            totalEur += currentUserAccounts[j].Amount;
                            break;
                        }

                        if (eurRates[i].Currency.ToString() == currentUserAccounts[j].Currency)
                        {
                            totalEur += (currentUserAccounts[j].Amount / eurRates[i].Rate);
                            break;
                        }
                    }
                }

                List<BankAccountViewModel> accountsViewModel = currentUserAccounts.Select(a => new BankAccountViewModel
                {
                    AccountID = a.AccountID,
                    Amount = a.Amount,
                    Currency = a.Currency
                }).ToList();

                for (int i = 0; i < accountsViewModel.Count(); i++)
                {
                    for (int j = 0; j < eurRates.Count(); j++)
                    {
                        if (accountsViewModel[i].Currency == "EUR")
                        {
                            accountsViewModel[i].TotalAmount = totalEur;
                        }
                        if (eurRates[j].Currency.ToString() == accountsViewModel[i].Currency)
                        {
                            accountsViewModel[i].TotalAmount = totalEur * eurRates[j].Rate;
                        }
                    }
                }

                return View(accountsViewModel);
            }
        }

        [HttpGet]
        public ActionResult Deposit()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Deposit(DepositViewModel depositModel)
        {
            if (ModelState.IsValid)
            {
                using (CryptoWalletDbContext ctx = new CryptoWalletDbContext())
                {
                    //noTracking = read-only mode
                    User currentUser = ctx.Users.AsNoTracking().FirstOrDefault(u => u.Email == User.Identity.Name);

                    UserBankAccount eurAccount = ctx.UsersBankAccounts.FirstOrDefault(a => a.Currency == "EUR" && a.UserID == currentUser.UserID);

                    if (eurAccount == null)
                    {
                        eurAccount = new UserBankAccount
                        {
                            Currency = "EUR",
                            UserID = currentUser.UserID,
                            Amount = 0
                        };
                        ctx.UsersBankAccounts.Add(eurAccount);
                    }

                    ctx.SaveChanges();

                    eurAccount.Amount += depositModel.Amount;

                    UserTransaction eurTransaction = new UserTransaction
                    {
                        Amount = depositModel.Amount,
                        FromAccountID = eurAccount.UserID,
                        ToAccountID = eurAccount.UserID,
                        TransactionDate = DateTime.Now,
                        CurrencyRate = 1
                    };

                    ctx.UsersTransactions.Add(eurTransaction);

                    ctx.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            return View(depositModel);
        }

        [HttpPost]
        public ActionResult SendTo(SendViewModel viewModel)
        {
            using (CryptoWalletDbContext ctx = new CryptoWalletDbContext())
            {
                User currentUser = ctx.Users.AsNoTracking().FirstOrDefault(u => u.Email == User.Identity.Name);
                UserBankAccount currentBankAccount = ctx.UsersBankAccounts.FirstOrDefault(u => u.AccountID == viewModel.SenderAccountId);

                if (currentBankAccount.Amount >= viewModel.Amount)
                {
                    User receiverUser = ctx.Users.AsNoTracking().FirstOrDefault(u => u.Email == viewModel.ReceiverName);

                    if (receiverUser != null)
                    {
                        UserBankAccount receiverBankAccount = ctx.UsersBankAccounts.FirstOrDefault(u => u.UserID == receiverUser.UserID && u.Currency == currentBankAccount.Currency);

                        if (receiverBankAccount != null)
                        {
                            currentBankAccount.Amount -= viewModel.Amount;
                            receiverBankAccount.Amount += viewModel.Amount;

                            UserTransaction newTransaction = new UserTransaction
                            {
                                Amount = viewModel.Amount,
                                FromAccountID = viewModel.SenderAccountId,
                                ToAccountID = receiverBankAccount.AccountID,
                                TransactionDate = DateTime.Now,
                                CurrencyRate = 1

                            };

                            ctx.UsersTransactions.Add(newTransaction);

                            ctx.SaveChanges();

                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ModelState.AddModelError("SenderAccountId", "The receiver does not have an active account.");
                            SetupSendViewModel(viewModel);
                            return View(viewModel);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("ReceiverName", "The receiver e-mail does not exist.");
                        SetupSendViewModel(viewModel);
                        return View(viewModel);
                    }
                }
                else
                {
                    ModelState.AddModelError("Amount", "Not enough money to send.");
                    SetupSendViewModel(viewModel);
                    return View(viewModel);
                }
            }
        }

        [HttpGet]
        public ActionResult SendTo()
        {
            SendViewModel viewModel = new SendViewModel();
            SetupSendViewModel(viewModel);
            return View(viewModel);
        }

        private void SetupSendViewModel(SendViewModel viewModel)
        {
            using (CryptoWalletDbContext ctx = new CryptoWalletDbContext())
            {
                User currentUser = ctx.Users.AsNoTracking().FirstOrDefault(u => u.Email == User.Identity.Name);

                List<BankAccountViewModel> accounts = ctx.UsersBankAccounts.Where(a => a.UserID == currentUser.UserID)
                    .Select(u => new BankAccountViewModel
                    {
                        AccountID = u.AccountID,
                        Amount = u.Amount,
                        Currency = u.Currency
                    }).ToList();

                viewModel.SenderAccounts.AddRange(accounts.Select(a => new SelectListItem
                {
                    Value = a.AccountID.ToString(),
                    Text = a.Currency + " (" + a.AccountID.ToString() + ')'
                }));
            }
        }

        [HttpGet]
        public ActionResult OpenAccount()
        {
            OpenAccountViewModel viewModel = new OpenAccountViewModel();

            using (CryptoWalletDbContext ctx = new CryptoWalletDbContext())
            {
                User currentUser = ctx.Users.AsNoTracking().FirstOrDefault(u => u.Email == User.Identity.Name);

                List<BankAccountViewModel> accounts = ctx.UsersBankAccounts.Where(a => a.UserID == currentUser.UserID)
                    .Select(u => new BankAccountViewModel
                    {
                        AccountID = u.AccountID,
                        Amount = u.Amount,
                        Currency = u.Currency
                    }).ToList();
            }
            //sterge din drop down list monedele unde am deja un cont
            /*viewModel.CurrencyList.AddRange(accounts.Select(a => new SelectListItem
            {
                Value = a.AccountID.ToString(),
                Text = a.Currency + " (" + a.AccountID.ToString() + ')'
            }));
            return View(viewModel);*/
            return null;
        }

        [HttpPost]
        public ActionResult OpenAccount(OpenAccountViewModel viewModel)
        {
            using (CryptoWalletDbContext ctx = new CryptoWalletDbContext())
            {
                User currentUser = ctx.Users.AsNoTracking().FirstOrDefault(u => u.Email == User.Identity.Name);

                UserBankAccount account = ctx.UsersBankAccounts.FirstOrDefault(a => a.Currency == viewModel.NewCurrency && a.UserID == currentUser.UserID);

                if (account == null)
                {
                    account = new UserBankAccount
                    {
                        Currency = viewModel.NewCurrency,
                        UserID = currentUser.UserID,
                        Amount = 0
                    };
                    ctx.UsersBankAccounts.Add(account);
                }
                else
                {
                    return View(viewModel);
                }

                ctx.SaveChanges();
            }
            return View();
        }

        [HttpGet]
        public ActionResult Transactions()
        {
            using (CryptoWalletDbContext ctx = new CryptoWalletDbContext())
            {
                User currentUser = ctx.Users.AsNoTracking().FirstOrDefault(u => u.Email == User.Identity.Name);

                //creez lista din baza de date cu 
                List<UserBankAccount> currentUserBankAccounts = ctx.UsersBankAccounts.Where(u => u.UserID == currentUser.UserID).ToList();

                //creez lista cu id-urile conturilor userului
                List<int> currentUserAccountsIds = currentUserBankAccounts.Select(b => b.AccountID).ToList();

                List<UserTransaction> currentUserTransactions = ctx.UsersTransactions.Where(t => currentUserAccountsIds.Contains(t.ToAccountID)
                                                                                    || currentUserAccountsIds.Contains(t.FromAccountID)).ToList();

                List<TransactionViewModel> viewModel = currentUserTransactions.Select(a => new TransactionViewModel
                {
                    Amount = a.Amount,
                    DateTime = a.TransactionDate,
                    Rate = a.CurrencyRate,

                    FromUser = a.FromAccount.User.Email,
                    ToUser = a.ToAccount.User.Email,

                    FromCurrency = a.FromAccount.Currency,
                    ToCurrency = a.ToAccount.Currency,
                }).ToList();

                return View(viewModel);
            }
        }

        private void SetupExchangeViewModel(ExchangeViewModel viewModel)
        {
            using (CryptoWalletDbContext ctx = new CryptoWalletDbContext())
            {
                User currentUser = ctx.Users.AsNoTracking().FirstOrDefault(u => u.Email == User.Identity.Name);

                string[] currenciesList = { "EUR", "USD", "GBP", "BTC", "XRP" };

                List<BankAccountViewModel> accounts = ctx.UsersBankAccounts.Where(a => a.UserID == currentUser.UserID)
                    .Select(u => new BankAccountViewModel
                    {
                        AccountID = u.AccountID,
                        Amount = u.Amount,
                        Currency = u.Currency
                    }).ToList();

                viewModel.FromCurrencyList.AddRange(accounts.Select(a => new SelectListItem
                {
                    Value = a.Currency,
                    Text = a.Currency + " (" + a.AccountID.ToString() + ')'
                }));


                foreach (string u in currenciesList)
                {
                    viewModel.ToCurrencyList.Add(new SelectListItem
                    {
                        Value = u,
                        Text = u
                    });
                }
            }
        }

        [HttpGet]
        public ActionResult Exchange()
        {
            ExchangeViewModel viewModel = new ExchangeViewModel();
            SetupExchangeViewModel(viewModel);
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Exchange(ExchangeViewModel viewModel)
        {
            using (CryptoWalletDbContext ctx = new CryptoWalletDbContext())
            {
                User currentUser = ctx.Users.AsNoTracking().FirstOrDefault(u => u.Email == User.Identity.Name);

                UserBankAccount fromAccount = ctx.UsersBankAccounts.FirstOrDefault(u => u.Currency == viewModel.FromCurrency.ToString()
                                                                                                                            && u.UserID == currentUser.UserID);

                UserBankAccount toAccount = ctx.UsersBankAccounts.FirstOrDefault(u => u.Currency == viewModel.ToCurrency
                                                                                                                            && u.UserID == currentUser.UserID);

                CurrencyRate rate = GetRates(viewModel.FromCurrency, viewModel.ToCurrency);

                if (fromAccount.Amount >= viewModel.Amount)
                {
                    if (toAccount == null)
                    {
                        UserBankAccount newAccount = new UserBankAccount
                        {
                            UserID = currentUser.UserID,
                            Currency = viewModel.ToCurrency,
                            Amount = 0

                        };

                        fromAccount.Amount -= viewModel.Amount;
                        newAccount.Amount += viewModel.Amount * rate.Rate;

                        ctx.UsersBankAccounts.Add(newAccount);

                        UserTransaction newTransaction = new UserTransaction
                        {
                            FromAccountID = fromAccount.AccountID,
                            ToAccountID = newAccount.AccountID,
                            Amount = viewModel.Amount,
                            CurrencyRate = rate.Rate,
                            TransactionDate = DateTime.Now
                        };

                        ctx.UsersTransactions.Add(newTransaction);
                    }
                    else
                    {
                        fromAccount.Amount -= viewModel.Amount;
                        toAccount.Amount += viewModel.Amount * rate.Rate;

                        UserTransaction newTransaction = new UserTransaction
                        {
                            FromAccountID = fromAccount.AccountID,
                            ToAccountID = toAccount.AccountID,
                            Amount = viewModel.Amount,
                            CurrencyRate = rate.Rate,
                            TransactionDate = DateTime.Now
                        };

                        ctx.UsersTransactions.Add(newTransaction);
                    }

                    ctx.SaveChanges();

                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("Amount", "Not enough money to exchange.");
                    SetupExchangeViewModel(viewModel);
                    return View(viewModel);
                }
            }
        }
    }
}