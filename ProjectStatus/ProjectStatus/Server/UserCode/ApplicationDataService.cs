using System.Linq;
using Microsoft.LightSwitch;
using System;
using System.Linq.Expressions;
using Microsoft.LightSwitch.Security;
using System.Drawing;
using System.IO;
using System.Web;

namespace LightSwitchApplication
{
    public partial class ApplicationDataService
    {

        partial void TimeForecasts_Validate(TimeForecast entity, EntitySetValidationResultsBuilder results)
        {

            if (Projects != null)
            {
                if (entity.EndDate < entity.StartDate)
                {
                    results.AddEntityError("End date must be greater than or equal to the start date.");
                }

                if (entity.EndDate > entity.Project.BookedEndDate)
                {
                    results.AddEntityError("Employee end date can not be after the Project End Date");
                }
            }
        }

        partial void FilterReviews_PreprocessQuery(string FilterTerm, ref IQueryable<ReviewHeader> query)
        {   //Code that filters the table with user values
            query = FilterControl.FilterExtensions.Filter(query, FilterTerm, this.Application);
        }

        #region SkillCategories
        partial void SkillCategories_CanDelete(ref bool result)
        {
            result = this.Application.User.HasPermission(Permissions.DODSysAdmin);
        }

        partial void SkillCategories_CanInsert(ref bool result)
        {
            result = this.Application.User.HasPermission(Permissions.DODSysAdmin);
        }

        partial void SkillCategories_CanUpdate(ref bool result)
        {
            result = this.Application.User.HasPermission(Permissions.DODSysAdmin);
        }
        #endregion
        #region Skills
        partial void Skills_CanInsert(ref bool result)
        {
            result = this.Application.User.HasPermission(Permissions.DODSysAdmin);
        }

        partial void Skills_CanDelete(ref bool result)
        {
            result = this.Application.User.HasPermission(Permissions.DODSysAdmin);
        }

        partial void Skills_CanUpdate(ref bool result)
        {
            result = this.Application.User.HasPermission(Permissions.DODSysAdmin);
        }
        #endregion

        #region Projects
        partial void Projects_Filter(ref Expression<Func<Project, bool>> filter)
        {
            IUser u = this.Application.User;
            string userId = u.Name;

            if (u.HasPermission(Permissions.DODCustomer))
            {
                filter = p => p.Customers.Any(c => c.UserID == userId);
            }
            /*if (u.HasPermission(Permissions.DODDeveloper) && !u.HasPermission(Permissions.DODSysAdmin))
            {
                filter = p => p.Director.UserID == userId || p.Manager.UserID == userId ||
                    p.EmployeeAssignment.Any(ea => ea.Employee.UserID == userId) || p.ForecastException == true;
            }*/
        }
        partial void Projects_CanDelete(ref bool result)
        {
            IUser u = this.Application.User;
            result = u.HasPermission(Permissions.ProjectAdminAll)
                || u.HasPermission(Permissions.DODManager);
        }
        partial void Projects_CanInsert(ref bool result)
        {
            IUser u = this.Application.User;
            result = u.HasPermission(Permissions.ProjectAdminAll)
                || u.HasPermission(Permissions.DODManager);
        }
        partial void Projects_CanUpdate(ref bool result)
        {
            IUser u = this.Application.User;
            result =
                u.HasPermission(Permissions.ProjectAdmin)
                || u.HasPermission(Permissions.ProjectAdminAll)
                || u.HasPermission(Permissions.DODManager);
        }
        #endregion
        #region ProjectRates
        partial void ProjectRates_CanDelete(ref bool result)
        {
            IUser u = this.Application.User;
            result =
                u.HasPermission(Permissions.ProjectAdmin)
                || u.HasPermission(Permissions.DODManager)
                || u.HasPermission(Permissions.ProjectAdminAll);
        }
        partial void ProjectRates_CanInsert(ref bool result)
        {
            IUser u = this.Application.User;
            result =
                u.HasPermission(Permissions.ProjectAdmin)
                || u.HasPermission(Permissions.DODManager)
                || u.HasPermission(Permissions.ProjectAdminAll);
        }
        partial void ProjectRates_CanUpdate(ref bool result)
        {
            IUser u = this.Application.User;
            result =
                u.HasPermission(Permissions.ProjectAdmin)
                || u.HasPermission(Permissions.DODManager)
                || u.HasPermission(Permissions.ProjectAdminAll);
        }
        #endregion

