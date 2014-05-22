using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DataImporter.ScriptGenerators
{
    public class ClientImportScriptGenerator
    {
        public static bool GenerateClientImportScript(string sourceFilename, string delimiter) 
        {
            var splitExpression = new Regex(@"(" + delimiter + @")(?=(?:[^""]|""[^""]*"")*$)");

            using (var reader = new StreamReader(sourceFilename))
            {
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    var columns = splitExpression.Split(line).Where(s => s != delimiter).ToArray();
                    string algo = string.Format(
                        @"IF (SELECT COUNT(1) from Clients WHERE Account = '{1}') = 1
                            UPDATE Clients SET Name = '{0}', IsActive = {2}, IsInternal = {3} WHERE Account = '{1}'
                        ELSE
                            INSERT INTO Clients(Name,Account,IsActive,IsInternal) Values('{0}','{1}',{2},{3})
                        GO",
                              columns[0].Replace("'","''").Replace("\"",""),
                              columns[1],
                              columns[2].Trim() == "True" ? 1 : 0,
                              columns[3].Trim() == "True" ? 1 : 0);

                    Console.WriteLine(algo);
                }

            }

            return true;
        }
    }
}
