using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

namespace OpenUO.Core.PresentationFramework.ComponentModel.Design
{
    [DataContract(Namespace = "http://www.openuo.com/")]
    public abstract class PropertyChangedNotifierBase : INotifyPropertyChanged, IDisposable
    {
        private readonly Dictionary<string, object> _properties;

        public event PropertyChangedEventHandler PropertyChanged;

        protected PropertyChangedNotifierBase()
        {
            _properties = new Dictionary<string, object>();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {

        }

        protected virtual T Get<T>(Expression<Func<T>> propertyExpression)
        {
            string propertyName = PropertySupport.ExtractPropertyName(propertyExpression);
            return Get<T>(propertyName);
        }

        protected virtual bool Set<T>(Expression<Func<T>> propertyExpression, T value)
        {
            string propertyName = PropertySupport.ExtractPropertyName(propertyExpression);
            return Set<T>(propertyName, value);
        }

        protected virtual T Get<T>(string propertyName)
        {
            EnsurePropertyInitialized(propertyName);
            return (T)_properties[propertyName];
        }

        protected virtual bool Set<T>(string propertyName, T value)
        {
            EnsurePropertyInitialized(propertyName);

            object newValue = value;
            object oldValue = (T)_properties[propertyName];

            if (oldValue != newValue)
            {
                ValidateProperty(propertyName, value);
                _properties[propertyName] = newValue;
                OnPropertyChanged(this, new PropertyChangedEventArgs(propertyName));

                return true;
            }

            return false;
        }

        protected void RaisePropertyChanged(params Expression<Func<object>>[] propertyExpressions)
        {
            for (int i = 0; i < propertyExpressions.Length; i++)
            {
                Expression<Func<object>> propertyExpression = propertyExpressions[i];
                OnPropertyChanged(this, new PropertyChangedEventArgs(PropertySupport.ExtractPropertyName<object>(propertyExpression)));
            }
        }

        protected void RaisePropertyChanged(params string[] propertyNames)
        {
            for (int i = 0; i < propertyNames.Length; i++)
                OnPropertyChanged(this, new PropertyChangedEventArgs(propertyNames[i]));
        }

        protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;

            if (handler != null)
                handler(sender, e);
        }

        private void InitializeProperty(PropertyInfo info, bool initialize)
        {
            Type underlyingType = Nullable.GetUnderlyingType(info.PropertyType);

            if (!info.PropertyType.IsValueType || (underlyingType != null))
            {
                _properties.Add(info.Name, null);

                if (initialize)
                {
                    if (info.PropertyType.Equals(typeof(string)))
                        Set(info.Name, "");
                    else if (!info.PropertyType.IsAbstract &&
                             info.PropertyType.GetConstructor(Type.EmptyTypes) != null)
                        Set(info.Name, Activator.CreateInstance(info.PropertyType));
                    else
                        Set(info.Name, Activator.CreateInstance(underlyingType));
                }
            }
            else
            {
                _properties.Add(info.Name, Activator.CreateInstance(info.PropertyType));
            }
        }

        private void EnsurePropertyInitialized(string propertyName)
        {
            if (!_properties.ContainsKey(propertyName))
            {
                PropertyInfo info = GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (info == null)
                    throw new Exception(string.Format("Cannot find property '{0}'", propertyName));

                InitializeProperty(info, false);

                object[] defaultValue = info.GetCustomAttributes(typeof(DefaultValueAttribute), false);

                if (defaultValue.Length > 0)
                    _properties[propertyName] = ((DefaultValueAttribute)defaultValue[0]).Value;
            }
        }

        protected void ValidateProperty(string name, object value)
        {
            PropertyInfo property = GetType().GetProperty(name);

            if (property != null)
            {
                object[] attributes = property.GetCustomAttributes(true);

                if (attributes != null &&  attributes.Where(a => a is ValidationAttribute).Count() > 0)
                    Validator.ValidateProperty(value, new ValidationContext(this, null, null) { MemberName = name });
            }
        }
    }
}
