using System;

namespace SharedKernel.Exceptions;

    public class ForbiddenException : AppException
    {
        public override int StatusCode => 403;
        public ForbiddenException(string message) : base(message) { }
    }
