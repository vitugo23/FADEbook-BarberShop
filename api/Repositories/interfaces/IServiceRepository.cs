
using Fadebook.Models;

namespace Fadebook.Repositories;

public interface IServiceRepository
{
    Task<ServiceModel?> GetByIdAsync(int serviceId);
    Task<IEnumerable<ServiceModel>> GetAll();
    Task<ServiceModel> AddAsync(ServiceModel serviceModel);
    Task<ServiceModel?> DeleteAsync(int serviceId);
}

/*


*/