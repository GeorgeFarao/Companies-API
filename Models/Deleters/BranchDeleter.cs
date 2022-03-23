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
    public class BranchDeleter
    {
        private MyAppContext _context { get; set; }
        private WorkStationDeleter _deleter { get; set; }
        public BranchDeleter(MyAppContext context, WorkStationDeleter deleter)
        {
            _context = context;
            _deleter = deleter;
        }

        public async Task Delete(List<Guid> branchIds)
        {
            var branches = await _context.Branches
                .Where(x => branchIds.Contains(x.Id)).ToListAsync();

            await this.Delete(branches);
        }

        public async Task Delete(List<Branch> branches)
        {
            var workstationIds = await _context.WorkStations
                .Where(x => branches.Select(y=>y.Id).Contains(x.BranchId))
                .Select(x=>x.Id).ToListAsync();

            
            await _deleter.Delete(workstationIds);
            

            _context.Branches.RemoveRange(branches);
            await _context.SaveChangesAsync();
        }

    }
}
