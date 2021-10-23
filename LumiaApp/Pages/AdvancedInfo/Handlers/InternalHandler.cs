using System;
using AdvancedInfoRT;

namespace AdvancedInfo.Handlers
{
    public class InternalHandler
    {
        private AIRT airt => LumiaApp.App.AIRT;

        public InternalHandler()
        {
            long RAMVal;
            bool result = airt.GetSystemRAM(out RAMVal);

            if (result)
            {
                RAM = (Math.Round(double.Parse(RAMVal.ToString()) / (1024d * 1024 * 1024) * 100, MidpointRounding.ToEven) / 100).ToString() +" GB";
            }

            FirmwareBuild = airt.GetFirmwareVersion() + " (" + airt.GetSystemFirmwareVersion() + ")";
        }

        public string RAM { get; internal set; }
        public string FirmwareBuild { get; internal set; }
    }
}
