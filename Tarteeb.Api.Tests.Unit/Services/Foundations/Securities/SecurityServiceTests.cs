﻿//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using System.Linq.Expressions;
using Moq;
using PasswordGenerator;
using Tarteeb.Api.Brokers.Loggings;
using Tarteeb.Api.Brokers.Tokens;
using Tarteeb.Api.Models.Foundations.Users;
using Tarteeb.Api.Services.Foundations.Securities;
using Tynamix.ObjectFiller;
using Xeptions;
using Xunit;

namespace Tarteeb.Api.Tests.Unit.Services.Foundations;

public partial class SecurityServiceTests
{
    private readonly Mock<ITokenBroker> tokenBrokerMock;
    private readonly Mock<ILoggingBroker> loggingBrokerMock;
    private readonly ISecurityService securityService;

    public SecurityServiceTests()
    {
        this.tokenBrokerMock = new Mock<ITokenBroker>();
        this.loggingBrokerMock = new Mock<ILoggingBroker>();

        this.securityService = new SecurityService(
            tokenBrokerMock.Object,
            loggingBrokerMock.Object);
    }

    private static string GetRandomPassword()
    {
        var password = new Password()
            .IncludeLowercase().IncludeUppercase().IncludeSpecial().IncludeNumeric();

        return password.Next();
    }

    public static TheoryData<string> InvalidPassword()
    {
        var invalidPassword = new Password()
            .IncludeLowercase().IncludeSpecial().IncludeNumeric();

        return new TheoryData<string>
            {
                invalidPassword.Next()
            };
    }

    private Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
        actualException => actualException.SameExceptionAs(expectedException);

    private static string CreateRandomString() =>
        new MnemonicString().GetValue();

    private static User CreateRandomUser() =>
        CreateUserFiller(dates: GetRandomDateTimeOffset()).Create();

    private static DateTimeOffset GetRandomDateTimeOffset() =>
        new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

    private static Filler<User> CreateUserFiller(DateTimeOffset dates)
    {
        var filler = new Filler<User>();

        filler.Setup()
            .OnType<DateTimeOffset>().Use(dates)
            .OnProperty(user => user.Password).Use(GetRandomPassword());

        return filler;
    }
}
