//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using Tarteeb.Api.Models.Foundations.Users;
using Tarteeb.Api.Models.Foundations.Users.Exceptions;
using Tarteeb.Api.Models.Securites.Exceptions;

namespace Tarteeb.Api.Services.Foundations.Securities
{
    public partial class SecurityService
    {
        private void ValidateUser(User user)
        {
            ValidateUserNotNull(user);

            Validate(
                (Rule: IsInvalid(user.Id), Parameter: nameof(User.Id)),
                (Rule: IsInvalid(user.Email), Parameter: nameof(User.Email)));
        }

        private void ValidatePassword(string password)
        {
            Validate(
                (Rule: IsInvalid(password), Parameter: "Password"),
                (Rule: IsLessThan8Chars(password), Parameter: "LengthComplexity"),
                (Rule: HasNoUppercase(password), Parameter: "UpperCaseComplexity"),
                (Rule: HasNoSymbol(password), Parameter: "SymbolComplexity"),
                (Rule: HasNoDigit(password), Parameter: "DigitComplexity"));
        }

        private bool HasNoSymbolRule(string password)
        {
            foreach (char c in password)
            {
                if (!(char.IsLetterOrDigit(c) || c == ' '))
                {
                    return false;
                }
            }

            return true;
        }

        private dynamic HasNoSymbol(string password) => new
        {
            Condition = HasNoSymbolRule(password),
            Message = "At least one symbol is required",
        };

        private bool HasNoDigitRule(string password)
        {
            foreach (char p in password)
            {
                if (char.IsDigit(p))
                {
                    return false;
                }
            }

            return true;
        }

        private dynamic HasNoDigit(string password) => new
        {
            Condition = HasNoDigitRule(password),
            Message = "At least one digit is required"
        };

        private bool HasNoUppercaseRule(string password)
        {
            foreach (char p in password)
            {
                if (char.IsUpper(p))
                {
                    return false;
                }
            }

            return true;
        }

        private dynamic HasNoUppercase(string password) => new
        {
            Condition = HasNoUppercaseRule(password),
            Message = "At least one capital letter is required"
        };

        private bool IsLessThan8CharsRule(string password) =>
            password.Length < 8;

        private dynamic IsLessThan8Chars(string password) => new
        {
            Condition = IsLessThan8CharsRule(password),
            Message = "At least 8 characters required"
        };

        private static dynamic IsInvalid(string text) => new
        {
            Condition = string.IsNullOrWhiteSpace(text),
            Message = "Text is required"
        };

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == default,
            Message = "Id is required"
        };

        private static void ValidateUserNotNull(User user)
        {
            if (user is null)
            {
                throw new NullUserException();
            }
        }

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidUserException = new InsecurePasswordException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidUserException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }
            invalidUserException.ThrowIfContainsErrors();
        }
    }
}
