using System;
using System.Linq;
using System.IO;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using Microsoft.LightSwitch;
using Microsoft.LightSwitch.Framework.Client;
using Microsoft.LightSwitch.Presentation;
using Microsoft.LightSwitch.Presentation.Extensions;
//gg added
using System.Windows.Data;
using System.ComponentModel;
using System.Windows.Controls;
using System.Reflection;
using Microsoft.LightSwitch.Threading;

namespace LightSwitchApplication
{
    public partial class ManageProjectAssignments
    {
        partial void ManageProjectAssignments_Created()
        {
            // GG added
            // Overrite the behavior of this windows to show/hide blocks dependinf 
            //if we are under the Feature, Incident or Task tab
            this.FindControl("FITContainer").ControlAvailable += (sender, eventargs) =>
                {
                   TabControl tabs= (TabControl)eventargs.Control;
                   tabs.SelectionChanged += (s, ea) =>
                   {
                       switch (tabs.SelectedIndex)
                       { 
                           //When Feature is selected
                           case 0:
                               this.FindControl("FeatureDetails").IsVisible = true;
                               this.FindControl("IncidentDetails").IsVisible = false;
                               this.FindControl("TasksDetails").IsVisible = false;
                               hideModalWindows();
                               break;

                           //Incident is selected
                           case 1:
                               this.FindControl("FeatureDetails").IsVisible = false;
                               this.FindControl("IncidentDetails").IsVisible = true;
                               this.FindControl("TasksDetails").IsVisible = false;
                               hideModalWindows();
                               break;

                           //Task is selected
                           case 2:
                               this.FindControl("FeatureDetails").IsVisible = false;
                               this.FindControl("IncidentDetails").IsVisible = false;
                               this.FindControl("TasksDetails").IsVisible = true;
                               hideModalWindows();
                               break;

                       }
                   };

                };

        }

