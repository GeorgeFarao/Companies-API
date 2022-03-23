using System;
using System.Collections;
using System.Collections.Generic;
using TodoApi2.Data;
using System.ComponentModel.DataAnnotations;

namespace TodoApi2.Models
{
    public class BranchModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? CompanyId { get; set; }

        public string CompanyName { get; set; }
    }

    public class BranchModelPersist
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid CompanyId { get; set; }
    }
}