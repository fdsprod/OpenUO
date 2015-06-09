using System;
using System.ComponentModel;

namespace OpenUO.Core.PresentationOpenUO.Core.Data
{
    public abstract class Item : INotifyPropertyChanged, IDisposable
    {
        public Item()
        {
            Disposed = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void NotifyPropertyChanged(string property)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        protected bool Disposed
        {
            get;
            private set;
        }

        protected virtual void Dispose(bool disposing)
        {
            if(!Disposed)
            {
                Disposed = true;
            }
        }

        ~Item()
        {
            Dispose(false);
        }
    }
}