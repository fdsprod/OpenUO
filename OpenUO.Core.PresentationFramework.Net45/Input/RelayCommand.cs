using System;

namespace OpenUO.Core.PresentationOpenUO.Core.Input
{
    public class RelayCommand<T> : CommandBase //where T : class
    {
        public RelayCommand(Action<T> executeMethod)
            : this(executeMethod, o => true)
        {
        }

        public RelayCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
            : base(o => executeMethod((T)o), o => canExecuteMethod((T)o))
        {
            Guard.RequireIsNotNull(executeMethod, "executeMethod");
            Guard.RequireIsNotNull(canExecuteMethod, "canExecuteMethod");
        }

        public bool CanExecute(T parameter)
        {
            return base.CanExecute(parameter);
        }

        public void Execute(T parameter)
        {
            base.Execute(parameter);
        }
    }

    public class RelayCommand : CommandBase
    {
        public RelayCommand(Action executeMethod)
            : this(executeMethod, () => true)
        {
        }

        public RelayCommand(Action executeMethod, Func<bool> canExecuteMethod)
            : base(o => executeMethod(), o => canExecuteMethod())
        {
            Guard.RequireIsNotNull(executeMethod, "executeMethod");
            Guard.RequireIsNotNull(canExecuteMethod, "canExecuteMethod");
        }

        public bool CanExecute()
        {
            return base.CanExecute(null);
        }

        public void Execute()
        {
            base.Execute(null);
        }
    }
}