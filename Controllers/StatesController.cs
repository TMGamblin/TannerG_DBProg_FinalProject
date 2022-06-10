using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TannerG_DBProg_FinalProject.Models;

namespace TannerG_DBProg_FinalProject.Controllers
{
    public class StatesController : Controller
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
            State state = context.States.Where(s => s.StateCode == id).FirstOrDefault();

            return View(state);
        }

        /// <summary>
        /// Handles sorts and searches, updates the table
        /// </summary>
        /// <param name="model">Used to update existing items in the table or create new items</param>
        /// <returns>Redirects to All when operation completes</returns>
        [HttpPost]
        public ActionResult Upsert(State newState)
        {
            BooksEntities context = new BooksEntities();

            try
            {
                if (context.States.Where(s => s.StateCode == newState.StateCode).Count() > 0)
                {
                    // state already exists
                    var stateToSave = context.States.Where(s => s.StateCode == newState.StateCode).FirstOrDefault();

                    stateToSave.StateName = newState.StateName;
                }
                else
                {
                    context.States.Add(newState);
                }
                context.SaveChanges();
            }
            catch (Exception ex) { }

            return RedirectToAction("All");
        }

        // GET: States
        /// <summary>
        /// The view to return a list of all states
        /// </summary>
        /// <param name="id">The search term</param>
        /// <param name="sortBy">Integer. 0 = StateCode. 1 = StateName.</param>
        /// <returns></returns>
        public ActionResult All(string id, int sortBy = 0)
        {
            BooksEntities context = new BooksEntities();
            List<State> states;

            switch (sortBy)
            {
                case 1:
                    {
                        states = context.States.OrderBy(s => s.StateName).ToList();
                        break;
                    }
                case 0:
                default:
                    {
                        states = context.States.OrderBy(s => s.StateCode).ToList();
                        break;
                    }
            }

            if (!string.IsNullOrWhiteSpace(id))
            {
                id = id.Trim().ToLower();

                states = states.Where(s =>
                            s.StateCode.ToLower().Contains(id) ||
                            s.StateName.ToLower().Contains(id)
                       ).ToList();
                
            }

            return View(states);
        }
    }
}