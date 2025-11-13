using Fadebook.Models;

namespace Fadebook.Services;


public interface IAppointmentManagementService
{
    Task<AppointmentModel> AddAppointmentAsync(AppointmentModel appointmentModel);
    Task<AppointmentModel> UpdateAppointmentAsync(int appointmentId, AppointmentModel appointment);
    Task<IEnumerable<AppointmentModel>> GetAppointmentsByDateAsync(DateTime dateTime);
    Task<IEnumerable<AppointmentModel>> GetAppointmentsByCustomerIdAsync(int customerId);
    Task<AppointmentModel> GetAppointmentByIdAsync(int appointmentId);
    Task<IEnumerable<AppointmentModel>> GetAppointmentsByBarberIdAsync(int barberId);
    Task<AppointmentModel> DeleteAppointmentAsync(int appointmentId);
    Task<IEnumerable<AppointmentModel>> LookupAppointmentsByUsernameAsync(string username);

}