        #region ProjectVpns
        partial void ProjectVpns_CanDelete(ref bool result)
        {
            IUser u = this.Application.User;
            result = u.HasPermission(Permissions.DODSysAdmin)
                || u.HasPermission(Permissions.DODManager)
                || u.HasPermission(Permissions.DODUser)
                || u.HasPermission(Permissions.DODDeveloper);
        }
        partial void ProjectVpns_CanInsert(ref bool result)
        {
            IUser u = this.Application.User;
            result = u.HasPermission(Permissions.DODSysAdmin)
                || u.HasPermission(Permissions.DODManager)
                || u.HasPermission(Permissions.DODUser)
                || u.HasPermission(Permissions.DODDeveloper);
        }
        partial void ProjectVpns_CanUpdate(ref bool result)
        {
            IUser u = this.Application.User;
            result = u.HasPermission(Permissions.DODSysAdmin)
                || u.HasPermission(Permissions.DODManager)
                || u.HasPermission(Permissions.DODUser)
                || u.HasPermission(Permissions.DODDeveloper);
        }
        #endregion
        #region ProjectWebTools
        partial void ProjectWebTools_CanDelete(ref bool result)
        {
            IUser u = this.Application.User;
            result = u.HasPermission(Permissions.DODSysAdmin)
                || u.HasPermission(Permissions.DODManager)
                || u.HasPermission(Permissions.DODUser)
                || u.HasPermission(Permissions.DODDeveloper);
        }

        partial void ProjectWebTools_CanInsert(ref bool result)
        {
            IUser u = this.Application.User;
            result = u.HasPermission(Permissions.DODSysAdmin)
                || u.HasPermission(Permissions.DODManager)
                || u.HasPermission(Permissions.DODUser)
                || u.HasPermission(Permissions.DODDeveloper);
        }

        partial void ProjectWebTools_CanUpdate(ref bool result)
        {
            IUser u = this.Application.User;
            result = u.HasPermission(Permissions.DODSysAdmin)
                || u.HasPermission(Permissions.DODManager)
                || u.HasPermission(Permissions.DODUser)
                || u.HasPermission(Permissions.DODDeveloper);
        }
        #endregion
        #region ProjectRules
        partial void ProjectRules_CanDelete(ref bool result)
        {
            IUser u = this.Application.User;
            result = u.HasPermission(Permissions.DODSysAdmin)
                || u.HasPermission(Permissions.DODManager)
                || u.HasPermission(Permissions.DODUser)
                || u.HasPermission(Permissions.DODDeveloper);
        }
        partial void ProjectRules_CanInsert(ref bool result)
        {
            IUser u = this.Application.User;
            result = u.HasPermission(Permissions.DODSysAdmin)
                || u.HasPermission(Permissions.DODManager)
                || u.HasPermission(Permissions.DODUser)
                || u.HasPermission(Permissions.DODDeveloper);
        }
        partial void ProjectRules_CanUpdate(ref bool result)
        {
            IUser u = this.Application.User;
            result = u.HasPermission(Permissions.DODSysAdmin)
                || u.HasPermission(Permissions.DODManager)
                || u.HasPermission(Permissions.DODUser)
                || u.HasPermission(Permissions.DODDeveloper);
        }
        #endregion
        #region ProjectEnvironments
        partial void ProjectEnvironments_CanDelete(ref bool result)
        {
            IUser u = this.Application.User;
            result = u.HasPermission(Permissions.DODSysAdmin)
                || u.HasPermission(Permissions.DODManager)
                || u.HasPermission(Permissions.DODUser)
                || u.HasPermission(Permissions.DODDeveloper);
        }
        partial void ProjectEnvironments_CanInsert(ref bool result)
        {
            IUser u = this.Application.User;
            result = u.HasPermission(Permissions.DODSysAdmin)
                || u.HasPermission(Permissions.DODManager)
                || u.HasPermission(Permissions.DODUser)
                || u.HasPermission(Permissions.DODDeveloper);
        }
        partial void ProjectEnvironments_CanUpdate(ref bool result)
        {
            IUser u = this.Application.User;
            result = u.HasPermission(Permissions.DODSysAdmin)
                || u.HasPermission(Permissions.DODManager)
                || u.HasPermission(Permissions.DODUser)
                || u.HasPermission(Permissions.DODDeveloper);
        }
        #endregion

