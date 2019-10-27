using System.Threading.Tasks;

namespace DaoApi.Controllers
{
    public interface ISendMessageToQueue
    {
        Task Dispatch(string phoneNumber, string message);
    }
}