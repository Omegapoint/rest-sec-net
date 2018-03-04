using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace IdentityService
{
    public class ClientStore : IClientStore
    {
        private readonly Dictionary<string, Client> clients = new Dictionary<string, Client>();

        public ClientStore()
        {
            var client = new Client
            {
                ClientId = "myclient",
                ClientName = "Client that you can use for testing",
                ClientSecrets = { new Secret("secret".Sha256()) },
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes = { "read:product" }
            };

            clients.Add(client.ClientId, client);
        }

        public Task<Client> FindClientByIdAsync(string clientId)
        {
            return clients.ContainsKey(clientId)
                ? Task.FromResult(clients[clientId])
                : Task.FromResult(default(Client));
        }
    }
}