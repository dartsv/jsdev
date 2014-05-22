using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.LightSwitch.Threading;

namespace LightSwitchApplication.UIHelpers
{
    public class WindowHelper
    {
        public static string BaseOnTimeUrl = "http://ontime/Ontime2013Web/ViewItem.aspx";

        public static void OpenInOntime(string type, string onTimeId, bool inNewWindow = true)
        {
            Dispatchers.Main.BeginInvoke(() =>
                {
                    string url = String.Format("{0}{1}{2}",
                        BaseOnTimeUrl,
                        "?type=" + type,
                        "&id=" + onTimeId);

                    Uri uri = new Uri(url);

                    string target = inNewWindow ? "_blank" : "_self";

                    System.Windows.Browser.HtmlPage.Window.Navigate(uri, target);
                });
        }
    }
}
