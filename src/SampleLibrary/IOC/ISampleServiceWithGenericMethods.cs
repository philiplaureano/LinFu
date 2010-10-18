namespace SampleLibrary.IOC
{
    public interface ISampleServiceWithGenericMethods
    {
        void SomeMethod<T1>();
        void SomeMethod<T1, T2>();
        void SomeMethod<T1, T2, T3>();
        void SomeMethod<T1, T2, T3, T4>();
    }
}