using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using LinFu.MxClone.Interfaces;
using LinFu.Reflection.Extensions;
namespace LinFu.MxClone.Indexes
{
    public class EventIndex : IEventIndex
    {
        private Dictionary<string, EventInfo> _events = new Dictionary<string, EventInfo>();
        private Type _targetType;
        public EventIndex(Type targetType)
        {
            _targetType = targetType;


            CreateIndexes(targetType);
        }
        public bool HasEvent(string eventName)
        {
            return _events.ContainsKey(eventName);
        }
        private void CreateIndexes(Type targetType)
        {
            var events = targetType.GetEvents();
            events.ForEach(e => _events[e.Name] = e);
        }

    }
}
