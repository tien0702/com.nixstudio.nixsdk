using System.Threading.Tasks;

namespace NIX.Module.StorageService
{
    public interface IDataStorage
    {
        string StorageKey { get; }
        Task<bool> SaveAsync<T>(string savedKey, T value);
        Task<T> LoadAsync<T>(string savedKey);
        Task<bool> DeleteAsync(string savedKey);
    }
}