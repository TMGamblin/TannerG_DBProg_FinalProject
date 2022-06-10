using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using TannerG_DBProg_FinalProject.Models;

namespace TannerG_DBProg_FinalProject.Controllers
{
    public class InvoiceLineItemsController : Controller
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
            InvoiceLineItem invoiceLineItem = context.InvoiceLineItems.Where(i => i.InvoiceID == id).FirstOrDefault();
            List<Invoice> invoices = context.Invoices.ToList();
            List<Product> products = context.Products.ToList();

            UpsertInvoiceLineItemModel viewModel = new UpsertInvoiceLineItemModel()
            {
                InvoiceLineItem = invoiceLineItem,
                Invoices = invoices,
                Products = products
            };

            return View(viewModel);
        }

        /// <summary>
        /// Handles sorts and searches, updates the table
        /// </summary>
        /// <param name="model">Used to update existing items in the table or create new items</param>
        /// <returns>Redirects to All when operation completes</returns>
        [HttpPost]
        public ActionResult Upsert(UpsertInvoiceLineItemModel model)
        {
            InvoiceLineItem newInvoiceLineItem = model.InvoiceLineItem;

            BooksEntities context = new BooksEntities();

            try
            {
                if (context.InvoiceLineItems.Where(i => i.InvoiceID == newInvoiceLineItem.InvoiceID).Count() > 0)
                {
                    // invoice already exists
                    var invoiceToSave = context.InvoiceLineItems.Where(i => i.InvoiceID == newInvoiceLineItem.InvoiceID).FirstOrDefault();

                    invoiceToSave.InvoiceID = newInvoiceLineItem.InvoiceID;
                    invoiceToSave.ProductCode = newInvoiceLineItem.ProductCode;
                    invoiceToSave.UnitPrice = newInvoiceLineItem.UnitPrice;
                    invoiceToSave.Quantity = newInvoiceLineItem.Quantity;
                    invoiceToSave.ItemTotal = newInvoiceLineItem.ItemTotal;
                }
                else
                {
                    context.InvoiceLineItems.Add(newInvoiceLineItem);
                }
                context.SaveChanges();
            }
            catch (Exception ex) { }

            return RedirectToAction("All");
        }

        // GET: Invoices
        /// <summary>
        /// The view to return a list of all invoices
        /// </summary>
        /// <param name="sortBy">Integer. 0 = ID. 1 = Product Code. 2 = Unit Price. 3 = Quantity. 4 = Item Total</param>
        /// <returns></returns>
        public ActionResult All(string id, int sortBy = 0)
        {
            BooksEntities context = new BooksEntities();
            List<InvoiceLineItem> invoiceLineItems;

            switch (sortBy)
            {
                case 1:
                    {
                        invoiceLineItems = context.InvoiceLineItems.OrderBy(i => i.ProductCode).ToList();
                        break;
                    }
                case 2:
                    {
                        invoiceLineItems = context.InvoiceLineItems.OrderBy(i => i.UnitPrice).ToList();
                        break;
                    }
                case 3:
                    {
                        invoiceLineItems = context.InvoiceLineItems.OrderBy(i => i.Quantity).ToList();
                        break;
                    }
                case 4:
                    {
                        invoiceLineItems = context.InvoiceLineItems.OrderBy(i => i.ItemTotal).ToList();
                        break;
                    }
                case 0:
                default:
                    {
                        invoiceLineItems = context.InvoiceLineItems.OrderBy(i => i.InvoiceID).ToList();
                        break;
                    }
            }

            if (!string.IsNullOrWhiteSpace(id))
            {
                id = id.Trim().ToLower();
                int numberLookup = 0;

                if (int.TryParse(id, out numberLookup))
                {
                    invoiceLineItems = invoiceLineItems.Where(i =>
                        i.InvoiceID == numberLookup ||
                        i.UnitPrice == numberLookup ||
                        i.Quantity == numberLookup ||
                        i.ItemTotal == numberLookup
                    ).ToList();
                }
                else // if id does not parse
                {
                    invoiceLineItems = invoiceLineItems.Where(i =>
                        i.ProductCode.ToLower().Contains(id)
                   ).ToList();
                }
            }

            return View(invoiceLineItems);
        }
    }
}