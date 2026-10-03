using System;

namespace SharedKernel.Exceptions;

    public class UnauthorizedException : AppException
    {
        public override int StatusCode => 401;
        public UnauthorizedException(string message) : base(message) { }
    }
