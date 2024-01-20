using Newtonsoft.Json;
using Nightingale.Core.Mock.Models;
using Nightingale.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace LocalServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write(@"
 _   _         _     _                       _         
( ) ( )_      ( )   ( )_ _                  (_ )       
|  \| |_)  __ | |__ |  _)_) ___    __    _ _ | |   __  
|     | |/ _  \  _  \ | | |  _  \/ _  \/ _  )| | / __ \
| | \ | | (_) | | | | |_| | ( ) | (_) | (_| || |(  ___/
(_) (_)_)\__  |_) (_)\__)_)_) (_)\__  |\__ _)___)\____)
        ( )_) |                 ( )_) |                
         \___/                   \___/                 
                   _       ___                               
/ \_/ \           ( )     (  _ \                             
|     |  _     ___| |/ )  | (_(_)  __  _ __ _   _   __  _ __ 
| (_) |/ _ \ / ___)   (    \__ \ / __ \  __) ) ( )/ __ \  __)
| | | | (_) ) (___| |\ \  ( )_) |  ___/ |  | \_/ |  ___/ |   
(_) (_)\___/ \____)_) (_)  \____)\____)_)   \___/ \____)_)                                                                
");
            Console.WriteLine("Press 'q' and enter to quit");
            Console.WriteLine();
            Initialize(args);

            while (Console.ReadLine() != "q") { }

            WebService.StopWebServer();
        }

        static async void Initialize(string[] args)
        {
            string configPath = null;
            if (args.Any(x => x.EndsWith("serverConfig.json")))
            {
                configPath = args.FirstOrDefault(x => x.EndsWith("serverConfig.json"));
            }

            // Navigate to package's local folder just in case we do something bad,
            // so we minimize the damage to the system.
            try
            {
                Directory.SetCurrentDirectory(ApplicationData.Current.LocalFolder.Path);
            }
            catch
            {
                Console.WriteLine("Tried to navigate to package root, but you're running outside a package.");
            }

            // get the config data from local folder
            StorageFile configFile;
            if (!string.IsNullOrWhiteSpace(configPath))
            {
                configFile = await StorageFile.GetFileFromPathAsync(configPath);
            }
            else
            {
                configFile = await ApplicationData.Current.LocalFolder.GetFileAsync("serverConfig.json");
            }

            string configString = await FileIO.ReadTextAsync(configFile);
            ServerConfiguration config = JsonConvert.DeserializeObject<ServerConfiguration>(configString);

            StorageFile ncfFile = await StorageFile.GetFileFromPathAsync(config.NcfPath);
            if (ncfFile == null)
            {
                Console.WriteLine("Could not find NCF. Cancelling initialization. Path: " + config.NcfPath);
                return;
            }

            string ncfString = await FileIO.ReadTextAsync(ncfFile);
            DocumentFile ncf = JsonConvert.DeserializeObject<DocumentFile>(ncfString);
            Console.Title = config.NcfPath;
            WebService.StartWebServer(config, ncf);
        }
    }
}
