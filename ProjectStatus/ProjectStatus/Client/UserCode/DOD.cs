﻿using System;
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
    public partial class DOD
    {
        partial void SkillCategories_SelectionChanged()
        {
            if (this.SkillCategories.SelectedItem != null)
            {
                //this.SkillFilter.SkillCategory = this.SkillCategories.SelectedItem;
            }
        }
    }
}
