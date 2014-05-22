using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CommonUtilities.Statuses
{
    public class FeatureStatus
    {
        public enum Statuses
        {
            [Description("Cancelled")]
            Canceled,
            [Description("Reassigned")]
            Reassigned,
            [Description("On-Hold")]
            OnHold,
            [Description("Completed")]
            Completed,
            [Description("Client Review")]
            ClientReview,
            [Description("Issue Resolution")]
            Rework,
            [Description("Testing")]
            UsTesting,
            [Description("Unit Testing/QA")]
            UnitTesting,
            [Description("Development")]
            Development,
            [Description("Analysis")]
            Analysis,
            [Description("Not Started")]
            NotStarted
        }

        public string Status { get; set; }
        public int Order { get; set; }

        public FeatureStatus(string value)
        {
            this.Status = value;
            this.Order = GetEnumValue(value);
        }

        public FeatureStatus(Statuses value)
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