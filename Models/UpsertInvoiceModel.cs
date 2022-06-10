using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TannerG_DBProg_FinalProject.Models
{
    public class UpsertInvoiceModel
    {
        public Invoice Invoice { get; set; }

        public List<Customer> Customers { get; set; }
    }
}