using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace OpenUO.Core.PresentationFramework.Input
{
    public abstract class CommandBase : ICommand, IRaiseCanExecuteChanged
    {
        private List<WeakReference> _canExecuteChangedHandlers;

        private readonly Func<object, bool> _canExecuteMethod;
        private readonly Action<object> _executeMethod;

        public event EventHandler CanExecuteChanged
        {
            add { WeakReferenceManager.AddWeakReferenceHandler(ref _canExecuteChangedHandlers, value, 2); }
            remove { WeakReferenceManager.RemoveWeakReferenceHandler(_canExecuteChangedHandlers, value); }
        }

        protected CommandBase(Action<object> executeMethod, Func<object, bool> canExecuteMethod)
        {
            Guard.AssertIsNotNull(executeMethod, "executeMethod");
            Guard.AssertIsNotNull(canExecuteMethod, "canExecuteMethod");

            _executeMethod = executeMethod;
            _canExecuteMethod = canExecuteMethod;
        }

        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged();
        }
        
        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute(parameter);
        }

        void ICommand.Execute(object parameter)
        {
            Execute(parameter);
        }
        
        public bool CanExecute(object parameter)
        {
            return ((_canExecuteMethod == null) ? true : _canExecuteMethod(parameter));
        }

        public void Execute(object parameter)
        {
            _executeMethod(parameter);
        }

        protected virtual void OnCanExecuteChanged()
        {
            WeakReferenceManager.CallWeakReferenceHandlers(this, _canExecuteChangedHandlers);
        }
    }
}
