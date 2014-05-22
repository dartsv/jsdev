using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;
using System.Diagnostics;

namespace LightSwitchApplication
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            //Deprecated: Used to be to create links for Dropbox
            //RouteTable.Routes.MapHttpRoute("LinksApi", "links/{controller}/{user}/{path}");

            //TODO: tidy api routes and also change controllers for each entity rather than format
            //Maybe add a format parameter
            RouteTable.Routes.MapHttpRoute(
               name: "GenerateRdc",
               routeTemplate: "{controller}/generate");

            RouteTable.Routes.MapHttpRoute(
               name: "ExportTdd",
               routeTemplate: "tdd/export/{controller}");

            RouteTable.Routes.MapHttpRoute(
                name: "ExportRevenueForecast",
                routeTemplate: "revenue-forecast/export/{controller}");

            RouteTable.Routes.MapHttpRoute(
                name: "ExportFeaturesAndIncidents",
                routeTemplate: "project/{searchedId}/{requestedData}/export/{controller}");

            RouteTable.Routes.MapHttpRoute(
                name: "TimeTrackingReport",
                routeTemplate: "timetracking/{employeeId}/{toDate}/export/{controller}");


        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}