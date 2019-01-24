using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace N2ImageAgent.AzureBlob
{

    public class ImageUtility
    {
        #region 產生縮圖
        /// <summary>
        /// 產生縮圖
        /// </summary>
        /// <param name="img">圖片物件</param>
        /// <param name="width">縮圖寬度</param>
        /// <param name="height">縮圖高度</param>
        /// <param name="mode">縮圖模式</param>
        /// <returns>縮圖後的圖片物件</returns>
        public Image MakeThumbnail(Image img, int width, int height, string mode)
        {
            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int ow = img.Width;
            int oh = img.Height;

            switch (mode)
            {
                case "HW"://指定高寬縮放（可能變形）                
                    break;
                case "W"://指定寬，高按比例                    
                    toheight = img.Height * width / img.Width;
                    break;
                case "H"://指定高，寬按比例
                    towidth = img.Width * height / img.Height;
                    break;
                case "Cut"://指定高寬裁減（不變形）                
                    if ((double)img.Width / (double)img.Height > (double)towidth / (double)toheight)
                    {
                        oh = img.Height;
                        ow = img.Height * towidth / toheight;
                        y = 0;
                        x = (img.Width - ow) / 2;
                    }
                    else
                    {
                        ow = img.Width;
                        oh = img.Width * height / towidth;
                        x = 0;
                        y = (img.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }


            Bitmap bitmap = new Bitmap(towidth, toheight);
            using (Graphics grphs = Graphics.FromImage(bitmap))
            {
                grphs.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                grphs.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                grphs.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                //grphs.DrawImage(img, 0, 0, bitmap.Width, bitmap.Height);
                grphs.DrawImage(img, new Rectangle(0, 0, towidth, toheight),
                    new Rectangle(x, y, ow, oh),
                    GraphicsUnit.Pixel);
            }
            return bitmap;
        }
        #endregion

        #region 產生jpg圖
        /// <summary>
        /// 產生jpg圖
        /// </summary>
        /// <param name="img">圖片物件</param>
        /// <returns>JPG圖片物件</returns>
        public Image MakeJpgImage(Image img)
        {
            int towidth = img.Width;
            int toheight = img.Height;

            int x = 0;
            int y = 0;

            Image bitmap = new System.Drawing.Bitmap(towidth, toheight);

            Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            g.Clear(Color.White);

            g.DrawImage(img, new Rectangle(0, 0, towidth, toheight),
                new Rectangle(x, y, towidth, toheight),
                GraphicsUnit.Pixel);
            img.Dispose();
            g.Dispose();
            return bitmap;
        }
        #endregion

        #region 儲存縮圖
        /// <summary>
        /// 儲存縮圖
        /// </summary>
        /// <param name="img">圖片物件</param>
        /// <param name="imgpath">儲存路徑</param>
        public void ImageSaveFile(Image img, string imgpath, string picxet)
        {
            try
            {


                EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);

                //Magic Code .開到三檔變成高畫質
                var encoderParams = new EncoderParameters(1);

                encoderParams.Param[0] = qualityParam;

                if (picxet.ToLower() == "gif")
                {
                    string lookupKey = "image/gif";
                    var jpegCodec = ImageCodecInfo.GetImageEncoders().Where(i => i.MimeType.Equals(lookupKey)).FirstOrDefault();

                    //以Gif格式保存縮略圖
                    img.Save(imgpath, jpegCodec, encoderParams);
                }
                if (picxet.ToLower() == "jpg" || picxet.ToLower() == "jpeg")
                {
                    string lookupKey = "image/jpeg";
                    var jpegCodec = ImageCodecInfo.GetImageEncoders().Where(i => i.MimeType.Equals(lookupKey)).FirstOrDefault();

                    //以Jpg格式保存縮略圖
                    img.Save(imgpath, jpegCodec, encoderParams);
                }
                if (picxet.ToLower() == "png")
                {
                    string lookupKey = "image/png";
                    var jpegCodec = ImageCodecInfo.GetImageEncoders().Where(i => i.MimeType.Equals(lookupKey)).FirstOrDefault();

                    //以Gif格式保存縮略圖
                    img.Save(imgpath, jpegCodec, encoderParams);
                }
            }
            catch (System.Exception ex)
            {
                System.IO.Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "errorlog");
                File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "errorlog" + Path.DirectorySeparatorChar + "[ImageThumber]" + DateTime.Now.ToString("yyyyMMdd hhmmss"), ex.Message + "\r\n" + ex.StackTrace);
            }
            finally
            {
                img.Dispose();
            }
        }
        #endregion


        #region 判斷縮圖模式
        /// <summary>
        /// 判斷縮圖模式
        /// </summary>
        /// <param name="img">圖片物件</param>
        /// <returns>模式</returns>
        public string DecisionThumbMode(Image img)
        {
            try
            {
                string mode = "";
                if (img.Width > img.Height)
                {
                    mode = "W";
                }
                else if (img.Width < img.Height)
                {
                    mode = "H";
                }
                else
                {
                    mode = "Cut";
                }
                return mode;
            }
            catch (Exception ex)
            {
                System.IO.Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "errorlog");
                File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "errorlog" + Path.DirectorySeparatorChar + "[ImageThumber]" + DateTime.Now.ToString("yyyyMMdd hhmmss"), ex.Message + "\r\n" + ex.StackTrace);
                return "";
            }
        }
        #endregion

        public Image MakeMerge(Image img1, Image img2, int width, int height, string mode)
        {
            int towidth = width;
            int toheight = img1.Height + img2.Height;

            int x = 0;
            int y = 0;
            int ow = img1.Width;
            int oh = img1.Height;
            int sw = img2.Width;
            int sh = img2.Height;

            switch (mode)
            {
                case "HW"://指定高寬縮放（可能變形）                
                    break;
                case "W"://指定寬，高按比例                    
                    toheight = img1.Height * width / img1.Width;
                    break;
                case "H"://指定高，寬按比例
                    towidth = img1.Width * height / img1.Height;
                    break;
                case "Cut"://指定高寬裁減（不變形）                
                    if ((double)img1.Width / (double)img1.Height > (double)towidth / (double)toheight)
                    {
                        oh = img1.Height;
                        ow = img1.Height * towidth / toheight;
                        y = 0;
                        x = (img1.Width - ow) / 2;
                    }
                    else
                    {
                        ow = img1.Width;
                        oh = img1.Width * height / towidth;
                        x = 0;
                        y = (img1.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }

            //新建一個bmp圖片
            Image bitmap = new System.Drawing.Bitmap(towidth, toheight + img2.Height);

            //新建一個畫板
            Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            //設定高品質插值法
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

            //設定高品質,低速度呈現平滑程度
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //清空畫布並以透明背景色填充
            g.Clear(Color.Empty);

            //在指定位置並且按指定大小繪制原圖片的指定部分
            g.DrawImage(img1, new Rectangle(0, 0, towidth, toheight),
                new Rectangle(x, y, ow, oh),
                GraphicsUnit.Pixel);
            g.DrawImage(img2, new Rectangle(0, toheight, towidth, sh),
                new Rectangle(x, y, sw, sh),
                GraphicsUnit.Pixel);

            img1.Dispose();
            img2.Dispose();
            g.Dispose();
            return bitmap;
        }

        /// <summary>
        /// 自動判斷圖片格式
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static System.Drawing.Imaging.ImageFormat GetImageFormat(System.Drawing.Image img)
        {
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
                return System.Drawing.Imaging.ImageFormat.Jpeg;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Bmp))
                return System.Drawing.Imaging.ImageFormat.Bmp;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Png))
                return System.Drawing.Imaging.ImageFormat.Png;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Emf))
                return System.Drawing.Imaging.ImageFormat.Emf;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Exif))
                return System.Drawing.Imaging.ImageFormat.Exif;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Gif))
                return System.Drawing.Imaging.ImageFormat.Gif;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Icon))
                return System.Drawing.Imaging.ImageFormat.Icon;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.MemoryBmp))
                return System.Drawing.Imaging.ImageFormat.MemoryBmp;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Tiff))
                return System.Drawing.Imaging.ImageFormat.Tiff;
            else
                return System.Drawing.Imaging.ImageFormat.Wmf;
        }
    }
}
