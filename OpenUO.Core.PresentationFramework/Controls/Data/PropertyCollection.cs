using System.Collections.Generic;
using System.ComponentModel;

namespace OpenUO.Core.PresentationFramework.Data
{
    public class PropertyCollection : CompositeItem
    {
        public PropertyCollection()
        {
        }

        //public PropertyCollection(object instance)
        //    : this(instance, false)
        //{ }

        public PropertyCollection(object instance, bool noCategory, bool automaticlyExpandObjects, string filter)
        {
            var groups = new Dictionary<string, PropertyCategory>();

            var useCustomTypeConverter = false;

            PropertyDescriptorCollection properties;
            if(instance != null)
            {
                var tc = TypeDescriptor.GetConverter(instance);
                if(tc == null || !tc.GetPropertiesSupported())
                {
                    if(instance is ICustomTypeDescriptor)
                    {
                        properties = ((ICustomTypeDescriptor)instance).GetProperties();
                    }
                    else
                    {
                        properties = TypeDescriptor.GetProperties(instance.GetType()); //I changed here from instance to instance.GetType, so that only the Direct Properties are shown!
                    }
                }
                else
                {
                    properties = tc.GetProperties(instance);
                    useCustomTypeConverter = true;
                }
            }
            else
            {
                properties = new PropertyDescriptorCollection(new PropertyDescriptor[]
                                                              {
                                                              });
            }

            var propertyCollection = new List<Property>();

            foreach(PropertyDescriptor propertyDescriptor in properties)
            {
                if(useCustomTypeConverter)
                {
                    var property = new Property(instance, propertyDescriptor);
                    propertyCollection.Add(property);
                }
                else
                {
                    CollectProperties(instance, propertyDescriptor, propertyCollection, automaticlyExpandObjects, filter);
                    if(noCategory)
                    {
                        propertyCollection.Sort(Property.CompareByName);
                    }
                    else
                    {
                        propertyCollection.Sort(Property.CompareByCategoryThenByName);
                    }
                }
            }

            if(noCategory)
            {
                foreach(var property in propertyCollection)
                {
                    if(filter == "" || property.Name.ToLower().Contains(filter))
                    {
                        Items.Add(property);
                    }
                }
            }
            else
            {
                foreach(var property in propertyCollection)
                {
                    if(filter == "" || property.Name.ToLower().Contains(filter))
                    {
                        PropertyCategory propertyCategory;
                        var category = property.Category ?? string.Empty; // null category handled here

                        if(groups.ContainsKey(category))
                        {
                            propertyCategory = groups[category];
                        }
                        else
                        {
                            propertyCategory = new PropertyCategory(property.Category);
                            groups[category] = propertyCategory;
                            Items.Add(propertyCategory);
                        }
                        propertyCategory.Items.Add(property);
                    }
                }
            }
        }

        private void CollectProperties(object instance, PropertyDescriptor descriptor, List<Property> propertyCollection, bool automaticlyExpandObjects, string filter)
        {
            if(descriptor.Attributes[typeof(FlatAttribute)] == null)
            {
                var property = new Property(instance, descriptor);
                if(descriptor.IsBrowsable)
                {
                    //Add a property with Name: AutomaticlyExpandObjects
                    var propertyType = descriptor.PropertyType;
                    if(automaticlyExpandObjects && propertyType.IsClass && !propertyType.IsArray && propertyType != typeof(string))
                    {
                        propertyCollection.Add(new ExpandableProperty(instance, descriptor, automaticlyExpandObjects, filter));
                    }
                    else if(descriptor.Converter.GetType() == typeof(ExpandableObjectConverter))
                    {
                        propertyCollection.Add(new ExpandableProperty(instance, descriptor, automaticlyExpandObjects, filter));
                    }
                    else
                    {
                        propertyCollection.Add(property);
                    }
                }
            }
            else
            {
                instance = descriptor.GetValue(instance);
                var properties = TypeDescriptor.GetProperties(instance);
                foreach(PropertyDescriptor propertyDescriptor in properties)
                {
                    CollectProperties(instance, propertyDescriptor, propertyCollection, automaticlyExpandObjects, filter);
                }
            }
        }
    }
}