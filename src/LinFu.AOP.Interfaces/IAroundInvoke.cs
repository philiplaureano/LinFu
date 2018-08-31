namespace LinFu.AOP.Interfaces
{
    /// <summary>
    ///     Represents a class that can wrap itself around any given method call.
    /// </summary>
    public interface IAroundInvoke : IBeforeInvoke, IAfterInvoke
    {
    }
}