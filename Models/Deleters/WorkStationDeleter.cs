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
    public class WorkStationDeleter
    {
        private MyAppContext _context { get; set; }
        private EmployeeDeleter _deleter { get; set; }
        public WorkStationDeleter(MyAppContext context, EmployeeDeleter deleter)
        {
            _context = context;
            _deleter = deleter;
        }

        public async Task Delete(List<Guid> workStationIds)
        {
            var workstations = await _context.WorkStations
                .Where(x => workStationIds.Contains(x.Id)).ToListAsync();

            await this.Delete(workstations);
        }

        public async Task Delete(List<WorkStation> workStations)
        {
            var employeeIds = await _context.Employees
                .Where(x => workStations.Select(y=>y.Id).Contains(x.WorkStationId))
                .Select(x=>x.Id).ToListAsync();

            
            await _deleter.Delete(employeeIds);
            

            _context.WorkStations.RemoveRange(workStations);
            await _context.SaveChangesAsync();
        }

    }
}