using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyAccountsController : ControllerBase
    {
        private readonly ICurrencyAccountService _currencyAccountService;
        public CurrencyAccountsController(ICurrencyAccountService currencyAccountService)
        {
            _currencyAccountService = currencyAccountService;
        }
        [HttpPost("add")]
        public IActionResult Add(CurrencyAccount currencyAccount)
        {
            var result = _currencyAccountService.Add(currencyAccount);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpPost("update")]
        public IActionResult Update(CurrencyAccount currencyAccount)
        {
            var result = _currencyAccountService.Update(currencyAccount);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpPost("delete")]
        public IActionResult Delete(CurrencyAccount currencyAccount)
        {
            var result = _currencyAccountService.Delete(currencyAccount);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpPost("getById")]
        public IActionResult GetById(int id)
        {
            var result = _currencyAccountService.Get(id);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpGet("getList")]
        public IActionResult GetList(int companyId)
        {
            var result = _currencyAccountService.GetList(companyId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }
    }
}
