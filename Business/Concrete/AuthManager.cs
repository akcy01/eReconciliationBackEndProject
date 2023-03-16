using Business.Abstract;
using Business.Constants;
using Business.ValidationRules.FluentValidation;
using Core.CrossCuttingConcerns.Validation;
using Core.Entities.Concrete;
using Core.Utilities.Hashing;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using Core.Utilities.Security.JWT;
using Entities.Concrete;
using Entities.Dtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class AuthManager : IAuthService
    {
        private readonly IUserService _userService;
        private readonly ITokenHelper _tokenHelper;
        private readonly ICompanyService _companyService;
        private readonly IMailParameterService _mailParameterService;
        private readonly IMailService _mailService;
        private readonly IMailTemplateService _mailTemplateService;
        public AuthManager(IUserService userService, ITokenHelper tokenHelper, ICompanyService companyService, IMailParameterService mailParameterService, IMailService mailService , IMailTemplateService mailTemplateService)
        {
            _userService = userService;
            _tokenHelper = tokenHelper;
            _companyService = companyService;
            _mailParameterService = mailParameterService;
            _mailService = mailService;
            _mailTemplateService = mailTemplateService;
        }

        public IResult CompanyExist(Company company)
        {
            var result = _companyService.CompanyExist(company);

            if (result.Success == false) // Bu kayıt daha önce varsa
            {
                return new ErrorResult(Messages.CompanyAlreadyExists);
            }
            return new SuccessResult();
        }

        public IDataResult<AccessToken> CreateAccesToken(User user,int companyId)
        {
            var claims = _userService.GetClaims(user, companyId);
            var accessToken = _tokenHelper.CreateToken(user, claims, companyId);
            return new SuccessDataResult<AccessToken>(accessToken);
        }

        public IDataResult<User> GetById(int id)
        {
            return new SuccessDataResult<User>(_userService.GetById(id));
        }

        public IDataResult<User> GetByMailConfirmValue(string value)
        {
            return new SuccessDataResult<User>(_userService.GetByMailConfirmValue(value));
        }

        public IDataResult<User> Login(UserForLogin userForLogin)
        {
            var userToCheck = _userService.GetByMail(userForLogin.Email);

            if (userToCheck == null)
            {
                return new ErrorDataResult<User>(Messages.UserNotFound);
            }

            if(!HashingHelper.VerifyPasswordHash(userForLogin.Password,userToCheck.PasswordHash,userToCheck.PasswordSalt)) 
            {
                return new ErrorDataResult<User>(Messages.PasswordError);
            }

            return new SuccessDataResult<User>(userToCheck, Messages.SuccessfullLogin);
        }
        /* Kullanıcı kayıt işlemi */
        public IDataResult<UserCompanyDto> Register(UserForRegister userForRegister, string password, Company company)
        {
            byte[] passwordHash, passwordSalt;
            HashingHelper.CreatePasswordHash(password, out passwordHash, out passwordSalt);
            var user = new User()
            {
                Email = userForRegister.Email,
                AddedAt = DateTime.Now,
                IsActive = true,
                MailConfirm = false,
                MailConfirmDate = DateTime.Now,
                MailConfirmValue = Guid.NewGuid().ToString(),
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Name = userForRegister.Name,
            };

            ValidationTool.Validate(new UserValidator(), user);
            ValidationTool.Validate(new CompanyValidator(), company);

            _userService.Add(user);
            _companyService.Add(company);

            _companyService.UserCompanyAdd(user.Id, company.Id);

            UserCompanyDto userCompanyDto = new UserCompanyDto()
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                AddedAt = user.AddedAt,
                CompanyId = company.Id,
                IsActive = true,
                MailConfirm = user.MailConfirm,
                MailConfirmDate = user.MailConfirmDate,
                MailConfirmValue = user.MailConfirmValue,
                PasswordHash = user.PasswordHash,
                PasswordSalt = user.PasswordSalt,
            };

            SendConfirmEmail(user);

            return new SuccessDataResult<UserCompanyDto>(userCompanyDto, Messages.UserRegistered);
        }

        void SendConfirmEmail(User user)
        {
            string subject = "Kullanıcı Kayıt Onay Maili";
            string body = "Kullanıcınız sisteme kayıt oldu. Kaydınızı tamamlamak için aşağıdaki linke tıklamanız gerkmektedir";
            string link = "https://localhost:7076/api/Auth/confirmuser?value=" + user.MailConfirmValue;
            string linkDescription = "Kaydı onaylamak için tıklayın.";

            var mailTemplate = _mailTemplateService.GetByTemplateName("deneme", 16);
            string templateBody = mailTemplate.Data.Value;
            templateBody = templateBody.Replace("{{title}}", subject);
            templateBody = templateBody.Replace("{{message}}", body);
            templateBody = templateBody.Replace("{{link}}", link);
            templateBody = templateBody.Replace("{{linkDescription}}", linkDescription);

            var mailParameter = _mailParameterService.Get(3);
            SendMailDto sendMailDto = new SendMailDto()
            {
                mailParameter = mailParameter.Data,
                email = user.Email,
                subject = "Kullanıcı Kayıt onay maili",
                body = templateBody
            };

            _mailService.SendMail(sendMailDto);

            user.MailConfirmDate = DateTime.Now;
            _userService.Update(user);
        }

        public IDataResult<User> RegisterSecondAccount(UserForRegister userForRegister, string password, int companyId)
        {
            byte[] passwordHash, passwordSalt;
            HashingHelper.CreatePasswordHash(password, out passwordHash, out passwordSalt);
            var user = new User()
            {
                Email = userForRegister.Email,
                AddedAt = DateTime.Now,
                IsActive = true,
                MailConfirm = false,
                MailConfirmDate = DateTime.Now,
                MailConfirmValue = Guid.NewGuid().ToString(),
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Name = userForRegister.Name,
            };

            _userService.Add(user);
            _companyService.UserCompanyAdd(user.Id, companyId);
            SendConfirmEmail(user);

            return new SuccessDataResult<User>(user, Messages.UserRegistered);
        }

        public IResult Update(User user)
        {
            _userService.Update(user);
            return new SuccessResult(Messages.UserMailConfirmSuccessfull);
        }

        /* Kayıt olan kullanıcı varsa tekrar kayıt olmasın diye yapılan metot */
        public IResult UserExist(string email)
        {
            if(_userService.GetByMail(email) != null)
            {
                return new ErrorResult(Messages.UserAlreadyExist);
            }
            return new SuccessResult();
        }

         IResult IAuthService.SendConfirmEmail(User user)
        {
            if(user.MailConfirm == true)
            {
                return new ErrorResult(Messages.MailAlreadyConfirm);
            }

            DateTime confirmMailDate = user.MailConfirmDate;
            DateTime now = DateTime.Now;
            if(confirmMailDate.ToShortDateString() == now.ToShortDateString()) //ToShortDateString metodu tarihteki saatleri siler. Burada eğer confirm mailinin gönderildiği tarih bugünse dedik
            {
                if(confirmMailDate.Hour == now.Hour && confirmMailDate.AddMinutes(5).Minute <= now.Minute) //Eğer confirm mailinin atıldığı saat şimdiyse ve 5 dakikayı geçtiyse maili gönder dedik.
                {
                    SendConfirmEmail(user);
                    return new SuccessResult(Messages.MailConfirmSendSuccessfull);
                }
                else
                {
                    return new ErrorResult(Messages.MailConfirmTimeHasNotExpired);
                }
            }

            SendConfirmEmail(user);
            return new SuccessResult(Messages.MailConfirmSendSuccessfull);
        }

        public IDataResult<UserCompany> GetCompany(int userId)
        {
            return new SuccessDataResult<UserCompany>(_companyService.GetCompany(userId).Data);
        }
    }
}
