using System;
using System.Linq;
using System.Text;
using RegistryRT;

#nullable enable

namespace USBFunctionMode
{
    public class USBFNController
    {
        private Registry reg => LumiaApp.App.Registry;

        public USBFNController()
        {

        }

        public static USBFNController Instance => new();

        private static string GetUSBFNLocation()
        {
            return @"SYSTEM\CurrentControlSet\Control\USBFN";
        }

        private static string GetUSBFNConfigurationsLocation()
        {
            return $@"{GetUSBFNLocation()}\Configurations";
        }

        private static (int idProduct, int idVendor) GetUSBEnumerationIDs(string FunctionModeName)
        {
            switch (FunctionModeName.ToLower())
            {
                case "dplcompositeconfig":
                    return (0x90b7, 0x5c6);
                case "rmnetcompositeconfig":
                    return (0x9001, 0x5c6);
                case "serialcompositeconfig":
                    return (0x319b, 0x5c6);
                case "retailconfig":
                    return (0xa00, 0x45e);
                case "default":
                case "vidstream":
                default:
                    return (0xf0ca, 0x45e);
            }
        }

        public string[]? GetListOfConfigurations()
        {
            reg.GetSubKeyList(RegistryHive.HKEY_LOCAL_MACHINE, GetUSBFNConfigurationsLocation(), out string[] result);
            return result == null ? null : result.Where(x => !x.Equals("default", StringComparison.InvariantCultureIgnoreCase)).ToArray();
        }

        public string? GetCurrentConfiguration()
        {
            reg.QueryValue(RegistryHive.HKEY_LOCAL_MACHINE, GetUSBFNLocation(), "CurrentConfiguration", out RegistryType type, out byte[] buffer);
            
            if (buffer != null && type == RegistryType.String)
            {
                return Encoding.Unicode.GetString(buffer);
            }
            
            return null;
        }

        public void SetCurrentConfiguration(string FunctionModeName)
        {
            (int idProduct, int idVendor) = GetUSBEnumerationIDs(FunctionModeName);

            reg.WriteValue(RegistryHive.HKEY_LOCAL_MACHINE, GetUSBFNLocation(), "CurrentConfiguration", Encoding.Unicode.GetBytes(FunctionModeName), RegistryType.String);
            reg.WriteValue(RegistryHive.HKEY_LOCAL_MACHINE, GetUSBFNLocation(), "idProduct", BitConverter.GetBytes(idProduct), RegistryType.Integer);
            reg.WriteValue(RegistryHive.HKEY_LOCAL_MACHINE, GetUSBFNLocation(), "idVendor", BitConverter.GetBytes(idVendor), RegistryType.Integer);
        }

        public static string GetFunctionRoleFriendlyName(string FunctionModeName)
        {
            switch (FunctionModeName.ToLower())
            {
                case "default":
                    return "Windows Default";
                case "retailconfig":
                    return "Windows Retail";
                case "dplcompositeconfig":
                    return "Qualcomm Data Protocol Logging";
                case "rmnetcompositeconfig":
                    return "Qualcomm Wireless Diagnostics";
                case "serialcompositeconfig":
                    return "Qualcomm Serial Diagnostics";
                case "vidstream":
                    return "Windows Video Stream";
                default:
                    return FunctionModeName;
            }
        }

        public static string GetFunctionRoleDescription(string FunctionModeName)
        {
            switch (FunctionModeName.ToLower())
            {
                case "default":
                    return "Default mode of the USB port. Provides File transfer capabilities and video stream support.";
                case "retailconfig":
                    return "Retail mode of the USB port. Provides File transfer capabilities, video stream support, and connection with THOR2/WPinternals/Windows Device Recovery Tool/Lumia Software Updater Pro.";
                case "dplcompositeconfig":
                    return "SoC Vendor specific Diagnostic mode. Provides diagnostic logging for QPST (Qualcomm Phone Support Tool).";
                case "rmnetcompositeconfig":
                    return "SoC Vendor specific Wireless mode. Provides wireless interfaces for QPST (Qualcomm Phone Support Tool) as well as Operating Systems. The device will turn into an USB modem/router, and can be used to send text messages.";
                case "serialcompositeconfig":
                    return "SoC Vendor specific Serial mode. Provides serial debugging interfaces for QPST (Qualcomm Phone Support Tool).";
                case "vidstream":
                    return "Video stream mode of the USB port. Provides video stream support.";
                default:
                    return "Unknown.";
            }
        }
    }
}
