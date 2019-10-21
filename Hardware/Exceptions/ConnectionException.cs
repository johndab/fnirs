using System;
using System.Collections.Generic;
using System.Text;

namespace fNIRS.Hardware.Exceptions
{
    public class ConnectionException : Exception
    {
        public ConnectionException(string message) : base(message) {}
    }
}
