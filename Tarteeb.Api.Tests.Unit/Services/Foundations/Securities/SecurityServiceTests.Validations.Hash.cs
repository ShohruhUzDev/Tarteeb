//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System.Threading.Tasks;
using System;
using FluentAssertions;
using Moq;
using Tarteeb.Api.Models.Foundations.Users;
using Tarteeb.Api.Models.Foundations.Users.Exceptions;
using Xunit;

namespace Tarteeb.Api.Tests.Unit.Services.Foundations
{
    public partial class SecurityServiceTests
    {
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void ShouldThrowValidationExceptionOnHashPasswordIfPasswordIsInvalidAndLogItAsync(
            string invalidString)
        {
            //given
            string invalidPassword = invalidString;

            var invalidUserException = new InvalidUserException();

            invalidUserException.AddData(
                key: nameof(User.Password),
                 "Text is required",
                 "Password is not valid");

            var expectedUserValidationException = new UserValidationException(
                invalidUserException);

            //when
            UserValidationException actualUserValidationException =
                Assert.Throws<UserValidationException>(() => 
                    this.securityService.HashPassword(invalidPassword));

            //then
            actualUserValidationException.Should().BeEquivalentTo(
                expectedUserValidationException);

            this.loggingBrokerMock.Verify(broker =>
              broker.LogError(It.Is(SameExceptionAs(
                 expectedUserValidationException))), Times.Once);

            this.tokenBrokerMock.Verify(broker =>
              broker.HashToken(It.IsAny<string>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.tokenBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(InvalidPassword))]
        public void ShouldThrowValidationExceptionOnOnHashPasswordIfIsNotValidPasswordAndLogItAsync(
               string invalidstring)
        {
            // given
            string randomPassword = invalidstring;
            string invalidPassword = randomPassword;
            var invalidUserException = new InvalidUserException();

            invalidUserException.AddData(
                key: nameof(User.Password),
                values: "Password is not valid");

            UserValidationException exceptedUserValidationException =
                new UserValidationException(invalidUserException);



            // when
            UserValidationException actualUserValidationException =
                 Assert.Throws<UserValidationException>(() =>
                     this.securityService.HashPassword(invalidPassword));

            // then
            actualUserValidationException.Should().BeEquivalentTo(
                exceptedUserValidationException);

            this.loggingBrokerMock.Verify(broker =>
              broker.LogError(It.Is(SameExceptionAs(
                 exceptedUserValidationException))), Times.Once);

            this.tokenBrokerMock.Verify(broker =>
              broker.HashToken(It.IsAny<string>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.tokenBrokerMock.VerifyNoOtherCalls();
        }
    }
}
