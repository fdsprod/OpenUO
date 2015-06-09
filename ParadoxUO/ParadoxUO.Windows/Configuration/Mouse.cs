#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// Mouse.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

#region Usings

using OpenUO.Core.ComponentModel;

using SiliconStudio.Paradox.Input;

#endregion

namespace OpenUO.Core.Configuration
{
    public class Mouse : NotifiableBase
    {
        private MouseButton _interactionButton;
        private bool _isEnabled;
        private MouseButton _movementButton;

        public Mouse(MouseButton interaction, MouseButton movement)
        {
            InteractionButton = interaction;
            MovementButton = movement;
            IsEnabled = true;
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { SetProperty(ref _isEnabled, value); }
        }

        public MouseButton MovementButton
        {
            get { return _movementButton; }
            set { SetProperty(ref _movementButton, value); }
        }

        public MouseButton InteractionButton
        {
            get { return _interactionButton; }
            set { SetProperty(ref _interactionButton, value); }
        }
    }
}