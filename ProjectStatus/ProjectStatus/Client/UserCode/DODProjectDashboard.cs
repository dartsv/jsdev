using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.LightSwitch;
using Microsoft.LightSwitch.Presentation;
using Microsoft.LightSwitch.Presentation.Extensions;
using System.Windows.Controls;
using Microsoft.LightSwitch.Threading;
using System.Collections.Specialized;
using System.Windows.Data;
using CustomControls.Converters;
using CustomControls.CustomCharts;
using System.Windows.Media;
using System.Windows;

namespace LightSwitchApplication
{
    public partial class DODProjectDashboard
    {

        partial void DODProjectDashboard_Created()
        {
            IContentItemProxy featureStatus = this.FindControl("FeatureStatus");
            featureStatus.SetBinding(TextBlock.ForegroundProperty, "Value", new WhiteColorConverter(), BindingMode.OneWay);

            //IContentItemProxy incidentsStatus = this.FindControl("IncidentStatus");
            //incidentsStatus.SetBinding(TextBlock.ForegroundProperty, "Value", new WhiteColorConverter(), BindingMode.OneWay);

            //Hide various elements to users that are customers
            //You know... For security reasons
            if (this.Application.User.HasPermission(Permissions.DODCustomer))
            {
                this.FindControl("ExportIncidentList").IsVisible = false;
                this.FindControl("ExportFeatureList").IsVisible = false;
                this.FindControl("WebToolsGroup").IsVisible = false;
                this.FindControl("ViewFeatureInOnTime").IsVisible = false;
                this.FindControl("ViewIncidentInOnTime").IsVisible = false;
                this.FindControl("ProgressStatusGroup").IsVisible = false;
                this.FindControl("OpenDailyTimeWindow").IsVisible = false;
                this.FindControl("OpenTDDWindow").IsVisible = false;
                this.FindControl("OpenIncidentDailyTimeWindow").IsVisible = false;
                this.FindControl("OpenIncidentsTDDWindow").IsVisible = false;
                this.FindControl("OpenTaskDailyTimeWindow").IsVisible = false;
            }

            //No longer used because dropbox was changed to OneDrive
            //Override event handling for Dropbox Link url
            /*this.FindControl("DropboxLink").ControlAvailable += (sender, eventargs) =>
            {
                HyperlinkButton dropboxLinkButton = (HyperlinkButton)eventargs.Control;

                string path = ((TextBlock)(((StackPanel)dropboxLinkButton.Content).Children[1])).Text;

                dropboxLinkButton.Click += (s, args) =>
                {
                    Dispatchers.Main.BeginInvoke(() =>
                    {
                        string url = "http://" + Application.DefaultDomain + "/links/dropbox/" + GlobalStrings.LoggedOnUser() + "/" + HttpUtility.UrlEncode(path.Replace("\\","=>"));
                        Uri uri = new Uri(url);
                        System.Windows.Browser.HtmlPage.Window.Navigate(uri);
                    });
                };
            };*/

            //Override event handling for modal windows
            this.FindControl("FeatureCommentsWindow").ControlAvailable += (sender, eventargs) =>
            {
                ChildWindow window = (ChildWindow)eventargs.Control;
                window.Closing += (s, e) =>
                {
                    if (!this.Details.ValidationResults.Any())
                    {
                        this.Details.Dispatcher.BeginInvoke(() =>
                        {
                            foreach (var comment in this.DataWorkspace.ApplicationData.Details.GetChanges().OfType<ProjectFeatureComment>())
                            {
                                comment.Details.DiscardChanges();
                            }
                        });
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                };
            };
            this.FindControl("IncidentCommentsWindow").ControlAvailable += (sender, eventargs) =>
            {
                ChildWindow window = (ChildWindow)eventargs.Control;
                window.Closing += (s, e) =>
                {
                    if (!this.Details.ValidationResults.Any())
                    {
                        this.Details.Dispatcher.BeginInvoke(() =>
                        {
                            foreach (var comment in this.DataWorkspace.ApplicationData.Details.GetChanges().OfType<ProjectIncidentComment>())
                            {
                                comment.Details.DiscardChanges();
                            }
                        });
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                };
            };
            this.FindControl("TaskCommentsWindow").ControlAvailable += (sender, eventargs) =>
            {
                ChildWindow window = (ChildWindow)eventargs.Control;
                window.Closing += (s, e) =>
                {
                    if (!this.Details.ValidationResults.Any())
                    {
                        this.Details.Dispatcher.BeginInvoke(() =>
                        {
                            foreach (var comment in this.DataWorkspace.ApplicationData.Details.GetChanges().OfType<ProjectTaskComment>())
                            {
                                comment.Details.DiscardChanges();
                            }
                        });
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                };
            };


            this.FindControl("FeatureTimeTrackingsWindow").ControlAvailable += (sender, eventargs) =>
            {
                ChildWindow window = (ChildWindow)eventargs.Control;
                window.Closing += (s, e) =>
                {
                    if (!this.Details.ValidationResults.Any())
                    {
                        this.Details.Dispatcher.BeginInvoke(() =>
                        {
                            foreach (var comment in this.DataWorkspace.ApplicationData.Details.GetChanges().OfType<TimeTracking>())
                            {
                                comment.Details.DiscardChanges();
                            }
                        });
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                };
            };
            this.FindControl("IncidentTimeTrackingsWindow").ControlAvailable += (sender, eventargs) =>
            {
                ChildWindow window = (ChildWindow)eventargs.Control;
                window.Closing += (s, e) =>
                {
                    if (!this.Details.ValidationResults.Any())
                    {
                        this.Details.Dispatcher.BeginInvoke(() =>
                        {
                            foreach (var comment in this.DataWorkspace.ApplicationData.Details.GetChanges().OfType<TimeTracking>())
                            {
                                comment.Details.DiscardChanges();
                            }
                        });
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                };
            };
            this.FindControl("TaskTimeTrackingsWindow").ControlAvailable += (sender, eventargs) =>
            {
                ChildWindow window = (ChildWindow)eventargs.Control;
                window.Closing += (s, e) =>
                {
                    if (!this.Details.ValidationResults.Any())
                    {
                        this.Details.Dispatcher.BeginInvoke(() =>
                        {
                            foreach (var comment in this.DataWorkspace.ApplicationData.Details.GetChanges().OfType<TimeTracking>())
                            {
                                comment.Details.DiscardChanges();
                            }
                        });
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                };
            };
            this.FindControl("FeatureTDDExtensionsWindow").ControlAvailable += (sender, eventargs) =>
            {
                ChildWindow window = (ChildWindow)eventargs.Control;
                window.Closing += (s, e) =>
                {
                    if (!this.Details.ValidationResults.Any())
                    {
                        this.Details.Dispatcher.BeginInvoke(() =>
                        {
                            foreach (var tdds in this.DataWorkspace.ApplicationData.Details.GetChanges().OfType<TDDExtension>())
                            {
                                tdds.Details.DiscardChanges();
                            }
                        });
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                };
            };

            this.FindControl("IncidentTDDExtensionsWindow").ControlAvailable += (sender, eventargs) =>
            {
                ChildWindow window = (ChildWindow)eventargs.Control;
                window.Closing += (s, e) =>
                {
                    if (!this.Details.ValidationResults.Any())
                    {
                        this.Details.Dispatcher.BeginInvoke(() =>
                        {
                            foreach (var tdds in this.DataWorkspace.ApplicationData.Details.GetChanges().OfType<TDDExtension>())
                            {
                                tdds.Details.DiscardChanges();
                            }
                        });
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                };
            };
        }

        private void GoToOnTime(string itemType, string itemValue)
        {
            Dispatchers.Main.BeginInvoke(() =>
            {
                string url = "http://ontime/Ontime2013Web/ViewItem.aspx?type=" + itemType + "&id=" + itemValue;
                Uri uri = new Uri(url);
                System.Windows.Browser.HtmlPage.Window.Navigate(uri, "_blank");
            });
        }

        partial void ViewFeatureInOnTime_Execute()
        {
            if (this.ProjectFeatures.SelectedItem != null)
            {
                string type = "features";
                string value = this.ProjectFeatures.SelectedItem.FeatureId;
                this.GoToOnTime(type, value);
            }

        }

        partial void ViewIncidentInOnTime_Execute()
        {
            if (this.ProjectIncidents.SelectedItem != null)
            {
                string type = "defects";
                string value = this.ProjectIncidents.SelectedItem.IncidentId;
                this.GoToOnTime(type, value);
            }

        }

        partial void Projects_SelectionChanged()
        {
            UpdateLinks();
            this.FeaturesSearchTerm = "";
            this.IncidentsSearchTerm = "";
            this.TasksSearchTerm = "";
        }

        private void UpdateLinks()
        {
            if (this.Projects.SelectedItem != null)
            {
                Project selected = this.Projects.SelectedItem;
                ProjectWebTool skydrive = (this.ProjectWebTools != null) ? this.ProjectWebTools.FirstOrDefault(pwt => pwt.Name.ToUpper() == "ONEDRIVE") : null;
                ProjectWebTool ontime = (this.ProjectWebTools != null) ? this.ProjectWebTools.FirstOrDefault(pwt => pwt.Name.ToUpper() == "ONTIME") : null;
                ProjectWebTool clientSharepoint = (this.ProjectWebTools != null) ? this.ProjectWebTools.FirstOrDefault(pwt => pwt.Name.ToUpper() == "CLIENT SHAREPOINT") : null;


                this.SharepointLink = selected.SharepointURL;
                if (skydrive != null)
                {
                    this.SkyDriveLink = skydrive.Path;
                }
                else
                {
                    this.SkyDriveLink = "Not defined";
                }
                if (ontime != null)
                {
                    this.OnTimeLink = ontime.Path;
                }
                else
                {
                    this.OnTimeLink = "Not defined";
                }

                if (clientSharepoint != null)
                {
                    this.ClientSharepointLink = clientSharepoint.Path;
                }
                else
                {
                    this.ClientSharepointLink = "Not defined";
                }
            }
        }

        partial void ProjectWebTools_Changed(NotifyCollectionChangedEventArgs e)
        {
            //UpdateLinks();
        }

        partial void OpenFeatureComments_Execute()
        {
            this.OpenModalWindow("FeatureCommentsWindow");
        }
        partial void OpenIncidentComments_Execute()
        {
            this.OpenModalWindow("IncidentCommentsWindow");
        }

        partial void SaveFeatureComments_Execute()
        {
            if (!this.Details.ValidationResults.Any())
            {
                this.CloseModalWindow("FeatureCommentsWindow");
                this.Save();
            }
        }
        partial void CancelFeatureComments_Execute()
        {
            foreach (var comment in this.DataWorkspace.ApplicationData.Details.GetChanges().OfType<ProjectFeatureComment>())
            {
                comment.Details.DiscardChanges();
            }
            this.CloseModalWindow("FeatureCommentsWindow");
        }

        partial void SaveIncidentComments_Execute()
        {
            if (!this.Details.ValidationResults.Any())
            {
                this.CloseModalWindow("IncidentCommentsWindow");
                this.Save();
            }
        }
        partial void CancelIncidentComments_Execute()
        {
            foreach (var comment in this.DataWorkspace.ApplicationData.Details.GetChanges().OfType<ProjectIncidentComment>())
            {
                comment.Details.DiscardChanges();
            }
            this.CloseModalWindow("IncidentCommentsWindow");
        }

        partial void ViewFeatureInOnTime_CanExecute(ref bool result)
        {
            // Escriba el código aquí.

        }

        partial void DODProjectDashboard_InitializeDataWorkspace(List<IDataService> saveChangesTo)
        {
            this.UserId = this.Application.User.Name;
            //this.FeatureUserIdFilter = this.Application.User.Name;

            int featureStatusIndex = 10;
            int incidentStatusIndex = 10;

            Dispatchers.Main.BeginInvoke(() =>
            {
                IContentItemProxy pgrid = this.FindControl("ProjectFeatures");
                pgrid.ControlAvailable += (gs, ga) =>
                {
                    var grid = ga.Control as DataGrid;
                    grid.LoadingRow += (rs, ra) =>
                    {
                        DataGridRow row = ra.Row;
                        DataGridCell statusCell;
                        AutoCompleteBox acb = grid.Columns[incidentStatusIndex].GetCellContent(row) as AutoCompleteBox;
                        statusCell = grid.Columns[featureStatusIndex].GetCellContent(row).Parent as DataGridCell;

                        Binding bg = new Binding("Status")
                        {
                            Mode = BindingMode.TwoWay,
                            Converter = new FeatureStatusToColorConverter()
                        };
                        
                        if (statusCell != null)
                        {
                            statusCell.SetBinding(DataGridCell.BackgroundProperty, bg);
                        }
                        
                    };
                };
                
            });

            /*Dispatchers.Main.BeginInvoke(() =>
            {
                IContentItemProxy igrid = this.FindControl("ProjectIncidents");
                igrid.ControlAvailable += (gs, ga) =>
                {
                    var grid = ga.Control as DataGrid;
                    grid.LoadingRow += (rs, ra) =>
                    {
                        DataGridRow row = ra.Row;
                        DataGridCell statusCell;
                       
                        statusCell = grid.Columns[incidentStatusIndex].GetCellContent(row).Parent as DataGridCell;
                        Binding bg = new Binding("Status")
                        {
                            Mode = BindingMode.TwoWay,
                            Converter = new IncidentStatusToColorConverter()
                        };
                        
                        if (statusCell != null)
                        {
                            statusCell.SetBinding(DataGridCell.BackgroundProperty, bg);
                        }
                    };
                };
            });*/
        }

        partial void ExportFeatureList_Execute()
        {
            Dispatchers.Main.BeginInvoke(() =>
            {
                string url = String.Format("http://{0}/project/{1}/features/export/excel",
                    Application.DefaultDomain,
                    this.Projects.SelectedItem.Id);
                Uri uri = new Uri(url);
                System.Windows.Browser.HtmlPage.Window.Navigate(uri);
            });

        }

        partial void ExportIncidentList_Execute()
        {

            Dispatchers.Main.BeginInvoke(() =>
            {
                string url = String.Format("http://{0}/project/{1}/incidents/export/excel",
                    Application.DefaultDomain,
                    this.Projects.SelectedItem.Id);
                Uri uri = new Uri(url);
                System.Windows.Browser.HtmlPage.Window.Navigate(uri);
            });


        }

        #region FeatureTimeTracking
        //Open window
        partial void OpenDailyTimeWindow_Execute()
        {
            this.OpenModalWindow("FeatureTimeTrackingsWindow");
        }
        //Close window
        partial void CancelFeatureTimeTrackings_Execute()
        {
            foreach (var timeTracking in this.DataWorkspace.ApplicationData.Details.GetChanges().OfType<TimeTracking>())
            {
                timeTracking.Details.DiscardChanges();
            }
            this.CloseModalWindow("FeatureTimeTrackingsWindow");
        }
        //Save
        partial void SaveFeatureTimeTrackings_Execute()
        {
            if (!this.Details.ValidationResults.Any())
            {
                this.CloseModalWindow("FeatureTimeTrackingsWindow");
                this.Save();
            }
        }
        #endregion

        #region IncidentTimeTracking
        //Open window
        partial void OpenIncidentDailyTimeWindow_Execute()
        {
            this.OpenModalWindow("IncidentTimeTrackingsWindow");
        }
        //Close window
        partial void CancelIncidentTimeTrackings_Execute()
        {
            foreach (var timeTracking in this.DataWorkspace.ApplicationData.Details.GetChanges().OfType<TimeTracking>())
            {
                timeTracking.Details.DiscardChanges();
            }
            this.CloseModalWindow("IncidentTimeTrackingsWindow");
        }
        //Save
        partial void SaveIncidentTimeTrackings_Execute()
        {
            if (!this.Details.ValidationResults.Any())
            {
                this.CloseModalWindow("IncidentTimeTrackingsWindow");
                this.Save();
            }
        }
        #endregion

        partial void DODProjectDashboard_Activated()
        {
            // Write your code here.

        }

        partial void OpenTaskDailyTimeWindow_Execute()
        {
            this.OpenModalWindow("TaskTimeTrackingsWindow");
        }

        partial void OpenTaskComments_Execute()
        {
            this.OpenModalWindow("TaskCommentsWindow");
        }

        partial void SaveTaskComments_Execute()
        {
            if (!this.Details.ValidationResults.Any())
            {
                this.CloseModalWindow("TaskCommentsWindow");
                this.Save();
            }
        }

        partial void CancelTaskComments_Execute()
        {
            foreach (var comment in this.DataWorkspace.ApplicationData.Details.GetChanges().OfType<ProjectTaskComment>())
            {
                comment.Details.DiscardChanges();
            }
            this.CloseModalWindow("TaskCommentsWindow");
        }

        partial void SaveTaskTimeTrackings_Execute()
        {
            if (!this.Details.ValidationResults.Any())
            {
                this.CloseModalWindow("TaskTimeTrackingsWindow");
                this.Save();
            }
        }

        partial void CancelTaskTimeTrackings_Execute()
        {
            foreach (var timeTracking in this.DataWorkspace.ApplicationData.Details.GetChanges().OfType<TimeTracking>())
            {
                timeTracking.Details.DiscardChanges();
            }
            this.CloseModalWindow("TaskTimeTrackingsWindow");
        }


        #region TDDs
        partial void OpenTDDWindow_Execute()
        {
            this.OpenModalWindow("FeatureTDDExtensionsWindow");
        }
        partial void SaveTDDExtensions_Execute()
        {
            if (!this.Details.ValidationResults.Any())
            {
                this.CloseModalWindow("FeatureTDDExtensionsWindow");
                this.CloseModalWindow("IncidentTDDExtensionsWindow");
                this.Save();
            }
        }
        partial void CancelTDDExtensions_Execute()
        {
            foreach (var tddExtension in this.DataWorkspace.ApplicationData.Details.GetChanges().OfType<TDDExtension>())
            {
                tddExtension.Details.DiscardChanges();
            }
            this.CloseModalWindow("IncidentTDDExtensionsWindow");
            this.CloseModalWindow("FeatureTDDExtensionsWindow");
        }
        #endregion

        partial void OpenIncidentsTDDWindow_Execute()
        {
            this.OpenModalWindow("IncidentTDDExtensionsWindow");
        }

        partial void ToogleCompletedFeatures_Execute()
        {
            if(this.ProjectFeatureStatusFilter == "Completed")
            {
                this.ProjectFeatureStatusFilter = "-";
                this.FindControl("ToogleCompletedFeatures").DisplayName = "Show Completed";
            }
            else
            {
                this.ProjectFeatureStatusFilter = "Completed";
                this.FindControl("ToogleCompletedFeatures").DisplayName = "Hide Completed";
            }

        }

        partial void ToogleCompletedIncidents_Execute()
        {
            if (this.ProjectIncidentStatusFilter == "Completed")
            {
                this.ProjectIncidentStatusFilter = "-";
                this.FindControl("ToogleCompletedIncidents").DisplayName = "Show Completed";
            }
            else
            {
                this.ProjectIncidentStatusFilter = "Completed";
                this.FindControl("ToogleCompletedIncidents").DisplayName = "Hide Completed";
            }

        }

        partial void ToogleCompletedTasks_Execute()
        {
            if (this.ProjectTaskStatusFilter == "Completed")
            {
                this.ProjectTaskStatusFilter = "-";
                this.FindControl("ToogleCompletedTasks").DisplayName = "Show Completed";
            }
            else
            {
                this.ProjectTaskStatusFilter = "Completed";
                this.FindControl("ToogleCompletedTasks").DisplayName = "Hide Completed";
            }

        }

        partial void ExportFeatureTdd_Execute()
        {
            Dispatchers.Main.BeginInvoke(() =>
            {
                string url = String.Format("http://{0}/tdd/export/excel?type=feature&tdd=tdd&searchedId={1}",
                    Application.DefaultDomain,
                    this.ProjectFeatures.SelectedItem.Id);
                Uri uri = new Uri(url);
                System.Windows.Browser.HtmlPage.Window.Navigate(uri);
            });

        }

        partial void ExportIncidentTdd_Execute()
        {
            Dispatchers.Main.BeginInvoke(() =>
            {
                string url = String.Format("http://{0}/tdd/export/excel?type=incident&tdd=tdd&searchedId={1}",
                    Application.DefaultDomain,
                    this.ProjectIncidents.SelectedItem.Id);
                Uri uri = new Uri(url);
                System.Windows.Browser.HtmlPage.Window.Navigate(uri);
            });

        }

    }

    public class WhiteColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                switch(value.ToString())
                {
                    case "Completed":
                    case "Rework":
                    case "Client Review":
                        return new SolidColorBrush(Color.FromArgb(0xE0, 0xEB, 0xEB, 0xEB));
                }
                return new SolidColorBrush(Colors.Black);
            }catch{ return new SolidColorBrush(Colors.Black); }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
