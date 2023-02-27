using Business.Abstract;
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
        public IActionResult Register(UserForRegister userForRegister)
        {
            var userExist = _authservice.UserExist(userForRegister.Email);

            if(!userExist.Success)
            {
                return BadRequest(userExist.Message);
            }

            var registerResult = _authservice.Register(userForRegister, userForRegister.Password);

            var result = _authservice.CreateAccesToken(registerResult.Data, 0); //bu if bize token'ı dönecek, altta yorum satırına aldığımızsa register işlemini komple geri döndürüyordu..
            if(result.Success)
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
            if(!userToLogin.Success)
            {
                return BadRequest(userToLogin.Message);
            }

            var result = _authservice.CreateAccesToken(userToLogin.Data, 0);
            if(result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }
    }
}
