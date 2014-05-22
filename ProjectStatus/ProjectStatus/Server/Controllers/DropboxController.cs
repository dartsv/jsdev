using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using System.Text;

namespace LightSwitchApplication.Controllers
{
    public class DropboxController : ApiController
    {

        public HttpResponse Get(string user, string path)
        {
            string tmpFilePath = System.IO.Path.GetRandomFileName();
            string generatedFileName = "shortcut.url";
            string tmpFolder = "shortcuts";
            string fileExtension = ".url";

            string decodedPath = HttpUtility.UrlDecode(path).Replace("=>","\\");

            StringBuilder localFilePath = new StringBuilder();
            localFilePath.AppendFormat("/{0}/{1}.{2}", tmpFolder, tmpFilePath, fileExtension);

            //You could omit these lines here as you may
            //not want to save the textfile to the server
            //I have just left them here to demonstrate that you could create the text file 
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(HttpContext.Current.Server.MapPath( localFilePath.ToString() )))
            {
                sw.WriteLine("[InternetShortcut]");
                sw.WriteLine("URL=file:///C:/Users/" + user +"/Dropbox/" + decodedPath);
                sw.Close();
            }

            System.IO.FileStream fs = null;
            fs = System.IO.File.Open(HttpContext.Current.Server.MapPath( localFilePath.ToString() ), System.IO.FileMode.Open);
            byte[] btFile = new byte[fs.Length];
            fs.Read(btFile, 0, Convert.ToInt32(fs.Length));
            fs.Close();

            System.IO.File.Delete(HttpContext.Current.Server.MapPath( localFilePath.ToString() ));

            HttpResponse Response = HttpContext.Current.Response;
            Response.AddHeader("Content-disposition", "attachment; filename=" +generatedFileName);
            Response.ContentType = "application/internet-shortcut";
            Response.BinaryWrite(btFile);
            Response.End();

            return Response;
        }
    }
}