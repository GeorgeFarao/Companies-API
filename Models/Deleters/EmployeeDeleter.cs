using System;
using System.Collections;
using System.Collections.Generic;
using TodoApi2.Data;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoApi2.Models.Deleters;

namespace TodoApi2.Models.Deleters
{
    public class EmployeeDeleter
    {
        private MyAppContext _context { get; set; }
        
        public EmployeeDeleter(MyAppContext context)
        {
            _context = context;
        }

        public async Task Delete(List<Guid> employeeIds)
        {
            var employees = await _context.Employees
                .Where(x => employeeIds.Contains(x.Id)).ToListAsync();

            _context.Employees.RemoveRange(employees);
            await _context.SaveChangesAsync();
        }

    }
}
