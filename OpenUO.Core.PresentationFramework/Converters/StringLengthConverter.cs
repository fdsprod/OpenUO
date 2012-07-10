#region File Header
/********************************************************
 * 
 *  $Id: StringLengthConverter.cs 111 2010-10-12 06:58:17Z jeff $
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
using System.Windows.Data;

namespace OpenUO.Core.PresentationFramework.Converters
{
    public class StringLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }

            string s = value.ToString();
            int length = 100;
            int outLength;

            if (parameter != null && int.TryParse(parameter.ToString(), out outLength))
            {
                length = outLength;
            }

            if (s.Length <= length)
            {
                return s;
            }

            return string.Concat(s.Substring(0, length - 3), "...");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
