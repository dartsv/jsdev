using System;
using System.Linq;
using System.IO;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using Microsoft.LightSwitch;
using Microsoft.LightSwitch.Framework.Client;
using Microsoft.LightSwitch.Presentation;
using Microsoft.LightSwitch.Presentation.Extensions;

namespace LightSwitchApplication
{
    public partial class ReviewForm
    {
        partial void ReviewForm_InitializeDataWorkspace(List<IDataService> saveChangesTo)
        {
            //set the title & department property for the employee being reviewed
            Employee Empl =  GetEmployeeByID(EmployeeIDParameter);
            //GG Changed
            //TitleProperty = Empl.Title; Not by title anymore
            //DepartmentProperty = Empl.Department;
            ECLevel = Empl.EmployeeTitle.EmployeeLevel.Id;
            ECDepartment = Empl.DeptEmployee.Id;
           

            //get the curren logged on employee
            Employee CurrentUser = GetLoggedOnUserID();
            if (this.ReviewHeaderID.HasValue) // We are editing an existing review
            {       /////if there is an existing Review, we will check if the review havent been completely signed
                if (ReviewHeader.SignedStatus == "Fully")
                {
                    this.FindControl("SupervisorEmployee").IsReadOnly = true;
                    this.FindControl("ReviewType").IsReadOnly = true;
                    this.FindControl("ProjectsCovered").IsReadOnly = true;
                    this.FindControl("Role").IsReadOnly = true;
                    this.FindControl("TimePeriodCoveredStart1").IsReadOnly = true;
                    this.FindControl("TimePeriodCoveredEnd1").IsReadOnly = true;
                    this.FindControl("MajorAssignments").IsReadOnly = true;
                    this.FindControl("Strengths1").IsReadOnly = true;
                    this.FindControl("DevelopmentNeeds").IsReadOnly = true;
                    this.FindControl("Published").IsReadOnly = true;
                    this.FindControl("SummaryRating").IsReadOnly = true;
                    this.FindControl("Promote").IsReadOnly = true;
                    //Grids
                    this.FindControl("ReviewCritieria").IsReadOnly = true;
                    this.FindControl("TrainingPlans").IsReadOnly = true;
                }
               

                    this.ReviewHeaderProperty = this.ReviewHeader;
                    // Only the reviewer is allowed to edit and save records
                    if (ReviewHeader.Reviewer != CurrentUser)
                    {
                        //turn off the save button
                        this.FindControl("Save").IsEnabled = false;
                        //turn off the reviewer signature button
                        this.FindControl("ReviewerSignature").IsEnabled = false;
                    }
                    else if (ReviewHeader.Employee != CurrentUser)
                    {
                        this.FindControl("EmployeeSignature").IsEnabled = false;
                    }
                    //turn off the employee review button if this is a self review
                    if (ReviewHeader.ReviewType == "Self")
                    {
                        this.FindControl("EmployeeSignature").IsEnabled = false;
                    }

                    // See if the review is signed by the employee or the reviewer
                    foreach (Signature JohnHancock in Signatures)
                    {
                        if (ReviewHeader.ReviewType == "Self" & JohnHancock.Employee == ReviewHeader.Employee)
                        {
                            this.FindControl("EmployeeSignature").IsEnabled = false;
                            this.FindControl("ReviewerSignature").DisplayName = "Self Signed " + DateTime.Today.ToShortDateString();
                            this.FindControl("ReviewerSignature").IsEnabled = false;
                            this.FindControl("Save").IsEnabled = false;
                        }
                        else if (JohnHancock.Employee == ReviewHeader.Employee)
                        {
                            this.FindControl("EmployeeSignature").IsEnabled = false;
                            this.FindControl("EmployeeSignature").DisplayName = "Employee Signed " + DateTime.Today.ToShortDateString();
                        }
                        else if (JohnHancock.Employee == ReviewHeader.Reviewer)
                        {
                            this.FindControl("ReviewerSignature").IsEnabled = false;
                            this.FindControl("ReviewerSignature").DisplayName = "Reviewer Signed " + DateTime.Today.ToShortDateString();
                            this.FindControl("Save").IsEnabled = false;
                        }
                    }
               
            }
            else //we are creating an new review
            {
                Employee emp = GetEmployeeByID(EmployeeIDParameter);
                ReviewHeader newReviewHeader = new ReviewHeader();
                newReviewHeader.Reviewer = GetLoggedOnUserID();
                newReviewHeader.TimePeriodCoveredStart = new DateTime(2013, 6, 1); //changed date
                newReviewHeader.TimePeriodCoveredEnd = new DateTime(2014, 5, 31); //changed date
                newReviewHeader.Employee = emp;
                newReviewHeader.SummaryRating = 2;
                newReviewHeader.SignedStatus = "Not Signed";
                newReviewHeader.Department =   emp.Department;
                newReviewHeader.Supervisor = emp.Supervsr.FirstName + " " + emp.Supervsr.LastName;
                newReviewHeader.SupervisorEmployee = this.DataWorkspace.ApplicationData.Employees_SingleOrDefault(emp.Supervsr.Id); //gg added
                newReviewHeader.Published = false;
                //set up the signature buttons
                //since this is a new review, we know the current user is the reviewer
                this.FindControl("ReviewerSignature").IsEnabled = true;
                this.FindControl("EmployeeSignature").IsEnabled = false;
                if (newReviewHeader.Reviewer == newReviewHeader.Employee)
                {
                    newReviewHeader.ReviewType = "Self";
                    this.FindControl("ReviewerSignature").IsEnabled = true;
                }
                else if (newReviewHeader.Reviewer == emp.Supervsr)
                {
                    newReviewHeader.ReviewType = "Annual";
                }
                else
                {
                    newReviewHeader.ReviewType = "Project";
                }
                
                this.ReviewHeaderProperty = newReviewHeader;
                //this.DataWorkspace.ApplicationData.SaveChanges();


                // now add the review criteria to the data grid-----------------------------modify here
                foreach (EvaluationCriteria EC in EvaluationCriterias)
                {
                    ReviewCritieria newReviewCriteria = this.DataWorkspace.ApplicationData.ReviewCritierias.AddNew();
                    newReviewCriteria.Category = EC.Category;
                    newReviewCriteria.Criteria = EC.Criteria;
                    newReviewCriteria.ReviewHeader = newReviewHeader;
                    newReviewCriteria.Rating = 2;
                }
                //this.DataWorkspace.ApplicationData.SaveChanges();
            }
        }

