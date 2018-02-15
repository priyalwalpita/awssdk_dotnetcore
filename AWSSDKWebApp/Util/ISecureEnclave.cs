using System.Threading.Tasks;

namespace AWSSDKWebApp.Util
{
    public interface ISecureEnclave
    {
        Task<string> ReadValue(string key);

        Task<bool> WriteValue(string key, string value);
    }
}