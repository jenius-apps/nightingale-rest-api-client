using Nightingale.Core.Workspaces.Models;
using System.Threading.Tasks;

namespace Nightingale.Core.CodeGenerators.Interfaces
{
    public interface ICodeGenerator
    {
        Task<string> GenerateCodeAsync(Item request);
    }
}
