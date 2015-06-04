using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using OpenUO.Core.PresentationFramework.Data;

namespace OpenUO.Core.PresentationFramework
{
    [TemplatePart(Name = "PART_PropertyName", Type = typeof(Label))]
    [TemplatePart(Name = "PART_PropertyDescription", Type = typeof(TextBlock))]
    [TemplatePart(Name = "PART_InstanceType", Type = typeof(TextBlock))]
    [TemplatePart(Name = "PART_InstanceName", Type = typeof(TextBlock))]
    [TemplatePart(Name = "PART_Thumb", Type = typeof(Thumb))]
    public class PropertyGrid : Control
    {
        public static readonly DependencyProperty SelectedPropertyProperty =
            DependencyProperty.Register("SelectedProperty", typeof(Property), typeof(PropertyGrid), new FrameworkPropertyMetadata(null));

        /// <value>Identifies the Instance dependency property</value>
        public static readonly DependencyProperty InstanceProperty =
            DependencyProperty.Register("Instance", typeof(object), typeof(PropertyGrid),
                new FrameworkPropertyMetadata(null, OnInstanceChanged, CoerceInstance));

        /// <value>Identifies the Filter dependency property</value>
        public static DependencyProperty FilterProperty =
            DependencyProperty.Register("Filter", typeof(string), typeof(PropertyGrid),
                new FrameworkPropertyMetadata("", OnFilterChanged));

        /// <value>Identifies the Categorized dependency property</value>
        public static DependencyProperty CategorizedProperty =
            DependencyProperty.Register("Categorized", typeof(bool), typeof(PropertyGrid),
                new FrameworkPropertyMetadata(true, OnCategorizedChanged));

        /// <summary>
        ///     DependenyProperty for the headline of the Propertygrid.
        /// </summary>
        public static readonly DependencyProperty HeadlineProperty = DependencyProperty.Register("Headline", typeof(string), typeof(PropertyGrid), new FrameworkPropertyMetadata("Properties"));

        public static DependencyProperty NameWidthProperty =
            DependencyProperty.Register("NameWidth", typeof(double), typeof(PropertyGrid),
                new PropertyMetadata(120.0));

        /// <value>Identifies the ShowDescription dependency property</value>
        public static DependencyProperty ShowDescriptionProperty =
            DependencyProperty.Register("ShowDescription", typeof(bool), typeof(PropertyGrid),
                new FrameworkPropertyMetadata(false));

        /// <value>Identifies the ShowPreview dependency property</value>
        public static DependencyProperty ShowPreviewProperty =
            DependencyProperty.Register("ShowPreview", typeof(bool), typeof(PropertyGrid),
                new FrameworkPropertyMetadata(false));

        /// <value>Identifies the AutomaticlyExpandObjects dependency property</value>
        /// The Change of this Property calls OnInstanceChanged, because the property List needs to be refreshed!
        public static DependencyProperty AutomaticlyExpandObjectsProperty =
            DependencyProperty.Register("AutomaticlyExpandObjects", typeof(bool), typeof(PropertyGrid),
                new FrameworkPropertyMetadata(false, OnInstanceChanged));

        /// <value>description for AutomaticlyExpandObjects Property</value>
        public bool AutomaticlyExpandObjects
        {
            get { return (bool)GetValue(AutomaticlyExpandObjectsProperty); }
            set { SetValue(AutomaticlyExpandObjectsProperty, value); }
        }

        private static void OnCategorizedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var propertyGrid = d as PropertyGrid;

            propertyGrid.Refresh();
        }

        /// <value>description for Categorized property</value>
        [Description("Propertys are Categorized or Alphabetical Sorted")]
        public bool Categorized
        {
            get { return (bool)GetValue(CategorizedProperty); }
            set { SetValue(CategorizedProperty, value); }
        }

        private static object CoerceInstance(DependencyObject d, object value)
        {
            var propertyGrid = d as PropertyGrid;
            if(value == null)
            {
                return propertyGrid.NullInstance;
            }
            return value;
        }

        private static void OnFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var propertyGrid = d as PropertyGrid;

