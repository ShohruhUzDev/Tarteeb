//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Tarteeb.Api.Models.Foundations.Emails;
using Tarteeb.Api.Models.Foundations.Users;
using Tarteeb.Api.Models.Orchestrations.UserTokens.Exceptions;
using Xunit;

namespace Tarteeb.Api.Tests.Unit.Services.Orchestrations
{
    public partial class UserSecurityOrchestrationServiceTests
    {
        [Fact]
        public async void ShouldThrowValidationExceptionOnCreateAccountIfUserOrRequestUrlAreInvalidAndLogItAsync()
        {
            //given
            User invalidUser = null;
            string invalidRequestUrl = string.Empty;
            var invalidUserCreadentialOrchestrationException = new InvalidUserCredentialOrchestrationException();

            invalidUserCreadentialOrchestrationException.AddData(
                key: nameof(User),
                values: "User is required");

            invalidUserCreadentialOrchestrationException.AddData(
                key: "RequestUrl",
                values: "Value is required");

            var expectedUserOrchestrationValidationException =
                new UserCredentialOrchestrationValidationException(invalidUserCreadentialOrchestrationException);

            // when
            ValueTask<User> createUserAccountTask =
                this.userSecurityOrchestrationService.CreateUserAccountAsync(invalidUser, invalidRequestUrl);

            UserCredentialOrchestrationValidationException actualUserCredentialOrchestrationValidationException =
               await Assert.ThrowsAsync<UserCredentialOrchestrationValidationException>(createUserAccountTask.AsTask);

            //then
            actualUserCredentialOrchestrationValidationException.Should().BeEquivalentTo(
               expectedUserOrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserOrchestrationValidationException))), Times.Once);

            this.userServiceMock.Verify(service =>
                service.AddUserAsync(invalidUser), Times.Never);

            this.securityServiceMock.Verify(service =>
                service.HashPassword(It.IsAny<string>()), Times.Never);

            this.emailServiceMock.Verify(service =>
                service.SendEmailAsync(It.IsAny<Email>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.emailServiceMock.VerifyNoOtherCalls();
            this.securityServiceMock.VerifyNoOtherCalls();
        }
    }
}
