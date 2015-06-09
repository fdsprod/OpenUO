using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using OpenUO.Core.PresentationOpenUO.Core.Data;

namespace OpenUO.Core.PresentationFramework
{
    public class PropertyTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var property = item as Property;
            if(property == null)
            {
                throw new ArgumentException("item must be of type Property");
            }
            var element = container as FrameworkElement;
            if(element == null)
            {
                return base.SelectTemplate(property.Value, container);
            }
            var template = FindDataTemplate(property, element);
            return template;
        }

        private DataTemplate FindDataTemplate(Property property, FrameworkElement element)
        {
            var propertyType = property.PropertyType;

            if(!(property.PropertyType == typeof(string)) && property.PropertyType is IEnumerable)
            {
                propertyType = typeof(List<object>);
            }

            var template = TryFindDataTemplate(element, propertyType);

            while(template == null && propertyType.BaseType != null)
            {
                propertyType = propertyType.BaseType;
                template = TryFindDataTemplate(element, propertyType);
            }
            if(template == null)
            {
                template = TryFindDataTemplate(element, "default");
            }
            return template;
        }

        private static DataTemplate TryFindDataTemplate(FrameworkElement element, object dataTemplateKey)
        {
            var dataTemplate = element.TryFindResource(dataTemplateKey);
            if(dataTemplate == null)
            {
                dataTemplateKey = new ComponentResourceKey(typeof(PropertyGrid), dataTemplateKey);
                dataTemplate = element.TryFindResource(dataTemplateKey);
            }
            return dataTemplate as DataTemplate;
        }
    }
}