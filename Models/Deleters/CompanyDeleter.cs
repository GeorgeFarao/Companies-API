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
    public class CompanyDeleter
    {
        private MyAppContext _context { get; set; }
        private BranchDeleter _deleter { get; set; }
        public CompanyDeleter(MyAppContext context, BranchDeleter deleter)
        {
            _context = context;
            _deleter = deleter;
        }

        public async Task Delete(List<Guid> companyIds)
        {
            var companies = await _context.Companies
                .Where(x => companyIds.Contains(x.Id)).ToListAsync();

           await this.Delete(companies);
        }

        public async Task Delete(List<Company> companies)
        {
            var branchIds = await _context.Branches
                .Where(x => companies.Select(y => y.Id).Contains(x.CompanyId))
                .Select(x => x.Id).ToListAsync();

            await _deleter.Delete(branchIds);
           
            _context.Companies.RemoveRange(companies);
            await _context.SaveChangesAsync();
        }

    }
}