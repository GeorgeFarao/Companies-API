using System;
using System.ComponentModel.DataAnnotations;
using FluentValidation;
using TodoApi2.Data;
using TodoApi2.Models;

namespace TodoApi2.Validator
{
    public class BranchValidator : AbstractValidator<BranchModelPersist>
    {
        public BranchValidator()
        {
            RuleFor(x => x.Name).NotNull().NotEmpty()
                .WithMessage("{PropertyName} should not be empty.");
           
        }

    }
}
