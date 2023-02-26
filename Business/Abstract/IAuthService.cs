﻿using Core.Entities.Concrete;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Security.JWT;
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
        IDataResult<User> Register(UserForRegister userForRegister, string password);
        IDataResult<User> Login(UserForRegister userForRegister);
        IResult UserExist(string email);
        IDataResult<AccessToken> CreateAccesToken(User user);
    }
}