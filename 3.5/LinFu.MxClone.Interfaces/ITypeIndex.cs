using System;
namespace LinFu.MxClone.Interfaces
{
    public interface ITypeIndex
    {
        Type Resolve(string typename);
    }
}
