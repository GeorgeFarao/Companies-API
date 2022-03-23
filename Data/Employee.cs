using System;
using System.ComponentModel.DataAnnotations;

namespace TodoApi2.Data
{
    public class Employee
    {
       
       public Guid Id { get; set; } 
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DoB { get; set; }
        public int Age { get; set; }
        public Guid WorkStationId { get; set; }
    }
}