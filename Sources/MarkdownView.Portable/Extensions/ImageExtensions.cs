namespace Xam.Forms.Markdown.Extensions
{
    using System;
    using System.IO;
    using System.Net;
    using SkiaSharp;
    using Xamarin.Forms;
    using System.Diagnostics;

    public static class ImageExtensions
    {
        public static async void RenderSvg(this Image view, string uri)
        {
            try
            {
                var req = (HttpWebRequest)WebRequest.Create(uri);

                var svg = new SkiaSharp.Extended.Svg.SKSvg();
                var res = await req.GetResponseAsync();
                using (var stream = res.GetResponseStream())
                {
                    if (stream != null)
                    {
                        var picture = svg.Load(stream);

                        using (var image = SKImage.FromPicture(picture, picture.CullRect.Size.ToSizeI()))
                        using (var data = image.Encode(SKEncodedImageFormat.Jpeg, 80))
                        {
                            var ms = new MemoryStream();

                            if (data != null && !data.IsEmpty)
                            {
                                data.SaveTo(ms);
                                ms.Seek(0, SeekOrigin.Begin);
                                ms.Position = 0;
                                view.Source = ImageSource.FromStream(() => ms);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to render svg: {ex}");
            }
        }
    }
}
