using System;
using System.Linq;
using System.IO;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using Microsoft.LightSwitch;
using Microsoft.LightSwitch.Framework.Client;
using Microsoft.LightSwitch.Presentation;
using Microsoft.LightSwitch.Presentation.Extensions;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows.Controls;
using System.Reflection;
using Microsoft.LightSwitch.Threading;
namespace LightSwitchApplication
{
    public partial class ManageMyAssignments
    {
        

        partial void ManageMyAssignments_Created()
        {
            //Override the behavior of this window to show or hide blocks
            // depending if we are under the Feature, Incident or Task tab
            this.FindControl("FITContainer").ControlAvailable += (sender, eventargs) =>
            {
                TabControl tabs = (TabControl)eventargs.Control;
                tabs.SelectionChanged += (s, ea) =>
                    {
                        switch (tabs.SelectedIndex)
                        {
                            //Feature is selected
                            //Show the details block for features
                            //Hide everything else
                            case 0:
                                this.FindControl("FeatureDetails").IsVisible = true;
                                this.FindControl("IncidentDetails").IsVisible = false;
                                this.FindControl("TaskDetails").IsVisible = false;
                                break;

                            //Incident is selected
                            case 1:
                                this.FindControl("FeatureDetails").IsVisible = false;
                                this.FindControl("IncidentDetails").IsVisible = true;
                                this.FindControl("TaskDetails").IsVisible = false;
                                break;

                            //Task is selected
                            case 2:
                                this.FindControl("FeatureDetails").IsVisible = false;
                                this.FindControl("IncidentDetails").IsVisible = false;
                                this.FindControl("TaskDetails").IsVisible = true;
                                break;
                        }
                    };
            };

        }

        partial void ManageMyAssignments_InitializeDataWorkspace(List<IDataService> saveChangesTo)
        {
            //Set the current employee to the currently logged user
            this.CurrentEmployee = this.DataWorkspace.ApplicationData.Employees.Where(e => e.UserID == this.Application.User.Name).FirstOrDefault();
            this.ProjectFeatureStatusFilter = "Completed";
            this.ProjectIncidentStatusFilter = "Completed";
            this.ProjectTaskStatusFilter = "Completed";

            //Create a dictionary for each modal window
            //The dictionary contains values like this:
            //{Key: <ModalWindowName>, Value: <AssociatedClassName>}
            Dictionary<string, string> modalWindows = new Dictionary<string, string>();

            modalWindows.Add("FeatureCommentsWindow",           "ProjectFeatureComment");
            modalWindows.Add("IncidentCommentsWindow",          "ProjectIncidentComment");
            modalWindows.Add("TaskCommentsWindow",              "ProjectTaskComment");
            modalWindows.Add("FeatureReqTimeTrackWindow",       "TimeTrack");
            modalWindows.Add("IncidentReqTimeTrackWindow",      "TimeTrack");
            modalWindows.Add("TaskReqTimeTrackWindow",          "TimeTrack");
            modalWindows.Add("FeatureIssueTimeTrackWindow",     "TimeTrack");
            modalWindows.Add("IncidentIssueTimeTrackWindow",    "TimeTrack");
            modalWindows.Add("TaskIssueTimeTrackWindow",        "TimeTrack");

            foreach (var modalWindow in modalWindows)
            {
                //Override event handling for modal windows
                //Override the Closing event for the modal windows
                //We do this because, if the window is closed by pressing the "X" at the top right corner
                //  we must discard the changes as if the cancel button was pressed
                this.FindControl(modalWindow.Key).ControlAvailable += (sender, eventargs) =>
                {
                    ChildWindow window = (ChildWindow)eventargs.Control;
                    window.Closing += (s, e) =>
                    {
                        this.OnModalWindowClose(modalWindow.Value, e);
                    };
                };
            }
        }


