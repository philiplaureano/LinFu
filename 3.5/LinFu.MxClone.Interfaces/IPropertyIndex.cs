using System;
namespace LinFu.MxClone.Interfaces
{
    public interface IPropertyIndex
    {
        Type GetPropertyType(string propertyName);
        bool HasCollectionProperty(string propertyName);
        bool HasProperty(string propertyName);
    }
}
