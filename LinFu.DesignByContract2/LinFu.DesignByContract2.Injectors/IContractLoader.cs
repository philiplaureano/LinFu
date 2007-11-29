using System;
using System.Collections.Generic;
using System.Text;

namespace LinFu.DesignByContract2.Injectors
{
    public interface IContractLoader
    {
        void LoadDirectory(string directory, string fileSpec);
    }
}
