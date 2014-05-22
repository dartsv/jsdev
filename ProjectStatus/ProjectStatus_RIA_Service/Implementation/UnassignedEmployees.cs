using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.DomainServices.Server;
using System.Text;

namespace ProjectStatus_RIA_Service
{
    public partial class ProjectStatus_RIA_Service : DomainService
    {
        #region GetAllUnassignedEmp

        [Query(IsDefault = true)]
        public IQueryable<UnassignedEmployees> GetAllUnassignedEmp()
        {
            //CREATE VIEW Unassigned_Employees AS
            //SELECT E.Id, E.FirstName,E.LastName,E.Title,E.UserID,E.Department
            //FROM ProjectStatus.dbo.Employees AS E LEFT OUTER JOIN ProjectStatus.dbo.EmployeeAssignments AS EA
            //ON E.Id = EA.EmployeeAssignment_Employee
            //WHERE EA.Id IS NULL

            var colUnassignedEmp = from Employees in this.Context.Employees
                                   join EmpAssign in this.Context.EmployeeAssignments on Employees.Id equals EmpAssign.EmployeeAssignment_Employee into EA
                                   from SubEmployee in EA.DefaultIfEmpty()
                                   where SubEmployee.EmployeeAssignment_Employee == null & !Employees.ExcludeFromAvailabilityReport
                                   orderby Employees.LastName
                                   select new UnassignedEmployees
                                   {

                                       EmployeeID = Employees.Id,
                                       FirstName = Employees.FirstName,
                                       LastName = Employees.LastName,
                                       UserID = Employees.UserID,
                                       Department = Employees.Department,
                                       Title = Employees.Title,
                                       Country = Employees.Country.Name
                                   };

            return colUnassignedEmp;
        }


        // Override the Count method in order for paging to work correctly
        protected override int Count<T>(IQueryable<T> query)
        {
            return query.Count();
        }

        #endregion
    }
}