        #region Customers
        partial void Customers_CanDelete(ref bool result)
        {
            IUser u = this.Application.User;
            result = u.HasPermission(Permissions.DODSysAdmin)
                || u.HasPermission(Permissions.DODManager)
                || u.HasPermission(Permissions.DODUser);
        }
        partial void Customers_CanUpdate(ref bool result)
        {
            IUser u = this.Application.User;
            result = u.HasPermission(Permissions.DODSysAdmin)
                || u.HasPermission(Permissions.DODManager)
                || u.HasPermission(Permissions.DODUser);
        }
        partial void Customers_CanInsert(ref bool result)
        {
            IUser u = this.Application.User;
            result = u.HasPermission(Permissions.DODSysAdmin)
                || u.HasPermission(Permissions.DODManager)
                || u.HasPermission(Permissions.DODUser);
        }
        #endregion

        #region ProjectFeatures
        partial void ProjectFeatures_CanDelete(ref bool result)
        {
            IUser u = this.Application.User;
            result = u.HasPermission(Permissions.DODSysAdmin)
                || u.HasPermission(Permissions.DODManager)
                || u.HasPermission(Permissions.DODUser)
                || u.HasPermission(Permissions.DODDeveloper);
        }
        partial void ProjectFeatures_CanInsert(ref bool result)
        {
            IUser u = this.Application.User;
            result = u.HasPermission(Permissions.DODSysAdmin)
                || u.HasPermission(Permissions.DODManager)
                || u.HasPermission(Permissions.DODUser)
                || u.HasPermission(Permissions.DODDeveloper);
        }
        partial void ProjectFeatures_CanUpdate(ref bool result)
        {
            IUser u = this.Application.User;
            result = u.HasPermission(Permissions.DODSysAdmin)
                || u.HasPermission(Permissions.DODManager)
                || u.HasPermission(Permissions.DODUser)
                || u.HasPermission(Permissions.DODDeveloper);
        }

        partial void ProjectFeatures_Inserting(ProjectFeature entity)
        {
            entity.LastUpdate = DateTime.Now;
        }

