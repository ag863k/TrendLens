using Microsoft.Extensions.DependencyInjection;
using TrendLens.Application.Services;
using TrendLens.Application.Validators;
using TrendLens.Core.DTOs;
using TrendLens.Core.Interfaces;
using FluentValidation;
using System.Reflection;

namespace TrendLens.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAnalyticsService, AnalyticsServiceV2>();
        services.AddScoped<IAnalyticsServiceV2, AnalyticsServiceV2>();
        services.AddScoped<IMachineLearningService, MachineLearningService>();
        services.AddScoped<ISalesService, SalesService>();
        
        services.AddScoped<IValidator<CreateSalesRecordDto>, CreateSalesRecordValidator>();
        services.AddScoped<IValidator<UpdateSalesRecordDto>, UpdateSalesRecordValidator>();
        
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
        return services;
    }
}
