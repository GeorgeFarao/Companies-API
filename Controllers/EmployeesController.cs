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
    public class EmployeesController : ControllerBase
    {
        private readonly EmployeeService _employeeService;
        private readonly IStringLocalizer<EmployeesController> _localizer;
        public EmployeesController(EmployeeService employeeService, IStringLocalizer<EmployeesController> localizer)
        {
            _employeeService = employeeService;
            _localizer = localizer;
        }

        // GET: api/Employees
        [HttpPost("lookup")]
        public async Task<ActionResult<IEnumerable<EmployeeModel>>> GetEmployees([FromBody] EmployeeLookup lookup)
        {
            //return await _context.Employees.ToListAsync();
            return  await _employeeService.GetEmployees(lookup);
        }

        // GET: api/Employees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeModel>> GetEmployee(Guid id)
        {
            var employee = await _employeeService.GetEmployee(id);

            if (employee == null)
            {
                return NotFound();
            }

            return employee;
        }

        // PUT: api/Employees/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(Guid id, EmployeeModelPersist employee)
        {
            if (id != employee.Id)
            {
                return BadRequest();
            }

            //_context.Entry(employee).State = EntityState.Modified;

            try
            {
                //await _context.SaveChangesAsync();
                await _employeeService.PutEmployee(id, employee);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
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

        // POST: api/Employees
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<EmployeeModel>> PostEmployee(EmployeeModelPersist employee)
        {
            //_context.Employees.Add(employee);
            //await _context.SaveChangesAsync();
            var validationResult = new EmployeeValidator().Validate(employee);
            if (validationResult.IsValid == false)
            {
                var returnString = _localizer["Invalid Employee"];
                throw new MyException(returnString);
            }
            await _employeeService.AddSave(employee);

            return CreatedAtAction("GetEmployee", new { id = employee.Id }, employee);
        }

        // DELETE: api/Employees/5
        [HttpDelete("{id}")] 
        public async Task<ActionResult<EmployeeModelPersist>> DeleteEmployee(Guid id)
        {
            var employee = await _employeeService.FindEmployee(id);
            if (employee == null)
            {
                return NotFound();
            }

            //_context.Employees.Remove(employee);
            //await _context.SaveChangesAsync();
            return await _employeeService.RemoveEmployee(employee);

            //return employee;
        }

        private bool EmployeeExists(Guid id)
        {
            //return _context.Employees.Any(e => e.Id == id);
            return _employeeService.EmployeeExists(id);
        }
    }
}
