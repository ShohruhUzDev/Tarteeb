//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using FluentAssertions;
using Moq;
using Tarteeb.Api.Models.Foundations.Users;
using Tarteeb.Api.Models.Foundations.Users.Exceptions;
using Tarteeb.Api.Models.Securites.Exceptions;
using Xunit;

namespace Tarteeb.Api.Tests.Unit.Services.Foundations
{

    public partial class SecurityServiceTests
    {
        [Fact]
        public void ShouldThrowValidationExceptionOnCreateJWTIfInputIsNullAndLogItAsync()
        {
            //given
            User noUser = null;
            var nullUserException = new NullUserException();

            var expectedUserValidationException =
                new UserValidationException(nullUserException);

            //when
            UserValidationException actualUserValidationException =
                Assert.Throws<UserValidationException>(() => this.securityService.CreateToken(noUser));

            //then
            actualUserValidationException.Should().BeEquivalentTo(
                expectedUserValidationException);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogError(It.Is(SameExceptionAs(
                  expectedUserValidationException))), Times.Once);

            this.tokenBrokerMock.Verify(broker =>
                broker.GenerateJWT(It.IsAny<User>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.tokenBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ShouldThrowValidationExceptionOnCreateJWTIfUserIsInvalidAndLogItAsync(
            string invalidString)
        {
            //given
            var invalidUser = new User
            {
                Email = invalidString
            };

            var insecurePasswordException = new InsecurePasswordException();

            insecurePasswordException.AddData(
                key: nameof(User.Id),
                values: "Id is required");

            insecurePasswordException.AddData(
                key: nameof(User.Email),
                values: "Text is required");

            var expectedUserValidationException = new UserValidationException(
                insecurePasswordException);

            //when
            UserValidationException actualUserValidationException =
                Assert.Throws<UserValidationException>(() => this.securityService.CreateToken(invalidUser));

            //then
            actualUserValidationException.Should().BeEquivalentTo(
                expectedUserValidationException);

            this.loggingBrokerMock.Verify(broker =>
              broker.LogError(It.Is(SameExceptionAs(
                 expectedUserValidationException))), Times.Once);

            this.tokenBrokerMock.Verify(broker =>
              broker.GenerateJWT(It.IsAny<User>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.tokenBrokerMock.VerifyNoOtherCalls();
        }
    }
}
