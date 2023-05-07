//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using FluentAssertions;
using Moq;
using Tarteeb.Api.Models.Foundations.Users.Exceptions;
using Tarteeb.Api.Models.Securites.Exceptions;
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

            var invalidPasswordException = new InsecurePasswordException();

            invalidPasswordException.AddData(
                key: "Password",
                values: "Text is required");

            invalidPasswordException.AddData(
                key: "LengthComplexity",
                values: "At least 8 characters required");

            invalidPasswordException.AddData(
                key: "UpperCaseComplexity",
                values: "At least one capital letter is required");

            invalidPasswordException.AddData(
                key: "SymbolComplexity",
                values: "At least one symbol is required");

            invalidPasswordException.AddData(
                key: "DigitComplexity",
                values: "At least one digit is required");

            var expectedUserValidationException = new UserValidationException(
                invalidPasswordException);

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
            var invalidPasswordException = new InsecurePasswordException();

            invalidPasswordException.AddData(
                key: "LengthComplexity",
                values: "At least 8 characters required");

            invalidPasswordException.AddData(
                key: "UpperCaseComplexity",
                values: "At least one capital letter is required");

            invalidPasswordException.AddData(
                key: "SymbolComplexity",
                values: "At least one symbol is required");

            invalidPasswordException.AddData(
                key: "DigitComplexity",
                values: "At least one digit is required");

            UserValidationException exceptedUserValidationException =
                new UserValidationException(invalidPasswordException);

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
