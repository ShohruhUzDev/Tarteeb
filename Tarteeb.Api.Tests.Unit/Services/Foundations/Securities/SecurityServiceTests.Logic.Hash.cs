using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Tarteeb.Api.Models.Foundations.Users;
using Tarteeb.Api.Services.Foundations.Securities;
using Xunit;

namespace Tarteeb.Api.Tests.Unit.Services.Foundations
{
    public partial class SecurityServiceTests
    {
        [Fact]
        public void ShouldHashPassword()
        {
            //given
            string randomPassword = CreateRandomString();
            string inputPassword = randomPassword;
            string expectedPassword = inputPassword;

            this.tokenBrokerMock.Setup(broker =>
                broker.HashToken(inputPassword)).Returns(expectedPassword);

            //when
            string actualPassword = securityService.HashPassword(inputPassword);

            //then
            actualPassword.Should().BeEquivalentTo(expectedPassword);

            this.tokenBrokerMock.Verify(broker =>
                broker.HashToken(inputPassword), Times.Once);

            this.tokenBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
