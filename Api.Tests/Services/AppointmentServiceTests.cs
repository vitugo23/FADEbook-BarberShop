using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;
using Fadebook.DB;
using Fadebook.Models;
using Fadebook.Repositories;
using FluentAssertions;
using Fadebook.Api.Tests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Api.Tests.Repositories;

public class AppointmentServiceTests : RepositoryTestBase
{
    // The repositories being tested/used for setup
    // private readonly Mock<NightOwlsDbContext> _mockDbContext;
    private readonly BarberRepository _barberRepository;
    private readonly CustomerRepository _customerRepository;
    private readonly ServiceRepository _serviceRepository;
    private readonly AppointmentRepository _appointmentRepository;
    private readonly BarberServiceRepository _barberServiceRepository;

    public AppointmentServiceTests()
    {
        _barberRepository = new BarberRepository(_context);
        _customerRepository = new CustomerRepository(_context);
        _serviceRepository = new ServiceRepository(_context);
        _appointmentRepository = new AppointmentRepository(_context);
        _barberServiceRepository = new BarberServiceRepository(_context);
    }

    /// Seeds core data required for appointment tests.
    private async Task SeedCoreDataAsync()
    {
        var barber = new BarberModel
        {
            BarberId = 1,
            Username = "sample_barber",
            Name = "John Barber",
            Specialty = "Classic Cuts",
            ContactInfo = "415-256-8844"
        };
        var customer = new CustomerModel
        {
            Username = "sample_customer",
            Name = "Jane Customer",
            ContactInfo = "415-256-8844"
        };
        var service = new ServiceModel
        {
            ServiceId = 1,
            ServiceName = "Haircut",
            ServicePrice = 20
        };
        var barberService = new BarberServiceModel
        {
            BarberId = barber.BarberId,
            ServiceId = service.ServiceId
        };

        await _context.serviceTable.AddAsync(service);
        await _context.customerTable.AddAsync(customer);
        await _context.barberTable.AddAsync(barber);
        await _context.barberServiceTable.AddAsync(barberService);

        await _context.SaveChangesAsync();
    }


    /*
    Task<AppointmentModel> AddAppointment(AppointmentModel appointmentModel);
    Task<AppointmentModel?> UpdateAppointment(AppointmentModel appointment);
    Task<IEnumerable<AppointmentModel>> GetAppointmentsByDate(DateTime dateTime);
    Task<AppointmentModel?> DeleteAppointment(AppointmentModel appointment);
    Task<IEnumerable<AppointmentModel>> LookupAppointmentsByUsername(string username);
    */

    [Fact]
    public async Task AddAppointment_IsAdded()
    {
        // Arrange
        await SeedCoreDataAsync();

        // Act
        CustomerModel customerModel = _context.customerTable.First();
        ServiceModel serviceModel = _context.serviceTable.First();
        BarberModel barberModel = _context.barberTable.First();

        AppointmentModel appointmentModel = new AppointmentModel()
        {
            CustomerId = customerModel.CustomerId,
            Customer = customerModel,
            ServiceId = serviceModel.ServiceId,
            Service = serviceModel,
            BarberId = barberModel.BarberId,
            Barber = barberModel,
            AppointmentDate = DateTime.UtcNow.AddDays(1),
            Status = "Pending"
        };

        AppointmentModel newAppointment = await _appointmentRepository.AddAsync(appointmentModel);
        await _context.SaveChangesAsync();

        // Assert
        // Assert.True(_context.appointmentTable.Count() > 0);
        _context.appointmentTable.Should().NotBeEmpty();
    }


    [Fact]
    public async Task UpdateAppointment_NewStatusIsUpdated()
    {
        // Arrange
        await SeedCoreDataAsync();

        CustomerModel customerModel = _context.customerTable.First();
        ServiceModel serviceModel = _context.serviceTable.First();
        BarberModel barberModel = _context.barberTable.First();
        //AppointmentModel appointmentModel = _context.appointmentTable.First();
        //appointmentModel.Status = "Completed";

        AppointmentModel appointmentModel = new AppointmentModel()
        {
            CustomerId = customerModel.CustomerId,
            Customer = customerModel,
            ServiceId = serviceModel.ServiceId,
            Service = serviceModel,
            BarberId = barberModel.BarberId,
            Barber = barberModel,
            AppointmentDate = DateTime.UtcNow.AddDays(1),
            Status = "Pending"
        };
        await _appointmentRepository.AddAsync(appointmentModel);
        await _context.SaveChangesAsync();

        appointmentModel.Status = "Completed";
        await _appointmentRepository.UpdateAsync(appointmentModel.AppointmentId, appointmentModel);
        await _context.SaveChangesAsync();

        _context.appointmentTable.Should().HaveCount(1);
        _context.appointmentTable.First().Status.Should().Be("Completed");
    }
}