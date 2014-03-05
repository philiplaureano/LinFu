using System;
using System.Runtime.Serialization;
using LinFu.AOP.Interfaces;

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
            Identifier = (Guid) info.GetValue("identifier", typeof (Guid));
        }

        public Guid Identifier { get; set; }


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