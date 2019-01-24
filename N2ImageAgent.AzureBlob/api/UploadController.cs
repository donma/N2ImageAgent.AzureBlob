using System;
using System.Drawing;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace N2ImageAgent.AzureBlob.api
{
    [Route("api/upload")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "Snorlax Core";

        }

        [HttpPost]
        public string Post([FromForm]string token, [FromForm]string tag, [FromForm]string filename, IFormFile file)
        {
            if (string.IsNullOrEmpty(token))
            {
                return "error:token null";
            }

            if (token != Startup.ServerToekn)
            {
                return "error:token error";

            }

            var myulid = NUlid.Ulid.NewUlid();
            string newFileName = myulid.ToString();
            if (!string.IsNullOrEmpty(filename))
            {
                newFileName = filename;
            }
            newFileName = newFileName.ToLower();

            Stream readStream = file.OpenReadStream();

            byte[] fileData = new byte[file.Length];

            readStream.Read(fileData, 0, fileData.Length);


            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "swapupload");
            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "info");

            System.IO.File.WriteAllBytes(AppDomain.CurrentDomain.BaseDirectory + "swapupload" + Path.DirectorySeparatorChar + newFileName + ".gif", fileData);
            readStream.Dispose();
            Image source;
            try
            {
                source = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "swapupload" + Path.DirectorySeparatorChar + newFileName + ".gif");
            }
            catch
            {
                return "error:not image source";
            }

            try
            {
                //JSON
                var imgInfo = new Models.ImageInfo();
                imgInfo.Id = newFileName;
                imgInfo.Width = source.Width;
                imgInfo.Height = source.Height;
                imgInfo.Extension = ImageUtility.GetImageFormat(source).ToString().ToLower();

                if (!string.IsNullOrEmpty(tag))
                {
                    imgInfo.Tag = tag;
                }
                System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "info" + Path.DirectorySeparatorChar + newFileName + ".json", JsonConvert.SerializeObject(imgInfo));
                source.Dispose();

            }
            catch (Exception ex)
            {
                return "error:" + ex.Message.Replace("\"", "").Replace("'", "");
            }

            try
            {
                BlobUtility.UpoloadImageSource(AppDomain.CurrentDomain.BaseDirectory + "swapupload" + Path.DirectorySeparatorChar + newFileName + ".gif", newFileName);
                BlobUtility.UpoloadImageInfoSource(AppDomain.CurrentDomain.BaseDirectory + "info" + Path.DirectorySeparatorChar + newFileName + ".json", newFileName);
            }
            catch (Exception ex)
            {
                return "error:" + ex.Message.Replace("\"", "").Replace("'", "");
            }
            try
            {
                System.IO.File.Delete(AppDomain.CurrentDomain.BaseDirectory + "swapupload" + Path.DirectorySeparatorChar + newFileName + ".gif");
                System.IO.File.Delete(AppDomain.CurrentDomain.BaseDirectory + "info" + Path.DirectorySeparatorChar + newFileName + ".json");

            }
            catch (Exception ex)
            {
                return "error:" + ex.Message.Replace("\"", "").Replace("'", "");
            }


            return "success:" + newFileName;
        }


    }
}