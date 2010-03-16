using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    /// Represents a type that can add a prolog and epilog instructions to a particular method body.
    /// </summary>
    public interface ISurroundMethodBody
    {
        /// <summary>
        /// Adds a prolog to the given method body.
        /// </summary>
        /// <param name="IL">The <see cref="CilWorker"/> that points to the given method body.</param>
        void AddProlog(CilWorker IL);

        /// <summary>
        /// Adds an epilog to the given method body.
        /// </summary>
        /// <param name="IL">The <see cref="CilWorker"/> that points to the given method body.</param>
        void AddEpilog(CilWorker IL);
    }
}