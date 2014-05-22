using System;
using System.Linq;
using System.IO;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using Microsoft.LightSwitch;
using Microsoft.LightSwitch.Framework.Client;
using Microsoft.LightSwitch.Presentation;
using Microsoft.LightSwitch.Presentation.Extensions;
namespace LightSwitchApplication
{
    public partial class ProjectRates
    {

        partial void ProjectRates_InitializeDataWorkspace(List<IDataService> saveChangesTo)
        {
            if (this.ConfigurationSet.Count == 0)
            {
                Configuration conf = this.DataWorkspace.ApplicationData.Configurations.AddNew();
                conf.Value = "0";
                conf.ValueType = "decimal";
                conf.Key = "DefaultRate";
                //this.DataWorkspace.ApplicationData.SaveChanges();
            }
            this.DefaultRate = Decimal.Parse(this.ConfigurationSet.First(x => x.Key == "DefaultRate").Value);
            this.DataWorkspace.ApplicationData.SaveChanges();
        }

        partial void DefaultRate_Changed()
        {
            this.DataWorkspace.ApplicationData.Configurations.Where(x => x.Key == "DefaultRate").First().Value = this.DefaultRate.ToString();
        }

    }
}
