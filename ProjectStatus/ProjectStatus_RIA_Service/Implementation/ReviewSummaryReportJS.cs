using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.DomainServices.Server;
using System.Text;

namespace ProjectStatus_RIA_Service
{
    public partial class ProjectStatus_RIA_Service : DomainService
    {
        #region ReviewSummaryReportJS
        [Query(IsDefault = true)]
        public IQueryable<ReviewSummaryReportJS> GetReviewSummaryReportJS()
        {
           // var reviewQuery = from rh in this.Context.ReviewHeaders
                            //  where rh.ReviewType == "Annual" 
             //                 select rh;
            var EmployeeQuery = from e in this.Context.Employees
                                select e;

            var query1 = from ReviewHeaders in this.Context.ReviewHeaders
                         join Empl in EmployeeQuery on ReviewHeaders.Employee.Id equals Empl.Id into RS
                         from subEmployee in RS.DefaultIfEmpty()
                         orderby ReviewHeaders.Employee.FirstName, ReviewHeaders.Employee.LastName
                         select new ReviewSummaryReportJS {
                         
                         };

         /*   var query = from Employees in this.Context.Employees
                        join RevHead in reviewQuery on Employees.Id equals RevHead.Employee.Id into RS
                        from subEmployee in RS.DefaultIfEmpty()
                        orderby Employees.LastName, Employees.FirstName
                        select new ReviewSummaryReportJS*/
                        /*{
                            EmployeeID = Employees.Id,
                            Alias = Employees.FirstName.Substring(0, 1) + Employees.LastName,//gg
                            FirstName = Employees.FirstName,
                            LastName = Employees.LastName,
                            UserID = Employees.UserID,
                            Department = Employees.DeptEmployee.Name,//Employees.Department, GG Changed
                            Level = Employees.EmployeeTitle.EmployeeLevel.Name, //GGadded
                            Title = Employees.EmployeeTitle.Name,//Employees.Title,  GG Changed
                            Country = Employees.Country.Name,
                            Office =Employees.Office,
                         
                           // Supervisor = Employees.Supervsr.LastName,
                            Region = Employees.Region,
                            SupervisorFullNameSorting = Employees.Supervsr.FirstName , //+ " "+ Employees.Supervsr.LastName, //GGAdded
                            //AnnualReviewStatus=subEmployee.SignedStatus
                            ReviewStatus = (subEmployee == null ? "Not Started" : subEmployee.SignedStatus),
                            Reviewer= subEmployee.Reviewer.FirstName +" "+ subEmployee.Reviewer.LastName, //GGAdded,
                            ReviewType= subEmployee.ReviewType,
                            ProjectsCovered= subEmployee.ProjectsCovered,
                            Role= subEmployee.Role,
                          // Promote= subEmployee.Promote,
                            Strengths= subEmployee.Strengths,
                            DevelopmentNeeds= subEmployee.DevelopmentNeeds,
                            // SummaryRating= subEmployee.SummaryRating,
                             MajorAssignments=subEmployee.MajorAssignments,
                           // EmployeeHasSigned= subEmployee.EmployeeHasSigned,
                            SignedStatus= subEmployee.SignedStatus,
                           // Published=subEmployee.Published
                             
                            
                        };*/

            return query1;

            /*
              var reviewQuery = from rh in this.Context.ReviewHeaders
                            //  where rh.ReviewType == "Annual" 
                              select rh;

            var query = from Employees in this.Context.Employees
                        join RevHead in reviewQuery on Employees.Id equals RevHead.Employee.Id into RS
                        from subEmployee in RS.DefaultIfEmpty()
                        orderby Employees.LastName, Employees.FirstName
                        select new ReviewSummaryReportJS
                        {
                            EmployeeID = Employees.Id,
                            Alias = Employees.FirstName.Substring(0, 1) + Employees.LastName,//gg
                            FirstName = Employees.FirstName,
                            LastName = Employees.LastName,
                            UserID = Employees.UserID,
                            Department = Employees.DeptEmployee.Name,//Employees.Department, GG Changed
                            Level = Employees.EmployeeTitle.EmployeeLevel.Name, //GGadded
                            Title = Employees.EmployeeTitle.Name,//Employees.Title,  GG Changed
                            Country = Employees.Country.Name,
                            Office =Employees.Office,
                         
                           // Supervisor = Employees.Supervsr.LastName,
                            Region = Employees.Region,
                            SupervisorFullNameSorting = Employees.Supervsr.FirstName , //+ " "+ Employees.Supervsr.LastName, //GGAdded
                            //AnnualReviewStatus=subEmployee.SignedStatus
                            ReviewStatus = (subEmployee == null ? "Not Started" : subEmployee.SignedStatus),
                            Reviewer= subEmployee.Reviewer.FirstName +" "+ subEmployee.Reviewer.LastName, //GGAdded,
                            ReviewType= subEmployee.ReviewType,
                            ProjectsCovered= subEmployee.ProjectsCovered,
                            Role= subEmployee.Role,
                          // Promote= subEmployee.Promote,
                            Strengths= subEmployee.Strengths,
                            DevelopmentNeeds= subEmployee.DevelopmentNeeds,
                            // SummaryRating= subEmployee.SummaryRating,
                             MajorAssignments=subEmployee.MajorAssignments,
                           // EmployeeHasSigned= subEmployee.EmployeeHasSigned,
                            SignedStatus= subEmployee.SignedStatus,
                           // Published=subEmployee.Published
                             
             * 
             * 
             */
        }
        #endregion
    }
}