        partial void ProjectFeatures_Updating(ProjectFeature entity)
        {
            entity.LastUpdate = DateTime.Now;
        }
        #endregion
        #region ProjectFeatureComments
        partial void ProjectFeatureComments_CanDelete(ref bool result)
        {
            IUser u = this.Application.User;
            result = u.HasPermission(Permissions.DODSysAdmin)
                || u.HasPermission(Permissions.DODManager)
                || u.HasPermission(Permissions.DODUser)
                || u.HasPermission(Permissions.DODDeveloper);
        }
        partial void ProjectFeatureComments_CanInsert(ref bool result)
        {
            IUser u = this.Application.User;
            result = u.HasPermission(Permissions.DODSysAdmin)
                || u.HasPermission(Permissions.DODManager)
                || u.HasPermission(Permissions.DODUser)
                || u.HasPermission(Permissions.DODDeveloper);
        }
        partial void ProjectFeatureComments_CanUpdate(ref bool result)
        {
            IUser u = this.Application.User;
            result = u.HasPermission(Permissions.DODSysAdmin)
                || u.HasPermission(Permissions.DODManager)
                || u.HasPermission(Permissions.DODUser)
                || u.HasPermission(Permissions.DODDeveloper);
        }
        partial void ProjectFeatureComments_Inserting(ProjectFeatureComment entity)
        {
            entity.PostDate = DateTime.Now;
            string userId = this.Application.User.Name;
            entity.Employee = this.Employees.Where(e => e.UserID == userId).FirstOrDefault();
        }
        partial void ProjectFeatureComments_Updating(ProjectFeatureComment entity)
        {
            entity.PostDate = DateTime.Now;
        }
        #endregion
        #region ProjectIncidents
        partial void ProjectIncidents_CanDelete(ref bool result)
        {
            IUser u = this.Application.User;
            result = u.HasPermission(Permissions.DODSysAdmin)
                || u.HasPermission(Permissions.DODManager)
                || u.HasPermission(Permissions.DODUser)
                || u.HasPermission(Permissions.DODDeveloper);
        }
        partial void ProjectIncidents_CanInsert(ref bool result)
        {
            IUser u = this.Application.User;
            result = u.HasPermission(Permissions.DODSysAdmin)
                || u.HasPermission(Permissions.DODManager)
                || u.HasPermission(Permissions.DODUser)
                || u.HasPermission(Permissions.DODDeveloper);
        }
        partial void ProjectIncidents_CanUpdate(ref bool result)
        {
            IUser u = this.Application.User;
            result = u.HasPermission(Permissions.DODSysAdmin)
                || u.HasPermission(Permissions.DODManager)
                || u.HasPermission(Permissions.DODUser)
                || u.HasPermission(Permissions.DODDeveloper);
        }
        partial void ProjectIncidents_Inserting(ProjectIncident entity)
        {
            entity.LastUpdate = DateTime.Now;
        }
        partial void ProjectIncidents_Updating(ProjectIncident entity)
        {
            entity.LastUpdate = DateTime.Now;
        }
        #endregion
        #region ProjectIncidentsComments
        partial void ProjectIncidentComments_CanDelete(ref bool result)
        {
            IUser u = this.Application.User;
            result = u.HasPermission(Permissions.DODSysAdmin)
                || u.HasPermission(Permissions.DODManager)
                || u.HasPermission(Permissions.DODUser)
                || u.HasPermission(Permissions.DODDeveloper);
        }
        partial void ProjectIncidentComments_CanInsert(ref bool result)
        {
            IUser u = this.Application.User;
            result = u.HasPermission(Permissions.DODSysAdmin)
                || u.HasPermission(Permissions.DODManager)
                || u.HasPermission(Permissions.DODUser)
                || u.HasPermission(Permissions.DODDeveloper);
        }
        partial void ProjectIncidentComments_CanUpdate(ref bool result)
        {
            IUser u = this.Application.User;
            result = u.HasPermission(Permissions.DODSysAdmin)
                || u.HasPermission(Permissions.DODManager)
                || u.HasPermission(Permissions.DODUser)
                || u.HasPermission(Permissions.DODDeveloper);
        }
        partial void ProjectIncidentComments_Inserting(ProjectIncidentComment entity)
        {
            entity.PostDate = DateTime.Now;
            string userId = this.Application.User.Name;
            entity.Employee = this.Employees.Where(e => e.UserID == userId).FirstOrDefault();
        }
        partial void ProjectIncidentComments_Updating(ProjectIncidentComment entity)
        {
            entity.PostDate = DateTime.Now;
        }
        #endregion
        #region ProjectTasks
        partial void ProjectTasks_CanDelete(ref bool result)
        {
            IUser u = this.Application.User;
            result = u.HasPermission(Permissions.DODSysAdmin)
                || u.HasPermission(Permissions.DODManager)
                || u.HasPermission(Permissions.DODUser)
                || u.HasPermission(Permissions.DODDeveloper);
        }
        partial void ProjectTasks_CanInsert(ref bool result)
        {
            IUser u = this.Application.User;
            result = u.HasPermission(Permissions.DODSysAdmin)
                || u.HasPermission(Permissions.DODManager)
                || u.HasPermission(Permissions.DODUser)
                || u.HasPermission(Permissions.DODDeveloper);
        }
        partial void ProjectTasks_CanUpdate(ref bool result)
        {
            IUser u = this.Application.User;
            result = u.HasPermission(Permissions.DODSysAdmin)
                || u.HasPermission(Permissions.DODManager)
                || u.HasPermission(Permissions.DODUser)
                || u.HasPermission(Permissions.DODDeveloper);
        }
        partial void ProjectTasks_Inserting(ProjectTask entity)
        {
            entity.LastUpdate = DateTime.Now;
        }
        partial void ProjectTasks_Updating(ProjectTask entity)
        {
            entity.LastUpdate = DateTime.Now;
        }
        #endregion
        #region ProjectTaskComments
        partial void ProjectTaskComments_CanDelete(ref bool result)
        {
            IUser u = this.Application.User;
            result = u.HasPermission(Permissions.DODSysAdmin)
                || u.HasPermission(Permissions.DODManager)
                || u.HasPermission(Permissions.DODUser)
                || u.HasPermission(Permissions.DODDeveloper);
        }
        partial void ProjectTaskComments_CanInsert(ref bool result)
        {
            IUser u = this.Application.User;
            result = u.HasPermission(Permissions.DODSysAdmin)
                || u.HasPermission(Permissions.DODManager)
                || u.HasPermission(Permissions.DODUser)
                || u.HasPermission(Permissions.DODDeveloper);
        }
        partial void ProjectTaskComments_CanUpdate(ref bool result)
        {
            IUser u = this.Application.User;
            result = u.HasPermission(Permissions.DODSysAdmin)
                || u.HasPermission(Permissions.DODManager)
                || u.HasPermission(Permissions.DODUser)
                || u.HasPermission(Permissions.DODDeveloper);
        }
        partial void ProjectTaskComments_Inserting(ProjectTaskComment entity)
        {
            entity.PostDate = DateTime.Now;
            string userId = this.Application.User.Name;
            entity.Employee = this.Employees.Where(e => e.UserID == userId).FirstOrDefault();
        }
        partial void ProjectTaskComments_Updating(ProjectTaskComment entity)
        {
            entity.PostDate = DateTime.Now;
        }
        #endregion

