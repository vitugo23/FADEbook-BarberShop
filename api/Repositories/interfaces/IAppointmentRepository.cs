using Fadebook.DB;
using Fadebook.Models;

namespace Fadebook.Repositories;

public interface IAppointmentRepository
{
    Task<AppointmentModel?> GetByIdAsync(int appointmentId);
    Task<IEnumerable<AppointmentModel>> GetAllAsync();
    Task<IEnumerable<AppointmentModel>> GetByDateAsync(DateTime targetDate);
    Task<IEnumerable<AppointmentModel>> GetByCustomerIdAsync(int customerId);
    Task<IEnumerable<AppointmentModel>> GetByBarberIdAsync(int barberId);
    Task<IEnumerable<AppointmentModel>> GetByServiceIdAsync(int serviceId);
    Task<AppointmentModel> AddAsync(AppointmentModel appointmentModel);
    Task<AppointmentModel> UpdateAsync(int appointmentId, AppointmentModel appointmentModel);
    Task<AppointmentModel> RemoveByIdAsync(int appointmentId);
}
