using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.DomainServices.Server;
using System.Text;

namespace ProjectStatus_RIA_Service
{
    public partial class ProjectStatus_RIA_Service : DomainService
    {
        #region AnnualReviewStatus
        [Query(IsDefault = true)]
 

        public IQueryable<AnnualReviewStatus> GetAllAnnualReviewStatus()
        {
           /* var reviewQuery = from rh in this.Context.ReviewHeaders
                              where rh.ReviewType == "Annual" 
                              select rh;*/
            var reviewQuery1 = from e in this.Context.Employees
                               //where e.Id==168
                               select e;

            var query1= from rh in this.Context.ReviewHeaders
                        join Emp in reviewQuery1 on rh.ReviewHeader_Employee equals Emp.Id into RS
                        from subEmployee in RS
                        orderby rh.Employee.FirstName, rh.Employee.LastName
                        where rh.ReviewType == "Annual" 
                        select new AnnualReviewStatus

                        

           /* var query = from Employees in this.Context.Employees
                        join RevHead in reviewQuery on Employees.Id equals RevHead.Employee.Id into RS
                        from subEmployee in RS.DefaultIfEmpty()
                        orderby Employees.LastName, Employees.FirstName
                        select new AnnualReviewStatus*/
                        {
                            EmployeeID= subEmployee.Id,
                            FirstName=subEmployee.FirstName,
                            LastName=subEmployee.LastName,
                            UserID=subEmployee.UserID,
                            Department=subEmployee.DeptEmployee.Name,
                            Level=subEmployee.EmployeeTitle.EmployeeLevel.Name,
                            Country=subEmployee.Country.Name,
                          Title=subEmployee.EmployeeTitle.Name,
                            
                            Supervisor=  rh.SupervisorEmployee.LastName,
                            Region= subEmployee.Region,
                            SupervisorFullNameSorting= rh.EmployeeSupervisorFullName,
                            ReviewStatus=(rh ==null? "Not Started": rh.SignedStatus),
                            ReviewDate=(rh==null? DateTime.Today: rh.ReviewDate),
                            YearReview=(rh==null? 0 : rh.ReviewDate.Year)


                            /*
                            EmployeeID = Employees.Id,
                            FirstName = Employees.FirstName,
                            LastName = Employees.LastName,
                            UserID = Employees.UserID,
                            Department = Employees.DeptEmployee.Name,//Employees.Department, GG Changed
                            Level=Employees.EmployeeTitle.EmployeeLevel.Name, //GGadded
                            Title = Employees.EmployeeTitle.Name,//Employees.Title,  GG Changed
                            Country = Employees.Country.Name,
                            Supervisor = Employees.Supervsr.LastName,
                            Region = Employees.Region,
                            SupervisorFullNameSorting=Employees.SupervisorFullNameSorting, //GGAdded
                            //AnnualReviewStatus=subEmployee.SignedStatus
                            ReviewStatus = (subEmployee == null ? "Not Started" : subEmployee.SignedStatus),
                            ReviewDate = (subEmployee == null ? DateTime.Today : subEmployee.ReviewDate),/// THIS WORKS!!!!
                            //ReviewDate = (subEmployee == null ? DateTime.ParseExact("2001-01-01 14:40:52,531", "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.InvariantCulture) : subEmployee.ReviewDate)
                            //ReviewDate = (subEmployee == null ? Convert.ToDateTime("01/01/2001") : subEmployee.ReviewDate)
                            // ReviewDate = (subEmployee == null ?  DateTime.Parse("01-01-01") : subEmployee.ReviewDate)
                            //ReviewDate=2013
                           YearReview=(subEmployee==null ? 0 : subEmployee.ReviewDate.Year) // si funciona
                          // ReviewDate= subEmployee.ReviewDate.ToString() //nO FUNCIONA TAMPOCO
                          // ReviewDate = DateTime.Today //---THIS WORKS PERO NO ME SIRVE 
                          //ReviewDate=subEmployee.ReviewDate.ToUniversalTime()
                         //NO FUNCIONAAA  ReviewDate= TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.SpecifyKind (subEmployee.ReviewDate.Date, DateTimeKind.Unspecified), "Eastern Standard Time", "UTC")
                         //ReviewDate=subEmployee.ReviewDate.Date
                        // ReviewDate= subEmployee.ReviewDate 
                        //ReviewDate=subEmployee.ReviewDate.ToLocalTime()
                        //ReviewDate= 
                            */
                        };

            return query1;

            //var colUnassignedEmp = from Employees in this.Context.Employees
            //                       join EmpAssign in this.Context.EmployeeAssignments on Employees.Id equals EmpAssign.EmployeeAssignment_Employee into EA
            //                       from SubEmployee in EA.DefaultIfEmpty()
            //                       where SubEmployee.EmployeeAssignment_Employee == null & !Employees.ExcludeFromAvailabilityReport
            //                       orderby Employees.LastName
            //                       select new UnassignedEmployees
            //                       {

            //                           EmployeeID = Employees.Id,
            //                           FirstName = Employees.FirstName,
            //                           LastName = Employees.LastName,
            //                           UserID = Employees.UserID,
            //                           Department = Employees.Department,
            //                           Title = Employees.Title
            //                       };

            //return colUnassignedEmp;
        }
        #endregion
    }
}
