//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using Microsoft.Extensions.Logging;
using Tarteeb.Api.Services.Extensions;

namespace Tarteeb.Api.Brokers.Loggings
{
    public class LoggingBroker : ILoggingBroker
    {
        private ILogger<LoggingBroker> logger;

        public LoggingBroker(ILogger<LoggingBroker> logger) =>
            this.logger = logger;

        public void LogError(Exception exception) =>
           this.logger.LogError(exception, $"{exception.Message} {exception.GetValidationSummary()}");

        public void LogCritical(Exception exception) =>
            this.logger.LogCritical(exception, $"{exception.Message} {exception.GetValidationSummary()}");
    }
}
