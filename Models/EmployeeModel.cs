using System;
using System.Collections;
using System.Collections.Generic;
using TodoApi2.Models;
using System.ComponentModel.DataAnnotations;

namespace TodoApi2.Models
{
    public class EmployeeModel
    {
       
       public Guid Id { get; set; } 
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DoB { get; set; }
        public int? Age { get; set; }
        public Guid? WorkStationId { get; set; }

        public string WorkStationName { get; set; }
    }

    public class EmployeeModelPersist
    {

        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DoB { get; set; }
        public int? Age { get; set; }
        //public Guid WorkStationId { get; set; }
        public Guid WorkStationId { get; set; }

    }
}