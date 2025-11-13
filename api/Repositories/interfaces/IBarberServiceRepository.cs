
using Fadebook.DB;
using Fadebook.Models;

namespace Fadebook.Repositories;

public interface IBarberServiceRepository
{
    Task<BarberServiceModel?> GetByIdAsync(int barberServiceId);
    Task<IEnumerable<BarberServiceModel>> GetByBarberIdAsync(int barberId);
    Task<IEnumerable<BarberServiceModel>> GetByServiceIdAsync(int serviceId);
    Task<BarberServiceModel?> GetByBarberIdServiceIdAsync(int barberId, int serviceId);
    Task<BarberServiceModel> AddAsync(BarberServiceModel barberServiceModel);
    Task<BarberServiceModel> RemoveByIdAsync(int barberServiceId);
    Task<BarberServiceModel> RemoveByBarberIdServiceId(int barberId, int serviceId);
}
