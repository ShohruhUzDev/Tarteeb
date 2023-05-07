//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using Xeptions;

namespace Tarteeb.Api.Models.Securites.Exceptions
{

    public class InsecurePasswordException : Xeption
    {
        public InsecurePasswordException()
          : base(message: "Password is not secure.")
        { }

    }
}