            propertyGrid.Refresh();
        }

        /// <value>description for Filter property</value>
        [Description("If you set a filter, only properties Containing this Text will be displayed")]
        public string Filter
        {
            get { return (string)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the Headline of the Propertygrid.
        /// </summary>
        public string Headline
        {
            get { return (string)GetValue(HeadlineProperty); }
            set { SetValue(HeadlineProperty, value); }
        }

        /// <summary>
        ///     Invoked on Instance change.
        /// </summary>
        /// <param name="d">The object that was changed</param>
        /// <param name="e">Dependency property changed event arguments</param>
        private static void OnInstanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var propertyGrid = d as PropertyGrid;

            propertyGrid.SelectedProperty = null;

            propertyGrid.Refresh();

            propertyGrid.RaiseInstanceChanged();
        }

        /// <value>description for Instance property</value>
        public object Instance
        {
            get { return GetValue(InstanceProperty); }
            set { SetValue(InstanceProperty, value); }
        }

        public double NameWidth
        {
            get { return (double)GetValue(NameWidthProperty); }
            set { SetValue(NameWidthProperty, value); }
        }

        /// <value>Identifies the NullInstance dependency property</value>
        public static readonly DependencyProperty NullInstanceProperty =
            DependencyProperty.Register("NullInstance", typeof(object), typeof(PropertyGrid),
                new FrameworkPropertyMetadata(null, OnNullInstanceChanged));

        /// <summary>
        ///     Invoked on NullInstance change.
        /// </summary>
        /// <param name="d">The object that was changed</param>
        /// <param name="e">Dependency property changed event arguments</param>
        private static void OnNullInstanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <value>description for NullInstance property</value>
        public object NullInstance
        {
            get { return GetValue(NullInstanceProperty); }
            set { SetValue(NullInstanceProperty, value); }
        }

        /// <value>Identifies the Properties dependency property</value>
        public static readonly DependencyProperty PropertiesProperty =
            DependencyProperty.Register("Properties", typeof(ObservableCollection<Item>), typeof(PropertyGrid),
                new FrameworkPropertyMetadata(new ObservableCollection<Item>(),
                    OnPropertiesChanged));

        private static void OnPropertiesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var propertyGrid = d as PropertyGrid;
            var properties = e.OldValue as ObservableCollection<Item>;
            foreach(var item in properties)
            {
                item.Dispose();
            }
        }

        /// <value>description for Properties property</value>
        public ObservableCollection<Item> Properties
        {
            get { return (ObservableCollection<Item>)GetValue(PropertiesProperty); }
            set { SetValue(PropertiesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedProperty.  This enables animation, styling, binding, etc...
        public Property SelectedProperty
        {
            get { return (Property)GetValue(SelectedPropertyProperty); }
            set { SetValue(SelectedPropertyProperty, value); }
        }

        /// <value>description for ShowDescription property</value>
        public bool ShowDescription
        {
            get { return (bool)GetValue(ShowDescriptionProperty); }
            set { SetValue(ShowDescriptionProperty, value); }
        }

        /// <value>description for ShowPreview property</value>
        [Description("If you set the Instance to a XAML Object you can show a little Preview Object in the PropertyGrid"
            )]
        public bool ShowPreview
        {
            get { return (bool)GetValue(ShowPreviewProperty); }
            set { SetValue(ShowPreviewProperty, value); }
        }

        static PropertyGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGrid),
                new FrameworkPropertyMetadata(typeof(PropertyGrid)));
        }

        public PropertyGrid()
        {
            DataContext = this;
            PreviewMouseDown += PropertyGrid_PreviewMouseDown;
            PreviewKeyDown += PropertyGrid_PreviewKeyDown;
        }

        /// <summary>
        ///     Event raised when Instance property is changed
        /// </summary>
        public event EventHandler InstanceChanged;

        //Finish Editing of a TextBox with Enter!
        private void PropertyGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(SelectedProperty != null && e.Key == Key.Enter && e.OriginalSource is TextBox)
            {
                SelectedProperty.Value = ((TextBox)e.OriginalSource).Text;
            }
            else if(e.Key == Key.Down && e.OriginalSource is TextBox)
            {
                e.Handled = true;
                ((TextBox)e.OriginalSource).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
            else if(e.Key == Key.Up && e.OriginalSource is TextBox)
            {
                e.Handled = true;
                ((TextBox)e.OriginalSource).MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
            }
        }

        //(I don't like the implemention, TODO, this should really be done by DataBinding to current property!)

        //Call OnInstanceChanged Again, if the Template was Applied after setting the Instance!      
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var myThumb = Template.FindName("PART_Thumb", this);
            if(myThumb != null && myThumb is Thumb)
            {
                ((Thumb)myThumb).DragDelta += PART_Thumb_DragDelta;
            }

            Refresh();
        }

        private void PropertyGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var OrginalSource = (e.OriginalSource as FrameworkElement);

            if(OrginalSource.DataContext != null && OrginalSource.DataContext.GetType() == typeof(Property))
            {
                //var selectedProperty = (Property)OrginalSource.DataContext;
                SelectedProperty = (Property)OrginalSource.DataContext;
                //selectedProperty.PropertyChanged += new PropertyChangedEventHandler(selectedProperty_PropertyChanged);

                /*
                object myDescriptionField = Template.FindName("Part_PropertyName", this);

                if (myDescriptionField != null && myDescriptionField is TextBlock)
                    ((TextBlock)myDescriptionField).Text = selectedProperty.Name;

                object myNameField = Template.FindName("Part_PropertyDescription", this);

                if (myNameField != null && myNameField is TextBlock)
                    ((TextBlock)myNameField).Text = selectedProperty.Description;
                */
            }
            else
            {
                SelectedProperty = null;
            }
        }

        private void ClearDescription()
        {
            if(Template != null)
            {
                var myDescriptionField = Template.FindName("Part_PropertyName", this);

                if(myDescriptionField != null && myDescriptionField is TextBlock)
                {
                    ((TextBlock)myDescriptionField).Text = "";
                }

                var myNameField = Template.FindName("Part_PropertyDescription", this);

                if(myNameField != null && myNameField is TextBlock)
                {
                    ((TextBlock)myNameField).Text = "";
                }
            }
        }

        public void Refresh()
        {
            if(Instance == null)
            {
                Properties = new ObservableCollection<Item>();
            }
            else
            {
                var properties = new PropertyCollection(Instance, !Categorized, AutomaticlyExpandObjects,
                    Filter.ToLower());
                Properties = properties.Items;

                if(Instance != null)
                {
                    /// Fill the Instance type and Name with the Data
                    if(Template != null)
                    {
                        var myInstanceType = Template.FindName("PART_InstanceType", this);

                        if(myInstanceType != null && myInstanceType is TextBlock)
                        {
                            var myType = "";
                            var myAttributes = Instance.GetType().GetCustomAttributes(false);
                            if(myAttributes.Length > 0)
                            {
                                foreach(var attr in myAttributes)
                                {
                                    if(attr is DisplayNameAttribute)
                                    {
                                        myType = ((DisplayNameAttribute)attr).DisplayName;
                                    }
                                }
                            }
                            if(myType.Length == 0)
                            {
                                myType = Instance.GetType().ToString();
                                myType = myType.Substring(myType.LastIndexOf('.') + 1);
                            }
                            ((TextBlock)myInstanceType).Text = myType;
                        }

                        var myInstanceName = Template.FindName("PART_InstanceName", this);

                        if(myInstanceName != null && myInstanceName is TextBlock)
                        {
                            if(Instance is FrameworkElement)
                            {
                                ((TextBlock)myInstanceName).Text = ((FrameworkElement)Instance).Name;
                            }
                            else if(Instance is System.Windows.Forms.Control)
                            {
                                ((TextBlock)myInstanceName).Text = ((System.Windows.Forms.Control)Instance).Name;
                            }
                            else
                            {
                                ((TextBlock)myInstanceName).Text = "";
                                var pi = Instance.GetType().GetProperty("Name");
                                if(pi != null)
                                {
                                    ((TextBlock)myInstanceName).Text = (string)pi.GetValue(Instance, null);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void RaiseInstanceChanged()
        {
            var handler = InstanceChanged;
            if(handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void PART_Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            NameWidth = Math.Max(0, NameWidth + e.HorizontalChange);
        }
    }
}