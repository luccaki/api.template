using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Api.Template.ApplicationCore.Dto;
using Api.Template.ApplicationCore.Interfaces.Services;

namespace Api.Template.Presentation.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/template")]
    public class TemplateController : ControllerBase
    {
        private readonly ITemplateService _templateService;
        public TemplateController(ITemplateService TemplateService)
        {
            _templateService = TemplateService;
        }

        /// <summary>
        /// Busca os Templates de acordo com os filtros
        /// </summary>
        /// <param name="pageNumber">Page number</param>
        /// <param name="name">Name of the template</param>
        /// <response code="200">Success</response>  
        /// <response code="404">None template found</response> 
        [HttpGet]
        [ProducesResponseType(typeof(TemplateResultDto), 200)]
        public async Task<IActionResult> GetTemplatesAsync([FromQuery] int? pageNumber, [FromQuery] string name)
        {
            return Ok(await _templateService.GetTemplateAsync(pageNumber, name));
        }

        /// <summary>
        /// Search for an template
        /// </summary>
        /// <param name="id">TemplateId</param>
        /// <response code="200">Success</response>  
        /// <response code="404">Template not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TemplateDto), 200)]
        public async Task<IActionResult> GetTemplateByIdAsync([FromRoute] int id)
        {
            return Ok(await _templateService.GetTemplateByIdAsync(id));
        }

        /// <summary>
        /// Update a specific template
        /// </summary>
        /// <param name="id">TemplateId</param>
        /// <param name="body">Template data</param>
        /// <response code="200">Success</response>  
        /// <response code="404">Template not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> UpdateTemplate([FromRoute] int id, [FromBody] TemplateDto body)
        {
            await _templateService.UpdateTemplateAsync(id, body);
            return Ok();
        }

        /// <summary>
        /// Insert a new template
        /// </summary>
        /// <param name="body">Template data</param>
        /// <response code="201">Success - Return TemplateId</response>
        [HttpPost()]
        [ProducesResponseType(typeof(int), 201)]
        public async Task<IActionResult> PostTemplate([FromBody] TemplatePostDto body)
        {
            return Created("", await _templateService.InsertTemplateAsync(body));
        }
    }
}