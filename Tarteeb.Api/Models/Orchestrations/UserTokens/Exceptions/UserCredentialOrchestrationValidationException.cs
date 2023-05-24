//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using Xeptions;

namespace Tarteeb.Api.Models.Orchestrations.UserTokens.Exceptions
{
    public class UserCredentialOrchestrationValidationException : Xeption
    {
        public UserCredentialOrchestrationValidationException(Xeption innerException)
        : base(message: "User validation error occurred, fix the errors and try again.", innerException)
        { }
    }
}