        /// <summary>
        /// This function is used to discard changes in case the "X" is pressed instead of the OK or Cancel buttons
        /// </summary>
        /// <param name="type">The entity type associated to the window</param>
        /// <param name="e">A CancelEventArgs object to be able to cancel the closing event</param>
        private void OnModalWindowClose(string type, CancelEventArgs e)
        {

            Dispatchers.Main.BeginInvoke(() =>
            {
                switch (type)
                {
                    case "ProjectFeatureComment":
                        foreach (var comment in this.DataWorkspace.ApplicationData.Details.GetChanges().OfType<ProjectFeatureComment>())
                        {
                            comment.Details.DiscardChanges();
                        }
                        break;
                    case "ProjectIncidentComment":
                        foreach (var comment in this.DataWorkspace.ApplicationData.Details.GetChanges().OfType<ProjectIncidentComment>())
                        {
                            comment.Details.DiscardChanges();
                        }
                        break;
                    case "ProjectTaskComment":
                        foreach (var comment in this.DataWorkspace.ApplicationData.Details.GetChanges().OfType<ProjectTaskComment>())
                        {
                            comment.Details.DiscardChanges();
                        }
                        break;
                    case "TimeTrack":
                        foreach (var comment in this.DataWorkspace.ApplicationData.Details.GetChanges().OfType<TimeTrack>())
                        {
                            comment.Details.DiscardChanges();
                        }
                        break;
                }

            });

        }

        /// <summary>
        /// This function is used to discard changes when the Cancel button is pressed
        /// </summary>
        /// <param name="type">The entity type associated to the window</param>
        private void CancelModalWindow(string windowName, string type)
        {
            switch (type)
            {
                case "ProjectFeatureComment":
                    foreach (var comment in this.DataWorkspace.ApplicationData.Details.GetChanges().OfType<ProjectFeatureComment>())
                    {
                        comment.Details.DiscardChanges();
                    }
                    break;
                case "ProjectIncidentComment":
                    foreach (var comment in this.DataWorkspace.ApplicationData.Details.GetChanges().OfType<ProjectIncidentComment>())
                    {
                        comment.Details.DiscardChanges();
                    }
                    break;
                case "ProjectTaskComment":
                    foreach (var comment in this.DataWorkspace.ApplicationData.Details.GetChanges().OfType<ProjectTaskComment>())
                    {
                        comment.Details.DiscardChanges();
                    }
                    break;
                case "TimeTrack":
                    foreach (var tt in this.DataWorkspace.ApplicationData.Details.GetChanges().OfType<TimeTrack>())
                    {
                        tt.Details.DiscardChanges();
                    }
                    break;
            }

            this.CloseModalWindow(windowName);

        }

        partial void OpenFeatureOnTimeWindow_Execute()
        {
            UIHelpers.WindowHelper.OpenInOntime("features", this.ProjectFeatures.SelectedItem.FeatureId.ToString());
        }

        partial void OpenIncidentOnTimeWindow_Execute()
        {
            UIHelpers.WindowHelper.OpenInOntime("defects", this.ProjectIncidents.SelectedItem.IncidentId.ToString());
        }


        //Generic process used when the OK button is pressed in a modal window
        private void SaveModalWindow(string windowName)
        {
            if (!this.Details.ValidationResults.Any())
            {
                this.CloseModalWindow(windowName);
                this.Save();
            }
        }

        //Open methods
        partial void OpenFeatureCommentsWindow_Execute()
        {
            this.OpenModalWindow("FeatureCommentsWindow"); 
        }

        partial void OpenTaskCommentsWindow_Execute()
        {
            this.OpenModalWindow("TaskCommentsWindow");
        }

        partial void OpenIncidentCommentsWindow_Execute()
        {
            this.OpenModalWindow("IncidentCommentsWindow");
        }
        //---
        partial void OpenFeatureReqTimeTrackWindow_Execute()
        {
            this.OpenModalWindow("FeatureReqTimeTrackWindow");
        }

        partial void OpenIncidentReqTimeTrackWindow_Execute()
        {
            this.OpenModalWindow("IncidentReqTimeTrackWindow");
        }

