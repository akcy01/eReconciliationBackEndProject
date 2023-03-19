using Business.Abstract;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : Controller
    {
        private readonly ICompanyService _companyService;
        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpGet("getcompanylist")]
        public IActionResult GetCompanyList()
        {
           var result = _companyService.GetList();
            if(result.Success) 
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }
        [HttpPost("addCompanyAndUserCompany")] // Kullanıcı bir şirket daha ekleyebilsin diye.Kullanıcı şirket ekleme.
        public IActionResult AddCompanyAndUserCompany(CompanyDto companyDto)
        {
            var result = _companyService.AddCompanyAndUserCompany(companyDto);
            if(result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }
    }
}
