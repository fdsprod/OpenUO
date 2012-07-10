#region File Header
/********************************************************
 * 
 *  $Id: BoolToOpacityConverter.cs 111 2010-10-12 06:58:17Z jeff $
 *  
 *  $Author: jeff $
 *  $Date: 2010-10-11 23:58:17 -0700 (Mon, 11 Oct 2010) $
 *  $Revision: 111 $
 *  
 *  $LastChangedBy: jeff $
 *  $LastChangedDate: 2010-10-11 23:58:17 -0700 (Mon, 11 Oct 2010) $
 *  $LastChangedRevision: 111 $
 *  
 *  (C) Copyright 2009 Jeff Boulanger
 *  All rights reserved. 
 *  
 ********************************************************/
#endregion

using System;
using System.Globalization;
using System.Windows.Data;

namespace OpenUO.Core.PresentationFramework.Converters
{
    public sealed class BoolToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if ((bool)value)
                {
                    return 1;
                }

                return 0;
            }
            catch
            {
                return 1;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
