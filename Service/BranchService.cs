using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi2.Data;
using TodoApi2.Models;
using TodoApi2.Models.Builders;
using TodoApi2.Models.Lookups;
using TodoApi2.Models.Deleters;

namespace TodoApi2.Service
{
    public class BranchService
    {

        private MyAppContext _context { get; set; }
        private BranchBuilder _builder { get; set; }
        private BranchDeleter _deleter { get; set; }
        public BranchService(MyAppContext context, BranchBuilder builder, BranchDeleter deleter)
        {
            _context = context;
            _builder = builder;
            _deleter = deleter;
        }

        public async Task<ActionResult<IEnumerable<BranchModel>>> GetBranches(BranchLookup lookup)
        {
            if (lookup.CompanyId == default && lookup.Like == default)
            {
                var Branches = await _context.Branches.ToListAsync();

                return await _builder.Build(Branches, null);
            }
            else
            {
                var branches = _context.Branches as IQueryable<Branch>;
                if (lookup.CompanyId != null && lookup.CompanyId.Count > 0)
                {
                    branches = branches.Where(x => lookup.CompanyId.Contains(x.CompanyId));
                }

                if (lookup.Like != null)
                {
                    branches = branches.Where(x => x.Name.Contains(lookup.Like));
                }

                if (lookup.PagingInfo!=null)
                {
                    if (lookup.PagingInfo.Offset !=0 || lookup.PagingInfo.Size !=0)
                    {
                        branches = branches.Skip(lookup.PagingInfo.Offset)
                            .Take(lookup.PagingInfo.Size);
                    }
                }


                if (lookup.Fields != null)
                {
                    var parameter = Expression.Parameter(typeof(Branch), "e");
                    var bindings = lookup.Fields.Select(x =>
                    {
                        var internalProperty = typeof(Branch).GetProperty(x);
                        var expression = Expression.Property(parameter, internalProperty);
                        return Expression.Bind(internalProperty, expression);
                    });
                    var body = Expression.MemberInit(Expression.New(typeof(Branch)), bindings);
                    var selector = Expression.Lambda<Func<Branch, Branch>>(body, parameter);
                    branches = branches.Select(selector);
                
                }

                return await _builder.Build(await branches.ToListAsync(), lookup.Fields);
            }

        }

        public async Task<ActionResult<BranchModel>> GetBranch(Guid id)
        {
            //return await _context.Branches.FindAsync(id);
            var branch = await _context.Branches.FindAsync(id);

            var branchList = new List<Branch>
            {
                branch
            };
            var branchesModels = await _builder.Build(branchList, null);

            return branchesModels.Value.First();

        }

        public async Task<int> PutBranch(Guid id, BranchModelPersist branchModel)
        {
            _context.Entry(_builder.BuildData(branchModel,id)).State = EntityState.Modified;


            return await _context.SaveChangesAsync();
        }

        public async Task<int> AddSave(BranchModelPersist branchModel)
        {
            _context.Branches.Add(await _builder.BuildData(branchModel, default));
            return await _context.SaveChangesAsync();
        }



        public async Task<ActionResult<BranchModelPersist>> RemoveBranch(BranchModelPersist branchModel)
        {
            var branch = await _context.Branches.FindAsync(branchModel.Id);
            //_context.Branches.Remove(branch);
            await _deleter.Delete(new List<Guid>
            {
                branch.Id
            });
           // await _context.SaveChangesAsync();
            return branchModel;
        }

        public async ValueTask<BranchModelPersist> FindBranch(Guid id)
        {
            var branch = await _context.Branches.FindAsync(id);
            var company = await _context.Companies.FindAsync(branch.CompanyId);

            return _builder.BuildModel(branch,company.Id);

        }

        public bool BranchExists(Guid id)
        {
            return _context.Branches.Any(e => e.Id == id);
        }

    }
}