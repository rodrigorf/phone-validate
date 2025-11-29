using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhoneValidate.Application.Services.Dto;
using PhoneValidate.Application.Services.Interfaces;

namespace PhoneValidate.Controllers
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

        [Authorize]
        [HttpPost(Name = "CreateRecipient")]
        public async Task<ActionResult<RecipientsDto>> Post(RecipientsDto recipientsDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Validation failed",
                    errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                });
            }

            var result = await _recipientAppService.CreateRecipient(recipientsDto);
            if (!result.Success)
                return Conflict(new { message = result.ErrorMessage });

            return CreatedAtAction(nameof(Get), new { phoneNumber = result.Data!.PhoneNumber }, result.Data);
        }

        [Authorize]
        [HttpPut("{id}", Name = "UpdateRecipient")]
        public async Task<ActionResult<RecipientsDto>> Put(Guid id, RecipientsDto recipientsDto)
        {
            var result = await _recipientAppService.UpdateRecipientAsync(id, recipientsDto);

            if (!result.Success)
            {
                if (result.ErrorMessage!.Contains("not found"))
                    return NotFound(new { message = result.ErrorMessage });

                return Conflict(new { message = result.ErrorMessage });
            }

            return Ok(result.Data);
        }
    }
}
