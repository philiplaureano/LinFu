using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.MxClone.Interfaces;
using LinFu.Reflection;

namespace LinFu.MxClone.Actions
{
    public class AddCollectionItem : IAction
    {
        private string _propertyName;
        private IInstance _host;
        private IInstance _item;
        public AddCollectionItem(string collectionPropertyName, IInstance collectionHost,
            IInstance collectionItem)
        {
            _propertyName = collectionPropertyName;
            _host = collectionHost;
            _item = collectionItem;
        }
        #region IAction Members

        public void Execute()
        {
            if (_host == null || _item == null)
                return;

            object host = _host.Evaluate();
            object item = _item.Evaluate();

            if (host == null || item == null)
                return;

            DynamicObject dynamic = new DynamicObject(host);

            // Get the collection property
            object collection = dynamic.Properties[_propertyName];

            // Add the item to the collection
            dynamic.Target = collection;
            dynamic.Methods["Add"](item);
        }

        #endregion
    }
}
