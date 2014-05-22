using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using System.Text;

using Microsoft.LightSwitch.Server;
using Microsoft.LightSwitch.Security;
using Microsoft.LightSwitch.Security.Server;
using LightSwitchApplication;
using LightSwitchApplication.Util;
using OfficeOpenXml;
using ProjectStatus_RIA_Service;
using System.Security.Authentication;
using System.Globalization;
using System.Web.Routing;

namespace LightSwitchApplication.Controllers
{
    public class ExcelController : ApiController
    {

        //GET 
        public HttpResponse Get(int searchedId, string requestedData)
        {
            try
            {
                //Used to get the Lightswitch Server context
                using (ServerApplicationContext context = ServerApplicationContext.CreateContext())
                {
                    HttpResponse response = HttpContext.Current.Response;

                    ExcelPackage file = null;
                    ExcelExporter ee;

                    Project project = null;
                    Employee employee = null;

                    string[] excludedProperties;

                    if (!context.Application.User.HasPermission(Permissions.ExportToExcel))
                        throw new AuthenticationException("Insuficient permissions");
                    switch (requestedData)
                    {
                        case "features":
                            var featureData =
                                context.DataWorkspace.ApplicationData.ProjectFeatures.GetQuery().Execute().Where(pf => pf.Project.Id == searchedId).ToArray();
                            excludedProperties = new string[] { "Id", "RowVersion", "LastUpdate", "ProjectFeatureComments", "ProjectFeatureCommentsQuery", "Details", "TrackedAnalysisHours", "TrackedDevelopmentHours", "TimeTrackings", "TimeTrackingsQuery", "TDDExtensions", "TDDExtensionsQuery" };
                            ee = new ExcelExporter(typeof(ProjectFeature), featureData);
                            ee.SeparatePropertiesNames = true;
                            file = ee.CreateFile(excludedProperties);
                            break;

                        case "incidents":
                            var incidentData =
                                context.DataWorkspace.ApplicationData.ProjectIncidents.GetQuery().Execute().Where(pf => pf.Project.Id == searchedId).ToArray();
                            excludedProperties = new string[] { "Id", "RowVersion", "LastUpdate", "ProjectIncidentComments", "ProjectIncidentCommentsQuery", "Details", "TrackedAnalysisHours", "TrackedDevelopmentHours", "TimeTrackings", "TimeTrackingsQuery", "TDDExtensions", "TDDExtensionsQuery" };
                            ee = new ExcelExporter(typeof(ProjectIncident), incidentData);
                            ee.SeparatePropertiesNames = true;
                            file = ee.CreateFile(excludedProperties);
                            break;

                        default:
                            response.Write(string.Format("Request is not valid ({0})", requestedData));
                            response.End();
                            break;
                    }
                    project = context.DataWorkspace.ApplicationData.Projects.GetQuery().Execute().Where(p => p.Id == searchedId).FirstOrDefault();
                    context.DataWorkspace.ApplicationData.Countries.GetQuery().Execute();

#if DEBUG
                employee = context.DataWorkspace.ApplicationData.Employees.GetQuery().Execute().Where(e => e.UserID == "testuser").FirstOrDefault();
#else
                    employee = context.DataWorkspace.ApplicationData.Employees.GetQuery().Execute().Where(e => e.UserID == this.User.Identity.Name).FirstOrDefault();
#endif

                    if (file == null)
                    {
                        response.Write("Could not export the requested data to a file");
                        response.End();
                        return response;
                    }

                    string saveAsFileName = string.Format("{0}_{1}_{2}.xlsx",
                                                project.ProjectName,
                                                CultureInfo.CurrentCulture.TextInfo.ToTitleCase(requestedData.ToLower()),
                                                DateTime.Now.AddHours((double)employee.Country.TimeZoneOffset).ToString("yyyyMMdd hhmm")).Replace(" ", "-");

                    response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", saveAsFileName));
                    response.Clear();
                    response.BinaryWrite(file.GetAsByteArray());
                    response.End();

                    return response;
                }
            }catch(Exception ex){
                HttpResponse response = HttpContext.Current.Response;
                response.Write(ex.Message);
                response.End();
                return response;
            }
        }

