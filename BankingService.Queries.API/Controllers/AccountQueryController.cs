using BankingService.Domain.Queries.Account;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BankingService.Queries.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountQueryController : ControllerBase
    {
        private readonly IMediator mediator;

        public AccountQueryController(IMediator mediator)
        {
            this.mediator = mediator;            
        }

        [HttpGet("GetAccountsByUserId", Name = "GetAccountsByUserId")]
        public async Task<ActionResult> GetAccountsByUserId([FromQuery] GetAccountByUserIdQuery query)
        {
            string jsonQuery = JsonSerializer.Serialize(query);

            var response = await mediator.Send(query);
            return Ok(response);
        }
    }
}
