using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml.Linq;
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
    public class CompanyService
    {

        private  MyAppContext _context { get; set; }
        private CompanyBuilder _builder { get; set; }
        private CompanyDeleter _deleter { get; set; }
        public CompanyService(MyAppContext context, CompanyBuilder builder, CompanyDeleter deleter)
        {
            _context = context;
            _builder = builder;
            _deleter = deleter;
        }

        public async Task<ActionResult<IEnumerable<CompanyModel>>> GetCompanies(CompanyLookup lookup)
        {
            if (lookup.Names == null)
            {
                var companies = await _context.Companies.ToListAsync();

                return await _builder.Build(companies, null);
            }
            else
            {
                var companies = _context.Companies as IQueryable<Company>;
                if (lookup.Names.Count > 0)
                {
                   companies = companies.Where(x => lookup.Names.Contains(x.Name)); // change maybe
                   //companies = companies.Where(x => x.Name.Contains());
                }

                if (lookup.PagingInfo != null)
                {
                    if (lookup.PagingInfo.Offset != 0 || lookup.PagingInfo.Size != 0)
                    {
                        companies = companies.Skip(lookup.PagingInfo.Offset)
                            .Take(lookup.PagingInfo.Size);
                    }
                }


                if (lookup.Fields != null)
                {
                    var parameter = Expression.Parameter(typeof(Company), "e");
                    var bindings = lookup.Fields.Select(x =>
                    {
                        var internalProperty = typeof(Company).GetProperty(x);
                        var expression = Expression.Property(parameter, internalProperty);
                        return Expression.Bind(internalProperty, expression);
                    });
                    var body = Expression.MemberInit(Expression.New(typeof(Company)), bindings);
                    var selector = Expression.Lambda<Func<Company, Company>>(body, parameter);
                    companies = companies.Select(selector);
                    
                }

                return await _builder.Build(await companies.ToListAsync(), lookup.Fields);
            }
        }

        public async Task<ActionResult<CompanyModel>> GetCompany(Guid id)
        {
            //return await _context.Companies.FindAsync(id);
            var company = await _context.Companies.FindAsync(id);

            var companyList = new List<Company>
            {
                company
            };
            var companiesModels = await _builder.Build(companyList, null);

            return companiesModels.Value.First();
        }

        public async Task<int> PutCompany(Guid id, CompanyModelPersist companyModel)
        {
            _context.Entry(_builder.BuildData(companyModel,id)).State = EntityState.Modified;

            return await _context.SaveChangesAsync();
        }

        public async Task<int> AddSave(CompanyModelPersist companyModel)
        {
            _context.Companies.Add(await _builder.BuildData(companyModel, default));
            return await _context.SaveChangesAsync();
        }

       

        public async Task<ActionResult<CompanyModelPersist>> RemoveCompany(CompanyModelPersist companyModel)
        {
            var company = await _context.Companies.FindAsync(companyModel.Id);
            //_context.Companies.Remove(company);
            await _deleter.Delete(new List<Guid>
            {
                company.Id
            });
            //await _context.SaveChangesAsync();      
            return companyModel;
        }

        public async ValueTask<CompanyModelPersist> FindCompany(Guid id)
        {
            var company =  await _context.Companies.FindAsync(id);
  
            return _builder.BuildModel(company);
        }

        public bool CompanyExists(Guid id)
        {
            return _context.Companies.Any(e => e.Id == id);
        }


    }
}