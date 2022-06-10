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
    public class InvoicesController : Controller
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
            Invoice invoice = context.Invoices.Where(i => i.InvoiceID == id).FirstOrDefault();
            List<Customer> customers = context.Customers.ToList();

            UpsertInvoiceModel viewModel = new UpsertInvoiceModel()
            {
                Invoice = invoice,
                Customers = customers
            };

            return View(viewModel);
        }

        /// <summary>
        /// Handles sorts and searches, updates the table
        /// </summary>
        /// <param name="model">Used to update existing items in the table or create new items</param>
        /// <returns>Redirects to All when operation completes</returns>
        [HttpPost]
        public ActionResult Upsert(UpsertInvoiceModel model)
        {
            Invoice newInvoice = model.Invoice;

            BooksEntities context = new BooksEntities();

            try
            {
                if (context.Invoices.Where(i => i.InvoiceID == newInvoice.InvoiceID).Count() > 0)
                { 
                    // invoice already exists
                    var invoiceToSave = context.Invoices.Where(i => i.InvoiceID == newInvoice.InvoiceID).FirstOrDefault();

                    invoiceToSave.CustomerID = newInvoice.CustomerID;
                    invoiceToSave.InvoiceDate = newInvoice.InvoiceDate;
                    invoiceToSave.ProductTotal = newInvoice.ProductTotal;
                    invoiceToSave.SalesTax = newInvoice.SalesTax;
                    invoiceToSave.Shipping = newInvoice.Shipping;
                    invoiceToSave.InvoiceTotal = newInvoice.InvoiceTotal;
                }
                else
                {
                    context.Invoices.Add(newInvoice);
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
        /// <param name="sortBy">Integer. 0 = ID. 1 = CustomerID. 2 = Invoice Date. 3 = Invoice Total</param>
        /// <returns></returns>
        public ActionResult All(string id, int sortBy = 0)
        {
            BooksEntities context = new BooksEntities();
            List<Invoice> invoices;

            switch (sortBy)
            {
                case 1:
                    {
                        invoices = context.Invoices.OrderBy(i => i.CustomerID).ToList();
                        break;
                    }
                case 2:
                    {
                        invoices = context.Invoices.OrderBy(i => i.InvoiceDate).ToList();
                        break;
                    }
                case 3:
                    {
                        invoices = context.Invoices.OrderBy(i => i.InvoiceTotal).ToList();
                        break;
                    }
                case 0:
                default:
                    {
                        invoices = context.Invoices.OrderBy(i => i.InvoiceID).ToList();
                        break;
                    }
            }

            if (!string.IsNullOrWhiteSpace(id))
            {
                id = id.Trim().ToLower();
                int numberLookup = 0;

                if (int.TryParse(id, out numberLookup))
                {
                    invoices = invoices.Where(i =>
                        i.InvoiceID == numberLookup ||
                        i.CustomerID == numberLookup
                    ).ToList();
                }
                else // if id does not parse
                {
                    invoices = invoices.Where(i =>
                        i.InvoiceDate.ToLongDateString().ToLower().Contains(id) ||
                        i.InvoiceTotal.ToString().Contains(id)
                   ).ToList();
                }
            }

            return View(invoices);
        }
    }
}
