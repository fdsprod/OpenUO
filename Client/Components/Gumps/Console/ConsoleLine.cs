using Client.Graphics;

namespace Client.Components
{
    public class ConsoleLine
    {
        private string _text;
        private Color _color;

        public string Text
        {
            get { return _text; }
        }

        public ConsoleLine(string text, Color color)
        {
            _text = text;
            _color = color;
        }
    }
}
