using System.Windows;

namespace OpenUO.Core.PresentationOpenUO.Core.TypeEditors
{
    public class NumberTypeEditor<T> : IntegerTypeEditor
    {
        static NumberTypeEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumberTypeEditor<T>), new FrameworkPropertyMetadata(typeof(IntegerTypeEditor)));
        }

        public string Typ
        {
            get;
            set;
        }
    }
}