using FluentValidation;
using TrendLens.Core.DTOs;

namespace TrendLens.Application.Validators;

public class CreateSalesRecordValidator : AbstractValidator<CreateSalesRecordDto>
{
    public CreateSalesRecordValidator()
    {
        RuleFor(x => x.Date)
            .NotEmpty()
            .WithMessage("Date is required")
            .LessThanOrEqualTo(DateTime.Now)
            .WithMessage("Date cannot be in the future");

        RuleFor(x => x.ProductName)
            .NotEmpty()
            .WithMessage("Product name is required")
            .MaximumLength(200)
            .WithMessage("Product name cannot exceed 200 characters");

        RuleFor(x => x.Category)
            .NotEmpty()
            .WithMessage("Category is required")
            .MaximumLength(100)
            .WithMessage("Category cannot exceed 100 characters");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than 0")
            .LessThan(1000000)
            .WithMessage("Amount cannot exceed 1,000,000");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0")
            .LessThan(100000)
            .WithMessage("Quantity cannot exceed 100,000");

        RuleFor(x => x.CustomerName)
            .NotEmpty()
            .WithMessage("Customer name is required")
            .MaximumLength(200)
            .WithMessage("Customer name cannot exceed 200 characters");

        RuleFor(x => x.Region)
            .NotEmpty()
            .WithMessage("Region is required")
            .MaximumLength(100)
            .WithMessage("Region cannot exceed 100 characters");

        RuleFor(x => x.SalesRep)
            .NotEmpty()
            .WithMessage("Sales representative is required")
            .MaximumLength(200)
            .WithMessage("Sales representative name cannot exceed 200 characters");
    }
}

public class UpdateSalesRecordValidator : AbstractValidator<UpdateSalesRecordDto>
{
    public UpdateSalesRecordValidator()
    {
        Include(new CreateSalesRecordValidator());
        
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Valid ID is required");
    }
}