        #region EmployeeAssignments
        partial void EmployeeAssignmentsByName_PreprocessQuery(ref IQueryable<EmployeeAssignment> query)
        {
            query = query.OrderBy(proj => proj.Employee.LastName);
        }
        partial void AssignedProjects_PreprocessQuery(ref IQueryable<Project> query)
        {
            if (this.Application.User.HasPermission(Permissions.DODUser) && !this.Application.User.HasPermission(Permissions.DODSysAdmin))
                query = query.Where(e => e.EmployeeAssignment.Any(ea => ea.Employee.UserID == this.Application.User.Name));
        }
        partial void EmployeeAssignments_CanDelete(ref bool result)
        {
            IUser u = this.Application.User;
            result =
                u.HasPermission(Permissions.ProjectAdmin)
                || u.HasPermission(Permissions.ProjectAdminAll)
                || u.HasPermission(Permissions.DODManager);
        }
        partial void EmployeeAssignments_CanUpdate(ref bool result)
        {
            IUser u = this.Application.User;
            result =
                u.HasPermission(Permissions.ProjectAdmin)
                || u.HasPermission(Permissions.ProjectAdminAll)
                || u.HasPermission(Permissions.DODManager);
        }
        partial void EmployeeAssignments_CanInsert(ref bool result)
        {
            IUser u = this.Application.User;
            result =
                u.HasPermission(Permissions.ProjectAdmin)
                || u.HasPermission(Permissions.ProjectAdminAll)
                || u.HasPermission(Permissions.DODManager);
        }
        #endregion
        #region StaffingRequirements
        partial void StaffingRequirements_CanDelete(ref bool result)
        {
            IUser u = this.Application.User;
            result =
                u.HasPermission(Permissions.ProjectAdmin)
                || u.HasPermission(Permissions.ProjectAdminAll)
                || u.HasPermission(Permissions.DODManager);
        }
        partial void StaffingRequirements_CanUpdate(ref bool result)
        {
            IUser u = this.Application.User;
            result =
                u.HasPermission(Permissions.ProjectAdmin)
                || u.HasPermission(Permissions.ProjectAdminAll)
                || u.HasPermission(Permissions.DODManager);
        }
        partial void StaffingRequirements_CanInsert(ref bool result)
        {
            IUser u = this.Application.User;
            result =
                u.HasPermission(Permissions.ProjectAdmin)
                || u.HasPermission(Permissions.ProjectAdminAll)
                || u.HasPermission(Permissions.DODManager);
        }
        #endregion

