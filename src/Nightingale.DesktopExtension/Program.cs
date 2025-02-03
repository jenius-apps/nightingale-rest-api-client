using CommunityToolkit.AppServices;
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
