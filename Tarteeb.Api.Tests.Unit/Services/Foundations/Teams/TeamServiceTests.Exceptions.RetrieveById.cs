﻿//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using Microsoft.Data.SqlClient;
using Moq;
using System.Threading.Tasks;
using System;
using Xunit;
using Tarteeb.Api.Models.Teams.Exceptions;
using Tarteeb.Api.Models.Teams;
using FluentAssertions;

namespace Tarteeb.Api.Tests.Unit.Services.Foundations.Teams
{
    public partial class TeamServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            SqlException sqlException = CreateSqlException();

            var failedTeamStorageException =
                new FailedTeamStorageException(sqlException);

            var expectedTeamDependencyException =
                new TeamDependencyException(failedTeamStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectTeamByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Team> retrieveTeamByIdTask =
                this.teamService.RetrieveTeamByIdAsync(someId);

            TeamDependencyException actaulTeamDependencyException =
                await Assert.ThrowsAsync<TeamDependencyException>(
                    retrieveTeamByIdTask.AsTask);

            // then
            actaulTeamDependencyException.Should().BeEquivalentTo(
                expectedTeamDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTeamByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedTeamDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}