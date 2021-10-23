using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Networking.NetworkOperators;

namespace AdvancedInfo.Handlers
{
    public class ModemHandler
    {
        private ModemHandler()
        {
            
        }

        public static async Task<ModemHandler> LoadHandlerAsync()
        {
            ModemHandler handler = new();
            await handler.Load();
            return handler;
        }

        private async Task Load()
        {
            string selectorStr = MobileBroadbandModem.GetDeviceSelector();
            DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(selectorStr);

            List<string> modemList = new();

            bool MoreThanOne = devices.Count > 1;

            int counter = 0;
            foreach (DeviceInformation device in devices)
            {
                counter++;
                MobileBroadbandModem modem = MobileBroadbandModem.FromId(device.Id);

                string suffix = ": ";

                if (MoreThanOne)
                {
                    suffix = " " + counter + ": ";
                }

                modemList.Add("IMEI" + suffix + modem.DeviceInformation.SerialNumber);
                if (modem.DeviceInformation.TelephoneNumbers.Count > 0)
                {
                    foreach (string number in modem.DeviceInformation.TelephoneNumbers)
                    {
                        modemList.Add("MDN" + suffix + number);
                    }
                }
            }

            ModemInformation = modemList.AsReadOnly();
        }

        public IReadOnlyList<string> ModemInformation { get; internal set; }
    }
}
