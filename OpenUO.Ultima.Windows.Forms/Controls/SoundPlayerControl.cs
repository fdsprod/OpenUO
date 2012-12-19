#region License Header

// /***************************************************************************
//  *   Copyright (c) 2011 OpenUO Software Team.
//  *   All Right Reserved.
//  *
//  *   SoundPlayerControl.cs
//  *
//  *   This program is free software; you can redistribute it and/or modify
//  *   it under the terms of the GNU General Public License as published by
//  *   the Free Software Foundation; either version 3 of the License, or
//  *   (at your option) any later version.
//  ***************************************************************************/

#endregion

#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

#endregion

namespace OpenUO.Ultima.Windows.Forms.Controls
{
    public partial class SoundPlayerControl : UserControl
    {
        private Sound _currentSound;
        private SoundFactory _soundFactory;

        public SoundPlayerControl()
        {
            InitializeComponent();
        }

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
            if (_soundFactory == null)
            {
                comboBox1.DataSource = null;
                return;
            }

            int length = _soundFactory.GetLength<Sound>();

            List<ListItem<Sound>> sounds = new List<ListItem<Sound>>();

            for (int i = 0; i < length; i++)
            {
                Sound sound = _soundFactory.GetSound<Sound>(i);

                if (sound == null)
                {
                    continue;
                }

                ListItem<Sound> listItem = new ListItem<Sound> {
                    Value = sound,
                    DisplayName = sound.Name
                };

                sounds.Add(listItem);
            }

            comboBox1.DataSource = sounds;
            comboBox1.DisplayMember = "DisplayName";
            comboBox1.ValueMember = "Value";
        }

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

        public class ListItem<T>
        {
            public T Value
            {
                get;
                set;
            }

            public string DisplayName
            {
                get;
                set;
            }
        }
    }
}