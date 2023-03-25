using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaBsReconciliationController : ControllerBase
    {
        private readonly IBaBsReconciliationService _baBsReconciliationService;
        public BaBsReconciliationController(IBaBsReconciliationService baBsReconciliationService)
        {
            _baBsReconciliationService = baBsReconciliationService;
        }

        [HttpPost("addFromExcel")]
        public IActionResult AddFromExcel(IFormFile file, int companyId) //IFormFile sayesinde dosya yükleyebiliyoruz.
        {
            if (file.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + ".xlsx";
                var filePath = $"{Directory.GetCurrentDirectory()}/Content/{fileName}";
                using (FileStream stream = System.IO.File.Create(filePath))
                {
                    file.CopyTo(stream);
                    stream.Flush();
                }

                var result = _baBsReconciliationService.AddToExcel(filePath, companyId);
                if (result.Success)
                {
                    return Ok(result);
                }
                return BadRequest(result.Message);
            }
            return BadRequest("Dosya seçimi yapmadınız");
        }

        [HttpPost("add")]
        public IActionResult Add(BaBsReconciliation baBsReconciliation)
        {
            var result = _baBsReconciliationService.Add(baBsReconciliation);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("update")]
        public IActionResult Update(BaBsReconciliation baBsReconciliation)
        {
            var result = _baBsReconciliationService.Update(baBsReconciliation);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("delete")]
        public IActionResult Delete(BaBsReconciliation baBsReconciliation)
        {
            var result = _baBsReconciliationService.Delete(baBsReconciliation);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("getById")]
        public IActionResult GetById(int id)
        {
            var result = _baBsReconciliationService.GetById(id);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("getList")]
        public IActionResult GetList(int companyId)
        {
            var result = _baBsReconciliationService.GetList(companyId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}
