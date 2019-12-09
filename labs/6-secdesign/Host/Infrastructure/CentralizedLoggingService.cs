using System.Threading.Tasks;
using SecureByDesign.Host.Application;

namespace SecureByDesign.Host.Infrastructure
{
    public class CentralizedLoggingService : ILoggingService
    {
        public Task Log(string username, string message)
        {
            //TODO: call externa logging service, remober to output encode and truncate too long messages... 
            return Task.Delay(100);
        }
    }
}