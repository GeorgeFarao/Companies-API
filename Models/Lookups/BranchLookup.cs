using System;
using System.Collections;
using System.Collections.Generic;
using TodoApi2.Data;
using System.ComponentModel.DataAnnotations;

namespace TodoApi2.Models.Lookups
{
    public class BranchLookup: BaseLookup
    {
        public List<Guid> CompanyId { get; set; }

        public string Like { get; set; }
    }
}