using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BoutiqueEducation.API.Filters;

public sealed class ValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Gelen her parametreyi (orn: DTO) kontrol ediyoruz
        foreach (var argument in context.ActionArguments.Values.Where(v => v != null))
        {
            var type = argument!.GetType();

            // Eğer parametre tipi (DTO) için bir FluentValidation Validator'ü yazılmışsa DI'dan istiyoruz
            var validatorType = typeof(IValidator<>).MakeGenericType(type);
            var validator = context.HttpContext.RequestServices.GetService(validatorType) as IValidator;

            if (validator != null)
            {
                var validationContext = new ValidationContext<object>(argument);
                var validationResult = await validator.ValidateAsync(validationContext);

                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                    // Validation hatası varsa kontroller'a gitmeden geriye 400 Bad Request dönüyoruz.
                    context.Result = new BadRequestObjectResult(new { Message = "Veri doğrulama hataları oluştu.", Errors = errors });
                    return;
                }
            }
        }

        await next();
    }
}
