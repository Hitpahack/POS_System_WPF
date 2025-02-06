using System.Threading.Tasks;

namespace FalcaPOS.Contracts
{
    public interface ISignalRService
    {
        Task ConnectToHubAsync(string connectionHubURL);

        Task DisconnectFromHubAsync();

        bool IsConnected();

        Task SendMessageToHubAsync(string methodName, object arg1);

        Task SendMessageToHubAsync(string methodName, object arg1, object arg2);

        //add additional menthods with overloads if required.and implement the same
    }
}
