using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.LightSwitch;
namespace LightSwitchApplication
{
    public partial class TrainingPlan
    {
        partial void CourseName_Compute(ref string result)
        {
            if (TrainingCourses != null)   result = TrainingCourses.CourseName;
        }

        partial void CourseAbstract_Compute(ref string result)
        {
            if (TrainingCourses != null) result = TrainingCourses.CourseAbstract;
        }

        partial void CourseDays_Compute(ref int result)
        {
            if (TrainingCourses != null) result = TrainingCourses.Days;
        }

        partial void CourseCost_Compute(ref decimal result)
        {
            if (TrainingCourses != null)
            {
                if (TrainingCourses.Cost != null) result = (decimal)TrainingCourses.Cost;
            }

        }

        partial void CourseProvider_Compute(ref string result)
        {
            if (TrainingCourses != null) result = TrainingCourses.Provider;
        }

        partial void TrainingPlan_Created()
        {
            TargetCompletionDate = new DateTime(2014, 5, 31);
        }

              
    }
}
