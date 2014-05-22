using Microsoft.Data.OData;
using System.Data.Services.Client;

namespace LightSwitchApplication.Implementation
{

    public partial class ApplicationData : global::Microsoft.LightSwitch.ClientGenerated.Implementation.DataServiceContext
    {
        partial void OnContextCreated()
        {
            this.SendingRequest2 += ApplicationData_SendingRequest2;
        }

        private void ApplicationData_SendingRequest2(object sender, SendingRequest2EventArgs e)
        {
            IODataRequestMessage requestMessage = (IODataRequestMessage)e.RequestMessage;
            if (requestMessage != null)
            {
                requestMessage.SetHeader("MinDataServiceVersion", "3.0");
            }

        }


    }

}
