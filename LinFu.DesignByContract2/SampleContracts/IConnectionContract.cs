using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using LinFu.DesignByContract2.Injectors;
using LinFu.DesignByContract2.Tests;

namespace SampleContracts
{
    [ContractFor(typeof(IDbConnection))]
    public interface IConnectionContract
    {
        [ConnectionStringNotEmpty] void Open();
    }
}
