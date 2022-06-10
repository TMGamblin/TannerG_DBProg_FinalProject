using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TannerG_DBProg_FinalProject.Models;

namespace TannerG_DBProg_FinalProject.Controllers
{
    public class OrderOptionsController : Controller
    {
        /// <summary>
        /// Creates a view model for upserts
        /// </summary>
        /// <param name="id">Uses the id to get the first element in the invoice table</param>
        /// <returns>viewModel for the upsert</returns>
        [HttpGet]
        public ActionResult Upsert(int id)
        {
            BooksEntities context = new BooksEntities();
            OrderOption orderOption = context.OrderOptions.Where(o => o.SalesTaxRate == id).FirstOrDefault();

            return View(orderOption);
        }

        /// <summary>
        /// Handles sorts and searches, updates the table
        /// </summary>
        /// <param name="model">Used to update existing items in the table or create new items</param>
        /// <returns>Redirects to All when operation completes</returns>
        [HttpPost]
        public ActionResult Upsert(OrderOption orderOption)
        {
            OrderOption newOrderOption = orderOption;

            BooksEntities context = new BooksEntities();

            try
            {
                context.OrderOptions.Add(newOrderOption);
                
                context.SaveChanges();
            }
            catch (Exception ex) { }

            return RedirectToAction("All");
        }

        // GET: OrderOptions
        /// <summary>
        /// The view to return a list of all order options
        /// </summary>
        /// <param name="sortBy">Integer. 0 = Sales Tax Rate. 1 = FirstBookShipCharge. 2 = AdditionalBookShipCharge.</param>
        /// <returns></returns>
        public ActionResult All(string id, int sortBy = 0)
        {
            BooksEntities context = new BooksEntities();
            List<OrderOption> orderOptions;

            switch (sortBy)
            {
                case 1:
                    {
                        orderOptions = context.OrderOptions.OrderBy(o => o.FirstBookShipCharge).ToList();
                        break;
                    }
                case 2:
                    {
                        orderOptions = context.OrderOptions.OrderBy(o => o.AdditionalBookShipCharge).ToList();
                        break;
                    }
                case 0:
                default:
                    {
                        orderOptions = context.OrderOptions.OrderBy(o => o.SalesTaxRate).ToList();
                        break;
                    }
            }

            if (!string.IsNullOrWhiteSpace(id))
            {
                id = id.Trim().ToLower();
                decimal numberLookup = 0.0m;

                if (decimal.TryParse(id, out numberLookup))
                {
                    orderOptions = orderOptions.Where(o =>
                        o.SalesTaxRate == numberLookup ||
                        o.FirstBookShipCharge == numberLookup ||
                        o.AdditionalBookShipCharge == numberLookup
                    ).ToList();
                }
            }

            return View(orderOptions);
        }
    }
}