using System.Diagnostics;
using System.Linq;

namespace FullTrust
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Contains("/Exemption"))
            {
                LocalhostExemption();
            }
            else if (args.Contains("/Server"))
            {
                var fullTrustPath = System.Reflection.Assembly.GetEntryAssembly().Location;
                var localServerPath = fullTrustPath.Replace(@"FullTrust\FullTrust.exe", @"LocalServer\LocalServer.exe");
                var processInfo = new ProcessStartInfo
                {
                    FileName = localServerPath,
                    WindowStyle = ProcessWindowStyle.Normal
                };

                using (var exeProcess = Process.Start(processInfo))
                {
                }
            }

        }

        static void LocalhostExemption()
        {
            ProcessStartInfo startInfo = GetLoopbackExemptProcessInfo(
                true,
                "S-1-15-2-2472482401-1297737560-3464812208-2778208509-1273584065-1826830168-474783446");

            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch
            {
                // Log error.
            }
        }

        static ProcessStartInfo GetLoopbackExemptProcessInfo(bool addExemption, string packageId)
        {
            return new ProcessStartInfo
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                CreateNoWindow = true,
                UseShellExecute = false,
                FileName = "CheckNetIsolation.exe",
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = $"loopbackexempt {(addExemption ? "-a" : "-d")} -p={packageId}"
            };
        }
    }
}
