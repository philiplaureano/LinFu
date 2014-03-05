using System;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;

namespace SampleLibrary.IOC.BugFixes
{
    [Factory(typeof (MyClass<string>))]
    public class MyFactory :
        //IFactory<MyClass<string>>
        IFactory
    {
        public object CreateInstance(IFactoryRequest request)
        {
            if (string.IsNullOrEmpty(request.ServiceName))
                throw new ArgumentNullException("ServiceName");

            var myClass = new MyClass<string>();
            myClass.Value = request.ServiceName;

            return myClass;
        }


        //MyClass<string> IFactory<MyClass<string>>.CreateInstance(IFactoryRequest request)
        //{
        //    return (MyClass<string>)CreateInstance(request);
        //}
    }
}