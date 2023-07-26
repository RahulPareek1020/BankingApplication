using BankingService.Domain.Commands.Transaction;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BankingService.Commands.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionCommandController : ControllerBase
    {
        private readonly IMediator mediator;
        public TransactionCommandController(IMediator mediator) {
            this.mediator = mediator;  
        }

        [HttpPost("DepositAmount", Name = "DepositAmount")]
        public async Task<ActionResult> DepositAmount(DepositAmountCommand command)
        {
            string jsonCommand = JsonConvert.SerializeObject(command);

            var respose = await mediator.Send(command);
            
            return this.Ok(respose);            
        }

        [HttpPost("WithdrawAmount", Name = "WithdrawAmount")]
        public async Task<ActionResult> WithdrawAmount(WithdrawAmountCommand command)
        {
            string jsonCommand = JsonConvert.SerializeObject(command);

            var respose = await mediator.Send(command);

            return this.Ok(respose);
        }
    }
}
