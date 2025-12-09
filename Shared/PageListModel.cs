using Common.Enums;
using Common.Utilities;
using DariaCMS.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Common.Utilities.SearchUtility;

namespace SharedModels
{
    public class PageListModel
    {
        public Pageres arg { get; set; } = new();
        public List<FilterCriteria> filters { get; set; }=new();
        public string sortField { get; set; }
        public bool ascending { get; set; } = false;
    }
}