        partial void DODProjects_PreprocessQuery(ref IQueryable<Project> query)
        {
            IUser u = this.Application.User;
            if (u.HasPermission(Permissions.DODDeveloper) && !u.HasPermission(Permissions.DODSysAdmin))
            {
                query = query.Where(e => e.EmployeeAssignment.Any(ea => ea.Employee.UserID == u.Name));
            }

            if (u.HasPermission(Permissions.DODUser) && !u.HasPermission(Permissions.DODSysAdmin))
            {
                query = query.Where(e => e.EmployeeAssignment.Any(ea => ea.Employee.UserID == u.Name));
            }
        }

        partial void Employees_Filter(ref Expression<Func<Employee, bool>> filter)
        {
            //This line makes it possible to not show employees marked as Inactive
            //filter = e => e.IsInactive == false;
        }

        partial void Projects_Updating(Project entity)
        {
            //TODO: Change this to a switch
            //TODO: Refactor as a method since it's used at insert and update
            if (entity.IsDOD)
            {
                if (!entity.ProjectWebTools.Any(t => t.Name.ToUpper() == "ONTIME"))
                {
                    this.AddProjectWebTool(entity, "OnTime", "/static/logos/ontime.jpg").Path = "http://ontime";
                }
                if (!entity.ProjectWebTools.Any(t => t.Name.ToUpper() == "ONEDRIVE"))
                {
                    this.AddProjectWebTool(entity, "OneDrive", "/static/logos/skydrive.jpg");
                }
                if (!entity.ProjectWebTools.Any(t => t.Name.ToUpper() == "CLIENT SHAREPOINT"))
                {
                    this.AddProjectWebTool(entity, "Client SharePoint", "/static/logos/sharepoint.png");
                }
                if (!entity.ProjectWebTools.Any(t => t.Name.ToUpper() == "JS SHAREPOINT"))
                {
                    if (String.IsNullOrEmpty(entity.SharepointURL))
                    {
                        this.AddProjectWebTool(entity, "JS SharePoint", "/static/logos/jssharepoint.jpg");
                    }
                    else
                    {
                        entity.ProjectWebTools.FirstOrDefault(pwt => pwt.Name.ToUpper() == "JS SHAREPOINT").Path = entity.SharepointURL;
                    }
                }

            }
        }

        private ProjectWebTool AddProjectWebTool(Project project, string name, string logoPath)
        {
            ProjectWebTool webtool = project.ProjectWebTools.AddNew();

            using (MemoryStream ms = new MemoryStream())
            {
                string relativePath = HttpContext.Current.Server.MapPath(logoPath);

                if (File.Exists(relativePath))
                {
                    Image logo = Image.FromFile(relativePath);
                    logo.Save(ms, logo.RawFormat);

                    webtool.Icon = ms.ToArray();
                }
                webtool.Name = name;
                webtool.Path = "Enter Link";
            }

            return webtool;
        }