        //GET export/excel/[feature|incident]/tdd/{id}
        public HttpResponse Get([FromUri]string type, [FromUri]string tdd, [FromUri]int searchedId)
        {
            try
            {
                //Used to get the Lightswitch Server context
                using (ServerApplicationContext context = ServerApplicationContext.CreateContext())
                {
                    HttpResponse response = HttpContext.Current.Response;

                    ExcelPackage file = null;
                    ExcelExporter ee = null;

                    Employee employee = null;
                    ProjectFeature pf = null;
                    ProjectIncident pi = null;
                    string documentName = "";

                    string[] excludedProperties = new string[] { 
                        "Id", "RowVersion", "Details", "ProjectFeature","ProjectIncident"
                    };
                            
                    if (!context.Application.User.HasPermission(Permissions.ExportToExcel))
                        throw new AuthenticationException("Insuficient permissions");
                    switch (type)
                    {
                        case "feature":
                            var featureData =
                                context.DataWorkspace.ApplicationData.TDDExtensions.GetQuery().Execute()
                                .Where(e => e.ProjectFeature != null)
                                .Where(e=> e.ProjectFeature.Id == searchedId)
                                .ToArray();
                            ee = new ExcelExporter(typeof(TDDExtension), featureData);
                            pf = context.DataWorkspace.ApplicationData.ProjectFeatures.GetQuery().Execute().Where(p => p.Id == searchedId).FirstOrDefault();
                            documentName = pf.DocumentName;
                            break;

                        case "incident":
                            var incidentData =
                                context.DataWorkspace.ApplicationData.TDDExtensions.GetQuery().Execute()
                                .Where(e => e.ProjectIncident != null)
                                .Where(e => e.ProjectIncident.Id == searchedId)
                                .ToArray();
                            ee = new ExcelExporter(typeof(TDDExtension), incidentData);
                            pi = context.DataWorkspace.ApplicationData.ProjectIncidents.GetQuery().Execute().Where(p => p.Id == searchedId).FirstOrDefault();
                            documentName = pi.Name;
                            break;
                    }

                    ee.SeparatePropertiesNames = true;
                    file = ee.CreateFile(excludedProperties);

                    if (file == null)
                    {
                        response.Write("Could not export the requested data to a file");
                        response.End();
                        return response;
                    }

#if DEBUG
                    employee = context.DataWorkspace.ApplicationData.Employees.GetQuery().Execute().Where(e => e.UserID == "testuser").FirstOrDefault();
#else
                    employee = context.DataWorkspace.ApplicationData.Employees.GetQuery().Execute().Where(e => e.UserID == this.User.Identity.Name).FirstOrDefault();
#endif

                    string saveAsFileName = string.Format("{0}_{1}_{2}.xlsx",
                                                documentName,
                                                "TDD-elements",
                                                DateTime.Now.AddHours((double)employee.Country.TimeZoneOffset).ToString("yyyyMMdd hhmm")).Replace(" ", "-");

                    response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", saveAsFileName));
                    response.Clear();
                    response.BinaryWrite(file.GetAsByteArray());
                    response.End();

                    return response;
                }
            }
            catch(Exception ex)
            {
                HttpResponse response = HttpContext.Current.Response;
                response.Write(ex.Message);
                response.End();
                return response;
            }
        }

        //GET export/excel/timetracking?toDate=&EmployeeId=----needs to be changed
        public HttpResponse Get([FromUri]int employeeId, [FromUri]DateTime toDate)
        {
            try
            {
                //Used to get the Lightswitch Server context
                using (ServerApplicationContext context = ServerApplicationContext.CreateContext())
                {
                    HttpResponse response = HttpContext.Current.Response;

                    ExcelPackage file = null;

                    //Just used to be the limit of the for loop
                    //It helps to hide the dates after "ToDate"
                    int maxDays = 17;

                    List<string> excludedProperties = new List<string>(){"RecordId", "Details"};
                    List<string> headers = new List<string>() { "Project", "Billable" };

                    DateTime fromDate = TimePeriod.PeriodStartDate(toDate);
                    DateTime indexDate = fromDate;
                    int days = (toDate - fromDate).Days + 1;
                    for(int i = 0; i < maxDays; i++)
                    {
                        
                        if (indexDate > toDate)
                        {
                            excludedProperties.Add("DayHours" + i);
                        }
                        else
                        {
                            headers.Add(indexDate.ToShortDateString());
                        }
                        indexDate = indexDate.AddDays(1);
                    }

                    headers.Add("Total Hours");

                    if (!context.Application.User.HasPermission(Permissions.ExportToExcel))
                        throw new AuthenticationException("Insuficient permissions");

                    ///////////////////CHECK THIS
                    var featureData =
                        context.DataWorkspace
                            .ProjectStatus_RIA_ServiceData.TimeTrackReport(employeeId,fromDate,toDate)
                            .Execute().ToArray();

                    ExcelExporter ee = new ExcelExporter(typeof(ProjectTimeLine), featureData, headers.ToArray());
                    file = ee.CreateFile(excludedProperties.ToArray());

                    if (file == null)
                    {
                        response.Write("Could not export the requested data to a file");
                        response.End();
                        return response;
                    }

                    Employee currentEmployee = context.DataWorkspace.ApplicationData.Employees.GetQuery().Execute().FirstOrDefault(e => e.Id == employeeId);

                    //Add employee name or something + time period
                    string saveAsFileName = 
                        string.Format("{0}-{1}-{2}-{3}.xlsx",
                        currentEmployee.FullName,
                        "TimeTrackingReport",
                        toDate.ToString("MMMdd"),
                        DateTime.Now.AddHours((double)currentEmployee.Country.TimeZoneOffset).ToString("yyyyMMdd hhmm").Replace(" ", "-"));
                    response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", saveAsFileName));
                    response.Clear();
                    response.BinaryWrite(file.GetAsByteArray());
                    response.End();
                    return response;
                }
            }
            catch(Exception ex)
            {
                HttpResponse response = HttpContext.Current.Response;
                response.Write(ex.Message);
                response.End();
                return response;
            }
        }

