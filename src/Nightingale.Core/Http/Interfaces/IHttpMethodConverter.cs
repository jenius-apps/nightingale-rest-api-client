namespace Nightingale.Core.Http.Interfaces
{
    public interface IHttpMethodConverter
    {
        string IntToFullMethodName(int methodIndex);

        string IntToShortMethodName(int methodIndex);

        int StringToInt(string method);
    }
}
