using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Interfaces;
using System.Runtime.Serialization;

namespace SampleLibrary.Proxy
{
    [Serializable]
    public class SerializableInterceptor : MarshalByRefObject, IInterceptor, ISerializable
    {
        public SerializableInterceptor()
        {
        }

        public SerializableInterceptor(SerializationInfo info, StreamingContext context)
        {
            Identifier = (Guid)info.GetValue("identifier", typeof(Guid));
        }

        public Guid Identifier
        {
            get;
            set;
        }

        public object Intercept(IInvocationInfo info)
        {
            return null;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("identifier", Identifier);
        }
    }
}
