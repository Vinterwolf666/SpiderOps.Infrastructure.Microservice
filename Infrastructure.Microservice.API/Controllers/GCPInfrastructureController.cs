using Infrastructure.Microservice.API.Services;
using Infrastructure.Microservice.APP;
using Infrastructure.Microservice.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;

namespace Infrastructure.Microservice.API.Controllers
{
    [ApiController]
    [Route("Customer.Infrastructure.Microservice.Infrastructure.API")]
    public class GCPInfrastructureController : Controller
    {
        private readonly IGCPInfrastructureServices _services;
        public GCPInfrastructureController(IGCPInfrastructureServices s)
        {
            _services = s;
        }

        [HttpPost]
        [Route("AllInfrastructureByID")]
        public ActionResult<List<GCPInfrastructure>> AllCustomerInfrastructureByID(int id)
        {
            try
            {
                var result = _services.AllInfrastructureByID(id);

                return Ok(result);


            }catch (Exception ex)
            {

               return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("NewInfrastructure")]
        public async Task<IActionResult> NewInfrastructure(int customer_id, string language, string template, int id, string REGION, string CLUSTER_NAME, string ARTIFACT_REGISTRY_REGION, string ARTIFACT_REGISTRY,string APP_NAME)
        {
            try
            {
                var result = await _services.CreateNewInfrastructure(customer_id, language, template, id, REGION,CLUSTER_NAME, ARTIFACT_REGISTRY_REGION, ARTIFACT_REGISTRY,APP_NAME);

                var rabbitMqProducer = new RabbitMQProducer();
                await rabbitMqProducer.InfrastrctureStageComplete();

                if (result == null)
                {
                    return NotFound("Template not found.");
                }

                return result;


            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }



        [HttpDelete]
        [Route("RemoveInfrastructureByID")]
        public async Task<ActionResult<string>> RemoveInfrastructureByID(int id)
        {
            try
            {
                var result = await _services.RemoveNewInfrastructure(id);

                return Ok(result);


            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }


    }
}
