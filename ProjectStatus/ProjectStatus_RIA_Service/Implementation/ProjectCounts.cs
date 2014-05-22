using LightSwitchApplication.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.DomainServices.Server;
using System.Text;
using CommonUtilities.Statuses;

namespace ProjectStatus_RIA_Service
{
    public partial class ProjectStatus_RIA_Service : DomainService
    {
        private static class ProjectStatuses
        {
            public static double Inactive = -1;
            public static double Red = 1;
            public static double Yellow = 0.85;
            public static double Green = 0.1;
        }

        [Query(IsDefault = true)]
        public IQueryable<DODProjectStatusChartData> ProjectConsolidates()
        {
            return new List<DODProjectStatusChartData>().AsQueryable();
        }
        [Query(IsDefault = true)]
        public IQueryable<DODFeatureChartData> FeatureConsolidates()
        {
            return new List<DODFeatureChartData>().AsQueryable();
        }
        [Query(IsDefault = true)]
        public IQueryable<DODIncidentChartData> IncidentConsolidates()
        {
            return new List<DODIncidentChartData>().AsQueryable();
        }

        public IQueryable<DODProjectStatusChartData> DODProjectCountByStatus(bool? includeJS)
        {
            bool includeInternal = (includeJS == null) ? false : (bool)includeJS;
            var projects = this.Context.Projects.Include("Client").Where(p => p.IsDOD == true).ToList();
            return GetProjectCountByStatus(projects, includeInternal);
        }

        public IQueryable<DODFeatureChartData> DODFeatureCountByStatus(int? projectId)
        {
            var projectFeatures = this.Context.ProjectFeatures.Where(p => p.Project.Id == projectId).ToList();
            return this.GetFeatureCountByStatus(projectFeatures);
        }

        public IQueryable<DODIncidentChartData> DODIncidentCountByStatus(int? projectId)
        {
            var projectIncidents = this.Context.ProjectIncidents.Where(p => p.Project.Id == projectId).ToList();
            return this.GetIncidentCountByStatus(projectIncidents);
        }

        private IQueryable<DODFeatureChartData> GetFeatureCountByStatus(List<ProjectFeature> features)
        {
            var featureCount = features.GroupBy(x => x.Status)
                                        .Select(group => new DODFeatureChartData()
                                        {
                                            FeatureStatus = group.Key,
                                            FeatureCount = group.Count()
                                        }).ToList();
            int i = 0;
            foreach (var item in featureCount)
            {
                item.Order = new FeatureStatus(item.FeatureStatus).Order;
                item.Id = i;
                i++;
            }

            return featureCount.OrderBy(item => item.Order).AsQueryable();
        }

        private IQueryable<DODIncidentChartData> GetIncidentCountByStatus(List<ProjectIncident> incidents)
        {
            var incidentCount = incidents.GroupBy(x => x.Status)
                                        .Select(group => new DODIncidentChartData()
                                        {
                                            IncidentStatus = group.Key,
                                            IncidentCount = group.Count()
                                        }).ToList();
            int i = 0;
            foreach (var item in incidentCount)
            {
                item.Order = new IncidentStatus(item.IncidentStatus).Order;
                item.Id = i;
                i++;
            }

            return incidentCount.OrderBy(item => item.Order).AsQueryable();
        }

        private IQueryable<DODProjectStatusChartData> GetProjectCountByStatus(List<Project> projects, bool includeInternal)
        {
            var projectCount = projects.Where(p => (p.IsDOD == true || p.IsDODOnly == true) 
                                                        && (p.Client.IsInternal == includeInternal || p.Client.IsInternal == false))
                                        .GroupBy(x => x.ProjectStatus)
                                        .Select(group => new DODProjectStatusChartData()
                                            {
                                                ProjectStatus = (group.Key <= ProjectStatuses.Inactive) ? "Inactive" :
                                                      (group.Key <= ProjectStatuses.Green) ? "Green" :
                                                      (group.Key <= ProjectStatuses.Yellow) ? "Yellow" : "Red",
                                                ProjectCount = group.Count()
                                            }).ToList();
            int i = 0;
            foreach (var item in projectCount)
            {
                item.Order = new ProjectStatus(item.ProjectStatus).Order;
                item.Id = i;
                i++;
            }

            return projectCount.OrderBy(item => item.Order).AsQueryable();
        }
    }
}
