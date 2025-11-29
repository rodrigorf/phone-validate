using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhoneValidate.Application.Services.Dto;
using PhoneValidate.Application.Services.Interfaces;

namespace PhoneValidation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RecipientController : ControllerBase
    {
        private readonly IRecipientAppService _recipientAppService;

        public RecipientController(IRecipientAppService weatherForecastAppService)
        {
            _recipientAppService = weatherForecastAppService;
        }

        [Authorize]
        [HttpGet(Name = "GetRecipient")]
        public async Task<ActionResult<RecipientsDto>> Get(string phoneNumber)
        {
            var result = await _recipientAppService.GetRecipientAsync(phoneNumber);

            if (result == null)
                return NotFound(new { message = $"Record with phoneNumber '{phoneNumber}' not found" });

            return Ok(result);
        }

    }
}
