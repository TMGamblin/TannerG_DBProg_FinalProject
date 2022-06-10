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
    public class CustomersController : Controller
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
            Customer customer = context.Customers.Where(c => c.CustomerID == id).FirstOrDefault();
            List<State> states = context.States.ToList();

            UpsertCustomerModel viewModel = new UpsertCustomerModel()
            {
                Customer = customer,
                States = states
            };

            return View(viewModel);
        }

        /// <summary>
        /// Handles sorts and searches, updates the table
        /// </summary>
        /// <param name="model">Used to update existing items in the table or create new items</param>
        /// <returns>Redirects to All when operation completes</returns>
        [HttpPost]
        public ActionResult Upsert(UpsertCustomerModel model)
        {
            Customer newCustomer = model.Customer;

            BooksEntities context = new BooksEntities();

            bool isNewCustomer = false;

            try
            {
                if (context.Customers.Where(c => c.CustomerID == newCustomer.CustomerID).Count() > 0)
                {
                    // customer already exists
                    var customerToSave = context.Customers.Where(c => c.CustomerID == newCustomer.CustomerID).FirstOrDefault();

                    customerToSave.Name = newCustomer.Name;
                    customerToSave.Address = newCustomer.Address;
                    customerToSave.City = newCustomer.City;
                    customerToSave.State = newCustomer.State;
                    customerToSave.ZipCode = newCustomer.ZipCode;
                }
                else
                {
                    context.Customers.Add(newCustomer);
                    isNewCustomer = true;
                }
                context.SaveChanges();

                if (isNewCustomer)
                {
                    // START - fields that are not "hard coded"
                    // Web.config OR from a Database
                    string fromEmail = ConfigurationManager.AppSettings.Get("fromEmail");
                    string toEmail = ConfigurationManager.AppSettings.Get("toEmail");
                    string fromPassword = ConfigurationManager.AppSettings.Get("fromPassword");
                    string fromName = ConfigurationManager.AppSettings.Get("fromName");
                    int fromPort = Convert.ToInt32(ConfigurationManager.AppSettings.Get("fromPort"));
                    string smtpHost = ConfigurationManager.AppSettings.Get("smtpHost");
                    // END - fields that are not "hard coded"

                    // send a welcome email
                    var fromAddress = new MailAddress(fromEmail, fromName);
                    var toAddress = new MailAddress(toEmail, newCustomer.Name);

                    string subject = "Welcome to our Tech Support Portal";
                    string body = "It's great to have you joining us!";

                    var smtp = new SmtpClient
                    {
                        Host = smtpHost,
                        Port = fromPort,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                    };

                    var message = new MailMessage(fromAddress.Address, toAddress.Address, subject, body);
                    smtp.Send(message);
                }
            }
            catch (Exception ex) { }

            return RedirectToAction("All");
        }

        // GET: Customers
        /// <summary>
        /// The view to return a list of all customers
        /// </summary>
        /// <param name="sortBy">Integer. 0 = ID. 1 = Name. 2 = City. 3 = State</param>
        /// <returns></returns>
        public ActionResult All(string id, int sortBy = 0)
        {
            BooksEntities context = new BooksEntities();
            List<Customer> customers;

            switch (sortBy)
            {
                case 1:
                    {
                        customers = context.Customers.OrderBy(c => c.Name).ToList();
                        break;
                    }
                case 2:
                    {
                        customers = context.Customers.OrderBy(c => c.City).ToList();
                        break;
                    }
                case 3:
                    {
                        customers = context.Customers.OrderBy(c => c.State).ToList();
                        break;
                    }
                case 0:
                default:
                    {
                        customers = context.Customers.OrderBy(c => c.CustomerID).ToList();
                        break;
                    }
            }

            if (!string.IsNullOrWhiteSpace(id))
            {
                id = id.Trim().ToLower();
                int customerIdLookup = 0;

                if (int.TryParse(id, out customerIdLookup))
                {
                    customers = customers.Where(c =>
                        c.CustomerID == customerIdLookup
                    ).ToList();
                }
                else // if id does not parse
                {
                    customers = customers.Where(c =>
                        c.Name.ToLower().Contains(id) ||
                        c.City.ToLower().Contains(id) ||
                        c.State.ToLower().Contains(id)
                   ).ToList();
                }
            }

            return View(customers);
        }
    }
}