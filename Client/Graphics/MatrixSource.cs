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

using SharpDX;

namespace Client.Graphics
{
	public abstract class MatrixSource 
	{
		public Matrix Value = Matrix.Identity;
		public int _index = 1;

		public abstract void UpdateValue(int frame);

		public bool Changed(ref int index)
		{
            bool changed = _index != index;
            _index = index;
			return changed;
		}
	}
}
