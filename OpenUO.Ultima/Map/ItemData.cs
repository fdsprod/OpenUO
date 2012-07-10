#region License Header
/***************************************************************************
 *   Copyright (c) 2011 OpenUO Software Team.
 *   All Right Reserved.
 *
 *   $Id: $:
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 ***************************************************************************/
 #endregion


namespace OpenUO.Ultima
{
    public struct ItemData
    {
        internal string _name;
        internal TileFlag _flags;
        internal byte _weight;
        internal byte _quality;
        internal byte _quantity;
        internal byte _value;
        internal byte _height;
        internal short _animation;

        public ItemData(string name, TileFlag flags, int weight, int quality, int quantity, int value, int height, int anim)
        {
            _name = name;
            _flags = flags;
            _weight = (byte)weight;
            _quality = (byte)quality;
            _quantity = (byte)quantity;
            _value = (byte)value;
            _height = (byte)height;
            _animation = (short)anim;
        }

        /// <summary>
        /// Gets the name of this item.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets the animation body index of this item.
        /// <seealso cref="Animations" />
        /// </summary>
        public int Animation
        {
            get { return _animation; }
        }

        /// <summary>
        /// Gets a bitfield representing the 32 individual flags of this item.
        /// <seealso cref="TileFlag" />
        /// </summary>
        public TileFlag Flags
        {
            get { return _flags; }
        }

        /// <summary>
        /// Whether or not this item is flagged as '<see cref="TileFlag.Background" />'.
        /// <seealso cref="TileFlag" />
        /// </summary>
        public bool Background
        {
            get { return ((_flags & TileFlag.Background) != 0); }
        }

        /// <summary>
        /// Whether or not this item is flagged as '<see cref="TileFlag.Bridge" />'.
        /// <seealso cref="TileFlag" />
        /// </summary>
        public bool Bridge
        {
            get { return ((_flags & TileFlag.Bridge) != 0); }
        }

        /// <summary>
        /// Whether or not this item is flagged as '<see cref="TileFlag.Impassable" />'.
        /// <seealso cref="TileFlag" />
        /// </summary>
        public bool Impassable
        {
            get { return ((_flags & TileFlag.Impassable) != 0); }
        }

        /// <summary>
        /// Whether or not this item is flagged as '<see cref="TileFlag.Surface" />'.
        /// <seealso cref="TileFlag" />
        /// </summary>
        public bool Surface
        {
            get { return ((_flags & TileFlag.Surface) != 0); }
        }

        /// <summary>
        /// Gets the weight of this item.
        /// </summary>
        public int Weight
        {
            get { return _weight; }
        }

        /// <summary>
        /// Gets the 'quality' of this item. For wearable items, this will be the layer.
        /// </summary>
        public int Quality
        {
            get { return _quality; }
        }

        /// <summary>
        /// Gets the 'quantity' of this item.
        /// </summary>
        public int Quantity
        {
            get { return _quantity; }
        }

        /// <summary>
        /// Gets the 'value' of this item.
        /// </summary>
        public int Value
        {
            get { return _value; }
        }

        /// <summary>
        /// Gets the height of this item.
        /// </summary>
        public int Height
        {
            get { return _height; }
        }

        /// <summary>
        /// Gets the 'calculated height' of this item. For <see cref="Bridge">bridges</see>, this will be: <c>(<see cref="Height" /> / 2)</c>.
        /// </summary>
        public int CalcHeight
        {
            get
            {
                if ((_flags & TileFlag.Bridge) != 0)
                    return _height / 2;

                return _height;
            }
        }
    }
}
