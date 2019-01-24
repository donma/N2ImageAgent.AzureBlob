using System;
using System.IO;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace N2ImageAgent.AzureBlob.Pages
{
    public class imageModel : PageModel
    {
        private int _w;
        private int _h;

        public void OnGet(string id, string w, string h)
        {

            if (string.IsNullOrEmpty(id))
            {
                Response.Redirect("https://upload.wikimedia.org/wikipedia/commons/thumb/5/57/Blue_Screen_of_Death.png/800px-Blue_Screen_of_Death.png");
            }

            var res = BlobUtility.IsFileExisted(id + ".gif");

            if (res)
            {

                int.TryParse(w, out _w);
                int.TryParse(h, out _h);


                #region  原圖處理 : _w=0 , _h=0


                if (_w == 0 && _h == 0)
                {
                    Response.Redirect("/source/" + id);
                    return;
                }

                #endregion



                if (BlobUtility.IsFileExisted(id + ".gif", "thumbs/" + w + "_" + h))
                {
                    var path = "";
                    var para = "";
                    BlobUtility.GetUriAndPermission(id + ".gif", out path, out para, Startup.UserTokenLifeSeconds, "thumbs/" + w + "_" + h);
                    Response.Redirect(path + para);
                }

                #region  寬圖處理 : _w>0 , _h=0

                var source = BlobUtility.DownloadFileFromBlob(id + ".gif");
                var info = BlobUtility.ReadInfoFromBlob(id);

                var random = NUlid.Ulid.NewUlid().ToString().ToLower();

                System.IO.Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "thumbswap");
                if (_w > 0 && _h == 0)
                {
                    var thumbHandler = new ImageUtility();
                    var source2 = thumbHandler.MakeThumbnail(source, _w, _h, "W");
                    thumbHandler.ImageSaveFile(source2, AppDomain.CurrentDomain.BaseDirectory + "thumbswap" + Path.DirectorySeparatorChar + id + "_" + random + ".gif", info.Extension);
                    BlobUtility.UpoloadImage(id, AppDomain.CurrentDomain.BaseDirectory + "thumbswap" + Path.DirectorySeparatorChar + id + "_" + random + ".gif", "thumbs/" + w + "_" + h);
                    source.Dispose();
                    source2.Dispose();
                }

                #endregion


                #region  高圖處理 : _w=0 , _h>0

                else if (_w == 0 && _h > 0)
                {
                    var thumbHandler = new ImageUtility();
                    var source2 = thumbHandler.MakeThumbnail(source, _w, _h, "H");
                    thumbHandler.ImageSaveFile(source2, AppDomain.CurrentDomain.BaseDirectory + "thumbswap" + Path.DirectorySeparatorChar + id + "_" + random + ".gif", info.Extension);
                    BlobUtility.UpoloadImage(id, AppDomain.CurrentDomain.BaseDirectory + "thumbswap" + Path.DirectorySeparatorChar + id + "_" + random + ".gif", "thumbs/" + w + "_" + h);
                    source.Dispose();
                    source2.Dispose();

                }

                #endregion

                #region  強迫處理 : _w>0 , _h>0

                else if (_w > 0 && _h > 0)
                {
                    var thumbHandler = new ImageUtility();
                    var source2 = thumbHandler.MakeThumbnail(source, _w, _h, "WH");
                    thumbHandler.ImageSaveFile(source2, AppDomain.CurrentDomain.BaseDirectory + "thumbswap" + Path.DirectorySeparatorChar + id + "_" + random + ".gif", info.Extension);
                    BlobUtility.UpoloadImage(id, AppDomain.CurrentDomain.BaseDirectory + "thumbswap" + Path.DirectorySeparatorChar + id + "_" + random + ".gif", "thumbs/" + w + "_" + h);
                    source.Dispose();
                    source2.Dispose();
                }


                var tmpPath = "";
                var tmpPara = "";
                BlobUtility.GetUriAndPermission(id + ".gif", out tmpPath, out tmpPara, Startup.UserTokenLifeSeconds, "thumbs/" + w + "_" + h);

                System.IO.File.Delete(AppDomain.CurrentDomain.BaseDirectory + "thumbswap" + Path.DirectorySeparatorChar + id + "_" + random + ".gif");
                Response.Redirect(tmpPath + tmpPara);

                #endregion

            }
            else
            {
                Response.Redirect("https://upload.wikimedia.org/wikipedia/commons/thumb/a/ac/No_image_available.svg/600px-No_image_available.svg.png");
            }


        }
    }
}