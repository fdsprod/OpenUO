using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenUO.Ultima.Windows.Forms.Controls
{
    public partial class SoundPlayerControl : UserControl
    {
        private SoundFactory _soundFactory;
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SoundFactory Factory
        {
            get { return _soundFactory; }
            set
            {
                if (_soundFactory != value)
                {
                    _soundFactory = value;

                    BindSounds();
                    Invalidate();
                }
            }
        }

        private void BindSounds()
        {
            if(_soundFactory == null)
            {
                comboBox1.DataSource = null;
                return;
            }

            int length = _soundFactory.GetLength<Sound>();

            List<ListItem<Sound>> sounds = new List<ListItem<Sound>>();

            for (int i = 0; i < length; i++)
            {
                var sound = _soundFactory.GetSound<Sound>(i);

                if (sound == null)
                {
                    continue;
                }

                var listItem = new ListItem<Sound>()
                {
                    Value = sound,
                    DisplayName = sound.Name
                };

                sounds.Add(listItem);
            }

            comboBox1.DataSource = sounds;
            comboBox1.DisplayMember = "DisplayName";
            comboBox1.ValueMember = "Value";
        }

        public class ListItem<T>
        {
            public T Value { get; set; }
            public string DisplayName { get; set; }
        }

        public SoundPlayerControl()
        {
            InitializeComponent();
        }

        private Sound _currentSound;

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1)
            {
                tableLayoutPanel1.Enabled = false;
                return;
            }

            tableLayoutPanel1.Enabled = true;
            _currentSound = (Sound)comboBox1.SelectedValue;
        }

        private void previousButton_Click(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex -= 1;
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            _currentSound.Stop();
        }

        private void pausePlayButton_Click(object sender, EventArgs e)
        {
            _currentSound.Play();
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex += 1;
        }
    }
}
