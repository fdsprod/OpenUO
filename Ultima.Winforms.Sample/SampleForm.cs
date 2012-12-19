#region License Header

// /***************************************************************************
//  *   Copyright (c) 2011 OpenUO Software Team.
//  *   All Right Reserved.
//  *
//  *   SampleForm.cs
//  *
//  *   This program is free software; you can redistribute it and/or modify
//  *   it under the terms of the GNU General Public License as published by
//  *   the Free Software Foundation; either version 3 of the License, or
//  *   (at your option) any later version.
//  ***************************************************************************/

#endregion

#region Usings

using System;
using System.Windows.Forms;
using OpenUO.Core.Patterns;
using OpenUO.Ultima;

#endregion

namespace Ultima.Winforms.Sample
{
    public partial class SampleForm : Form
    {
        private readonly IContainer _container;
        private ArtworkFactory _artFactory;
        private GumpFactory _gumpFactory;
        private SoundFactory _soundFactory;

        public SampleForm(IContainer container)
        {
            _container = container;

            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Initialize();
        }

        private void Initialize()
        {
            if (uoInstallationComboBox1.SelectedInstallation == null)
            {
                return;
            }

            if (_artFactory != null)
            {
                _artFactory.Dispose();
                _artFactory = null;
            }

            if (_gumpFactory != null)
            {
                _gumpFactory.Dispose();
                _gumpFactory = null;
            }

            if (_soundFactory != null)
            {
                _soundFactory.Dispose();
                _soundFactory = null;
            }

            _gumpFactory = new GumpFactory(uoInstallationComboBox1.SelectedInstallation, _container);
            _artFactory = new ArtworkFactory(uoInstallationComboBox1.SelectedInstallation, _container);
            _soundFactory = new SoundFactory(uoInstallationComboBox1.SelectedInstallation, _container);

            artworkControl1.Factory = _artFactory;
            artworkControl2.Factory = _artFactory;
            gumpControl.Factory = _gumpFactory;
            soundControl.Factory = _soundFactory;
        }

        private void uoInstallationComboBox1_SelectedInstallationChanged(object sender, EventArgs e)
        {
            Initialize();
        }
    }
}