using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LinFu.Delegates
{
    public static class EventBinder
    {
        public static MulticastDelegate BindToEvent(string eventName, object source, CustomDelegate body)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            
            // Find the matching event defined on that type
            Type sourceType = source.GetType();
            EventInfo targetEvent = sourceType.GetEvent(eventName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

            if (targetEvent == null)
                throw new ArgumentException(
                    string.Format("Event '{0}' was not found on source type '{1}'", eventName, sourceType.FullName));

            // When the event fires, redirect call from the event
            // handler to the CustomDelegate body
            Type delegateType = targetEvent.EventHandlerType;
            MulticastDelegate result = DelegateFactory.DefineDelegate(delegateType, body);

            targetEvent.AddEventHandler(source, result);
            return result;
        }
        public static void UnbindFromEvent(string eventName, object source, MulticastDelegate target)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            Type sourceType = source.GetType();
            EventInfo targetEvent = sourceType.GetEvent(eventName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

            if (targetEvent == null)
                throw new ArgumentException(
                string.Format("Event '{0}' was not found on source type '{1}'", eventName, sourceType.FullName));

            Type delegateType = targetEvent.EventHandlerType;
            targetEvent.RemoveEventHandler(source, target);
        }

    }
}
