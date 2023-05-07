//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Moq;
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
        const int minLength = 8;
        const string uppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lowercaseChars = "abcdefghijklmnopqrstuvwxyz";
        const string digitChars = "0123456789";
        const string specialChars = "!@#$%^&*()";
        const string allChars = uppercaseChars + lowercaseChars + digitChars + specialChars;

        StringBuilder res = new StringBuilder();
        Random rnd = new Random();

        int length = rnd.Next(minLength, minLength * 2);
        res.Append(uppercaseChars[rnd.Next(uppercaseChars.Length)]);
        res.Append(lowercaseChars[rnd.Next(lowercaseChars.Length)]);
        res.Append(digitChars[rnd.Next(digitChars.Length)]);
        res.Append(specialChars[rnd.Next(specialChars.Length)]);

        for (int i = 4; i < length; i++)
            res.Append(allChars[rnd.Next(allChars.Length)]);

        return new string(res.ToString().ToCharArray().OrderBy(x => rnd.Next()).ToArray());
    }

    public static string GetInvalidRandomPassword()
    {
        const int minLength = 2;
        const string allChars = "abcdefghijklmnopqrstuvwxyz";
        StringBuilder res = new StringBuilder();
        Random rnd = new Random();
        int length = rnd.Next(minLength, minLength * 2);

        for (int i = 0; i < length; i++)
            res.Append(allChars[rnd.Next(allChars.Length)]);

        return res.ToString();
    }

    public static TheoryData<string> InvalidPassword()
    {
        return new TheoryData<string>
            {
                GetInvalidRandomPassword()
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
