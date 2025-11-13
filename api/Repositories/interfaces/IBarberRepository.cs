using Fadebook.DB;
using Fadebook.Models;

namespace Fadebook.Repositories;

public interface IBarberRepository
{
    Task<BarberModel?> GetByIdAsync(int id);
    Task<IEnumerable<BarberModel>> GetAllAsync();
    Task<BarberModel> AddAsync(BarberModel barber);
    Task<BarberModel> UpdateAsync(int barberId, BarberModel barberModel);
    Task<BarberModel> RemoveByIdAsync(int barberId);
}