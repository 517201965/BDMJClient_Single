using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.IO;

namespace BDMJClient_Single
{
    class Image_Common
    {
        public static BitmapImage ReadFromPath(string path)
        {
            BitmapImage image = null;
            if (File.Exists(path))
            {
                using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(path)))
                {
                    image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;//设置缓存模式
                    image.StreamSource = ms;
                    image.EndInit();
                    image.Freeze();
                }
            }
            return image;
        }
    }
}
