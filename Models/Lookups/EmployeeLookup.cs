using System;
using System.Collections;
using System.Collections.Generic;
using TodoApi2.Data;
using System.ComponentModel.DataAnnotations;

namespace TodoApi2.Models.Lookups
{
    public class EmployeeLookup : BaseLookup
    {
        public List<Guid> WorkStationId { get; set; }
        public string Name { get; set; }
        public List<int> Ages { get; set; }
    }
}