        partial void ReviewForm_Saved()
        {
            // Write your code here.
            this.Close(false);
            Application.Current.ShowReviewForm(this.ReviewHeaderProperty.Id, EmployeeIDParameter, ParentScreen);
        }

        partial void ReviewCritieria_Loaded(bool succeeded)
        {
            if (this.ReviewCritieria.Count == 0) //don't try to calculate an average on an empty set. It creates an error.
            {
                this.AverageRating = 0;
            }
            else
            {
                // AverageRating is a property on the screen
                // This lambda expression calculates the average
                this.AverageRating = (decimal)this.ReviewCritieria.Average(p => p.Rating);
            }
           
        }

       
        private Employee GetLoggedOnUserID()
        {
            // This method defaults the manager and director to the current logged on user and displays the edit screen

            // First find the employee number of the current logged on user
            string UserName = GlobalStrings.LoggedOnUser();
            int i = -1;
            foreach (Employee emp in this.DataWorkspace.ApplicationData.Employees)
            {
                i++;
                if (emp.UserID == UserName)
                {
                    return emp;
                }
            }
            // Throw an error if not found, otherwise initialize the fields and display the edit screen
            string Message = "There is not an employee record for the current logged on user. Unable to create a new review.";
            Exception myEX = new Exception(Message);
            throw myEX;
        }

        private Employee GetEmployeeByID(int EmployeeID)
        {
            int i = -1;
            foreach (Employee emp in this.DataWorkspace.ApplicationData.Employees)
            {
                i++;
                if (emp.Id == EmployeeID)
                {
                    return emp;
                }
            }
            // Throw an error if not found, otherwise initialize the fields and display the edit screen
            // Throw an error if not found, otherwise initialize the fields and display the edit screen
            string Message = "Cannot find employee record. Unable to create a new review.";
            Exception myEX = new Exception(Message);
            throw myEX;
        }



        partial void ReviewForm_Saving(ref bool handled)
        {
            if (this.FindControl("Save").IsEnabled == false)
            {
                this.DataWorkspace.ApplicationData.Details.DiscardChanges();
                handled = true;
                this.ShowMessageBox("You are not authorized to update this review.");
                //Application.Current.ShowReviewForm(this.ReviewHeaderProperty.Id, EmployeeIDParameter);
            }
        }

        partial void EmployeeSignature_Execute()
        {
            Employee CurrentUser = GetLoggedOnUserID();
            if (ReviewHeader.Employee == CurrentUser)
            {
                //Check for pending changes to the form
                //get change set
                EntityChangeSet ChangeSet = this.DataWorkspace.ApplicationData.Details.GetChanges();
                if (ChangeSet.Count() > 0)
                {
                    this.ShowMessageBox("All changes must be saved before signing. Signature not accepted.");
                }
                else if (this.FindControl("ReviewerSignature").DisplayName == "Reviewer Signature" & ReviewHeader.ReviewType != "Self")
                {
                    this.ShowMessageBox("Reviewer must sign review first to indicate that all editing is complete.");
                }
                else
                {
                    //ReviewCritieria newReviewCriteria = this.DataWorkspace.ApplicationData.ReviewCritierias.AddNew();
                    Signature JohnHancock = this.DataWorkspace.ApplicationData.Signatures.AddNew();
                    //Signature JohnHancock = new Signature();
                    JohnHancock.Employee = CurrentUser;
                    JohnHancock.SigningDate = DateTime.Today;
                    JohnHancock.ReviewHeader = this.ReviewHeader;
                    ReviewHeader.SignedStatus = "Fully";
                    this.DataWorkspace.ApplicationData.SaveChanges();
                    this.FindControl("EmployeeSignature").IsEnabled = false;
                    this.FindControl("EmployeeSignature").DisplayName = "Employee Signed " + DateTime.Today.ToShortDateString();
                    RefreshParentScreen();
                }
            }
            else
            {
                this.ShowMessageBox("You are not authorized to sign this review as the employee.");
            }
        }

