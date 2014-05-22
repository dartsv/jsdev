using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;

namespace LightSwitchApplication.Util
{
    public class ExcelExporter
    {
        public object[] Data { get; set; }
        public string[] HeaderTitles { get; set; }
        public Type DataType { get; set; }

        public bool ShowHeaders { get; set; }
        public bool SeparatePropertiesNames { get; set; }

        public ExcelExporter(Type objectType, object[] data)
        {
            this.Data = data;
            this.DataType = objectType;
            this.HeaderTitles = null;
            this.ShowHeaders = true;
            this.SeparatePropertiesNames = false;
        }

        public ExcelExporter(Type objectType, object[] data, string[] headerTitles)
        {
            this.Data = data;
            this.DataType = objectType;
            this.HeaderTitles = headerTitles;
            this.ShowHeaders = true;
            this.SeparatePropertiesNames = false;
        }

        public ExcelPackage CreateFile() 
        {
            return CreateFile(this.DataType, this.Data, new string[] {""}, this.HeaderTitles);
        }

        public ExcelPackage CreateFile(string[] excludedProperties)
        {
            return CreateFile(this.DataType, this.Data, excludedProperties, this.HeaderTitles);
        }

        /// <summary>
        /// Creates an Excel file with a single sheet that contains the objects as rows
        /// Values are based on the property's ToString method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objects">Array of objects to be written in the Excel file</param>
        /// <param name="excludedProperties">Names of the properties not to be written in the file</param>
        /// <returns>ExcelPackage instance that represents the newly created file</returns>
        public ExcelPackage CreateFile(Type objectsType, object[] objects, string[] excludedProperties)
        {
            return CreateFile(objectsType, objects, excludedProperties, this.HeaderTitles);
        }

        /// <summary>
        /// Creates an Excel file with a single sheet that contains the objects as rows
        /// Values are based on the property's ToString method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objects">Array of objects to be written in the Excel file</param>
        /// <param name="excludedProperties">Names of the properties not to be written in the file</param>
        /// <param name="headerTitles">Names to be written as column header titles</param>
        /// <returns></returns>
        public  ExcelPackage CreateFile(Type objectsType, object[] objects, string[] excludedProperties, string[] headerTitles)
        {
            Type myType = objects.GetType();
            PropertyInfo[] objectProperties = objectsType.GetProperties();
            ExcelPackage file = new ExcelPackage();

            var workbook = file.Workbook;
            var sheet = workbook.Worksheets.Add(objectsType.Name);

            //Excel indexes start at 1
            int rowIndex = 1;
            int columnIndex = 1;

            //Write headers row
            if (this.ShowHeaders)
            {
                var headers = (headerTitles == null) ? objectProperties.Select(op => op.Name) : headerTitles;
                foreach (var header in headers)
                {
                    if (!excludedProperties.Contains(header))
                    {
                        sheet.Cells[rowIndex, columnIndex].Value =
                            (this.SeparatePropertiesNames) ?
                                System.Text.RegularExpressions.Regex.Replace(
                                    header,
                                    "(?<=[a-z])([A-Z])", " $1",
                                    System.Text.RegularExpressions.RegexOptions.Compiled).Trim()
                            : header;
                        columnIndex++;
                    }
                }
                rowIndex++;
                columnIndex = 1;
            }

            //Write the actual data
            foreach (var row in objects)
            {
                foreach (var property in objectProperties)
                {
                    if (!excludedProperties.Contains(property.Name))
                    {
                        var access = BuildGetAccessor(property.GetGetMethod());
                        var result = access(row);


                        sheet.Cells[rowIndex, columnIndex].Value = (result == null) ? "" : result;

                        columnIndex++;
                    }
                }
                rowIndex++;
                columnIndex = 1;
            }
            return file;
        }

        /// <summary>
        /// Private method used to create a single-use method call to accelerate the performance of reflection calls
        /// </summary>
        /// <param name="method">Get accesor of a property</param>
        /// <returns></returns>
        private static Func<object, object> BuildGetAccessor(MethodInfo method)
        {
            var obj = Expression.Parameter(typeof(object), "o");

            Expression<Func<object, object>> expr =
                Expression.Lambda<Func<object, object>>(
                    Expression.Convert(
                        Expression.Call(
                            Expression.Convert(obj, method.DeclaringType),
                            method),
                        typeof(object)),
                    obj);

            return expr.Compile();
        }
    }
}