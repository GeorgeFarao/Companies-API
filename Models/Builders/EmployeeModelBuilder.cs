using System;
using System.Collections;
using System.Collections.Generic;
using TodoApi2.Data;
using Microsoft.AspNetCore.Mvc;
using TodoApi2.Service;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TodoApi2.Data;
using TodoApi2.ExceptionHandle;

namespace TodoApi2.Models.Builders
{
    public class EmployeeBuilder
    {
        private MyAppContext _context { get; set; }
        
        public EmployeeBuilder(MyAppContext context)
        {
            _context = context;
            
        }
        public async Task<ActionResult<IEnumerable<EmployeeModel>>> Build(IEnumerable<Employee> items, string[] fields)
        {
            var employeesModels = new List<EmployeeModel>();

            var workstations =  _context.WorkStations
                .Where(x=> items.Select(y=> y.WorkStationId).Contains(x.Id))
                .ToDictionary(x => x.Id);

            if (fields != null)
            {
                foreach (var item in items)
                {
                    WorkStation workStation = workstations[item.WorkStationId];
                    // WorkStation workStation = workstations.Where(x => x.Id.Equals(item.Id)).First();
                    
                    EmployeeModel employeeModel = new EmployeeModel();
                    if (fields.Contains(nameof(EmployeeModel.Id))) employeeModel.Id = item.Id;
                    if (fields.Contains(nameof(EmployeeModel.FirstName))) employeeModel.FirstName = item.FirstName;
                    if (fields.Contains(nameof(EmployeeModel.LastName))) employeeModel.LastName = item.LastName;
                    if (fields.Contains(nameof(EmployeeModel.DoB))) employeeModel.DoB = item.DoB;
                    if (fields.Contains(nameof(EmployeeModel.Age))) employeeModel.Age = item.Age;
                    if (fields.Contains(nameof(EmployeeModel.WorkStationId)))
                        employeeModel.WorkStationId = item.WorkStationId;
                    if (fields.Contains(nameof(EmployeeModel.WorkStationName)))
                        employeeModel.WorkStationName = workStation.Name;

                    employeesModels.Add(employeeModel);
                   
                }
            }
            else
            {
                foreach (var item in items)
                {
                    WorkStation workStation = workstations[item.WorkStationId];
                    // WorkStation workStation = workstations.Where(x => x.Id.Equals(item.Id)).First();
                    
                    EmployeeModel employeeModel = new EmployeeModel
                    {
                        Id = item.Id,
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        DoB = item.DoB,
                        Age = item.Age,
                        WorkStationId = item.WorkStationId,
                        WorkStationName = workStation.Name
                    };

                    employeesModels.Add(employeeModel);
                    
                }
            }

            return employeesModels;
        }

        public async Task<Employee>BuildData(EmployeeModelPersist employeeModel, Guid id)
        {
            Employee employee;
            if (id != default)
            {
                employee = await _context.Employees.FindAsync(id);
                if (employee == null)
                {
                    throw new MyException("Could not find employee.");
                }

                employee.FirstName = employeeModel.FirstName;
                employee.LastName = employeeModel.LastName;
                employee.DoB = employeeModel.DoB is null ? default : (DateTime) employeeModel.DoB;
                employee.Age = employeeModel.Age is null ? default : (int) employeeModel.Age;
                employee.WorkStationId = employeeModel.WorkStationId;
                employee.Id = id;

                _context.Employees.Update(employee);
                await _context.SaveChangesAsync();
            }
            else
            {
                employee = new Employee
                {
                    FirstName = employeeModel.FirstName,
                    LastName = employeeModel.LastName,
                    DoB = employeeModel.DoB is null ? default : (DateTime)employeeModel.DoB,
                    Age = employeeModel.Age is null ? default : (int)employeeModel.Age,
                    WorkStationId = employeeModel.WorkStationId,
                    Id = employeeModel.Id
            };
            }

            return employee;
        }

        public EmployeeModelPersist BuildModel(Employee employee, Guid memberId)
        {
            EmployeeModelPersist employeeModel = new EmployeeModelPersist
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                DoB = employee.DoB,
                Age = employee.Age,
                WorkStationId = employee.WorkStationId,
                
            };
            return employeeModel;
        }
    }
}