        partial void ManageProjectAssignments_InitializeDataWorkspace(List<IDataService> saveChangesTo)
        {
            // GGaldamez code added 05/20/2014

            //Set to show completed features, incidents and tasks
            

            //Creating a dictionary for each modal windows
            //The dictionary contains values like this:
            //{Key: <ModalWindowName>, Value: <AssociatedClassName>}
            Dictionary<string, string> modalWindows = new Dictionary<string, string>();
            modalWindows.Add("FeatureRequirementTimeTracking","TimeTrack");
            modalWindows.Add("FeatureIssueTimeTracking","TimeTrack");
            modalWindows.Add("IncidentRequirementTimeTracking","TimeTrack");
            modalWindows.Add("IncidentIssueTimeTracking","TimeTrack");
            modalWindows.Add("TaskRequirementTimeTracking","TimeTrack");
            modalWindows.Add("TaskIssueTimeTracking", "TimeTrack");
            modalWindows.Add("ProjectFeatureComments", "ProjectFeatureComment");
            modalWindows.Add("ProjectIncidentsComments","ProjectIncidentComment");
            modalWindows.Add("ProjectTasksComments", "ProjectTaskComment");

            foreach (var modalWindow in modalWindows)
            {   //Overriding event handling for maodal windows
                //Overriding the Closing event for the modal windows
                //This is necesarry because if the window is closed by pressing the "X" at the top right corner
                //we must discard the changes just the same as if the CANCEL button was pressed
                this.FindControl(modalWindow.Key).ControlAvailable += (sender, eventargs) =>
                    {
                        ChildWindow window = (ChildWindow)eventargs.Control;
                        window.Closing += (s, e) =>
                        {
                            this.OnModalWindowClose(modalWindow.Value, e); //Function to discard changes when "X" is pressed
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
                           // this.FindControl("ProjectFeatureComments").IsVisible = false;
                            hideModalWindows();
                        }
                        break;
                    case "ProjectIncidentComment":
                        foreach (var comment in this.DataWorkspace.ApplicationData.Details.GetChanges().OfType<ProjectIncidentComment>())
                        {
                            comment.Details.DiscardChanges();
                            hideModalWindows();
                        }
                        break;
                    case "ProjectTaskComment":
                        foreach (var comment in this.DataWorkspace.ApplicationData.Details.GetChanges().OfType<ProjectTaskComment>())
                        {
                            comment.Details.DiscardChanges();
                            hideModalWindows();
                        }
                        break;
                    case "TimeTrack":
                        foreach (var comment in this.DataWorkspace.ApplicationData.Details.GetChanges().OfType<TimeTrack>())
                        {
                            comment.Details.DiscardChanges();
                            hideModalWindows();
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
                        //this.FindControl("ProjectFeatureComments").IsVisible = false;
                        hideModalWindows();
                    }
                    break;
                case "ProjectIncidentComment":
                    foreach (var comment in this.DataWorkspace.ApplicationData.Details.GetChanges().OfType<ProjectIncidentComment>())
                    {
                        comment.Details.DiscardChanges();
                        hideModalWindows();
                    }
                    break;
                case "ProjectTaskComment":
                    foreach (var comment in this.DataWorkspace.ApplicationData.Details.GetChanges().OfType<ProjectTaskComment>())
                    {
                        comment.Details.DiscardChanges();
                        hideModalWindows();
                    }
                    break;
                case "TimeTrack":
                    foreach (var tt in this.DataWorkspace.ApplicationData.Details.GetChanges().OfType<TimeTrack>())
                    {
                        tt.Details.DiscardChanges();
                        hideModalWindows();
                    }
                    break;
            }

            this.CloseModalWindow(windowName);

        }


        /// <summary>
        /// The following functions are for opening Features and Incidents in OnTIme
        /// </summary>
        partial void OpenFeatureOnTimeWindow_Execute()
        {
            UIHelpers.WindowHelper.OpenInOntime("features", this.Features.SelectedItem.FeatureId.ToString());
        }
        partial void OpenIncidentOnTimeWindow_Execute()
        {
            UIHelpers.WindowHelper.OpenInOntime("defects", this.ProjectIncidentsItem.SelectedItem.IncidentId.ToString());
        }
        

        //Generic process used when the OK button is pressed in a modal window
        private void SaveModalWindow(string windowName)
        {
            if (!this.Details.ValidationResults.Any())
            {
                this.CloseModalWindow(windowName);
                this.Save();
                hideModalWindows();
            }
        }

        /// <summary>
        /// The following methods are for opening modal windows
        /// </summary>
        //Open methods..
        //Features-Incidents-Tasks comments
        partial void OpenFeatureCommentsWindow_Execute()
        {
            this.OpenModalWindow("ProjectFeatureComments");
            //this.FindControl("ProjectFeatureComments").IsVisible = false;
            hideModalWindows();
        }
        partial void OpenIncidentCommentsWindow_Execute()
        {
            this.OpenModalWindow("ProjectIncidentsComments");
            hideModalWindows();
        }
        partial void ProjectTasksComments_Execute()
        {
            this.OpenModalWindow("ProjectTasksComments");
            hideModalWindows();
        }

        //---Features
        partial void OpenFeatureReqTimeTrackWindow_Execute()
        {
            this.OpenModalWindow("FeatureRequirementTimeTracking");
            hideModalWindows();
        }
        partial void OpenFeatureIssueTimeTrackWindow_Execute()
        {
            this.OpenModalWindow("FeatureIssueTimeTracking");
            hideModalWindows();        
        }
        
        //---Incidents
        partial void OpenIncidentReqTimeTrackWindow_Execute()
        {
            this.OpenModalWindow("IncidentRequirementTimeTracking");
            hideModalWindows();
        }
        partial void OpenIncidentIssueTimeTrackWindow_Execute()
        {
            this.OpenModalWindow("IncidentIssueTimeTracking");
            hideModalWindows();
        }
        
        //---Tasks
        partial void OpenTaskReqTimeTrackWindow_Execute()
        {
            this.OpenModalWindow("TaskRequirementTimeTracking");
            hideModalWindows();
        }
        partial void OpenTaskIssueTimeTrackWindow_Execute()
        {
            this.OpenModalWindow("TaskIssueTimeTracking");
            hideModalWindows();
        }
        //End open methods


        /// <summary>
        /// The following methods are for saving data from Modal Windows
        /// </summary>
        //Save methods..
        //Features-Incidents-Tasks comments   
        partial void SaveFeatureComments_Execute()
        {
            this.SaveModalWindow("ProjectFeatureComments");
           // this.FindControl("ProjectFeatureComments").IsVisible = false;
            hideModalWindows();
        }
        partial void SaveIncidentComments_Execute()
        {
            this.SaveModalWindow("ProjectIncidentsComments");
            hideModalWindows();
        }
        partial void SaveTaskComments_Execute()
        {
            this.SaveModalWindow("ProjectTasksComments");
            hideModalWindows();
        }
        
        //---Requirementes : features, incidents, tasks
        partial void FeatureRequirementTimeTrackingOK_Execute()
        {
            this.SaveModalWindow("FeatureRequirementTimeTracking");
            hideModalWindows();
        }
        partial void IncidentsRequirementTimeTracksOK_Execute()
        {
            this.SaveModalWindow("IncidentRequirementTimeTracking");
            hideModalWindows();
        }
        partial void TaskRequirementTimeTrackingOK_Execute()
        {
            this.SaveModalWindow("TaskRequirementTimeTracking");
            hideModalWindows();
        }
        
        //---Issues : features, incidents, tasks
        partial void FeatureIssuesTimeTrackingOK_Execute()
        {
            this.SaveModalWindow("FeatureIssueTimeTracking");
            hideModalWindows();
        }
        partial void IncidentIssueTimeTrackingOK_Execute()
        {
            this.SaveModalWindow("IncidentIssueTimeTracking");
            hideModalWindows();
        }
        partial void TaskIssueTimeTrackingOK_Execute()
        {
            this.SaveModalWindow("TaskIssueTimeTracking");
            hideModalWindows();
        }


        /// <summary>
        /// The following methods are when you push "CANCEL" button in ModalWindows
        /// </summary>
        //Cancel button methods
        //Features-Incidents-Tasks comments  
        partial void CancelFeatureComments_Execute()
        {
            this.CancelModalWindow("ProjectFeatureComments", "ProjectFeatureComment");
            hideModalWindows();
        }
        partial void CancelIncidentComments_Execute()
        {
            this.CancelModalWindow("ProjectIncidentsComments", "ProjectIncidentComment");
            hideModalWindows();
        }
        partial void CancelTaskComments_Execute()
        {
            this.CancelModalWindow("ProjectTasksComments", "ProjectTaskComment");
            hideModalWindows();
        }

        //--Features
        partial void FeatureRequirementTimeTrackingCANCEL_Execute()
        {
            this.CancelModalWindow("FeatureRequirementTimeTracking", "TimeTrack");
            hideModalWindows();
        }
        partial void FeatureIssuesTimeTrackingCANCEL_Execute()
        {
            this.CancelModalWindow("FeatureIssueTimeTracking", "TimeTrack");
            hideModalWindows();
        }

        //--Incidents
        partial void IncidentsRequirementsTimeTracksCANCEL_Execute()
        {
            this.CancelModalWindow("IncidentRequirementTimeTracking", "TimeTrack");
            hideModalWindows();
        }
        partial void IncidentIssueTimeTrackingCANCEL_Execute()
        {
            this.CancelModalWindow("IncidentIssueTimeTracking", "TimeTrack");
            hideModalWindows();
        }
        
        //--Tasks
        partial void TaskRequirementTimeTrackingCANCEL_Execute()
        {
            this.CancelModalWindow("TaskRequirementTimeTracking", "TimeTrack");
            hideModalWindows();
        }
        partial void TaskIssueTimeTrackingCANCEL_Execute()
        {
            this.CancelModalWindow("TaskIssueTimeTracking", "TimeTrack");
            hideModalWindows();
        }

    
        //End button methods

      

        //Hiding all ModalWindows
        public void hideModalWindows()
        {
            this.FindControl("ProjectFeatureComments").IsVisible = false;
            this.FindControl("ProjectIncidentsComments").IsVisible = false;
            this.FindControl("ProjectTasksComments").IsVisible = false;
            this.FindControl("FeatureRequirementTimeTracking").IsVisible = false;
            this.FindControl("FeatureIssueTimeTracking").IsVisible = false;
            this.FindControl("IncidentRequirementTimeTracking").IsVisible = false;
            this.FindControl("IncidentIssueTimeTracking").IsVisible = false;
            this.FindControl("TaskRequirementTimeTracking").IsVisible = false;
            this.FindControl("TaskIssueTimeTracking").IsVisible = false;


        }

    } //Public Partial class ManageProjectAssignments
} //Namespace lightswitch
