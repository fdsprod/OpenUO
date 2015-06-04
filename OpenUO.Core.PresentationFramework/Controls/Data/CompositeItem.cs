using System.Collections.ObjectModel;

namespace OpenUO.Core.PresentationFramework.Data
{
    public abstract class CompositeItem : Item
    {
        private readonly ObservableCollection<Item> _items = new ObservableCollection<Item>();

        public ObservableCollection<Item> Items
        {
            get { return _items; }
        }

        protected override void Dispose(bool disposing)
        {
            if(Disposed)
            {
                return;
            }
            if(disposing)
            {
                foreach(var item in Items)
                {
                    item.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}