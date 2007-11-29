using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using LinFu.DesignByContract2.Core;

namespace LinFu.DesignByContract2.Attributes
{
    public class AttributeExtractor
    {
        public virtual void ExtractAttributesFrom<T>(ICustomAttributeProvider attributeProvider, IList<T> targetList)
            where T : class
        {
            if (attributeProvider == null)
                return;
            
            object[] attributes = attributeProvider.GetCustomAttributes(typeof(T), false);
            foreach (Attribute current in attributes)
            {
                if (current == null)
                    continue;

                // Convert the current type to the target method check type
                T currentCheck = current as T;
                if (currentCheck == null)
                    continue;

                targetList.Add(currentCheck);
            }
        }
    }
}
