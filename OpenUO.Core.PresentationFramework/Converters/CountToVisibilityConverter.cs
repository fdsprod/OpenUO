#region File Header
/********************************************************
 * 
 *  $Id: CountToVisibility.cs 111 2010-10-12 06:58:17Z jeff $
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
using System.Windows;
using System.Windows.Data;

namespace OpenUO.Core.PresentationFramework.Converters
{
    public sealed class CountToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value != null) && (((int)value) > 0))
            {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
