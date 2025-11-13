
using Fadebook.Models;
using Fadebook.DB;
using Microsoft.EntityFrameworkCore;

namespace Fadebook.Repositories;

public class BarberRepository(
    FadebookDbContext _fadebookDbContext
    ) : IBarberRepository
{
    public async Task<BarberModel?> GetByIdAsync(int id)
    {
        return await _fadebookDbContext.barberTable.FindAsync(id);
    }

    public async Task<IEnumerable<BarberModel>> GetAllAsync()
    {
        return await _fadebookDbContext.barberTable.ToListAsync();
    }

    public async Task<BarberModel> AddAsync(BarberModel barber)
    {
        var result = await _fadebookDbContext.barberTable.AddAsync(barber);
        return result.Entity;
    }

    public async Task<BarberModel> UpdateAsync(int barberId, BarberModel barberModel)
    {
        var foundBarberModel = await GetByIdAsync(barberId);
        if (foundBarberModel is null)
            return null!;
        if (barberModel.Username != null && foundBarberModel.Username != barberModel.Username)
        {
            var usernameBarberModel = await _fadebookDbContext.barberTable.Where(bm => bm.Username == barberModel.Username).FirstOrDefaultAsync();
            if (usernameBarberModel != null)
                return null!; // signal conflict/not-updated
        }
        foundBarberModel.Username = barberModel.Username;
        foundBarberModel.Name = barberModel.Name;
        foundBarberModel.Specialty = barberModel.Specialty;
        foundBarberModel.ContactInfo = barberModel.ContactInfo;
        _fadebookDbContext.barberTable.Update(foundBarberModel);
        return foundBarberModel;
    }

    public async Task<BarberModel> RemoveByIdAsync(int barberId)
    {
        var foundBarberModel = await GetByIdAsync(barberId);
        if (foundBarberModel is null)
            return null!;

        _fadebookDbContext.barberTable.Remove(foundBarberModel);
        return foundBarberModel;
    }
}
