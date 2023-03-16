using Core.Entities.Concrete;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ValidationRules.FluentValidation
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(p => p.Name).NotEmpty().WithMessage("Kullanıcı adı boş olamaz ! ");
            RuleFor(p => p.Name).MinimumLength(3).WithMessage("Kullanıcı adı en az 3 karakter olmalıdır.");
            RuleFor(p => p.Email).NotEmpty().WithMessage("Mail boş olamaz.");
            RuleFor(p => p.Email).EmailAddress().WithMessage("Lütfen geçerli bir mail adresi yazın.");
        }
    }
}
