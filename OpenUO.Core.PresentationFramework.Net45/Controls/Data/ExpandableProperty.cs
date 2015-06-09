using System.Collections.ObjectModel;
using System.ComponentModel;

namespace OpenUO.Core.PresentationOpenUO.Core.Data
{
    internal class ExpandableProperty : Property
    {
        private readonly bool _automaticlyExpandObjects;
        private readonly string _filter;
        private PropertyCollection _propertyCollection;

        public ExpandableProperty(object instance, PropertyDescriptor property, bool automaticlyExpandObjects, string filter)
            : base(instance, property)
        {
            _automaticlyExpandObjects = automaticlyExpandObjects;
            _filter = filter;
        }

        public ObservableCollection<Item> Items
        {
            get
            {
                if(_propertyCollection == null)
                {
                    //Lazy initialisation prevent from deep search and looping
                    _propertyCollection = new PropertyCollection(_property.GetValue(_instance), true, _automaticlyExpandObjects, _filter);
                }

                return _propertyCollection.Items;
            }
        }
    }
}