        partial void OpenTaskReqTimeTrackWindow_Execute()
        {
            this.OpenModalWindow("TaskReqTimeTrackWindow");
        }
        //---
        partial void OpenFeatureIssueTimeTrackWindow_Execute()
        {
            this.OpenModalWindow("FeatureIssueTimeTrackWindow");
        }

        partial void OpenIncidentIssueTimeTrackWindow_Execute()
        {
            this.OpenModalWindow("IncidentIssueTimeTrackWindow");
        }

        partial void OpenTaskIssueTimeTrackWindow_Execute()
        {
            this.OpenModalWindow("TaskIssueTimeTrackWindow");
        }
        //End open methods


        // Button save methods
        partial void SaveFeatureComments_Execute()
        {
            this.SaveModalWindow("FeatureCommentsWindow");
        }
        partial void SaveIncidentComments_Execute()
        {
            this.SaveModalWindow("IncidentCommentsWindow");
        }
        partial void SaveTaskComments_Execute()
        {
            this.SaveModalWindow("TaskCommentsWindow");
        }
        //--
        partial void SaveFeatureReqTimeTracks_Execute()
        {
            this.SaveModalWindow("FeatureReqTimeTrackWindow");
        }
        partial void SaveIncidentReqTimeTracks_Execute()
        {
            this.SaveModalWindow("IncidentReqTimeTrackWindow");
        }
        partial void SaveTaskReqTimeTracks_Execute()
        {
            this.SaveModalWindow("TaskReqTimeTrackWindow");
        }
        //--
        partial void SaveFeatureIssueTimeTracks_Execute()
        {
            this.SaveModalWindow("FeatureIssueTimeTrackWindow");
        }
        partial void SaveIncidentIssueTimeTracks_Execute()
        {
            this.SaveModalWindow("IncidentIssueTimeTrackWindow");
        }
        partial void SaveTaskIssueTimeTracks_Execute()
        {
            this.SaveModalWindow("TaskIssueTimeTrackWindow");
        }

        //
        //Cancel button methods
        partial void CancelFeatureComments_Execute()
        {
            this.CancelModalWindow("FeatureCommentsWindow", "ProjectFeatureComment");
        }
        partial void CancelIncidentComments_Execute()
        {
            this.CancelModalWindow("IncidentCommentsWindow", "ProjectIncidentComment");
        }
        partial void CancelTaskComments_Execute()
        {
            this.CancelModalWindow("TaskCommentsWindow", "ProjectTaskComment");
        }
        //--
        partial void CancelFeatureReqTimeTracks_Execute()
        {
            this.CancelModalWindow("FeatureReqTimeTrackWindow", "TimeTrack");
        }
        partial void CancelIncidentReqTimeTracks_Execute()
        {
            this.CancelModalWindow("IncidentReqTimeTrackWindow", "TimeTrack");
        }
        partial void CancelTaskReqTimeTracks_Execute()
        {
            this.CancelModalWindow("TaskReqTimeTrackWindow", "TimeTrack");
        }
        //--
        partial void CancelFeatureIssueTimeTracks_Execute()
        {
            this.CancelModalWindow("FeatureIssueTimeTrackWindow", "TimeTrack");
        }
        partial void CancelIncidentIssueTimeTracks_Execute()
        {
            this.CancelModalWindow("IncidentIssueTimeTrackWindow", "TimeTrack");
        }
        partial void CancelTaskIssueTimeTracks_Execute()
        {
            this.CancelModalWindow("TaskIssueTimeTrackWindow", "TimeTrack");
        }
        //End button methods

        partial void ToggleCompletedFeatures_Execute()
        {
            if (this.ProjectFeatureStatusFilter == "Completed")
            {
                this.ProjectFeatureStatusFilter = "-";
                this.FindControl("ToggleCompletedFeatures").DisplayName = "Show Completed";
            }
            else
            {
                this.ProjectFeatureStatusFilter = "Completed";
                this.FindControl("ToggleCompletedFeatures").DisplayName = "Hide Completed";
            }

        }

