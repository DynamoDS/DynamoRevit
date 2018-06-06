using System;
using Microsoft.Win32;
using System.Security.AccessControl;
using System.Diagnostics;

namespace Dynamo.Applications.Tools
{
    internal static class Presistence
    {
        /// <summary>
        /// Dynamo Registry Key which may be null if couldn't be created or open
        /// Typical: 'HKEY_CURRENT_USER\SOFTWARE\Dynamo'
        /// </summary>
        private static RegistryKey RegKey = null;
        
        /// <summary>
        /// Dynamo Registry Key Security (i.e. access rights)
        /// </summary>
        private static RegistrySecurity RS = null;

        /// <summary>
        /// Constructor
        /// </summary>
        static Presistence()
        {
            const string KeyName = @"SOFTWARE\Dynamo";
            string user = Environment.UserDomainName + "\\" + Environment.UserName;
            RS = new RegistrySecurity();
            RS.AddAccessRule(new RegistryAccessRule(user,
                RegistryRights.FullControl,
                InheritanceFlags.None,
                PropagationFlags.None,
                AccessControlType.Allow));
            RegKey = OpenOrCreateSubKey(Registry.CurrentUser, KeyName, RS);
            if (RegKey != null) RegKey.Close();
            
            // keep it open
            RegKey = OpenOrCreateSubKey(Registry.CurrentUser, KeyName, RS);
        }

        private static readonly string VERSION = @"ActiveVersion";

        public static Version ActiveVersion {
            /// <summary>
            /// Returns last dynamo version persisted
            /// or null if no version was ever selected
            /// </summary>
            get
            {
                Object obj = null;
                GetValue(RegKey, VERSION, ref obj);
                if (obj == null) return null;
                if (obj.GetType() == typeof(String))
                {
                    try {
                        return new Version((string)obj);
                    } catch (Exception e) {
                        Debug.Write(e);
                        return null;
                    }
                }
                return null;
            }

            /// <summary>
            /// Persist current Dynamo Version selected
            /// </summary>
            set
            {
                if (value == null)
                {
                    SetValue(RegKey, VERSION, null, RS);
                }
                else
                {
                    SetValue(RegKey, VERSION, value.ToString(4), RS);
                }
            }
        }

        #region SafeVersionOfRegistry
        /// <summary>
        /// Safetly Open or Create a SubKey
        /// </summary>
        private static RegistryKey OpenOrCreateSubKey(RegistryKey key, string keyName, RegistrySecurity rs) {
            RegistryKey rk = null;
            /* open if exist */
            try {
                rk = key.OpenSubKey(keyName, true);
            } catch (Exception e) {
                Debug.Write(e);
            }
            if (rk != null) return rk;

            /* create one otherwise */
            try
            {
                rk = key.CreateSubKey(keyName, RegistryKeyPermissionCheck.Default, rs);
            }
            catch (Exception e)
            {
                Debug.Write(e);
            }
            return rk;
        }

        /// <summary>
        /// Safe version of SetValue
        /// </summary>
        private static void SetValue(RegistryKey key, String name, object value, RegistrySecurity rs)
        {
            try
            {
                if (value == null)
                {
                    key.DeleteValue(name);
                }
                else
                {
                    key.SetValue(name, value);
                }
                
            }
            catch (Exception e)
            {
                Debug.Write(e);
            }
        }

        /// <summary>
        /// Safe version of GetValue
        /// </summary>
        private static void GetValue(RegistryKey key, String name, ref object obj)
        {
            try
            {
                obj = key.GetValue(name);
            }
            catch (Exception e)
            {
                Debug.Write(e);
            }
        }
        #endregion
    }
}