using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace NotifcationTrigger.Configuration
{

    public class TenantConfiguration
    {

        //production path
        //private static string _file = @"D:\home\site\wwwroot\ConfigurationFile\SCIMConfig.xml";
        //local testing path
        private static string _file = @"ConfigurationFile\SCIMConfig.xml";

        public static string XMLConfigurationFilePath
        {
            get { return _file; }
            set { _file = value; }
        }
        public static bool IsValidSCIMClient(Guid tenantGUID)
        {
            bool rv = false;
            try
            {
                List<TenantConfigurationInfo> tcInfos = TenantConfiguration.LoadXMLConfiguration();

                string UIDisplay = string.Empty;
                foreach (TenantConfigurationInfo tInfo in tcInfos)
                {
                    if (Guid.TryParse(tInfo.TenantID, out Guid tenantConfigGUID))
                    {
                        //is the the GUID in the EntityChange
                        if (tenantConfigGUID == tenantGUID)
                        {
                            //found a valid client
                            rv = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return rv;
        }
        private static List<TenantConfigurationInfo> LoadXMLConfiguration()
        {
            XmlDocument document = new XmlDocument();

            try
            {

                document.Load(XMLConfigurationFilePath);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to load xml doc from " + XMLConfigurationFilePath + " " + ex.Message);

            }
            XmlNodeList nodes = document.SelectNodes("/Tenants/Tenant");
            List<TenantConfigurationInfo> tenantConfigurations = new List<TenantConfigurationInfo>();
            foreach (XmlNode item in nodes)
            {
                TenantConfigurationInfo tenantCInfo = new TenantConfigurationInfo();
                foreach (XmlNode childNode in item.ChildNodes)
                {
                    try
                    {
                        switch (childNode.Name)
                        {
                            case nameof(TenantXMLElementNames.OrganizationId):
                                tenantCInfo.OrganizationId = childNode.InnerText;
                                break;
                            case nameof(TenantXMLElementNames.TenantId):
                                tenantCInfo.TenantID = childNode.InnerText;
                                break;
                            case nameof(TenantXMLElementNames.ClientId):
                                tenantCInfo.ClientID = childNode.InnerText;
                                break;
                            case nameof(TenantXMLElementNames.HomeRealm):
                                tenantCInfo.HomeRealm = childNode.InnerText;
                                break;
                            case nameof(TenantXMLElementNames.SCIMVersion):
                                tenantCInfo.SCIMVersion = childNode.InnerText;
                                break;
                            case nameof(TenantXMLElementNames.SecretKey):
                                tenantCInfo.SecretKey = childNode.InnerText;
                                break;
                            case nameof(TenantXMLElementNames.TargetURL):
                                tenantCInfo.TargetURL = childNode.InnerText;
                                break;
                        }
                        string myStr = childNode.Name + childNode.InnerText;
                    }
                    catch (ArgumentException ex)
                    {
                        //invalid value
                    }
                }//end 
                tenantConfigurations.Add(tenantCInfo);
            }
            return tenantConfigurations;

        }
    }
    public class TenantConfigurationInfo
    {
        public string TenantID { get; set; }
        public string OrganizationId { get; set; }
        public string TargetURL { get; set; }
        public string HomeRealm { get; set; }
        public string ClientID { get; set; }
        public string SecretKey { get; set; }
        public string SCIMVersion { get; set; }
    }
    /// <summary>
    /// XML element names in the SCIMConfig file
    /// </summary>
    public enum TenantXMLElementNames
    {
        TenantId = 1,
        OrganizationId,
        TargetURL,
        HomeRealm,
        ClientId,
        SecretKey,
        SCIMVersion
    }
    public enum EntityTypes
    {
        Person=1,
        Group
    }
}
