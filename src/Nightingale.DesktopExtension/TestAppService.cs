using System.Threading.Tasks;

namespace Nightingale.DesktopExtension;

public sealed partial class TestAppService : ITestAppService
{
    public async Task<int> SumAsync(int x, int y)
    {
        return x + y;
    }
}
