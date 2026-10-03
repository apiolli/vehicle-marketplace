using System;

namespace SharedKernel.Exceptions;

    public class ConflictException : AppException
    {
        public override int StatusCode => 409;
        public ConflictException(string message) : base(message) { }
    }
