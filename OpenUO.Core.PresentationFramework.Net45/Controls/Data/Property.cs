using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace OpenUO.Core.PresentationOpenUO.Core.Data
{
    public class Property : Item, IDisposable, INotifyPropertyChanged
    {
        public static readonly IComparer<Property> CompareByCategoryThenByName = new ByCategoryThenByNameComparer();
        public static readonly IComparer<Property> CompareByName = new ByNameComparer();
        protected object _instance;
        protected PropertyDescriptor _property;

        public Property(object instance, PropertyDescriptor property)
        {
            if(instance is ICustomTypeDescriptor)
            {
                _instance = ((ICustomTypeDescriptor)instance).GetPropertyOwner(property);
            }
            else
            {
                _instance = instance;
            }

            _property = property;

            _property.AddValueChanged(_instance, instance_PropertyChanged);

            NotifyPropertyChanged("PropertyType");
        }

        private class ByCategoryThenByNameComparer : IComparer<Property>
        {
            public int Compare(Property x, Property y)
            {
                if(ReferenceEquals(x, null) || ReferenceEquals(y, null))
                {
                    return 0;
                }
                if(ReferenceEquals(x, y))
                {
                    return 0;
                }
                var val = x.Category.CompareTo(y.Category);
                if(val == 0)
                {
                    return x.Name.CompareTo(y.Name);
                }
                return val;
            }
        }

        private class ByNameComparer : IComparer<Property>
        {
            public int Compare(Property x, Property y)
            {
                if(ReferenceEquals(x, null) || ReferenceEquals(y, null))
                {
                    return 0;
                }
                if(ReferenceEquals(x, y))
                {
                    return 0;
                }
                return x.Name.CompareTo(y.Name);
            }
        }

        /// <value>
        ///     Initializes the reflected instance property
        /// </value>
        /// <exception cref="NotSupportedException">
        ///     The conversion cannot be performed
        /// </exception>
        public object Value
        {
            get { return _property.GetValue(_instance); }
            set
            {
                var currentValue = _property.GetValue(_instance);
                if(value != null && value.Equals(currentValue))
                {
                    return;
                }
                var propertyType = _property.PropertyType;
                if(propertyType == typeof(object) ||
                   value == null && propertyType.IsClass ||
                   value != null && propertyType.IsAssignableFrom(value.GetType()))
                {
                    _property.SetValue(_instance, value);
                }
                else
                {
                    var converter = TypeDescriptor.GetConverter(_property.PropertyType);
                    try
                    {
                        var convertedValue = converter.ConvertFrom(value);
                        _property.SetValue(_instance, convertedValue);
                    }
                    catch(Exception)
                    {
                    }
                }
                NotifyPropertyChanged("Value");
            }
        }

        public string Name
        {
            get { return _property.DisplayName ?? _property.Name; }
        }

        public string Description
        {
            get { return _property.Description; }
        }

        public bool IsWriteable
        {
            get { return !IsReadOnly; }
        }

        public bool IsReadOnly
        {
            get { return _property.IsReadOnly; }
        }

        public Type PropertyType
        {
            get { return _property.PropertyType; }
        }

        public string Category
        {
            get { return _property.Category; }
        }

        private void instance_PropertyChanged(object sender, EventArgs e)
        {
            NotifyPropertyChanged("Value");
        }

        protected override void Dispose(bool disposing)
        {
            if(Disposed)
            {
                return;
            }
            if(disposing)
            {
                _property.RemoveValueChanged(_instance, instance_PropertyChanged);
            }
            base.Dispose(disposing);
        }
    }
}