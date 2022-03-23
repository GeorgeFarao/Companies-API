using System;
using System.ComponentModel.DataAnnotations;

namespace TodoApi2.Data
{
    public class Branch
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid CompanyId { get; set; }
    }
}