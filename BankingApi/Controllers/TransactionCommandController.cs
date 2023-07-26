using BankingService.Domain.Commands.Transaction;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BankingService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionCommandController : ControllerBase
    {
        private readonly IMediator mediator;
        public TransactionCommandController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("DepositAmount", Name = "DepositAmount")]
        public async Task<ActionResult> DepositAmount(DepositAmountCommand command)
        {
            string jsonCommand = JsonConvert.SerializeObject(command);

            var response = await mediator.Send(command);

            if (response == "Deposit amount exceeds the limit.")
            {
                return this.BadRequest(response);
            }

            return this.Ok(response);
        }

        [HttpPost("WithdrawAmount", Name = "WithdrawAmount")]
        public async Task<ActionResult> WithdrawAmount(WithdrawAmountCommand command)
        {
            string jsonCommand = JsonConvert.SerializeObject(command);

            var response = await mediator.Send(command);

            if (response == "Withdraw amount exceeds the limit." || response == "Cannot withdraw. Minimum balance requirement not met.")
            {
                return this.BadRequest(response);
            }

            return this.Ok(response);
        }
    }
}
