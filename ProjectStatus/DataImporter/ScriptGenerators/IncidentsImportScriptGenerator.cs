using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataImporter.ScriptGenerators
{
    public class IncidentsImportScriptGenerator
    {
        public static bool GenerateIncidentImportScript(string sourceFilename, string delimiter)
        {
            var splitExpression = new Regex(@"(" + delimiter + @")(?=(?:[^""]|""[^""]*"")*$)");

            using (var reader = new StreamReader(sourceFilename))
            {
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    var columns = splitExpression.Split(line).Where(s => s != delimiter).ToArray();
                    
                    string algo = string.Format(
                        @"INSERT INTO ProjectIncidents(
                            IncidentId, ClientId, Name, DateFound,Priority,
                            Severity,AssignedDate,DODTeam,Status,PercentageCompleted,
                            CompletedDate, ProjectIncident_Project, ProjectIncident_Employee)
                        VALUES ('{0}','{1}','{2}', '{3}',{4},
                                {5},'{6}','{7}','{8}',{9},
                                '{10}',{11},{12} )",
                              columns[0].Replace("'", "''").Replace("\"", ""),
                              columns[1].Replace("'", "''").Replace("\"", ""),
                              columns[2].Replace("'", "''").Replace("\"", ""),
                              columns[3],
                              columns[4],

                              columns[5],
                              columns[6],
                              columns[7],
                              columns[8],
                              columns[9],

                              columns[10],
                              columns[11],
                              columns[12]);

                    Console.WriteLine(algo);
                }

            }

            return true;
        }
    }
}
