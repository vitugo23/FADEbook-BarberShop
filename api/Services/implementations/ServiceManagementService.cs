using Fadebook.DB;
using Fadebook.Exceptions;
using Fadebook.Models;
using Fadebook.Repositories;

namespace Fadebook.Services;

public class ServiceManagementService(
    IDbTransactionContext _dbTransactionContext,
    IServiceRepository _serviceRepository
) : IServiceManagementService
{
    public async Task<IEnumerable<ServiceModel>> GetAllAsync()
    {
        return await _serviceRepository.GetAll();
    }

    public async Task<ServiceModel> GetByIdAsync(int id)
    {
        var entity = await _serviceRepository.GetByIdAsync(id);
        if (entity is null)
            throw new NotFoundException($"Service with ID {id} not found.");
        return entity;
    }

    public async Task<ServiceModel> CreateAsync(ServiceModel serviceModel)
    {
        var created = await _serviceRepository.AddAsync(serviceModel);
        await _dbTransactionContext.SaveChangesAsync();
        return created;
    }

    public async Task<ServiceModel> DeleteAsync(int id)
    {
        var deleted = await _serviceRepository.DeleteAsync(id);
        if (deleted is null)
            throw new NotFoundException($"Service with ID {id} not found.");
        await _dbTransactionContext.SaveChangesAsync();
        return deleted;
    }
}
