using BoutiqueEducation.Business.Models.DTOs;
using FluentValidation;

namespace BoutiqueEducation.Business.Validators;

public class QuestionCreateDtoValidator : AbstractValidator<QuestionCreateDto>
{
    public QuestionCreateDtoValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Soru içeriği boş olamaz.")
            .MaximumLength(2000).WithMessage("Soru en fazla 2000 karakter olabilir.");

        RuleFor(x => x.ImageUrl)
            .MaximumLength(500).WithMessage("Resim URL'i çok uzun olamaz.")
            .When(x => !string.IsNullOrEmpty(x.ImageUrl));
    }
}
