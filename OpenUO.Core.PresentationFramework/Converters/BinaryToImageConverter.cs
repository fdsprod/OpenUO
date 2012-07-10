#region File Header
/********************************************************
 * 
 *  $Id: BinaryToImageConverter.cs 111 2010-10-12 06:58:17Z jeff $
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
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace OpenUO.Core.PresentationFramework.Converters
{
    public class BinaryToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            BitmapImage defaultImage = null;

            if (parameter is BitmapImage)
            {
                defaultImage = parameter as BitmapImage;
            }

            string url = string.Empty;

            if (parameter is string && !string.IsNullOrWhiteSpace(url = parameter.ToString()))
            {
                defaultImage = new BitmapImage(new Uri(url, UriKind.Relative));
            }

            byte[] buffer = value as byte[];

            if (buffer == null || buffer.Length == 0)
            {
                return defaultImage;
            }

            BitmapImage image = new BitmapImage();

            try
            {
                MemoryStream streamSource = new MemoryStream(buffer);

#if SILVERLIGHT
                image.SetSource(streamSource);

#else
                image.BeginInit();
                image.StreamSource = streamSource;
                image.EndInit();
#endif
            }
            catch { image = defaultImage; } // TODO: Need to log this, or do something other then swallow the exception...

            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
