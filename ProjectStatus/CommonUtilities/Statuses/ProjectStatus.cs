using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CommonUtilities.Statuses
{
    public class ProjectStatus
    {
        public enum Statuses
        {
            [Description("Inactive")]
            Inactive,
            [Description("Green")]
            Green,
            [Description("Yellow")]
            Yellow,
            [Description("Red")]
            Red
            
        }

        public string Status { get; set; }
        public int Order { get; set; }

        public ProjectStatus(string value)
        {
            this.Status = value;
            this.Order = GetEnumValue(value);
        }

        public ProjectStatus(Statuses value)
        {
            this.Order = (int)value;
            this.Status = GetEnumDescription(value);
        }

        private static int GetEnumValue(string description)
        {
            Array statuses = Enum.GetValues(typeof(Statuses));
            for (int i = 0; i < statuses.Length; i++)
            {
                if (description == GetEnumDescription((Statuses)statuses.GetValue(i))) return i;
            }
            return int.MaxValue;
        }

        private static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
    }
}