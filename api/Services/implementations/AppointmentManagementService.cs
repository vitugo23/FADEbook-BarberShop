
using Fadebook.DB;
using Fadebook.Models;
using Fadebook.Repositories;
using Fadebook.Exceptions;


namespace Fadebook.Services;

public class AppointmentManagementService(
    IDbTransactionContext _dbTransactionContext,
    IAppointmentRepository _appointmentRepository,
    ICustomerRepository _customerRepository
    ) : IAppointmentManagementService
{

    public async Task<AppointmentModel> AddAppointmentAsync(AppointmentModel appointmentModel)
    {
        try
        {
            var newAppointment = await _appointmentRepository.AddAsync(appointmentModel);
            if (newAppointment is null)
                throw new BadRequestException("Unable to create appointment. Verify that Customer, Barber, and Service IDs exist.");
            await _dbTransactionContext.SaveChangesAsync();
            return newAppointment;
        }
        catch
        {
            // Nothing for us to correct here
            // So we pass the exception on.
            // Also, apparently this is how we rethrow an expception per CS2200
            // Manually rethrowing an exception results in a stack trace rewrite causing lost info.
            throw;
        }
    }
    public async Task<AppointmentModel> UpdateAppointmentAsync(int appointmentId, AppointmentModel appointmentModel)
    {
        try
        {
            var updatedAppointment = await _appointmentRepository.UpdateAsync(appointmentId, appointmentModel);
            if (updatedAppointment is null)
                throw new NotFoundException($"Appointment with ID {appointmentId} not found or invalid foreign keys.");
            await _dbTransactionContext.SaveChangesAsync();
            return updatedAppointment;
        }
        catch
        {
            throw;
        }
    }
    public async Task<IEnumerable<AppointmentModel>> GetAppointmentsByDateAsync(DateTime dateTime)
    {
        return await _appointmentRepository.GetByDateAsync(dateTime);
    }
    public async Task<IEnumerable<AppointmentModel>> GetAppointmentsByCustomerIdAsync(int customerId)
    {
        return await _appointmentRepository.GetByCustomerIdAsync(customerId);
    }
    public async Task<AppointmentModel> GetAppointmentByIdAsync(int appointmentId)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
        if (appointment is null)
            throw new NotFoundException($"Appointment with id \"{appointmentId} does not exist");
        return appointment; 
    }
    public async Task<IEnumerable<AppointmentModel>> GetAppointmentsByBarberIdAsync(int barberId)
    {
        return await _appointmentRepository.GetByBarberIdAsync(barberId);
    }
    public async Task<AppointmentModel> DeleteAppointmentAsync(int appointmentId)
    {
        try
        {
            var removedAppointment = await _appointmentRepository.RemoveByIdAsync(appointmentId);
            if (removedAppointment is null)
                throw new NotFoundException($"Appointment with ID {appointmentId} not found.");
            await _dbTransactionContext.SaveChangesAsync();
            return removedAppointment;
        }
        catch
        {
            throw;
        }
    }
    // Throws NoUsernameException
    public async Task<IEnumerable<AppointmentModel>> LookupAppointmentsByUsernameAsync(string username)
    {
        var foundCustomer = await _customerRepository.GetByUsernameAsync(username);
        if (foundCustomer == null)
            throw new NotFoundException($"Customer with the username \"{username}\" was not found");
        return await _appointmentRepository.GetByCustomerIdAsync(foundCustomer.CustomerId);
    }
}
