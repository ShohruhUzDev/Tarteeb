//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Microsoft.Data.SqlClient;
using Moq;
using PasswordGenerator;
using Tarteeb.Api.Brokers.DateTimes;
using Tarteeb.Api.Brokers.Loggings;
using Tarteeb.Api.Brokers.Storages;
using Tarteeb.Api.Models.Foundations.Users;
using Tarteeb.Api.Services.Foundations.Users;
using Tynamix.ObjectFiller;
using Xeptions;
using Xunit;

namespace Tarteeb.Api.Tests.Unit.Services.Foundations.Users
{
    public partial class UserServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IUserService userService;

        public UserServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.userService = new UserService(
                storageBroker: this.storageBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static IQueryable<User> CreateRandomUsers()
        {
            return CreateUserFiller(dates: GetRandomDateTimeOffset())
                .Create(count: GetRandomNumber())
                    .AsQueryable();
        }

        public static TheoryData<int> InvalidSeconds()
        {
            int secondsInPast = -1 * new IntRange(
                min: 60,
                max: short.MaxValue).GetValue();

            int secondsInFuture = new IntRange(
                min: 0,
                max: short.MaxValue).GetValue();

            return new TheoryData<int>
            {
                secondsInPast,
                secondsInFuture
            };
        }

        public static string GetRandomEmail()
        {
            var chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var domain = new String(stringChars);
            var email = domain + "@" + domain + ".com";

            return email;
        }
        public static string GetInvalidEmail()
        {
            var chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var invalidEmail = new String(stringChars) + ".com";

            return invalidEmail;
        }

        public static TheoryData<string> InvalidEmail()
        {
            return new TheoryData<string>
            {
               GetInvalidEmail()
            };
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

        public static string GetInvalidPassword()
        {
            var invalidPassword = new Password()
                .IncludeLowercase().IncludeSpecial().IncludeNumeric();

            return invalidPassword.Next();
        }

        private static string GetRandomPassword()
        {
            var password = new Password()
                .IncludeLowercase().IncludeUppercase().IncludeSpecial().IncludeNumeric();

            return password.Next();
        }

        private static User CreateRandomUser() =>
             CreateUserFiller(dates: GetRandomDateTimeOffset()).Create();

        private static User CreateRandomUser(DateTimeOffset dates) =>
            CreateUserFiller(dates).Create();

        private static int GetRandomNegativeNumber() =>
            -1 * new IntRange(min: 2, max: 10).GetValue();

        private static User CreateRandomModifyUser(DateTimeOffset dates)
        {
            int randomDaysInPast = GetRandomNegativeNumber();
            User randomUser = CreateRandomUser(dates);

            randomUser.CreatedDate =
                randomUser.CreatedDate.AddDays(randomDaysInPast);

            return randomUser;
        }

        private static SqlException GetSqlException() =>
           (SqlException)FormatterServices.GetUninitializedObject(typeof(SqlException));

        private Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static SqlException CreateSqlException() =>
            (SqlException)FormatterServices.GetUninitializedObject(typeof(SqlException));

        private static string GetRandomString() =>
             new MnemonicString().GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static string GetRandomMessage() =>
           new MnemonicString(wordCount: GetRandomNumber()).GetValue();

        private static int GetRandomNumber() =>
           new IntRange(min: 2, max: 10).GetValue();

        public static TheoryData MinutsBeforeOrAfter()
        {
            int randomNumber = GetRandomNumber();
            int randomNegativeNumber = GetRandomNegativeNumber();

            return new TheoryData<int>
            {
                randomNumber,
                randomNegativeNumber
            };
        }

        private static Filler<User> CreateUserFiller(DateTimeOffset dates)
        {
            var filler = new Filler<User>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dates)
                .OnProperty(user => user.Password).Use(GetRandomPassword())
                 .OnProperty(user => user.Email).Use(GetRandomEmail());

            return filler;
        }
    }
}