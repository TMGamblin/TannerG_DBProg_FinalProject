using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TannerG_DBProg_FinalProject.Models
{
    public class UpsertCustomerModel
    {
        public Customer Customer { get; set; }

        public List<State> States { get; set; }
    }
}