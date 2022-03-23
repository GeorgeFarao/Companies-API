using System;
using System.Collections;
using System.Collections.Generic;
using TodoApi2.Data;
using System.ComponentModel.DataAnnotations;

namespace TodoApi2.Models.Lookups
{
    public class CompanyLookup: BaseLookup
    {
        public List<string> Names { get; set; }
       
    }
}