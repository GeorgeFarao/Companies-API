using System;
using System.Collections;
using System.Collections.Generic;
using TodoApi2.Data;
using System.ComponentModel.DataAnnotations;

namespace TodoApi2.Models.Lookups
{
    public class WorkStationLookup: BaseLookup
    {
        public List<Guid> BranchId { get; set; }

        public string Like { get; set; }
    }
}