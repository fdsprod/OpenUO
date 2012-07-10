using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace OpenUO.Core.PresentationFramework.Data
{
    public class Property : Item, IDisposable, INotifyPropertyChanged
    {
        #region Fields

        protected object _instance;
        protected PropertyDescriptor _property;


        #endregion

        #region Initialization

        public Property(object instance, PropertyDescriptor property)
        {
            if (instance is ICustomTypeDescriptor)
            {
                this._instance = ((ICustomTypeDescriptor)instance).GetPropertyOwner(property);
            }
            else
            {
                this._instance = instance;
            }

            this._property = property;

            this._property.AddValueChanged(_instance, instance_PropertyChanged);

            NotifyPropertyChanged("PropertyType");
        }

        #endregion

        #region Properties

        /// <value>
        /// Initializes the reflected instance property
        /// </value>
        /// <exception cref="NotSupportedException">
        /// The conversion cannot be performed
        /// </exception>
        public object Value
        {
            get { return _property.GetValue(_instance); }
            set
            {
                object currentValue = _property.GetValue(_instance);
                if (value != null && value.Equals(currentValue))
                {
                    return;
                }
                Type propertyType = _property.PropertyType;
                if (propertyType == typeof(object) ||
                    value == null && propertyType.IsClass ||
                    value != null && propertyType.IsAssignableFrom(value.GetType()))
                {
                    _property.SetValue(_instance, value);
                }
                else
                {
                    TypeConverter converter = TypeDescriptor.GetConverter(_property.PropertyType);
                    try
                    {
                        object convertedValue = converter.ConvertFrom(value);
                        _property.SetValue(_instance, convertedValue);
                    }
                    catch (Exception)
                    { }
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

        #endregion

        #region Event Handlers

        void instance_PropertyChanged(object sender, EventArgs e)
        {
            NotifyPropertyChanged("Value");
        }

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            if (Disposed)
            {
                return;
            }
            if (disposing)
            {
                _property.RemoveValueChanged(_instance, instance_PropertyChanged);
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Comparer for Sorting

        private class ByCategoryThenByNameComparer : IComparer<Property>
        {

            public int Compare(Property x, Property y)
            {
                if (ReferenceEquals(x, null) || ReferenceEquals(y, null)) return 0;
                if (ReferenceEquals(x, y)) return 0;
                int val = x.Category.CompareTo(y.Category);
                if (val == 0) return x.Name.CompareTo(y.Name);
                return val;
            }
        }

        private class ByNameComparer : IComparer<Property>
        {

            public int Compare(Property x, Property y)
            {
                if (ReferenceEquals(x, null) || ReferenceEquals(y, null)) return 0;
                if (ReferenceEquals(x, y)) return 0;
                return x.Name.CompareTo(y.Name);
            }
        }

        public readonly static IComparer<Property> CompareByCategoryThenByName = new ByCategoryThenByNameComparer();
        public readonly static IComparer<Property> CompareByName = new ByNameComparer();

        #endregion
    }
}
