using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.MxClone.Interfaces
{
    public interface IMxEngine
    {
        IMxInterpreter Interpreter { get; set; }
        void Execute(string mxFilename, IInstanceHolder holder);
    }
}