        //GET export/excel/revenue-forecast?client=&year=...
        public HttpResponse Get([FromUri]string client = "", [FromUri]int year = 0, [FromUri]int quarter = 0, [FromUri]int countryId = 0, [FromUri]decimal expensePercentage = 0)
        {
            DateTime today = DateTime.Now;
            int usedYear = (year == 0) ? today.Year : year;
            int usedQuarter = (quarter == 0) ? TimePeriod.GetQuarter(today) : quarter;
            decimal? usedExpensePercentage = null;
            if ((decimal)expensePercentage != decimal.Zero) usedExpensePercentage = (decimal)expensePercentage;

            ExcelPackage file = null;
            HttpResponse response = HttpContext.Current.Response;

            using (ServerApplicationContext context = ServerApplicationContext.CreateContext())
            {
                if (!context.Application.User.HasPermission(Permissions.RevenueForecastUser))
                    throw new AuthenticationException("Insuficient permissions");

                var featureData =
                   context.DataWorkspace
                   .ProjectStatus_RIA_ServiceData.GetRevenueForecasts(usedExpensePercentage, countryId, client, usedQuarter, usedYear)
                   .Execute().Where(rfi => rfi.Client != "TOTALS").ToArray();

                string[] excludedProperties = new string[] { "RecordId", "Details" };
                string[] headers = RevenueForecastHeaders(usedYear, usedQuarter);
                ExcelExporter ee = new ExcelExporter(typeof(RevenueForecastItem), featureData, headers);
                file = ee.CreateFile(excludedProperties);

                //project = context.DataWorkspace.ApplicationData.Projects.GetQuery().Execute().Where(p => p.Id == projectId).FirstOrDefault();
            }

            if (file == null)
            {
                response.Write("Could not export the requested data to a file");
                response.End();
                return response;
            }

            string saveAsFileName = string.Format("{0}.xlsx", "RevenueForecastReport");
            response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", saveAsFileName));
            response.Clear();
            response.BinaryWrite(file.GetAsByteArray());
            response.End();
            return response;
        }
        private static string[] RevenueForecastHeaders(int year, int quarter)
        {
            List<string> headers = new List<string>();
            DateTime index = new DateTime(year, ((quarter - 1) * 3) + 1, 1);
            DateTime periodEnd = new DateTime();

            headers.Add("CLIENT");
            headers.Add("ACCOUNT");
            headers.Add("PROJECT");

            for (int i = 1; i <= 3; i++)
            {
                periodEnd = TimePeriod.PeriodEndDate(index);
                headers.Add(periodEnd.ToString("MMM dd").ToUpper());
                periodEnd = TimePeriod.PeriodEndDate(periodEnd.AddDays(1));
                headers.Add(periodEnd.ToString("MMM dd").ToUpper());

                headers.Add(string.Format("{0} EXP. EST.", periodEnd.ToString("MMM")).ToUpper());
                headers.Add(string.Format("{0} REVENUE", periodEnd.ToString("MMM")).ToUpper());
                index = periodEnd.AddDays(1);
            }
            periodEnd = TimePeriod.PeriodEndDate(index);
            headers.Add(string.Format("Q{0} {1} REVENUE", quarter, year));

            return headers.ToArray();
        }

       
    }
}