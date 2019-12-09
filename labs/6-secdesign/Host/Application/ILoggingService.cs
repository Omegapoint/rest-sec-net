using System.Threading.Tasks;

namespace SecureByDesign.Host.Application
{
    public interface ILoggingService
    {
        Task Log(string user, string message);
    }
}