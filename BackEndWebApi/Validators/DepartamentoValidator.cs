using BackEndWebApi.Models;
using FluentValidation;

namespace BackEndWebApi.Validators
{
    public class DepartamentoValidator : AbstractValidator<DepartamentoDTO>
    {
        public DepartamentoValidator()
        {
            RuleFor(p => p.Codigo)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage("Código inválido.");
            
            RuleFor(p => p.Descricao)
                .NotEmpty()
                .WithMessage("Descrição inválida.");

        }

    }

}