        partial void Projects_Inserting(Project entity)
        {
            if (entity.IsDOD)
            {
                if (!entity.ProjectWebTools.Any(t => t.Name.ToUpper() == "ONTIME"))
                {
                    this.AddProjectWebTool(entity, "OnTime", "/static/logos/ontime.jpg").Path = "http://ontime";
                }
                if (!entity.ProjectWebTools.Any(t => t.Name.ToUpper() == "ONEDRIVE"))
                {
                    this.AddProjectWebTool(entity, "OneDrive", "/static/logos/skydrive.jpg");
                }
                if (!entity.ProjectWebTools.Any(t => t.Name.ToUpper() == "CLIENT SHAREPOINT"))
                {
                    this.AddProjectWebTool(entity, "Client SharePoint", "/static/logos/sharepoint.png");
                }
                if (!entity.ProjectWebTools.Any(t => t.Name.ToUpper() == "JS SHAREPOINT"))
                {
                    if (String.IsNullOrEmpty(entity.SharepointURL))
                    {
                        this.AddProjectWebTool(entity, "JS SharePoint", "/static/logos/jssharepoint.jpg");
                    }
                    else
                    {
                        entity.ProjectWebTools.FirstOrDefault(pwt => pwt.Name.ToUpper() == "JS SHAREPOINT").Path = entity.SharepointURL;
                    }
                }

            }
        }

        partial void EmployeeProjects_PreprocessQuery(int? EmployeeId, ref IQueryable<Project> query)
        {
            query = query.Where(p => p.EmployeeAssignment.Any(ea => ea.Employee.Id == EmployeeId) || p.Manager.Id == EmployeeId || p.Director.Id == EmployeeId);
        }


        #region Requirements
        partial void Requirements_Inserting(Requirement entity)
        {
            entity.LastUpdate = DateTime.Now;
        }
        partial void Requirements_Updating(Requirement entity)
        {
            entity.LastUpdate = DateTime.Now;
        }
        #endregion

        #region Issues
        partial void Issues_Inserting(Issue entity)
        {
            entity.LastUpdate = DateTime.Now;
        }

        partial void Issues_Updating(Issue entity)
        {
            entity.LastUpdate = DateTime.Now;
        }
        #endregion

        partial void TimeTracks_Inserting(TimeTrack entity)
        {
            if (entity.Issue != null) entity.IsForIssue = true;
        }

        partial void TimeTracks_Updating(TimeTrack entity)
        {
            if (entity.Issue != null) entity.IsForIssue = true;
        }

        partial void Employees_Deleting(Employee entity)
        {

        }

        partial void ESEmployees_PreprocessQuery(int? Country, ref IQueryable<Employee> query)
        {

        }

        partial void SortBySupervisor_PreprocessQuery(ref IQueryable<Employee> query)
        {
            query = query.OrderBy(Employee => Employee.Supervsr.LastName);
        }

        partial void Employees_Inserting(Employee entity) //GG added
        {
      //entity.SupervisorFullNameSorting = entity.FirstName + " "+ entity.LastName;
            entity.SupervisorFullNameSorting = entity.Supervsr.FirstName + " " + entity.Supervsr.LastName;  
        }

        partial void Employees_Updating(Employee entity) // GG added
        {

            entity.SupervisorFullNameSorting = entity.Supervsr.FirstName + " " + entity.Supervsr.LastName;
        }

        partial void ReviewHeaders_Inserting(ReviewHeader entity)// GG added
        {
            entity.EmployeeReviewerFullName = entity.Reviewer.FirstName + " " + entity.Reviewer.LastName;
            entity.EmployeeSupervisorFullName = entity.SupervisorEmployee.FirstName + " " + entity.SupervisorEmployee.LastName;
        }

        partial void ReviewHeaders_Updating(ReviewHeader entity) //GGadded
        {
            entity.EmployeeReviewerFullName = entity.Reviewer.FirstName + " " + entity.Reviewer.LastName;
            entity.EmployeeSupervisorFullName = entity.SupervisorEmployee.FirstName + " " + entity.SupervisorEmployee.LastName;
        }

        partial void EvaluationCriteriasFilter_PreprocessQuery(string FilterTerm, ref IQueryable<EvaluationCriteria> query)
        {
            //Code that filters the table with user values....GG added
            query = FilterControl.FilterExtensions.Filter(query, FilterTerm, this.Application);
        
        }

      

  


    }
}
