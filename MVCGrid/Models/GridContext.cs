﻿using MVCGrid.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MVCGrid.Models
{
    public class GridContext
    {
        internal IMVCGridDefinition GridDefinition { get; set; }
        public HttpContext CurrentHttpContext { get; set; }
        public QueryOptions QueryOptions { get; set; }
        public System.Web.Mvc.UrlHelper UrlHelper { get; set; }
        public string GridName { get; set; }
    }
}