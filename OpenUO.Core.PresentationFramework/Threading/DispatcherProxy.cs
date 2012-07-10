using System;
using System.Windows;
using System.Windows.Threading;

namespace OpenUO.Core.PresentationFramework.Threading
{
    public sealed class DispatcherProxy
    {
        private Dispatcher _dispatcher;

        private DispatcherProxy(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public DispatcherOperation BeginInvoke(Delegate method, params object[] args)
        {
            return _dispatcher.BeginInvoke(method, DispatcherPriority.Normal, args);
        }

        public bool CheckAccess()
        {
            return _dispatcher.CheckAccess();
        }

        public static DispatcherProxy CreateDispatcher()
        {
            if (Application.Current == null)
                return null;

            return new DispatcherProxy(Application.Current.Dispatcher);
        }

        public void CallHandler(object sender, EventHandler handler)
        {
            if (handler != null)
            {
                if (CheckAccess())
                    BeginInvoke(new Action<object, EventHandler>(CallHandler), new object[] { sender, handler });
                else
                    handler(sender, EventArgs.Empty);
            }
        }
    }

 

}
