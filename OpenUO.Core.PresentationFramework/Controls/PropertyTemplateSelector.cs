using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using OpenUO.Core.PresentationFramework.Data;

namespace OpenUO.Core.PresentationFramework
{
    public class PropertyTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            Property property = item as Property;
            if (property == null)
            {
                throw new ArgumentException("item must be of type Property");
            }
            FrameworkElement element = container as FrameworkElement;
            if (element == null)
            {
                return base.SelectTemplate(property.Value, container);
            }
            DataTemplate template = FindDataTemplate(property, element);
            return template;
        }

        private DataTemplate FindDataTemplate(Property property, FrameworkElement element)
        {
            Type propertyType = property.PropertyType;


            if (!(property.PropertyType == typeof(string)) && property.PropertyType is IEnumerable)
                propertyType = typeof(List<object>);

            DataTemplate template = TryFindDataTemplate(element, propertyType);

            while (template == null && propertyType.BaseType != null)
            {
                propertyType = propertyType.BaseType;
                template = TryFindDataTemplate(element, propertyType);
            }
            if (template == null)
            {
                template = TryFindDataTemplate(element, "default");
            }
            return template;
        }

        private static DataTemplate TryFindDataTemplate(FrameworkElement element, object dataTemplateKey)
        {
            object dataTemplate = element.TryFindResource(dataTemplateKey);
            if (dataTemplate == null)
            {
                dataTemplateKey = new ComponentResourceKey(typeof(PropertyGrid), dataTemplateKey);
                dataTemplate = element.TryFindResource(dataTemplateKey);
            }
            return dataTemplate as DataTemplate;
        }
    }
}
