using System;

namespace NitroNet.Sitecore.Exceptions
{
    public class NitroNetSitecoreArgumentException : ArgumentException
    {
        public NitroNetSitecoreArgumentException()
        {
        }

        public NitroNetSitecoreArgumentException(string message)
            : base(message)
        {
        }

        public NitroNetSitecoreArgumentException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
