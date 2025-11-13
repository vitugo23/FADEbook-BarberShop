
using Fadebook.Models;

namespace Fadebook.Services;

public interface ICustomerAppointmentService
{
    Task<IEnumerable<ServiceModel>> ListAvailableServicesAsync();

    // Task<IEnumerable<CustomerModel>> AddCustomerAsync(CustomerModel customerModel);
    Task<IEnumerable<BarberModel>> ListAvailableBarbersByServiceAsync(int serviceId);
    Task<IEnumerable<AppointmentModel>> GetAppointmentsByCustomerIdAsync(int customerId);
    Task<AppointmentModel> MakeAppointmentAsync(AppointmentModel appointmentModel);
}
