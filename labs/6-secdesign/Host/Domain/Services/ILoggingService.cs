using System.Threading.Tasks;

namespace SecureByDesign.Host.Domain.Services
{
    public interface ILoggingService
    {
        Task Log(string user, string message);
    }
}