using BankingService.Domain.Commands.Account;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BankingService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountCommandController : ControllerBase
    {
        private readonly IMediator mediator;

        public AccountCommandController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("AddAccount", Name = "AddAccount")]
        public async Task<ActionResult> AddAccount(AddAccountCommand command)
        {
            string jsonCommand = JsonConvert.SerializeObject(command);

            var response = await mediator.Send(command);

            return this.Ok(response);
        }

        [HttpPost("DeleteAccount", Name = "DeleteAccount")]
        public async Task<ActionResult> DeleteAccount(DeleteAccountCommand command)
        {
            string jsonCommand = JsonConvert.SerializeObject(command);

            var response = await mediator.Send(command);

            return this.Ok(response);
        }
    }
}
