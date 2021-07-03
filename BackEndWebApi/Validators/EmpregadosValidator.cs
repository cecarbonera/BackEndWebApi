using BackEndWebApi.Models;
using FluentValidation;

namespace BackEndWebApi.Validators
{
    public class EmpregadosValidator : AbstractValidator<EmpregadosDTO>
    {
        public EmpregadosValidator()
        {
            RuleFor(p => p.Codigo).GreaterThan(0).WithMessage("Código inválido.");
            RuleFor(p => p.Nome).NotEmpty().WithMessage("Nome inválida.");
            RuleFor(p => p.CodigoDepto).NotEmpty().WithMessage("Departamento inválido.");
            RuleFor(p => p.DataEntrada).NotEmpty().WithMessage("Data de Entrada inválida.");
            RuleFor(p => p.Foto).NotEmpty().WithMessage("Foto inválida.");

        }

    }

}