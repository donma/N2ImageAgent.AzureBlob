using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace N2ImageAgent.AzureBlob.Pages
{
    public class infoModel : PageModel
    {
        public void OnGet(string id)
        {


            if (string.IsNullOrEmpty(id))
            {
                Response.Body.Write(System.Text.Encoding.UTF8.GetBytes(
                    JsonConvert.SerializeObject(new Models.ImageInfo { Id = "ERROR", Tag = "NULL ID" })
                    ));
            }
            else
            {
                var res = BlobUtility.IsFileExisted(id + ".gif");

                if (res)
                {
                    try
                    {
                        var info = BlobUtility.ReadInfoFromBlob(id);
                        Response.Body.Write(System.Text.Encoding.UTF8.GetBytes(
                   JsonConvert.SerializeObject(info)
                   ));
                    }
                    catch (Exception ex)
                    {
                        Response.Body.Write(System.Text.Encoding.UTF8.GetBytes(
                   JsonConvert.SerializeObject(new Models.ImageInfo { Id = "ERROR", Tag = ex.Message.Replace("\"", "").Replace("'", "") })
                   ));
                    }
                }
                else
                {
                    Response.Body.Write(System.Text.Encoding.UTF8.GetBytes(
                    JsonConvert.SerializeObject(new Models.ImageInfo { Id = "ERROR", Tag = "NOT EXISTED" })
                    ));
                }

            }

        }
    }
}