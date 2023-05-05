//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Tarteeb.Api.Models.Foundations.Emails;
using Tarteeb.Api.Models.Foundations.Users;
using Xunit;

namespace Tarteeb.Api.Tests.Unit.Services.Orchestrations
{
    public partial class UserSecurityOrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldCreateUserAccountAsync()
        {
            // given
            string randomUrl = CreateRandomUrl();
            string requestUrl = randomUrl;
            User randomUser = CreateRandomUser();
            User inputUser = randomUser;
            User persistedUser = inputUser;
            User expectedUser = persistedUser.DeepClone();
            Email emailToSend = CreateUserEmail();
            Email deliveredEmail = emailToSend.DeepClone();

            this.userServiceMock.Setup(service =>
                service.AddUserAsync(inputUser))
                    .ReturnsAsync(persistedUser);

            this.emailServiceMock.Setup(service =>
                service.SendEmailAsync(emailToSend))
                    .ReturnsAsync(deliveredEmail);

            // when
            User actualUser = await this.userSecurityOrchestrationService
                .CreateUserAccountAsync(inputUser, requestUrl);

            // then
            actualUser.Should().BeEquivalentTo(expectedUser);

            this.userServiceMock.Verify(service =>
                service.AddUserAsync(inputUser), Times.Once);

            this.emailServiceMock.Verify(service =>
                 service.SendEmailAsync(It.IsAny<Email>()), Times.Once);

            this.userServiceMock.VerifyNoOtherCalls();
            this.emailServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
