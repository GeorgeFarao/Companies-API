using System;
using System.Collections.Generic;
using TodoApi2.Data;
using System.ComponentModel.DataAnnotations;

namespace TodoApi2.Models
{
    public class WorkStationModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
       // public Guid BranchId { get; set; }

        public Guid? BranchId { get; set; }
        public string BranchName { get; set; }
       // public ICollection<Employee> Employees { get; set; }
    }

    public class WorkStationModelPersist
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        // public Guid BranchId { get; set; }

        public Guid BranchId { get; set; }
        
        // public ICollection<Employee> Employees { get; set; }
    }
}