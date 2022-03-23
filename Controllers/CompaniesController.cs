using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using TodoApi2.Data;
using TodoApi2.ExceptionHandle;
using TodoApi2.Models;
using TodoApi2.Models.Lookups;
using TodoApi2.Service;
using TodoApi2.Validator;

namespace TodoApi2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly CompanyService _companyService;
        private readonly IStringLocalizer<CompaniesController> _localizer;
        public CompaniesController(CompanyService companyService, IStringLocalizer<CompaniesController> localizer)
        {
            _companyService = companyService;
            _localizer = localizer;
        }

        // GET: api/Companies
        [HttpPost("lookup")]
        public async Task<ActionResult<IEnumerable<CompanyModel>>> GetCompanies([FromBody] CompanyLookup lookup)
        {
            //return await _context.Companies.ToListAsync();
            return await _companyService.GetCompanies(lookup);
        }

        // GET: api/Companies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CompanyModel>> GetCompany(Guid id)
        {
            var company = await _companyService.GetCompany(id);

            if (company == null)
            {
                return NotFound();
            }

            return company;
        }

        // PUT: api/Companies/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompany(Guid id, CompanyModelPersist company)
        {
            if (id != company.Id)
            {
                return BadRequest();
            }

            //_context.Entry(company).State = EntityState.Modified;

            try
            {   
                //await _context.SaveChangesAsync();
                await _companyService.PutCompany(id,company);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Companies
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<CompanyModel>> PostCompany(CompanyModelPersist company)
        {
            //_context.Companies.Add(company);
            //await _context.SaveChangesAsync();
            var validationResult = new CompanyValidator().Validate(company);
            if (validationResult.IsValid == false)
            {
                var returnString = _localizer["Invalid Company"];
                throw new MyException(returnString);
            }
            await _companyService.AddSave(company);
            return CreatedAtAction("GetCompany", new { id = company.Id }, company);
        }

        // DELETE: api/Companies/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<CompanyModelPersist>> DeleteCompany(Guid id)
        {
            var company = await _companyService.FindCompany(id);
            if (company == null)
            {
                return NotFound();
            }

            //_context.Companies.Remove(company);
            //await _context.SaveChangesAsync();
            return await _companyService.RemoveCompany(company);

            //return company;
        }

        private bool CompanyExists(Guid id)
        {
            //return _context.Companies.Any(e => e.Id == id);
            return _companyService.CompanyExists(id);
        }
    }
}
