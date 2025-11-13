using Fadebook.Models;

namespace Fadebook.Services;

public interface IServiceManagementService
{
    Task<IEnumerable<ServiceModel>> GetAllAsync();
    Task<ServiceModel> GetByIdAsync(int id);
    Task<ServiceModel> CreateAsync(ServiceModel serviceModel);
    Task<ServiceModel> DeleteAsync(int id);
}
