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
            string hashedPassword = GetRandomString();
            User randomUser = CreateRandomUser();
            randomUser.Password = hashedPassword;
            User inputUser = randomUser;
            User persistedUser = inputUser;
            User expectedUser = persistedUser.DeepClone();
            Email emailToSend = CreateUserEmail();
            Email deliveredEmail = emailToSend.DeepClone();

            this.securityServiceMock.Setup(service =>
                service.HashPassword(inputUser.Password))
                    .Returns(hashedPassword);

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

            this.securityServiceMock.Verify(service =>
                service.HashPassword(inputUser.Password), Times.Once());

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
