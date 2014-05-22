using LightSwitchApplication.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.DomainServices.Server;
using System.Text;

namespace ProjectStatus_RIA_Service
{
    public partial class ProjectStatus_RIA_Service : DomainService
    {
        #region EmpAssignmentReport

        [Query(IsDefault = true)]
        public IQueryable<EmployeeAssignmentReport> GetAllEmployeeAssignmentReport()
        {

            var employeesQuery = from employee in this.Context.Employees.Include("Country")
                                 select employee;

            List<Employee> emps = employeesQuery.ToList();

            List<Employee> EmpList = emps;
            List<EmployeeAssignment> EmpAssignList = this.Context.EmployeeAssignments.ToList();

            List<EmployeeAssignmentReport> EmpAssReptList = new List<EmployeeAssignmentReport>();
            foreach (EmployeeAssignment EA in EmpAssignList)
            {
                foreach (Employee emp in EmpList)
                {
                    if (emp.Id == EA.EmployeeAssignment_Employee)
                    {
                        EmployeeAssignmentReport emr = new EmployeeAssignmentReport();
                        emr.RecId = EA.Id;
                        emr.LastName = emp.LastName;
                        emr.FirstName = emp.FirstName;
                        emr.ProjectID = EA.EmployeeAssignment_Project;
                        emr.EmployeeID = emp.Id;
                        emr.Department = emp.Department;
                        emr.Percent = EA.Percent;
                        emr.StartDate = EA.StartDate;
                        emr.EndDate = EA.EndDate;
                        emr.Rolloff = EA.RolloffSheduled;
                        emr.Chargeable = EA.Chargeable;
                        emr.Title = EA.Employee.Title;
                        emr.Region = EA.Employee.Region;
                        emr.Country = emp.Country.Name;
                        EmpAssReptList.Add(emr);
                        break;
                    }
                }
            }
            //pull the project table into memory
            List<Project> ProjList = this.Context.Projects.Include("Client").ToList();

            // join to the project table
            foreach (EmployeeAssignmentReport emr in EmpAssReptList)
            {
                foreach (Project pjct in ProjList)
                {
                    if (emr.ProjectID == pjct.Id)
                    {
                        emr.ProjectName = pjct.ProjectName;
                        emr.ClientName = pjct.Client.Name;
                        emr.BookedEndDate = pjct.BookedEndDate;
                        emr.ForecastEndDate = pjct.ForecastEndDate;
                        emr.Exception = pjct.ForecastException;
                    }
                }
            }
            return EmpAssReptList.OrderBy(x => x.LastName).AsQueryable<EmployeeAssignmentReport>();

        }


        #endregion
    }
}
