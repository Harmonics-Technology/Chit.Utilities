using Microsoft.Extensions.Configuration;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace Chit.Utilities;

public class KeyVaultService : IKeyVaultService
{
    private readonly IConfiguration _configuration;
    private readonly SecretClient _client;

    public KeyVaultService(IConfiguration configuration)
    {
        _configuration = configuration;
        _client = new SecretClient(new Uri(_configuration.GetSection("KeyVault:Vault").Value), new ClientSecretCredential(_configuration.GetSection("KeyVault:TenantId").Value, _configuration.GetSection("KeyVault:ClientId").Value, _configuration.GetSection("KeyVault:ClientSecret").Value));
    }

    public async Task<string> Get(string key)
    {
        var secret = await _client.GetSecretAsync(key);
        return secret.Value.Value;
    }

    public async Task Set(string key, string value)
    {
        await _client.SetSecretAsync(key, value);
    }
}
