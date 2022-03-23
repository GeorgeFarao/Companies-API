using System;
using System.Collections;
using System.Collections.Generic;
using TodoApi2.Data;
using System.ComponentModel.DataAnnotations;

namespace TodoApi2.Models
{
    public class CompanyModel
    {
        
        public Guid Id { get; set; }
        public string Name { get; set; }
              
    }

    public class CompanyModelPersist
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}