using System;
using System.Linq;
using System.IO;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using Microsoft.LightSwitch;
using Microsoft.LightSwitch.Framework.Client;
using Microsoft.LightSwitch.Presentation;
using Microsoft.LightSwitch.Presentation.Extensions;
using System.Collections.Specialized;
using System.Windows.Controls;
using Microsoft.LightSwitch.Threading;

namespace LightSwitchApplication
{
    public partial class ManageMySkills
    {
        private bool CanEdit = false;
        private bool DiscardNewSkill = true;
        private bool JustOpened = true;

        void EmployeeSkillDialog_ControlAvailable(object sender, ControlAvailableEventArgs e)
        {
            ChildWindow window = (ChildWindow)e.Control;
            window.Closed += delegate {
                if (this.DiscardNewSkill)
                {
                    this.Details.Dispatcher.BeginInvoke(() =>
                        {
                            this.EmployeeSkills.DeleteSelected();
                        });
                }

                this.DiscardNewSkill = true;
                    
            };
        }

        partial void ManageMySkills_Activated()
        {
            this.DataWorkspace.ApplicationData.SkillCategories.GetQuery().Execute();
            if (this.SkillCategory == null)
            {
                this.SkillCategory = this.DataWorkspace.ApplicationData.SkillCategories.First();
            }

            this.FindControl("EmployeeSkillDialog").ControlAvailable += new EventHandler<ControlAvailableEventArgs>(EmployeeSkillDialog_ControlAvailable);
        }

        partial void EmployeeSkillsAddNew_CanExecute(ref bool result)
        {
            result = this.EmployeeSkills.CanAddNew && CanEdit;
        }

        partial void EmployeeSkillsAddNew_Execute()
        {
            var newEmployeeSkill = this.EmployeeSkills.AddNew();
            this.EmployeeSkills.SelectedItem = newEmployeeSkill;
            this.SkillCategory = this.DataWorkspace.ApplicationData.SkillCategories.First();
            this.OpenModalWindow("EmployeeSkillDialog");
        }

        partial void EmployeeSkillDialogOk_Execute()
        {
            this.DiscardNewSkill = false;
            this.CloseModalWindow("EmployeeSkillDialog");
        }

        partial void EmployeeSkillsEditSelected1_CanExecute(ref bool result)
        {
            result = this.EmployeeSkills.CanEdit && CanEdit;
        }

        partial void EmployeeSkillsEditSelected1_Execute()
        {
            this.EmployeeSkills.EditSelected();
        }

        partial void EmployeeSkillsDeleteSelected_CanExecute(ref bool result)
        {
            result = this.EmployeeSkills.CanDelete && CanEdit;
        }

        partial void EmployeeSkillsDeleteSelected_Execute()
        {
            this.EmployeeSkills.DeleteSelected();
        }

        partial void EmployeeSkillDialogCancel_Execute()
        {
            this.DiscardNewSkill = true;
            this.CloseModalWindow("EmployeeSkillDialog");
        }

        partial void SkillCategory_Changed()
        {
            var selectedbeforeRefresh = this.EmployeeSkills.SelectedItem;
            //This needs to be done in the UI thread so that the refresh gets done synchronously
            Dispatchers.Main.Invoke(() => 
            {
                this.Skills.Refresh();
            });
            this.EmployeeSkills.SelectedItem = selectedbeforeRefresh;

            if (!JustOpened)
            {
                this.EmployeeSkills.SelectedItem.Skill = this.Skills.FirstOrDefault();
            }
            JustOpened = false;
        }

        partial void ManageMySkills_Created()
        {
            if (UserId == null || UserId == GlobalStrings.LoggedOnUser())
            {
                UserId = GlobalStrings.LoggedOnUser();
                CanEdit = true;
            }
            else
            {
                this.DisplayName = this.DataWorkspace.ApplicationData.Employees.Where(x => x.UserID == UserId).FirstOrDefault().FullName + " Skills";
                CanEdit = false;
            }

            if (this.Application.User.HasPermission(Permissions.SecurityAdministration))
            {
                CanEdit = true;
            }
        }


    }
}
