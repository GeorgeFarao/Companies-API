using System;
using System.ComponentModel.DataAnnotations;
using FluentValidation;
using TodoApi2.Data;
using TodoApi2.Models;

namespace TodoApi2.Validator
{
    public class EmployeeValidator: AbstractValidator<EmployeeModelPersist>
    {
        public EmployeeValidator()
        {
            RuleFor(x => x.FirstName).NotNull().NotEmpty()
                .WithMessage("{PropertyName} should not be empty.");
            RuleFor(x => x.LastName).NotNull().NotEmpty()
                .WithMessage("{PropertyName} should not be empty.");
            RuleFor(x => x.Age).GreaterThan(0);
        }
       
    }
}