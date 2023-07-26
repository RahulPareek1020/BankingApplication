using BankingService.API.Controllers;
using BankingService.Domain.Commands.Account;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

namespace BankingService.API.Tests
{
    [TestClass]
    public class AccountCommandControllerTests
    {
        [TestMethod]
        public async Task AddAccount_ValidCommand_ReturnsOkResult()
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();
            var controller = new AccountCommandController(mediatorMock.Object);
            var validCommand = new AddAccountCommand { UserId = 1, Balance = 100 };
            string successMessage = "Account Added successfully.";

            mediatorMock.Setup(m => m.Send(validCommand, It.IsAny<CancellationToken>())).ReturnsAsync(successMessage);

            // Act
            var result = await controller.AddAccount(validCommand);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is OkObjectResult);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(successMessage, okResult.Value);
        }

        [TestMethod]
        public async Task DeleteAccount_ValidCommand_ReturnsOkResult()
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();
            var controller = new AccountCommandController(mediatorMock.Object);
            var validCommand = new DeleteAccountCommand {UserId = 1, AccountId = 2 };

            string successMessage = "Account Deleted successfully.";

            mediatorMock.Setup(m => m.Send(validCommand, It.IsAny<CancellationToken>())).ReturnsAsync(successMessage);

            // Act
            var result = await controller.DeleteAccount(validCommand);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is OkObjectResult);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(successMessage, okResult.Value);
        }
    }
}