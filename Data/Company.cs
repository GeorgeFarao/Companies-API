using System;
using System.ComponentModel.DataAnnotations;

namespace TodoApi2.Data
{
    public class Company
    {
        
        public Guid Id { get; set; }
        public string Name { get; set; }
        
    }
}