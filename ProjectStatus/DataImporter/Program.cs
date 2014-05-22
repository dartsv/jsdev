
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataImporter.ScriptGenerators;

namespace DataImporter
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string tablename = args[0].ToUpper();
                string filename = args[1];
                switch (tablename) 
                {
                    case "CLIENTS":
                        ClientImportScriptGenerator.GenerateClientImportScript(filename, ",");
                        break;
                    case "PROJECTINCIDENTS":
                        IncidentsImportScriptGenerator.GenerateIncidentImportScript(filename, ",");
                        break;
                    default:
                        Console.WriteLine(string.Format("Method to import data to table '{0}' does not exists", tablename));
                        break;
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
