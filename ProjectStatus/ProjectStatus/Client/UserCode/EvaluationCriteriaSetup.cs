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
    public partial class EvaluationCriteriaSetup
    {
        partial void ImportFromExcel_Execute()
        {
          
          
        //          'Use the Office Integration Pack Extension to do a variety of things with MS Office
        //          'Download here: http://visualstudiogallery.msdn.microsoft.com/35c4cf2a-5148-4716-afcf-0ccf8899cabf 
        //          'Dim canAccessCOM As Boolean = System.Runtime.InteropServices.Automation.AutomationFactory.IsAvailable

            //if the application type is web you cant use this library
//OfficeIntegration.Excel.Import(this.EvaluationCriterias);
            ExcelImporter.Importer.ImportFromExcel(this.EvaluationCriterias);
            //OfficeIntegration.Excel.Export(this.EvaluationCriterias);
        }

        partial void ImportFromExcel1_Execute()
        {
            // Write your code here.
            ExcelImporter.Importer.ImportFromExcel(this.EvaluationCriterias);
           // OfficeIntegration.Excel.Import(this.EvaluationCriterias);
        }

        partial void ImportFromExcel1a_Execute()
        {
            // Write your code here.//gg added
            ExcelImporter.Importer.ImportFromExcel(this.EvaluationCriterias);
            // OfficeIntegration.Excel.Import(this.EvaluationCriterias);

        }

        partial void EvaluationCriteriaSetup_Created()
        {
// Write your code here.

        }
    }
}
