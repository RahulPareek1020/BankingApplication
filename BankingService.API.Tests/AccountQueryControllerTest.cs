using BankingService.API.Controllers;
using BankingService.Domain.Commands.ViewModels;
using BankingService.Domain.Queries.Account;
using Infrastucture.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BankingService.API.Tests
{
    [TestClass]
    public class AccountQueryControllerTests
    {
        private AccountQueryController controller;
        private Mock<IMediator> mediatorMock;

        [TestInitialize]
        public void Setup()
        {
            mediatorMock = new Mock<IMediator>();
            controller = new AccountQueryController(mediatorMock.Object);
        }

        [TestMethod]
        public async Task GetAccountsByUserId_ValidQuery_ReturnsOk()
        {
            // Arrange
            int userId = 123;
            var validQuery = new GetAccountByUserIdQuery { UserId = userId };

            // Mock mediator's response
            var expectedResponse = new List<UserAccountViewModel>
            {
                new UserAccountViewModel { AccountId = 1, UserId = userId, AccountBalance = 100 },
                new UserAccountViewModel { AccountId = 2, UserId = userId, AccountBalance = 200 }
            };

            mediatorMock.Setup(m => m.Send(validQuery, It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);


            // Act
            var result = await controller.GetAccountsByUserId(validQuery);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.AreEqual(expectedResponse, okResult.Value);
        }       
    }
}
