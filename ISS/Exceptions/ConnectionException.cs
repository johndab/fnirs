using System;
using System.Collections.Generic;
using System.Text;

namespace MiBrain.ISS.Exceptions
{
    public class ConnectionException : Exception
    {
        public ConnectionException(string message) : base(message) {}
    }
}
