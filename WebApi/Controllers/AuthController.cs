using Business.Abstract;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authservice;
        public AuthController(IAuthService authservice)
        {
            _authservice = authservice;
        }

        [HttpPost("register")]
        public IActionResult Register(UserAndCompanyRegisterDto userAndCompanyRegister)
        {
            var userExist = _authservice.UserExist(userAndCompanyRegister.UserForRegister.Email);
            if (!userExist.Success)
            {
                return BadRequest(userExist.Message);
            }

            var companyExist = _authservice.CompanyExist(userAndCompanyRegister.Company);
            if (!companyExist.Success)
            {
                return BadRequest(companyExist.Message);
            }

            var registerResult = _authservice.Register(userAndCompanyRegister.UserForRegister, userAndCompanyRegister.UserForRegister.Password, userAndCompanyRegister.Company);

            var result = _authservice.CreateAccesToken(registerResult.Data, registerResult.Data.CompanyId); //bu if bize token'ı dönecek, altta yorum satırına aldığımızsa register işlemini komple geri döndürüyordu..
            if (result.Success)
            {
                return Ok(result.Data);
            }

            return BadRequest(registerResult.Message);
        }

        [HttpPost("registerSecondAccount")]
        public IActionResult RegisterSecondAccount(UserForRegister userForRegister, int companyId)
        {
            var userExist = _authservice.UserExist(userForRegister.Email);

            if (!userExist.Success)
            {
                return BadRequest(userExist.Message);
            }

            var registerResult = _authservice.RegisterSecondAccount(userForRegister, userForRegister.Password);

            var result = _authservice.CreateAccesToken(registerResult.Data, companyId); //bu if bize token'ı dönecek, altta yorum satırına aldığımızsa register işlemini komple geri döndürüyordu..
            if (result.Success)
            {
                return Ok(result.Data);
            }

            //if(registerResult.Success)
            //{
            //    return Ok(registerResult);
            //}
            return BadRequest(registerResult.Message);
        }

        [HttpPost("login")]
        public IActionResult Login(UserForLogin userForLogin)
        {
            var userToLogin = _authservice.Login(userForLogin);
            if (!userToLogin.Success)
            {
                return BadRequest(userToLogin.Message);
            }

            var result = _authservice.CreateAccesToken(userToLogin.Data, 0);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        [HttpGet("confirmuser")]
        public IActionResult ConfirmUser(string value) //Users'daki mailconfirmvalue'yi yakalıyoruz string value ile.
        {
            var user = _authservice.GetByMailConfirmValue(value).Data;
            user.MailConfirm = true;
            user.MailConfirmDate = DateTime.Now;
            var result = _authservice.Update(user);
            if (result.Success)
            {
                return Ok();
            }
            return BadRequest(result.Message);
        }

        [HttpGet("sendConfirmEmail")] //Mailin tekrardan gönderilmesi işlemi.
        public IActionResult SendConfirmEmail(int id)
        {
            var user = _authservice.GetById(id).Data;
            var result = _authservice.SendConfirmEmail(user);
            if (result.Success)
            {
                return Ok();
            }
            return BadRequest(result.Message); 
        }
    }
}
