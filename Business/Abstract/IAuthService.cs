using Core.Entities.Concrete;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Security.JWT;
using Entities.Concrete;
using Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    /* Giriş işlemleri için(kullanıcı girişi,kullanıcı kaydı,kullanıcı kontrolü) yapılan servis */
    public interface IAuthService
    {
        IDataResult<UserCompanyDto> Register(UserForRegister userForRegister, string password, Company company);
        IDataResult<User> RegisterSecondAccount(UserForRegister userForRegister, string password);
        IDataResult<User> Login(UserForLogin userForLogin);
        IResult UserExist(string email);
        IResult CompanyExist(Company company);
        IDataResult<AccessToken> CreateAccesToken(User user, int companyId);
    }
}
