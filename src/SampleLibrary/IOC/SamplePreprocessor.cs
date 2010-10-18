using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;

namespace SampleLibrary.IOC
{
    [Preprocessor]
    public class SamplePreprocessor : IPreProcessor
    {
        #region IPreProcessor Members

        public void Preprocess(IServiceRequest result)
        {
            // Do nothing
        }

        #endregion
    }
}