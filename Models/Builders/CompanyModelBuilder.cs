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
    public class CompanyBuilder
    {
        private MyAppContext _context { get; set; }

        public CompanyBuilder(MyAppContext context)
        {
            _context = context;

        }
        public async Task<ActionResult<IEnumerable<CompanyModel>>> Build(IEnumerable<Company> items, string[] fields)
        {
            var companiesModels = new List<CompanyModel>();

            if (fields != null)
            {
                foreach (var item in items)
                {

                    CompanyModel companyModel = new CompanyModel();
                    if (fields.Contains(nameof(CompanyModel.Id))) companyModel.Id = item.Id;
                    if (fields.Contains(nameof(CompanyModel.Name))) companyModel.Name = item.Name;

                    companiesModels.Add(companyModel);
                }
            }
            else
            {
                foreach (var item in items)
                {

                    CompanyModel companyModel = new CompanyModel
                    {
                        Id = item.Id,
                        Name = item.Name
                    };

                    companiesModels.Add(companyModel);
                }
            }


            return companiesModels;
        }

        public async Task<Company> BuildData(CompanyModelPersist companyModel, Guid id)
        {

            Company company = null;

            if (id!=default)
            {
                company = await _context.Companies.FindAsync(id);
                if (company == null)
                {
                    throw new MyException("Could not find company.");
                }

                company.Name = companyModel.Name;
                company.Id = id;

                _context.Companies.Update(company);
                await _context.SaveChangesAsync();
            }
            else
            {
                company = new Company
                {
                    Name = companyModel.Name,
                    Id = companyModel.Id
                };
            }

            return company;
        }

        public CompanyModelPersist BuildModel(Company company)
        {
            CompanyModelPersist companyModel = new CompanyModelPersist
            {
                Id = company.Id,
                Name = company.Name,
                
            };
            return companyModel;
        }
    }
}
