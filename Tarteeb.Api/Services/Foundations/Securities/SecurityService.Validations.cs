//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using System.Text.RegularExpressions;
using Tarteeb.Api.Models.Foundations.Users;
using Tarteeb.Api.Models.Foundations.Users.Exceptions;

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
                (Rule: IsInvalid(password), Parameter: nameof(User.Password)),
                (Rule: IsInvalidPassword(password), Parameter: nameof(User.Password)));
        }

        private bool IsInvalidPasswordRule(string password)
        {
            string pattern = "^(?=(.*[a-z]){1,})(?=(.*[A-Z]){1,})(?=(.*[0-9]){1,})(?=(.*[!@#$%^&*()\\-__+.]){1,}).{8,}$";
            var regex = new Regex(pattern);
           
            return !regex.IsMatch(password);
        }

        private dynamic IsInvalidPassword(string password) => new
        {
            Condition = IsInvalidPasswordRule(password),
            Message = "Password is not valid"
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
            var invalidUserException = new InvalidUserException();

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
