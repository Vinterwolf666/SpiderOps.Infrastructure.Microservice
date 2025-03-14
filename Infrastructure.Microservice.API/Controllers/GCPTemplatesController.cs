using Infrastructure.Microservice.APP;
using Infrastructure.Microservice.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Infrastructure.Microservice.API.Controllers
{
    [ApiController]
    [Route("Customer.Infrastructure.Microservice.Templates.API")]
    public class GCPTemplatesController : Controller
    {
        private readonly IGCPTemplatesServices _s;
        public GCPTemplatesController(IGCPTemplatesServices s)
        {
            _s = s;
        }

        [HttpPost]
        [Route("UploadATemplate")]
        public async Task<IActionResult> UploadTemplate([FromForm] IFormFile file, [FromForm] string templateName, [FromForm]string descriptions)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("Invalid archive, try another one");
                }

                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                byte[] fileBytes = memoryStream.ToArray();

                string result = await _s.SaveNewTemplate(templateName, fileBytes,descriptions);

                return Ok(new { Message = result });

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); 
            }
        }

        [HttpGet]
        [Route("AllTemplates")]
        public ActionResult<List<GCPTemplates>> AllTemplates()
        {
            try
            {
                var result = _s.AllTemplates();

                return Ok(result);

            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }


        [HttpDelete]
        [Route("DeleteATemplateByID")]
        public async Task<ActionResult<string>> DeleteATemplateByID(int id)
        {
            try
            {
                var result = await _s.RemoveTemplate(id);

                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }
    }
}
