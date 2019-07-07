using CryptoWallet.Models;
using CryptoWalletExchange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CryptoWallet.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Description page.";

            return View();
        }

        public ActionResult Rates()
        {
            ViewBag.Message = "The table below represents the rates for 1 EUR";

            ExchangeService exchangeService = new ExchangeService();
            List<CurrencyRate> rates = exchangeService.GetConversionRate(Currency.EUR, new Currency[] { Currency.USD, Currency.GBP, Currency.BTC, Currency.XRP });

            List<CurrencyRateViewModel> viewModel = rates.Select(a => new CurrencyRateViewModel
            {
                Currency = a.Currency.ToString(),
                Rate = a.Rate
            }).ToList();

            return View(viewModel);
        }
    }
}