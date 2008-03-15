namespace LinFu.DesignByContract2.Core
{
    public interface IErrorView
    {
        void ShowInvariantError(string message);
        void ShowPreconditionError(string message);
        void ShowPostconditionError(string message);
    }
}