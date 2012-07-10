using System;

namespace OpenUO.Core.PresentationFramework.ComponentModel.Design
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class RaisePropertyChangedAttribute : Attribute
    {
        private string[] _properties;

        public string[] Properties
        {
            get { return _properties; }
        }

        public RaisePropertyChangedAttribute(params string[] properties)
        {
            _properties = properties;
        }
    }
}
