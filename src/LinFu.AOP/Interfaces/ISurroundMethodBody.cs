using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil
{
    public interface ISurroundMethodBody
    {
        void AddProlog(CilWorker IL);
        void AddEpilog(CilWorker IL);
    }
}