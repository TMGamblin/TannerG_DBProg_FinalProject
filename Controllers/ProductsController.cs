using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TannerG_DBProg_FinalProject.Models;

namespace TannerG_DBProg_FinalProject.Controllers
{
    public class ProductsController : Controller
    {
        /// <summary>
        /// Creates a view model for upserts
        /// </summary>
        /// <param name="id">Uses the id to get the first element in the invoice table</param>
        /// <returns>viewModel for the upsert</returns>
        [HttpGet]
        public ActionResult Upsert(string id)
        {
            BooksEntities context = new BooksEntities();
            Product product = context.Products.Where(p => p.ProductCode == id).FirstOrDefault();

            return View(product);
        }

        /// <summary>
        /// Handles sorts and searches, updates the table
        /// </summary>
        /// <param name="model">Used to update existing items in the table or create new items</param>
        /// <returns>Redirects to All when operation completes</returns>
        [HttpPost]
        public ActionResult Upsert(Product product)
        {
            Product newProduct = product;

            BooksEntities context = new BooksEntities();

            try
            {
                if (context.Products.Where(p => p.ProductCode == newProduct.ProductCode).Count() > 0)
                {
                    // product already exists
                    var productToSave = context.Products.Where(p => p.ProductCode == newProduct.ProductCode).FirstOrDefault();

                    productToSave.ProductCode = newProduct.ProductCode;
                    productToSave.Description = newProduct.Description;
                    productToSave.UnitPrice = newProduct.UnitPrice;
                    productToSave.OnHandQuantity = newProduct.OnHandQuantity;
                }
                else
                {
                    context.Products.Add(newProduct);
                }
                context.SaveChanges();
            }
            catch (Exception ex) { }

            return RedirectToAction("All");
        }

        // GET: Products
        /// <summary>
        /// The view to return a list of all products
        /// </summary>
        /// <param name="sortBy">Integer. 0 = Product Code. 1 = Description. 2 = Unit Price. 3 = OnHandQuantity</param>
        /// <returns></returns>
        public ActionResult All(string id, int sortBy = 0)
        {
            BooksEntities context = new BooksEntities();
            List<Product> products;

            switch (sortBy)
            {
                case 1:
                    {
                        products = context.Products.OrderBy(p => p.Description).ToList();
                        break;
                    }
                case 2:
                    {
                        products = context.Products.OrderBy(p => p.UnitPrice).ToList();
                        break;
                    }
                case 3:
                    {
                        products = context.Products.OrderBy(p => p.OnHandQuantity).ToList();
                        break;
                    }
                case 0:
                default:
                    {
                        products = context.Products.OrderBy(p => p.ProductCode).ToList();
                        break;
                    }
            }

            if (!string.IsNullOrWhiteSpace(id))
            {
                id = id.Trim().ToLower();
                int numberLookup = 0;

                if (int.TryParse(id, out numberLookup))
                {
                    products = products.Where(p =>
                        p.OnHandQuantity == numberLookup
                    ).ToList();
                }
                else // if id does not parse
                {
                    products = products.Where(p =>
                        p.ProductCode.ToLower().Contains(id) ||
                        p.Description.ToLower().Contains(id)
                   ).ToList();
                }
            }

            return View(products);
        }
    }
}