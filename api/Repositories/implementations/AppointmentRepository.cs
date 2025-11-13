
using Fadebook.Models;
using Fadebook.DB;
using Microsoft.EntityFrameworkCore;

namespace Fadebook.Repositories;

public class AppointmentRepository(
    FadebookDbContext _fadebookDbContext
    ) : IAppointmentRepository
{
    public async Task<AppointmentModel?> GetByIdAsync(int appointmentId)
    {
        return await _fadebookDbContext.appointmentTable.FindAsync(appointmentId);
    }
    public async Task<IEnumerable<AppointmentModel>> GetAllAsync()
    {
        return await _fadebookDbContext.appointmentTable.ToListAsync();
    }
    public async Task<IEnumerable<AppointmentModel>> GetByDateAsync(DateTime targetDate)
    {
        return await _fadebookDbContext.appointmentTable
            .Where(a => a.AppointmentDate.Date == targetDate.Date)
            .ToListAsync();
    }
    public async Task<IEnumerable<AppointmentModel>> GetByCustomerIdAsync(int customerId)
    {
        return await _fadebookDbContext.appointmentTable
            .Where(a => a.CustomerId == customerId)
            .ToListAsync();
    }
    public async Task<IEnumerable<AppointmentModel>> GetByBarberIdAsync(int barberId)
    {
        return await _fadebookDbContext.appointmentTable
            .Where(a => a.BarberId == barberId)
            .ToListAsync();
    }
    public async Task<IEnumerable<AppointmentModel>> GetByServiceIdAsync(int serviceId)
    {
        return await _fadebookDbContext.appointmentTable
            .Where(a => a.ServiceId == serviceId)
            .ToListAsync();
    }
    public async Task<AppointmentModel> AddAsync(AppointmentModel appointmentModel)
    {
      
        var foundAppointment = await this.GetByIdAsync(appointmentModel.AppointmentId);
        if (foundAppointment != null) return foundAppointment;

        // Validate foreign keys exist
        var customerExists = await _fadebookDbContext.customerTable
            .AnyAsync(c => c.CustomerId == appointmentModel.CustomerId);
        if (!customerExists) return null;

        var barberExists = await _fadebookDbContext.barberTable
            .AnyAsync(b => b.BarberId == appointmentModel.BarberId);
        if (!barberExists) return null;

        var serviceExists = await _fadebookDbContext.serviceTable
            .AnyAsync(s => s.ServiceId == appointmentModel.ServiceId);
        if (!serviceExists) return null;

        await _fadebookDbContext.appointmentTable.AddAsync(appointmentModel);
        return appointmentModel;
    }

    public async Task<AppointmentModel> UpdateAsync(int appointmentId, AppointmentModel appointmentModel)
    {
    
        var foundAppointmentModel = await this.GetByIdAsync(appointmentId);
        if (foundAppointmentModel is null) return null;

        // Validate foreign keys exist before updating
        var customerExists = await _fadebookDbContext.customerTable
            .AnyAsync(c => c.CustomerId == appointmentModel.CustomerId);
        if (!customerExists) return null;

        var barberExists = await _fadebookDbContext.barberTable
            .AnyAsync(b => b.BarberId == appointmentModel.BarberId);
        if (!barberExists) return null;

        var serviceExists = await _fadebookDbContext.serviceTable
            .AnyAsync(s => s.ServiceId == appointmentModel.ServiceId);
        if (!serviceExists) return null;

        foundAppointmentModel.BarberId = appointmentModel.BarberId;
        foundAppointmentModel.CustomerId = appointmentModel.CustomerId;
        foundAppointmentModel.ServiceId = appointmentModel.ServiceId;
        foundAppointmentModel.AppointmentDate = appointmentModel.AppointmentDate;
        foundAppointmentModel.Status = appointmentModel.Status;
        _fadebookDbContext.appointmentTable.Update(foundAppointmentModel);
        return foundAppointmentModel;
    }
    public async Task<AppointmentModel> RemoveByIdAsync(int appointmentId)
    {
        var foundAppointmentModel = await this.GetByIdAsync(appointmentId);
        if (foundAppointmentModel is null) return null;
        _fadebookDbContext.appointmentTable.Remove(foundAppointmentModel);
        return foundAppointmentModel;
    }
}