        partial void ToogleCompletedIncidents_Execute()
        {
            if (this.ProjectIncidentStatusFilter == "Completed")
            {
                this.ProjectIncidentStatusFilter = "-";
                this.FindControl("ToggleCompletedIncidents").DisplayName = "Show Completed";
            }
            else
            {
                this.ProjectIncidentStatusFilter = "Completed";
                this.FindControl("ToggleCompletedIncidents").DisplayName = "Hide Completed";
            }
        }

        partial void ToogleCompletedTasks_Execute()
        {
            if (this.ProjectTaskStatusFilter == "Completed")
            {
                this.ProjectTaskStatusFilter = "-";
                this.FindControl("ToggleCompletedTasks").DisplayName = "Show Completed";
            }
            else
            {
                this.ProjectTaskStatusFilter = "Completed";
                this.FindControl("ToggleCompletedTasks").DisplayName = "Hide Completed";
            }
        }

        
        partial void ToggleCompletedFeatureRequirements_Execute()
        {
            // GG Added
            if (this.FeatureRequirementStatusFilter == "Completed")
            {
                this.FeatureRequirementStatusFilter = "-";
                this.FindControl("ToggleCompletedFeatureRequirements").DisplayName = "Show Completed";
            }
            else
            {
                this.FeatureRequirementStatusFilter = "Completed";
                this.FindControl("ToggleCompletedFeatureRequirements").DisplayName = "Hide Completed";
            }
        }

   

        partial void MethodToggleCompletedFeatureIssue_Execute()
        {
            // GG Added
            if (this.FeatureIssueStatusFilter == "Completed")
            {
                this.FeatureIssueStatusFilter = "-";
                this.FindControl("MethodToggleCompletedFeatureIssue").DisplayName = "Show Completed";
            }
            else
            {
                this.FeatureIssueStatusFilter = "Completed";
                this.FindControl("MethodToggleCompletedFeatureIssue").DisplayName = "Hide Completed";
            }
        }

        partial void ToggleCompletedIncidentRequirement_Execute()
        {
            // GG Added
            if (this.IncidentRequirementStatusFilter == "Completed")
            {
                this.IncidentRequirementStatusFilter = "-";
                this.FindControl("ToggleCompletedIncidentRequirement").DisplayName = "Show Completed";
            }
            else
            {
                this.IncidentRequirementStatusFilter = "Completed";
                this.FindControl("ToggleCompletedIncidentRequirement").DisplayName = "Hide Completed";
            }
        }

        partial void ToggleCompletedIncidentIssue_Execute()
        {
            // GG Added
            if (this.IncidentIssueStatusFilter == "Completed")
            {
                this.IncidentIssueStatusFilter = "-";
                this.FindControl("ToggleCompletedIncidentIssue").DisplayName = "Show Completed";
            }
            else
            {
                this.IncidentIssueStatusFilter = "Completed";
                this.FindControl("ToggleCompletedIncidentIssue").DisplayName = "Hide Completed";
            }
        }

        partial void ToggleCompletedTaskRequirement_Execute()
        {
            // GG Added
            if (this.TaskRequirementStatusFilter == "Completed")
            {
                this.TaskRequirementStatusFilter = "-";
                this.FindControl("ToggleCompletedTaskRequirement").DisplayName = "Show Completed";
            }
            else
            {
                this.TaskRequirementStatusFilter = "Completed";
                this.FindControl("ToggleCompletedTaskRequirement").DisplayName = "Hide Completed";
            }

        }

        partial void ToggleCompletedTaskIssue_Execute()
        {
            // GG Added
            if (this.TaskIssueStatusFilter == "Completed")
            {
                this.TaskIssueStatusFilter = "-";
                this.FindControl("ToggleCompletedTaskIssue").DisplayName = "Show Completed";
            }
            else
            {
                this.TaskIssueStatusFilter = "Completed";
                this.FindControl("ToggleCompletedTaskIssue").DisplayName = "Hide Completed";
            }
        }
    }
}
