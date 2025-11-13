
// TODO: G3

using Fadebook.DB;
using Fadebook.Models;
using Microsoft.EntityFrameworkCore;
using Fadebook.Exceptions;

namespace Fadebook.Repositories;

public class ServiceRepository(
    FadebookDbContext _fadebookDbContext
    ) : IServiceRepository
{
    public async Task<ServiceModel?> GetByIdAsync(int serviceId)
    {
        return await _fadebookDbContext.serviceTable
            .FindAsync(serviceId);
    }
    public async Task<IEnumerable<ServiceModel>> GetAll()
    {
        return await _fadebookDbContext.serviceTable.ToListAsync();
    }

    public async Task<ServiceModel> AddAsync(ServiceModel serviceModel)
    {
        await _fadebookDbContext.serviceTable.AddAsync(serviceModel);
        return serviceModel;
    }

    public async Task<ServiceModel?> DeleteAsync(int serviceId)
    {
        var service = await GetByIdAsync(serviceId);
        if (service is null)
            return null;
        
        _fadebookDbContext.serviceTable.Remove(service);
        return service;
    }
}
