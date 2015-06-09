namespace OpenUO.Core.PresentationOpenUO.Core.Data
{
    public class PropertyCategory : PropertyCollection
    {
        private readonly string _categoryName;

        public PropertyCategory()
        {
            _categoryName = "Misc";
        }

        public PropertyCategory(string categoryName)
        {
            _categoryName = categoryName;
        }

        public string Category
        {
            get { return _categoryName; }
        }
    }
}