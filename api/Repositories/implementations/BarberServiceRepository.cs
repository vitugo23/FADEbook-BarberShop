

using Fadebook.DB;
using Fadebook.Models;
using Microsoft.EntityFrameworkCore;

namespace Fadebook.Repositories;

public class BarberServiceRepository(
    FadebookDbContext _fadebookDbContext
    ) : IBarberServiceRepository
{
    public async Task<BarberServiceModel?> GetByIdAsync(int barberServiceId)
    {
        // Note: BarberServiceModel uses composite key (BarberId, ServiceId), not Id
        // This method searches by the Id field which is not the primary key
        return await _fadebookDbContext.barberServiceTable
            .Where(bsm => bsm.Id == barberServiceId)
            .FirstOrDefaultAsync();
    }
    public async Task<IEnumerable<BarberServiceModel>> GetByBarberIdAsync(int barberId)
    {
        return await _fadebookDbContext.barberServiceTable
            .Where(bsm => bsm.BarberId == barberId)
            .Include(bsm => bsm.Service)
            .Include(bsm => bsm.Barber)
            .ToListAsync();
    }
    public async Task<IEnumerable<BarberServiceModel>> GetByServiceIdAsync(int serviceId)
    {
        return await _fadebookDbContext.barberServiceTable
            .Where(bsm => bsm.ServiceId == serviceId)
            .Include(bsm => bsm.Service)
            .Include(bsm => bsm.Barber)
            .ToListAsync();
    }
    public async Task<BarberServiceModel?> GetByBarberIdServiceIdAsync(int barberId, int serviceId)
    {
        return await _fadebookDbContext.barberServiceTable
            .Where(bsm => bsm.BarberId == barberId && bsm.ServiceId == serviceId)
            .Include(bsm => bsm.Service)
            .Include(bsm => bsm.Barber)
            .FirstOrDefaultAsync();
    }
    public async Task<BarberServiceModel> AddAsync(BarberServiceModel barberServiceModel)
    {
        var foundBarberService = await this.GetByBarberIdServiceIdAsync(barberServiceModel.BarberId, barberServiceModel.ServiceId);
        if (foundBarberService != null)
            return foundBarberService; // idempotent
        await _fadebookDbContext.barberServiceTable.AddAsync(barberServiceModel);
        return barberServiceModel;
    }
    public async Task<BarberServiceModel> RemoveByIdAsync(int barberServiceId)
    {
        var foundBarberService = await this.GetByIdAsync(barberServiceId);
        if (foundBarberService is null)
            return null!;
        _fadebookDbContext.barberServiceTable.Remove(foundBarberService);
        return foundBarberService;
    }
    public async Task<BarberServiceModel> RemoveByBarberIdServiceId(int barberId, int serviceId)
    {
        var foundBarberService = await this.GetByBarberIdServiceIdAsync(barberId, serviceId);
        if (foundBarberService is null)
            return null!;
        _fadebookDbContext.barberServiceTable.Remove(foundBarberService);
        return foundBarberService;
    }
}
