using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi2.Data;
using TodoApi2.Models;
using TodoApi2.Models.Builders;
using TodoApi2.Models.Deleters;
using TodoApi2.Models.Lookups;

namespace TodoApi2.Service
{
    public class EmployeeService
    {

        private MyAppContext _context { get; set; }
        private EmployeeBuilder _builder { get; set; }
        public EmployeeDeleter _deleter { get; set; }
        public EmployeeService(MyAppContext context, EmployeeBuilder builder, EmployeeDeleter deleter)
        {
            _context = context;
            _builder = builder;
            _deleter = deleter;
        }

        public async Task<ActionResult<IEnumerable<EmployeeModel>>> GetEmployees(EmployeeLookup lookup)
        {
            if ((lookup.WorkStationId == null || lookup.WorkStationId.Count == 0) && lookup.Name== null && lookup.Ages==null)
            {
                var employees = await _context.Employees.ToListAsync();

                return await _builder.Build(employees,null);
            }
            else
            {
                var employees = _context.Employees as IQueryable<Employee>;
                if (lookup.WorkStationId != null && lookup.WorkStationId.Count > 0)
                {
                    employees = employees.Where(x => lookup.WorkStationId.Contains(x.WorkStationId));
                }

                if (lookup.Ages != null && lookup.Ages.Count > 0)
                {
                    employees = employees.Where(x => lookup.Ages.Contains(x.Age));
                }

                if (lookup.Name !=null )
                {
                    employees = employees.Where(x => (lookup.Name.Contains(x.FirstName) || lookup.Name.Contains(x.LastName)));
                }

                if (lookup.PagingInfo != null)
                {
                    if (lookup.PagingInfo.Offset != 0 || lookup.PagingInfo.Size != 0)
                    {
                        employees = employees.Skip(lookup.PagingInfo.Offset)
                            .Take(lookup.PagingInfo.Size);
                    }
                }

                if (lookup.Fields != null)  
                {
                    var parameter = Expression.Parameter(typeof(Employee), "e");
                    var bindings = lookup.Fields.Select(x =>
                    {
                        var internalProperty = typeof(Employee).GetProperty(x);
                        var expression = Expression.Property(parameter, internalProperty);
                        return Expression.Bind(internalProperty, expression);
                    });
                    var body = Expression.MemberInit(Expression.New(typeof(Employee)), bindings);
                    var selector = Expression.Lambda<Func<Employee, Employee>>(body, parameter); 
                    employees = employees.Select(selector);
                    //var bindings = lookup.Fields
                    //    .Select(name => Expression.PropertyOrField(parameter, name))
                    //    .Select(member => Expression.Bind(member.Member, member));
                    //var body = Expression.MemberInit(Expression.New(typeof(Employee)), bindings);
                    //var selector = Expression.Lambda<Func<Employee, Employee>>(body, parameter);
                    //employees = employees.Select(selector);

                }
                return await _builder.Build( await employees.ToListAsync(),lookup.Fields);

            }
            
        }

        public async Task<ActionResult<EmployeeModel>> GetEmployee(Guid id)
        {
            //return await _context.Employees.FindAsync(id);
            var employee = await _context.Employees.FindAsync(id);
   
            var employeeList = new List<Employee>
            {
                employee
            };

            var employeesModels = await _builder.Build(employeeList, null);

            return employeesModels.Value.First();

        }

        public async Task<int> PutEmployee(Guid id, EmployeeModelPersist employeeModel)
        {
            _context.Entry(_builder.BuildData(employeeModel, id)).State = EntityState.Modified;

            return await _context.SaveChangesAsync();

        }

        public async Task<int> AddSave(EmployeeModelPersist employeeModel)
        {
            _context.Employees.Add( await _builder.BuildData(employeeModel, default));
            return await _context.SaveChangesAsync();
        }



        public async Task<ActionResult<EmployeeModelPersist>> RemoveEmployee(EmployeeModelPersist employeeModel)
        {
            var employee = await _context.Employees.FindAsync(employeeModel.Id);

            //_context.Employees.Remove(employee);


            await _deleter.Delete(new List<Guid>
            {
                employee.Id
            });
            //await _context.SaveChangesAsync();
            return employeeModel;
        }

        public async ValueTask<EmployeeModelPersist> FindEmployee(Guid id)
        {
            var employee = await _context.Employees.FindAsync(id);
            var workStation = await _context.WorkStations.FindAsync(employee.WorkStationId);

            return _builder.BuildModel(employee,workStation.Id);
        }

        public bool EmployeeExists(Guid id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }

    }
}