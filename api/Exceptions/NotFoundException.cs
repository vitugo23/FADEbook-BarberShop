using System;

namespace Fadebook.Exceptions;
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }