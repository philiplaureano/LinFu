using LinFu.DynamicProxy;

namespace LinFu.DesignByContract2.Core
{
    public interface IContractChecker : IInvokeWrapper
    {
        IErrorView ErrorView { get; set; }
        IContractProvider ContractProvider { get; set; }
        object Target { get; set; }
    }
}