namespace Chit.Utilities;

public interface IKeyVaultService
{
    Task<string> Get(string key);
    Task Set(string key, string value);
}
