using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using System.IO;
using System.Threading.Tasks;

namespace AdvancedInfo.Handlers
{
    public class DPPHandler
    {
        private StorageFolder DPP;
        private StorageFolder Vendor;
        private StorageFolder RegScreen;
        private StorageFolder Certs;

        public static async Task<DPPHandler> LoadHandlerAsync()
        {
            var handler = new DPPHandler();
            await handler.Load();
            return handler;
        }

        private DPPHandler()
        {
            
        }

        private async Task Load()
        {
            DPP = await StorageFolder.GetFolderFromPathAsync(Environment.GetEnvironmentVariable("SystemDrive") + @"\DPP");
            IReadOnlyList<StorageFolder> EnumeratedFolders = await DPP.GetFoldersAsync();
            bool IsMMODevice = EnumeratedFolders.Any(x => string.Equals(x.Name, "MMO", StringComparison.InvariantCultureIgnoreCase));
            Vendor = await DPP.GetFolderAsync(IsMMODevice ? "MMO" : "Nokia");

            IReadOnlyList<StorageFolder> EnumeratedVendorFolders = await Vendor.GetFoldersAsync();
            IReadOnlyList<StorageFile> EnumeratedVendorFiles = await Vendor.GetFilesAsync();

            bool HasRegScreen = EnumeratedVendorFolders.Any(x => string.Equals(x.Name, "RegScreen", StringComparison.InvariantCultureIgnoreCase));
            bool HasCerts = EnumeratedVendorFolders.Any(x => string.Equals(x.Name, "certs", StringComparison.InvariantCultureIgnoreCase));

            if (HasRegScreen)
            {
                RegScreen = await Vendor.GetFolderAsync("RegScreen");
                IReadOnlyList<StorageFile> RegScreenFiles = await RegScreen.GetFilesAsync();
                
                if (RegScreenFiles.Any(x => string.Equals(x.Name, "coo.txt", StringComparison.InvariantCultureIgnoreCase)))
                {
                    StorageFile COOTxt = await RegScreen.GetFileAsync("coo.txt");
                    COO = await FileIO.ReadTextAsync(COOTxt);
                }

                if (RegScreenFiles.Any(x => string.Equals(x.Name, "imagelabel_dark.png", StringComparison.InvariantCultureIgnoreCase)))
                {
                    StorageFile ImageLabel = await RegScreen.GetFileAsync("imagelabel_dark.png");
                    RegulatoryBlack = new BitmapImage();
                    await RegulatoryBlack.SetSourceAsync(await ImageLabel.OpenReadAsync());
                }

                if (RegScreenFiles.Any(x => string.Equals(x.Name, "imagelabel_light.png", StringComparison.InvariantCultureIgnoreCase)))
                {
                    StorageFile ImageLabel = await RegScreen.GetFileAsync("imagelabel_light.png");
                    RegulatoryWhite = new BitmapImage();
                    await RegulatoryWhite.SetSourceAsync(await ImageLabel.OpenReadAsync());
                }
            }

            if (HasCerts)
            {
                Certs = await Vendor.GetFolderAsync("certs");
                IReadOnlyList<StorageFile> CertsFiles = await Certs.GetFilesAsync();

                if (CertsFiles.Any(x => string.Equals(x.Name, "npc", StringComparison.InvariantCultureIgnoreCase)))
                {
                    StorageFile NPC = await Certs.GetFileAsync("npc");

                    IRandomAccessStreamWithContentType npcStream = await NPC.OpenReadAsync();
                    using (BinaryReader br = new BinaryReader(npcStream.AsStreamForRead()))
                    {
                        br.BaseStream.Seek(0xA0, SeekOrigin.Begin);
                        string[] imeielements = BitConverter.ToString(br.ReadBytes(0x8)).Split('-');
                        string IMEIBuffer = string.Join("", imeielements.Select(x => string.Join("", x.Reverse())));
                        IMEI = string.Join("", IMEIBuffer.Skip(1)) + "-" + IMEIBuffer[0];
                    }
                }
            }

            if (EnumeratedVendorFiles.Any(x => string.Equals(x.Name, "product.dat", StringComparison.InvariantCultureIgnoreCase)))
            {
                StorageFile ProductDAT = await Vendor.GetFileAsync("product.dat");
                Product = await FileIO.ReadTextAsync(ProductDAT);
            }
        }

        public string IMEI { get; internal set; }
        public string COO { get; internal set; }
        public string CoverColor { get; internal set; }
        public BitmapImage RegulatoryBlack { get; internal set; }
        public BitmapImage RegulatoryWhite { get; internal set; }
        public string Product { get; internal set; }
    }
}
