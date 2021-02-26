using System;
using System.Runtime.Serialization;

namespace SHC
{
    [Serializable]
    class SHCNotFoundException : Exception
    {
        public SHCNotFoundException() : base()
        {
        }

        public SHCNotFoundException(string message) : base(message)
        {
        }

        public SHCNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SHCNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
