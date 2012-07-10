using System;
using System.Drawing;
using System.Windows.Forms;
using OpenUO.Core.Patterns;

namespace Ultima.Winforms.Sample
{
    public partial class Form1 : Form
    {
        IoCContainer _container;

        public Form1(IoCContainer container)
        {
            _container = container;
            InitializeComponent();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            const int MAX_WIDTH = 44;
            const int MAX_HEIGHT = 44;

            Rectangle itemBounds = new Rectangle(0, 0, MAX_WIDTH, MAX_HEIGHT);

            OpenUO.Ultima.Maps maps = new OpenUO.Ultima.Maps(uoInstallationComboBox1.SelectedInstallation);
            OpenUO.Ultima.TexmapFactory factory = new OpenUO.Ultima.TexmapFactory(uoInstallationComboBox1.SelectedInstallation, _container);

            int columns = Math.Max(0, ((Width - 1) / MAX_WIDTH));
            int rows = Math.Max(0, ((Height - 1) / MAX_HEIGHT));

            int charxPos = 115 - (columns / 2);
            int charyPos = 1478 - (rows / 2);

            for (int y = 0; y < columns; y++)
            {
                e.Graphics.TranslateTransform(0, y * MAX_HEIGHT);

                for (int x = 0; x < rows; x++)
                {
                    var tile = maps.Felucca.Tiles.GetLandTile(charxPos + x, charyPos + y);
                    var bmp = factory.GetTexmap<Bitmap>(tile.Id);

                    if (bmp != null)
                    {
                        e.Graphics.DrawImageUnscaledAndClipped(bmp, itemBounds);
                    }

                    e.Graphics.TranslateTransform(MAX_WIDTH, 0);
                }

                e.Graphics.ResetTransform();
            }

            e.Graphics.ResetTransform();
        }
    }
}
