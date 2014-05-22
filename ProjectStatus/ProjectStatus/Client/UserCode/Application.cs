using System;
using System.Linq;
using System.IO;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using Microsoft.LightSwitch;
using Microsoft.LightSwitch.Framework.Client;
using Microsoft.LightSwitch.Presentation;
using Microsoft.LightSwitch.Presentation.Extensions;
using LightSwitchApplication;
namespace LightSwitchApplication
{
    public partial class Application
    {
        #if DEBUG
        public static string DefaultDomain = "localhost:21086";
        #else
        //public static string DefaultDomain = "jstftest.azurewebsites.net"; //To publish to TEST OTHERWISE
        public static string DefaultDomain = "jstf.azurewebsites.net"; //To publish to PROD
        //public static string DefaultDomainOneDrive= "jstftest.azurewebsites.net"; // 
        
        //public static string OneDrive = "onedrive.live.com/?cid=0dd5425313926582&id=DD5425313926582%215315&ithint=folder,.xlsx&authkey=!ANhJEq3_29bw_uw";
        #endif

        partial void Application_Initialize()
        {
            this.Details.ClientTimeout = 999999;
            //throw new NotImplementedException();
        }

        partial void EmployeeUpdate_CanRun(ref bool result)
        {
            result = User.HasPermission(Permissions.EmployeeAdmin);
        }

        partial void StatusBoardAllProjects_CanRun(ref bool result)
        {
            result = User.HasPermission(Permissions.ProjectAdminAll);
        }

        partial void StatusBoardFiltered_CanRun(ref bool result)
        {
            result = User.HasPermission(Permissions.MgmtReportAccess);
        }

        partial void TimeForecastScreen_CanRun(ref bool result)
        {
            result = User.HasPermission(Permissions.TimeForecastUser);
        }

        partial void StatusBoardReport_CanRun(ref bool result)
        {
            result = User.HasPermission(Permissions.MgmtReportAccess);
        }

        partial void EmployeeAssignReport_CanRun(ref bool result)
        {
            result = User.HasPermission(Permissions.MgmtReportAccess);
        }

        partial void OpenStaffingRequirementsReport_CanRun(ref bool result)
        {
            result = User.HasPermission(Permissions.MgmtReportAccess);
        }

        partial void UnassignedEmployeeReport_CanRun(ref bool result)
        {
            result = User.HasPermission(Permissions.MgmtReportAccess);
        }

        partial void ProjectExceptions_CanRun(ref bool result)
        {
            result = User.HasPermission(Permissions.ProjectAdminAll) & User.HasPermission(Permissions.TimeForecastAdmin);
        }

        partial void ManageHolidays_CanRun(ref bool result)
        {
            result = User.HasPermission(Permissions.EmployeeAdmin);
        }

        partial void TimeForecastReport_CanRun(ref bool result)
        {
            result = User.HasPermission(Permissions.MgmtReportAccess);
        }

        partial void ManageAnnouncements_CanRun(ref bool result)
        {
            result = User.HasPermission(Permissions.SecurityAdministration);
        }

        partial void EmployeeAssignmentDetailScreen_CanRun(ref bool result)
        {
            result = User.HasPermission(Permissions.EmpAssignAdmin) || User.HasPermission(Permissions.EmployeeAdmin);
        }

        partial void EvaluationCriteriaSetup_CanRun(ref bool result)
        {
            result = User.HasPermission(Permissions.ReviewSetupAdmin);
        }

        partial void ManageTrainingCourses_CanRun(ref bool result)
        {
            result = User.HasPermission(Permissions.ReviewSetupAdmin);
        }

        partial void ReviewListByEmployee_CanRun(ref bool result)
        {
            if (User.HasPermission(Permissions.CreateReviews) && User.HasPermission(Permissions.AccessReviewTab))
            {
                result = true;
            }
            else
            {
                result = false;
            }
        }

        partial void ManageMyReviews_CanRun(ref bool result)
        {
            result = User.HasPermission(Permissions.AccessReviewTab);
        }

        partial void ReviewSummaryReport_CanRun(ref bool result)
        {
            if (User.HasPermission(Permissions.ReviewAdmin) && User.HasPermission(Permissions.AccessReviewTab))
            {
                result = true;
            }
            else
            {
                result = false;
            }
        }

        partial void TrainingPlanReport_CanRun(ref bool result)
        {
            if (User.HasPermission(Permissions.ReviewAdmin) & User.HasPermission(Permissions.AccessReviewTab))
            {
                result = true;
            }
            else
            {
                result = false;
            }
        }

        partial void AnnualReviewStatus_CanRun(ref bool result)
        {
            result = User.HasPermission(Permissions.ReviewAdmin);
        }

