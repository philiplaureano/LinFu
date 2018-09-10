namespace LinFu.AOP.Interfaces
{
    /// <summary>
    /// Represents the simplest possible implementation of the visitor pattern.
    /// </summary>
    public interface IVisitable
    {
        /// <summary>
        /// Accepts a visitor and does a walk through of the
        /// current visitor heirarchy
        /// </summary>
        /// <param name="visitor">the visitor instance</param>
        void Accept(object visitor);
    }
}