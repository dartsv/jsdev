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
    public class RdcController : ApiController
    {

        public HttpResponse Get()
        {
            string tmpFilePath = System.IO.Path.GetRandomFileName();
            string generatedFileName = "env.rdp";
            string tmpFolder = "shortcuts";
            string fileExtension = ".rdp";

            //string decodedPath = HttpUtility.UrlDecode(path).Replace("=>","\\");

            StringBuilder localFilePath = new StringBuilder();
            localFilePath.AppendFormat("/{0}/{1}.{2}", tmpFolder, tmpFilePath, fileExtension);

            //You could omit these lines here as you may
            //not want to save the textfile to the server
            //I have just left them here to demonstrate that you could create the text file 
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(HttpContext.Current.Server.MapPath(localFilePath.ToString())))
            {
                //sw.WriteLine("[InternetShortcut]");
                //sw.WriteLine("URL=file:///C:/Users/" + user + "/Dropbox/" + decodedPath);
                sw.Write(@"screen mode id:i:2
use multimon:i:0
desktopwidth:i:1920
desktopheight:i:1080
session bpp:i:32
winposstr:s:0,3,0,0,800,600
compression:i:1
keyboardhook:i:2
audiocapturemode:i:0
videoplaybackmode:i:1
connection type:i:7
networkautodetect:i:1
bandwidthautodetect:i:1
displayconnectionbar:i:1
username:s:cmartinez
enableworkspacereconnect:i:0
disable wallpaper:i:0
allow font smoothing:i:0
allow desktop composition:i:0
disable full window drag:i:1
disable menu anims:i:1
disable themes:i:0
disable cursor setting:i:0
bitmapcachepersistenable:i:1
full address:s:166.78.226.226:2712
audiomode:i:0
redirectprinters:i:1
redirectcomports:i:0
redirectsmartcards:i:1
redirectclipboard:i:1
redirectposdevices:i:0
drivestoredirect:s:
autoreconnection enabled:i:1
authentication level:i:2
prompt for credentials:i:0
negotiate security layer:i:1
remoteapplicationmode:i:0
alternate shell:s:
shell working directory:s:
gatewayhostname:s:
gatewayusagemethod:i:4
gatewaycredentialssource:i:4
gatewayprofileusagemethod:i:0
promptcredentialonce:i:0
use redirection server name:i:0
rdgiskdcproxy:i:0
kdcproxyname:s:");
                sw.Close();
            }

            System.IO.FileStream fs = null;
            fs = System.IO.File.Open(HttpContext.Current.Server.MapPath(localFilePath.ToString()), System.IO.FileMode.Open);
            byte[] btFile = new byte[fs.Length];
            fs.Read(btFile, 0, Convert.ToInt32(fs.Length));
            fs.Close();

            System.IO.File.Delete(HttpContext.Current.Server.MapPath(localFilePath.ToString()));

            HttpResponse Response = HttpContext.Current.Response;
            Response.AddHeader("Content-disposition", "attachment; filename=" + generatedFileName);
            Response.ContentType = "application/rdp";
            Response.BinaryWrite(btFile);
            Response.End();

            return Response;
        }
    }
}