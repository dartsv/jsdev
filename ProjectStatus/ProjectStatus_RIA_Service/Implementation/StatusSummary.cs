using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.DomainServices.Server;
using System.Text;

namespace ProjectStatus_RIA_Service
{
    public partial class ProjectStatus_RIA_Service : DomainService
    {
        #region StatusSummary

        [Query(IsDefault = true)]
        public IQueryable<StatusSummary> GetAllStatusSummaries()
        {
            // set up a variable to hold todays date to simplify the lambda expression calls
            DateTime tdy = DateTime.Today;

            //Create a table that contains a left outer join of projects and aggregated employee assignments
            var Query1 = from Projects in this.Context.Projects
                         join EmpAssign in this.Context.EmployeeAssignments
                         on Projects.Id equals EmpAssign.EmployeeAssignment_Project into EA
                         from EAs in EA.DefaultIfEmpty()
                         select new
                         {
                             ProjectID = Projects.Id,
                             ProjectName = Projects.ProjectName,
                             AssignedFTE = (EA.Where(o => o.EndDate >= tdy && o.StartDate <= tdy).Sum(o => (decimal?)o.Percent) ?? 0M) / 100.0M,
                             Client = Projects.Client,
                             Phase = Projects.Phase,
                             ProjectStatus = Projects.ProjectStatus,
                             StaffingStatus = Projects.StaffingStatus,
                             StartDate = Projects.StartDate,
                             BookedEndDate = Projects.BookedEndDate,
                             ForecastEndDate = Projects.ForecastEndDate,
                             Inactive = Projects.Inactive,
                             Chargeable = Projects.Chargeable,
                             ForecastException = Projects.ForecastException,
                             FollowOn = Projects.FollowOn,
                             Notes = Projects.Notes,
                             Director = Projects.Director,
                             Manager = Projects.Manager,
                             Region = Projects.Region,
                             Planned = Projects.Planned
                         };

            // create a table that contains a left outer join of projects and aggregated staffing requirements
            var Query2 = from Projects in this.Context.Projects
                         join StaffReq in this.Context.StaffingRequirements
                         on Projects.Id equals StaffReq.StaffingRequirement_Project into PR
                         from PRs in PR.DefaultIfEmpty()
                         select new
                         {
                             ProjectID = Projects.Id,
                             //ProjectName = Projects.ProjectName,
                             RequiredFTE = PR.Sum(o => (decimal?)o.FTEs) ?? 0M
                         };

            //Create an inner join of the two tables created above.
            //Exclude inactive projects and forecast exceptions
            var Query3 = from q1 in Query1
                         join q2 in Query2 on q1.ProjectID equals q2.ProjectID
                         where q1.Inactive == false && q1.ForecastException == false
                         select new StatusSummary
                         {
                             ProjectID = q1.ProjectID,
                             ProjectName = q1.ProjectName,
                             AssignedFTE = q1.AssignedFTE,
                             RequiredFTE = q2.RequiredFTE,
                             Client = q1.Client.Name,
                             Phase = q1.Phase,
                             ProjectStatus = q1.ProjectStatus,
                             StaffingStatus = q1.StaffingStatus,
                             StartDate = q1.StartDate,
                             BookedEndDate = q1.BookedEndDate,
                             ForecastEndDate = q1.ForecastEndDate,
                             Inactive = q1.Inactive,
                             Chargeable = q1.Chargeable,
                             ForecastException = q1.ForecastException,
                             FollowOn = q1.FollowOn,
                             Notes = q1.Notes,
                             Director = q1.Director.FirstName + " " + q1.Director.LastName,
                             Manager = q1.Manager.FirstName + " " + q1.Manager.LastName,
                             Region = q1.Region,
                             Planned = q1.Planned
                         };

            return Query3;

        }



        #endregion
    }
}