        partial void ReviewerSignature_Execute()
        {
            if (ReviewHeader == null)
            {
                
                this.ShowMessageBox("You cannot sign a review that has not been edited or saved.");
                return;
            }

            //prompt to validate save
            string MessageText = "Signing will lock this review and prevent any further editing or deletion. Are you sure you want to sign the review?";
            if (this.ShowMessageBox(MessageText, "Warning", MessageBoxOption.YesNo) == System.Windows.MessageBoxResult.No) return;

            Employee CurrentUser = GetLoggedOnUserID();
            if (ReviewHeader.Reviewer == CurrentUser)
            {
                //Check for pending changes to the form
                //get change set
                EntityChangeSet ChangeSet = this.DataWorkspace.ApplicationData.Details.GetChanges();
                if (ChangeSet.Count() > 0)
                {
                    this.ShowMessageBox("All changes must be saved before signing. Signature not accepted.");
                }
                else
                {
                    //make sure the review is set to published
                    ReviewHeader.Published = true;
                    //sign the review
                    Signature JohnHancock = this.DataWorkspace.ApplicationData.Signatures.AddNew();
                    JohnHancock.Employee = CurrentUser;
                    JohnHancock.SigningDate = DateTime.Today;
                    JohnHancock.ReviewHeader = this.ReviewHeader;
                    ReviewHeader.SignedStatus = "Reviewer";
                    this.DataWorkspace.ApplicationData.SaveChanges();
                    //update the display
                    this.FindControl("ReviewerSignature").IsEnabled = false;
                    this.FindControl("ReviewerSignature").DisplayName = "Reviewer Signed " + DateTime.Today.ToShortDateString();
                    this.FindControl("Save").IsEnabled = false;
                    RefreshParentScreen();
                }
            }
            else
            {
                this.ShowMessageBox("You are not authorized to sign this review as the reviewer.");
            }
        }

        partial void ReviewForm_Closing(ref bool cancel)
        {
            RefreshParentScreen();
        }

        partial void ReverseSignatures_Execute()
        {
            // Blow away any edits. They are not allowed.
            this.DataWorkspace.ApplicationData.Details.DiscardChanges();

            int RowCount = ReviewHeader.Signatures.Count();
            for (int i = RowCount; i > 0; i--) //Must iterate backwards when deleting
            {
                //delete the signatures
                ReviewHeader.Signatures.ElementAt(i - 1).Delete();
            }
           
            // Reset the signed status to Not Signed
            ReviewHeader.SignedStatus = "Not Signed";
            this.DataWorkspace.ApplicationData.SaveChanges();
            this.FindControl("ReviewerSignature").DisplayName = "Reviewer Signature";
            this.FindControl("EmployeeSignature").DisplayName = "Employee Signature";
            this.FindControl("ReverseSignatures").DisplayName = "Signatures Reversed";
            this.FindControl("ReverseSignatures").IsEnabled = false;
            RefreshParentScreen();
        }

        partial void ReverseSignatures_CanExecute(ref bool result)
        {
            //result = this.Application.User.HasPermission(Permissions.SignatureOverride);
            if (this.Application.User.HasPermission(Permissions.SignatureOverride))
            {
                result = true;
                this.FindControl("ReverseSignatures").IsVisible = true;
            }
        }
        private void RefreshParentScreen()
        {
            //this code will refresh the ReviewsHeaders grid on the parent screen
            if (ParentScreen == "ReviewListByEmployee")
            {
                var parentScreen = Application.ActiveScreens.Where(a => a.Screen is ReviewListByEmployee).FirstOrDefault();
                parentScreen.Screen.Details.Dispatcher.BeginInvoke(() =>
                {
                    //  refreshes the entire parent screen. 
                    //  parentScreen.Screen.Refresh();
                    //  refreshes just the ReviewHeaders grid on the parent screen and preserves selected employee state
                    ((ReviewListByEmployee)parentScreen.Screen).ReviewHeaders.Refresh();
                });
            }
            else if (ParentScreen == "ManageMyReviews")
            {
                var parentScreen = Application.ActiveScreens.Where(a => a.Screen is ManageMyReviews).FirstOrDefault();
                parentScreen.Screen.Details.Dispatcher.BeginInvoke(() =>
                {
                    //  refreshes the entire parent screen. 
                    //  parentScreen.Screen.Refresh();
                    //  refreshes just the ReviewHeaders grid on the parent screen and preserves selected employee state
                    ((ManageMyReviews)parentScreen.Screen).ReviewHeaders.Refresh();
                });
            }
           
        }

        partial void ReviewForm_Activated()
        {
            // Write your code here.

        }

        partial void ReviewForm_Created()
        {
            // Write your code here.

        }

        partial void ReviewCritieriaAddAndEditNew_CanExecute(ref bool result)
        {
            // Write your code here.

        }

        partial void ReviewCritieriaAddAndEditNew_Execute()
        {
            // Write your code here.
            

        }

    }
}