namespace SampleLibrary
{
    public class SampleClass : ISampleService, ISampleGenericService<int>
    {
        public bool Called
        {
            get; set;
        }

        public void DoSomething()
        {
            throw new System.NotImplementedException();
        }
    }
}