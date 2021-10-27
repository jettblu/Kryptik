using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
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
        public static Stream CropImage(IFormFile photoFile)
        {
            Image croppedImage;
            MemoryStream outStream = new MemoryStream();
            // shrink image and crop to square
            using (var inStream = photoFile.OpenReadStream())
            {
                using (var image = SixLabors.ImageSharp.Image.Load(inStream, out IImageFormat format))
                {
                    //Save three sizes Cropped:
                    var jpegEncoder = new JpegEncoder { Quality = 75 };
                    croppedImage = image.Clone(context => context
                        .Resize(new ResizeOptions
                        {
                            Mode = ResizeMode.Crop,
                            Size = new Size(400, 400)
                        }));
                    croppedImage.Save(outStream, jpegEncoder);
                }
            }
            // set stream position to beginning of stream
            outStream.Position = 0;
            return outStream;
        }
    }
}
