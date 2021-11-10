using System.Threading.Tasks;

namespace Interfaces
{
    public interface IRestApi
    {
        Task<T> GetAsync<T>(string route);
    }
}
