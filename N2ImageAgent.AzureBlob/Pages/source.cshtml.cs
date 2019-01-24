using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace N2ImageAgent.AzureBlob.Pages
{
    public class sourceModel : PageModel
    {
        public void OnGet(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Response.Redirect("https://upload.wikimedia.org/wikipedia/commons/thumb/5/57/Blue_Screen_of_Death.png/800px-Blue_Screen_of_Death.png");
            }
            else
            {
                var res = BlobUtility.IsFileExisted(id + ".gif");

                if (res)
                {
                    var path = "";
                    var para = "";
                    BlobUtility.GetUriAndPermission(id + ".gif", out path, out para, Startup.UserTokenLifeSeconds);
                    Response.Redirect(path + para);
                }
                else
                {
                    Response.Redirect("https://upload.wikimedia.org/wikipedia/commons/thumb/a/ac/No_image_available.svg/600px-No_image_available.svg.png");
                }

            }

        }
    }
}