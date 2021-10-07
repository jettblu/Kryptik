using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;

namespace CrypticPay.ProfilePhoto
{
    public static class Avatar
    {
        private static List<string> _BaseAvatars = new List<string> { "https://crypticblob.blob.core.windows.net/media/avaGreen.jpg", 
                                                                      "https://crypticblob.blob.core.windows.net/media/avaPas.jpg", 
                                                                      "https://crypticblob.blob.core.windows.net/media/avaPink.jpg", 
                                                                      "https://crypticblob.blob.core.windows.net/media/avaPurp.jpg" };

        public static string RetrieveRandomURI()
        {
            var randomIndex = new Random().Next(0, _BaseAvatars.Count - 1);
            var baseAvatarURI = _BaseAvatars[randomIndex];
            return baseAvatarURI;
        }
    }
}
