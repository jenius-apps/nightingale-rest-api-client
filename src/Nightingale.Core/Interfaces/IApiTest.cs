namespace Nightingale.Core.Interfaces
{
    public interface IApiTest : IStorageItem
    {
        string Name { get; set; }

        string CodeContent { get; set; }
    }
}
