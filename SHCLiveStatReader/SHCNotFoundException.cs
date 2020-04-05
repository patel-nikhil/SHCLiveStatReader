using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
