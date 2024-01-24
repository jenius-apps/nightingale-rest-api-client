using CommunityToolkit.AppServices;
using System.Threading.Tasks;

namespace Nightingale.DesktopExtension;

[AppService("NightingaleAppService")]
public interface ITestAppService
{
    Task<int> SumAsync(int x, int y);
}