using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;

namespace SampleLibrary
{
    [PostProcessor]
    public class SamplePostProcessor : IPostProcessor
    {
        public void PostProcess(IServiceRequestResult result)
        {
            // Do nothing
        }
    }
}