        partial void ManageCountries_CanRun(ref bool result)
        {
            result = User.HasPermission(Permissions.EmployeeAdmin);

        }

        partial void DeleteAssignmentHistory_CanRun(ref bool result)
        {
            result = User.HasPermission(Permissions.ProjectAdminAll);
        }

        partial void EmployeeTitles_CanRun(ref bool result)
        {
            result = User.HasPermission(Permissions.EmployeeAdmin);
        }

        partial void ProjectRates_CanRun(ref bool result)
        {
            result = User.HasPermission(Permissions.ProjectAdminAll);
        }

        partial void GlobalConfiguration_CanRun(ref bool result)
        {
            result = User.HasPermission(Permissions.SecurityAdministration);
        }

        partial void RevenueForecast_CanRun(ref bool result)
        {
            result = User.HasPermission(Permissions.RevenueForecastUser);
        }

        partial void DODProjectsPortal_CanRun(ref bool result)
        {
            result = User.HasPermission(Permissions.DODSysAdmin)
                  || User.HasPermission(Permissions.DODUser)
                  || User.HasPermission(Permissions.DODManager);
        }

        partial void DODProjectDashboard_CanRun(ref bool result)
        {
            result = User.HasPermission(Permissions.DODSysAdmin)
                  || User.HasPermission(Permissions.DODManager)
                  || User.HasPermission(Permissions.DODDeveloper)
                  || User.HasPermission(Permissions.DODUser)
                  || User.HasPermission(Permissions.DODCustomer);
        }

        partial void DODTimeForecastReport_CanRun(ref bool result)
        {
            result = User.HasPermission(Permissions.DODSysAdmin)
                  || User.HasPermission(Permissions.DODManager)
                  || User.HasPermission(Permissions.DODUser);
        }

        partial void ManageMySkills_CanRun(ref bool result, string UserId)
        {
            result = (User.HasPermission(Permissions.DODSysAdmin)
                   || User.HasPermission(Permissions.DODManager)
                   || User.HasPermission(Permissions.DODDeveloper)
                   || User.HasPermission(Permissions.DODUser)
                   || User.HasPermission(Permissions.TimeForecastUser))
                   && !User.HasPermission(Permissions.DODCustomer);
        }

        partial void ManageSkills_CanRun(ref bool result)
        {
            result = !User.HasPermission(Permissions.DODCustomer);
        }

        partial void SkillsReport_CanRun(ref bool result)
        {
            result =  (User.HasPermission(Permissions.DODSysAdmin)
                   || User.HasPermission(Permissions.DODManager)
                   || User.HasPermission(Permissions.DODDeveloper)
                   || User.HasPermission(Permissions.DODUser)
                   || User.HasPermission(Permissions.TimeForecastUser))
                   && !User.HasPermission(Permissions.DODCustomer);
        }

        partial void Home_CanRun(ref bool result)
        {
            result = true;//!User.HasPermission(Permissions.DODCustomer);
        }

        partial void ManageClients_CanRun(ref bool result)
        {
            result = User.HasPermission(Permissions.SecurityAdministration);
        }

        partial void TimeTrackReportScreen_CanRun(ref bool result)
        {
            result = User.HasPermission(Permissions.TimeForecastUser);
        }

        partial void PTOReportScreen_CanRun(ref bool result)
        {
            result = User.HasPermission(Permissions.PTOReportUser);
        }

        partial void ManageMyAssignments_CanRun(ref bool result)
        {
           // result = User.HasPermission(Permissions.SecurityAdministration);
            result = User.HasPermission(Permissions.DODSysAdmin)
                 || User.HasPermission(Permissions.DODManager)
                 || User.HasPermission(Permissions.DODDeveloper)
                 || User.HasPermission(Permissions.DODUser);
              //   || User.HasPermission(Permissions.DODCustomer);
        }

        partial void ManageDepartments_CanRun(ref bool result)
        {
            // Set result to the desired field value
            result = User.HasPermission(Permissions.EmployeeAdmin);

        }

        partial void ManageLevels_CanRun(ref bool result)
        {
            // Set result to the desired field value
            result = User.HasPermission(Permissions.EmployeeAdmin);

        }

        partial void ReviewHeaderEdit_CanRun(ref bool result)
        {
            // Set result to the desired field value
            result = User.HasPermission(Permissions.EmployeeAdmin);
        }

        partial void AnnualReviewStatus1_CanRun(ref bool result)
        {
            // Set result to the desired field value---GG added
            result = User.HasPermission(Permissions.ReviewAdmin);

        }
    }

    
}
