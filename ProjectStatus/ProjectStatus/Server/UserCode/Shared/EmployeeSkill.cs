using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.LightSwitch;
namespace LightSwitchApplication
{
    public partial class EmployeeSkill
    {
        partial void SkillCategoryName_Compute(ref string result)
        {
            if(this.Skill == null) result = "-";
            else{
                result = (this.Skill.SkillCategory == null) ? "-" : this.Skill.SkillCategory.Name;
            }
        }
    }
}
