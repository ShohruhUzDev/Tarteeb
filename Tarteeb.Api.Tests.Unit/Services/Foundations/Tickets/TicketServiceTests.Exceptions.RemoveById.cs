﻿//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Threading.Tasks;
using Tarteeb.Api.Models.Tickets;
using Tarteeb.Api.Models.Tickets.Exceptions;
using Xunit;

namespace Tarteeb.Api.Tests.Unit.Services.Foundations.Tickets
{
    public partial class TicketServiceTests
    {
        [Fact]
        public async Task ShouldThrowDependencyValidationOnRemoveIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            Guid someTicketId = Guid.NewGuid();

            var databaseUpdateConcurrencyException =
                new DbUpdateConcurrencyException();

            var lockedTicketException =
                new LockedTicketException(databaseUpdateConcurrencyException);

            var expectedTicketDependencyValidationException =
                new TicketDependencyValidationException(lockedTicketException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectTicketByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(databaseUpdateConcurrencyException);

            // when
            ValueTask<Ticket> removeTicketByIdTask =
                this.ticketService.RemoveTicketByIdAsync(someTicketId);

            TicketDependencyValidationException actualTicketDependencyValidationException =
                await Assert.ThrowsAsync<TicketDependencyValidationException>(
                    removeTicketByIdTask.AsTask);

            // then
            actualTicketDependencyValidationException.Should().BeEquivalentTo(
                expectedTicketDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTicketByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedTicketDependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteTicketAsync(It.IsAny<Ticket>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}