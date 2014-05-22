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
       // [Query(IsDefault = true)]
        public IQueryable<AnnualReviewStatus> GetDifferencesReport()
        {
            DateTime currentPeriodStart = TimePeriod.PeriodStartDate(DateTime.Today);
            DateTime currentPeriodEnd = TimePeriod.PeriodEndDate(DateTime.Today);

            var query = from assignment in this.Context.EmployeeAssignments
                        where 
                            assignment.StartDate >= ((assignment.StartDate > currentPeriodStart) ? assignment.StartDate : currentPeriodStart)
                            && assignment.EndDate <= ((assignment.EndDate < currentPeriodEnd) ? assignment.EndDate : currentPeriodEnd)
                        select assignment;

            query.ToList();


            return null;
        }
        #endregion
    }
}
