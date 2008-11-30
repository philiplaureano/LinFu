using LinFu.IoC.Configuration;
using LinFu.IoC;
using LinFu.IoC.Interfaces;

namespace SampleLibrary
{
    [PostProcessor]
    public class SamplePostProcessor : IPostProcessor
    {
        #region IPostProcessor Members

        public void PostProcess(IServiceRequestResult result)
        {
            // Do nothing
        }

        #endregion
    }
}