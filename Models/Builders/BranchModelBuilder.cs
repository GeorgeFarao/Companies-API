using System;
using System.Collections;
using System.Collections.Generic;
using TodoApi2.Data;
using Microsoft.AspNetCore.Mvc;
using TodoApi2.Service;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TodoApi2.ExceptionHandle;

namespace TodoApi2.Models.Builders
{
    public class BranchBuilder
    {
        private MyAppContext _context { get; set; }

        public BranchBuilder(MyAppContext context)
        {
            _context = context;

        }
        public async Task<ActionResult<IEnumerable<BranchModel>>> Build(IEnumerable<Branch> items, string[] fields)
        {
            var BranchesModels = new List<BranchModel>();

            var companies = _context.Companies
                .Where(x => items.Select(y => y.CompanyId).Contains(x.Id))
                .ToDictionary(x => x.Id);

            if (fields!=null)
            {
                foreach (var item in items)
                {
                    Company company = companies[item.CompanyId];

                    BranchModel branchModel = new BranchModel();
                    if (fields.Contains(nameof(BranchModel.Id))) branchModel.Id = item.Id;
                    if (fields.Contains(nameof(BranchModel.Name))) branchModel.Name = item.Name;
                    if (fields.Contains(nameof(BranchModel.CompanyId))) branchModel.CompanyId = item.CompanyId;
                    if (fields.Contains(nameof(BranchModel.CompanyName))) branchModel.CompanyName = company.Name;

                    BranchesModels.Add(branchModel);
                }
            }
            else
            {
                foreach (var item in items)
                {
                    Company company = companies[item.CompanyId];

                    BranchModel branchModel = new BranchModel
                    {
                        Id = item.Id,
                        Name = item.Name,
                        CompanyId = item.CompanyId,
                        CompanyName = company.Name
                    };

                    BranchesModels.Add(branchModel);
                }
            }


            return BranchesModels;
        }

        public async Task<Branch> BuildData(BranchModelPersist branchModel, Guid id)
        {
            Branch branch;

            if (id != default)
            {
                branch = await _context.Branches.FindAsync(id);
                if (branch==null)
                {
                    throw new MyException("Could not find branch.");
                }

                branch.Name = branchModel.Name;
                branch.CompanyId = branchModel.CompanyId;
                branch.Id = id;

                _context.Branches.Update(branch);
                await _context.SaveChangesAsync();
            }
            else
            {
                branch = new Branch
                {
                    Name = branchModel.Name,
                    CompanyId = branchModel.CompanyId,
                    Id = branchModel.Id
                };
            }


            return branch;
        }

        public BranchModelPersist BuildModel(Branch branch, Guid memberId)
        {
            BranchModelPersist branchModel = new BranchModelPersist
            {
                Id = branch.Id,
                Name = branch.Name,
                CompanyId = memberId
            };
            return branchModel;
        }
    }
}