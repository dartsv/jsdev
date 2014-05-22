using System;


namespace LightSwitchApplication
{
    // This is a manually created class
    // The client.lmsl file was edited to reference this class by adding the following xaml to the top of the file:
    //  <GlobalValueContainerDefinition Name="GlobalStrings">
    //    <GlobalValueDefinition Name="LoggedOnUser" ReturnType=":String">
    //        <GlobalValueDefinition.Attributes>
    //            <DisplayName Value="LoggedOnUser" />
    //            <Description Value="Gets the currently logged on user." />
    //        </GlobalValueDefinition.Attributes>
    //    </GlobalValueDefinition>
    //  </GlobalValueContainerDefinition>
    public class GlobalStrings
    {
        //LoggedOnUser matches the name of the GlobalValueDefinition addition to the Common.lsml file
        //This function will return the username without the domain name attached
        //For example JS\nbova will result in a returned value of nbova

        //public static String LoggedOnUser()
        //{
        //   return Application.Current.User.Name;
        //}
        public static String LoggedOnUser()
        {
            string result = "";
            string LogonName = Application.Current.User.Name;

            try
            {
                if (LogonName.Contains("\\"))
                {
                    string[] parts = LogonName.Split('\\');
                    result = parts[1];
                }
                else
                {
                    result = LogonName.ToLower();
                }
            }
            catch (Exception ex)
            {
                result = "";
            }

            return result;
        }

    }
}
