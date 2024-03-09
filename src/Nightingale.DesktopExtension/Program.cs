using CommunityToolkit.AppServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nightingale.DesktopExtension
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await AppServiceComponent.RunAsync<TestAppService>();
        }
    }
}
