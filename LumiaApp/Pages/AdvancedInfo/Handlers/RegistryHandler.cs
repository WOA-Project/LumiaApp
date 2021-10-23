using System.Text;
using RegistryRT;

namespace AdvancedInfo.Handlers
{
    public class RegistryHandler
    {
        private Registry regrt => LumiaApp.App.Registry;

        public RegistryHandler()
        {
            ProductCode = ReadRegistryStringFromDTI("PhoneManufacturerModelName");
            MobileOperator = ReadRegistryStringFromDTI("PhoneMobileOperatorName");
            ServiceProvider = ReadRegistryStringFromDTI("PhoneMobileOperatorDisplayName");
            SOC = ReadRegistryStringFromDTI("PhoneSOCVersion");
            Manufacturer = ReadRegistryStringFromDTI("PhoneManufacturerDisplayName");
            if (Manufacturer == null)
                Manufacturer = ReadRegistryStringFromDTI("PhoneManufacturer");
            ModelName = ReadRegistryStringFromDTI("PhoneModelName");
            ProductCodeCleaned = ReadRegistryStringFromDTI("PhoneHardwareVariant");
            ReleaseName = ReadRegistryStringFromDTI("PhoneReleaseVersion");
        }

        private string ReadRegistryStringFromDTI(string Value)
        {
            return ReadRegistryString(RegistryHive.HKEY_LOCAL_MACHINE, "SYSTEM\\Platform\\DeviceTargetingInfo", Value);
        }

        private string ReadRegistryString(RegistryHive hive, string Key, string Value)
        {
            uint regtype;
            byte[] buffer;
            bool result = regrt.QueryValue(hive, Key, Value, out regtype, out buffer);
            if (result)
            {
                return Encoding.ASCII.GetString(buffer);
            }
            return null;
        }

        public string ProductCode { get; internal set; }
        public string ProductCodeCleaned { get; internal set; }
        public string MobileOperator { get; internal set; }
        public string ServiceProvider { get; internal set; }
        public string SOC { get; internal set; }
        public string Manufacturer { get; internal set; }
        public string ModelName { get; internal set; }
        public string ReleaseName { get; internal set; }
    }
}
