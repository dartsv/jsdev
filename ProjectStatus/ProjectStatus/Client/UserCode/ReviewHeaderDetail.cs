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
    public partial class ReviewHeaderDetail
    {
        partial void ReviewHeader_Loaded(bool succeeded)
        {
            // Write your code here.
            this.SetDisplayNameFromEntity(this.ReviewHeader);
        }

        partial void ReviewHeader_Changed()
        {
            // Write your code here.
            this.SetDisplayNameFromEntity(this.ReviewHeader);
        }

        partial void ReviewHeaderDetail_Saved()
        {
            // Write your code here.
            this.SetDisplayNameFromEntity(this.ReviewHeader);
        }

        partial void ReviewHeaderDetail_InitializeDataWorkspace(List<IDataService> saveChangesTo)
        {
            if (!this.ReviewHeaderId.HasValue)
            {
                this.ReviewHeaderId = new ReviewHeader().Id;
            }
            this.ReviewHeader.Employee.Id = EmployeeIDParameter;
        }
    }
}