using System;
using System.Collections.Generic;
using OpenUO.Core.PresentationOpenUO.Core.Threading;

namespace OpenUO.Core.PresentationFramework
{
    public sealed class WeakReferenceManager
    {
        private static int CleanupOldHandlers(List<WeakReference> handlers, EventHandler[] callees)
        {
            var count = 0;

            for(var i = handlers.Count - 1; i >= 0; i--)
            {
                var reference = handlers[i];
                var handler = reference.Target as EventHandler;

                if(handler == null)
                {
                    handlers.RemoveAt(i);
                }
                else
                {
                    callees[count] = handler;
                    count++;
                }
            }

            return count;
        }

        public static void CallWeakReferenceHandlers(object sender, List<WeakReference> handlers)
        {
            if(handlers != null)
            {
                var callees = new EventHandler[handlers.Count];
                var count = CleanupOldHandlers(handlers, callees);

                for(var i = 0; i < count; i++)
                {
                    CallHandler(sender, callees[i]);
                }
            }
        }

        public static void RemoveWeakReferenceHandler(List<WeakReference> handlers, EventHandler handler)
        {
            if(handlers != null)
            {
                for(var i = handlers.Count - 1; i >= 0; i--)
                {
                    var reference = handlers[i];
                    var existingHandler = reference.Target as EventHandler;

                    if((existingHandler == null) || (existingHandler == handler))
                    {
                        handlers.RemoveAt(i);
                    }
                }
            }
        }

        public static void AddWeakReferenceHandler(ref List<WeakReference> handlers, EventHandler handler, int defaultListSize)
        {
            if(handlers == null)
            {
                handlers = (defaultListSize > 0) ? new List<WeakReference>(defaultListSize) : new List<WeakReference>();
            }

            handlers.Add(new WeakReference(handler));
        }

        private static void CallHandler(object sender, EventHandler eventHandler)
        {
            var dispatcher = DispatcherProxy.CreateDispatcher();

            if(eventHandler != null)
            {
                if(!((dispatcher == null) || dispatcher.CheckAccess()))
                {
                    dispatcher.BeginInvoke(new Action<object, EventHandler>(CallHandler), sender, eventHandler);
                }
                else
                {
                    eventHandler(sender, EventArgs.Empty);
                }
            }
        }
    }
}