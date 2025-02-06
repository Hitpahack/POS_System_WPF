using System;

namespace FalcaPOS.Common.Helper
{
    public class PosException : Exception
    {
        public PosException(string message) : base(message)
        {
        }
    }
}
