using BankingService.API.Controllers;
using BankingService.Domain.Commands.Transaction;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

namespace BankingService.API.Tests
{
    [TestClass]
    public class TransactionCommandControllerTests
    {
        private TransactionCommandController controller;
        private Mock<IMediator> mediatorMock;

        [TestInitialize]
        public void Setup()
        {
            mediatorMock = new Mock<IMediator>();
            controller = new TransactionCommandController(mediatorMock.Object);
        }

        [TestMethod]
        public async Task DepositAmount_ValidCommand_ReturnsOk()
        {
            // Arrange
            var validDepositCommand = new DepositAmountCommand { AmountToDeposit = 100 };

            // Mock mediator's response
            string expectedResponse = "Success";
            mediatorMock.Setup(m => m.Send(validDepositCommand, It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);

            // Act
            var result = await controller.DepositAmount(validDepositCommand);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.AreEqual(expectedResponse, okResult.Value);
        }

        [TestMethod]
        public async Task DepositAmount_ExceedsLimit_ReturnsBadRequest()
        {
            // Arrange
            var commandExceedingLimit = new DepositAmountCommand { AmountToDeposit = 1000 };

            // Mock mediator's response
            string expectedResponse = "Deposit amount exceeds the limit.";
            mediatorMock.Setup(m => m.Send(commandExceedingLimit, It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);

            // Act
            var result = await controller.DepositAmount(commandExceedingLimit);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual(expectedResponse, badRequestResult.Value);
        }

        [TestMethod]
        public async Task WithdrawAmount_ValidCommand_ReturnsOk()
        {
            // Arrange
            var validWithdrawCommand = new WithdrawAmountCommand { WithdrawalAmount = 50 };

            // Mock mediator's response
            string expectedResponse = "Success";
            mediatorMock.Setup(m => m.Send(validWithdrawCommand, It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);

            // Act
            var result = await controller.WithdrawAmount(validWithdrawCommand);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.AreEqual(expectedResponse, okResult.Value);
        }

        [TestMethod]
        public async Task WithdrawAmount_ExceedsLimitOrMinimumBalance_ReturnsBadRequest()
        {
            // Arrange
            var commandExceedingLimit = new WithdrawAmountCommand { WithdrawalAmount = 500 };
            var commandInsufficientBalance = new WithdrawAmountCommand { WithdrawalAmount = 200 };

            // Mock mediator's response
            string expectedResponse1 = "Withdraw amount exceeds the limit.";
            string expectedResponse2 = "Cannot withdraw. Minimum balance requirement not met.";
            mediatorMock.Setup(m => m.Send(commandExceedingLimit, It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse1);
            mediatorMock.Setup(m => m.Send(commandInsufficientBalance, It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse2);

            // Act
            var result1 = await controller.WithdrawAmount(commandExceedingLimit);
            var result2 = await controller.WithdrawAmount(commandInsufficientBalance);

            // Assert
            Assert.IsNotNull(result1);
            Assert.IsInstanceOfType(result1, typeof(BadRequestObjectResult));
            var badRequestResult1 = result1 as BadRequestObjectResult;
            Assert.AreEqual(expectedResponse1, badRequestResult1.Value);

            Assert.IsNotNull(result2);
            Assert.IsInstanceOfType(result2, typeof(BadRequestObjectResult));
            var badRequestResult2 = result2 as BadRequestObjectResult;
            Assert.AreEqual(expectedResponse2, badRequestResult2.Value);
        }
    }
}