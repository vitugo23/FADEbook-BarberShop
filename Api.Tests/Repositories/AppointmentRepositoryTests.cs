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

namespace Api.Tests.Repositories;

public class AppointmentRepositoryTests: RepositoryTestBase
{
    private readonly AppointmentRepository _repo;

    public AppointmentRepositoryTests()
    {
        _repo = new AppointmentRepository(_context);
    }

    [Fact]
    public async Task GetByIdAsync_WhenAppointmentExists_ShouldReturnAppointment()
    {
        // Arrange
        var customer = new CustomerModel { Username = "customer_one", Name = "C One", ContactInfo = "c1" };
        var service = new ServiceModel { ServiceName = "svc", ServicePrice = 10 };
        var barber = new BarberModel { Username = "barber_one", Name = "B One", Specialty = "Fade", ContactInfo = "b1" };
        _context.customerTable.Add(customer);
        _context.serviceTable.Add(service);
        _context.barberTable.Add(barber);
        await _context.SaveChangesAsync();

        var appt = new AppointmentModel
        {
            CustomerId = customer.CustomerId,
            ServiceId = service.ServiceId,
            BarberId = barber.BarberId,
            AppointmentDate = new DateTime(2025, 01, 02),
            Status = "Scheduled"
        };
        _context.appointmentTable.Add(appt);
        await _context.SaveChangesAsync();
        var testId = appt.AppointmentId;

        // Act
        var found = await _repo.GetByIdAsync(testId);

        // Assert
        found.Should().NotBeNull();
        found!.AppointmentId.Should().Be(testId);
    }

    [Fact]
    public async Task GetByIdAsync_WhenAppointmentDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var customer = new CustomerModel { Username = "customer_one", Name = "C One", ContactInfo = "c1" };
        var service = new ServiceModel { ServiceName = "svc", ServicePrice = 10 };
        var barber = new BarberModel { Username = "barber_one", Name = "B One", Specialty = "Fade", ContactInfo = "b1" };
        _context.customerTable.Add(customer);
        _context.serviceTable.Add(service);
        _context.barberTable.Add(barber);
        await _context.SaveChangesAsync();

        var appt = new AppointmentModel
        {
            CustomerId = customer.CustomerId,
            ServiceId = service.ServiceId,
            BarberId = barber.BarberId,
            AppointmentDate = new DateTime(2025, 01, 02),
            Status = "Scheduled"
        };
        _context.appointmentTable.Add(appt);
        await _context.SaveChangesAsync();

        // Act
        var found = await _repo.GetByIdAsync(2);

        // Assert
        found.Should().BeNull();
    }

    [Fact]
    public async Task GetAll_WhenAppointmentsExist_ShouldReturnAppointments()
    {
        // Arrange
        var customer1 = new CustomerModel { Username = "cust1", Name = "n1", ContactInfo = "c1" };
        var customer2 = new CustomerModel { Username = "cust2", Name = "n2", ContactInfo = "c2" };
        var service1 = new ServiceModel { ServiceName = "s1", ServicePrice = 10 };
        var service2 = new ServiceModel { ServiceName = "s2", ServicePrice = 20 };
        var barber1 = new BarberModel { Username = "b1", Name = "B1", Specialty = "sp1", ContactInfo = "b1" };
        var barber2 = new BarberModel { Username = "b2", Name = "B2", Specialty = "sp2", ContactInfo = "b2" };
        _context.customerTable.AddRange(customer1, customer2);
        _context.serviceTable.AddRange(service1, service2);
        _context.barberTable.AddRange(barber1, barber2);
        await _context.SaveChangesAsync();

        var appt1 = new AppointmentModel
        {
            CustomerId = customer1.CustomerId,
            ServiceId = service1.ServiceId,
            BarberId = barber1.BarberId,
            AppointmentDate = new DateTime(2025, 01, 02),
            Status = "Scheduled"
        };
        var appt2 = new AppointmentModel
        {
            CustomerId = customer2.CustomerId,
            ServiceId = service2.ServiceId,
            BarberId = barber2.BarberId,
            AppointmentDate = new DateTime(2025, 01, 03),
            Status = "Scheduled"
        };
        _context.appointmentTable.Add(appt1);
        _context.appointmentTable.Add(appt2);
        await _context.SaveChangesAsync();

        // Act
        var found = await _repo.GetAllAsync();

        // Assert
        found.Should().NotBeNull();
        found.Should().HaveCount(2);
        found.Should().Contain(a => a.CustomerId == customer1.CustomerId);
        found.Should().Contain(a => a.CustomerId == customer2.CustomerId);
    }

    [Fact]
    public async Task GetApptsByDate_WhenAppointmentsForDateExist_ShouldReturnThoseAppointments()
    {
        // Arrange
        var date = new DateTime(2025, 01, 02);
        var c1 = new CustomerModel { Username = "cust1", Name = "n1", ContactInfo = "c1" };
        var c2 = new CustomerModel { Username = "cust2", Name = "n2", ContactInfo = "c2" };
        var c3 = new CustomerModel { Username = "cust3", Name = "n3", ContactInfo = "c3" };
        var s1 = new ServiceModel { ServiceName = "s1", ServicePrice = 10 };
        var s2 = new ServiceModel { ServiceName = "s2", ServicePrice = 20 };
        var s3 = new ServiceModel { ServiceName = "s3", ServicePrice = 30 };
        var b1 = new BarberModel { Username = "b1", Name = "B1", Specialty = "sp1", ContactInfo = "b1" };
        var b2 = new BarberModel { Username = "b2", Name = "B2", Specialty = "sp2", ContactInfo = "b2" };
        var b3 = new BarberModel { Username = "b3", Name = "B3", Specialty = "sp3", ContactInfo = "b3" };
        _context.customerTable.AddRange(c1, c2, c3);
        _context.serviceTable.AddRange(s1, s2, s3);
        _context.barberTable.AddRange(b1, b2, b3);
        await _context.SaveChangesAsync();

        var appt1 = new AppointmentModel
        {
            CustomerId = c1.CustomerId,
            ServiceId = s1.ServiceId,
            BarberId = b1.BarberId,
            AppointmentDate = date,
            Status = "Scheduled"
        };
        var appt2 = new AppointmentModel
        {
            CustomerId = c2.CustomerId,
            ServiceId = s2.ServiceId,
            BarberId = b2.BarberId,
            AppointmentDate = date,
            Status = "Scheduled"
        };
        var apptOtherDay = new AppointmentModel
        {
            CustomerId = c3.CustomerId,
            ServiceId = s3.ServiceId,
            BarberId = b3.BarberId,
            AppointmentDate = date.AddDays(1),
            Status = "Scheduled"
        };
        _context.appointmentTable.AddRange(appt1, appt2, apptOtherDay);
        await _context.SaveChangesAsync();

        // Act
        var found = await _repo.GetByDateAsync(date);

        // Assert
        found.Should().HaveCount(2);
        found.Should().OnlyContain(a => a.AppointmentDate.Date == date.Date);
    }

    [Fact]
    public async Task GetByCustomerId_WhenAppointmentsForCustomerExist_ShouldReturnThoseAppointments()
    {
        // Arrange
        var c1 = new CustomerModel { Username = "cust1", Name = "n1", ContactInfo = "c1" };
        var c2 = new CustomerModel { Username = "cust2", Name = "n2", ContactInfo = "c2" };
        var s1 = new ServiceModel { ServiceName = "s1", ServicePrice = 10 };
        var s2 = new ServiceModel { ServiceName = "s2", ServicePrice = 20 };
        var s3 = new ServiceModel { ServiceName = "s3", ServicePrice = 30 };
        var b1 = new BarberModel { Username = "b1", Name = "B1", Specialty = "sp1", ContactInfo = "b1" };
        var b2 = new BarberModel { Username = "b2", Name = "B2", Specialty = "sp2", ContactInfo = "b2" };
        var b3 = new BarberModel { Username = "b3", Name = "B3", Specialty = "sp3", ContactInfo = "b3" };
        _context.customerTable.AddRange(c1, c2);
        _context.serviceTable.AddRange(s1, s2, s3);
        _context.barberTable.AddRange(b1, b2, b3);
        await _context.SaveChangesAsync();

        var appt1 = new AppointmentModel
        {
            CustomerId = c1.CustomerId,
            ServiceId = s1.ServiceId,
            BarberId = b1.BarberId,
            AppointmentDate = DateTime.UtcNow.Date,
            Status = "Scheduled"
        };
        var appt2 = new AppointmentModel
        {
            CustomerId = c1.CustomerId,
            ServiceId = s2.ServiceId,
            BarberId = b2.BarberId,
            AppointmentDate = DateTime.UtcNow.Date,
            Status = "Scheduled"
        };
        var apptOther = new AppointmentModel
        {
            CustomerId = c2.CustomerId,
            ServiceId = s3.ServiceId,
            BarberId = b3.BarberId,
            AppointmentDate = DateTime.UtcNow.Date,
            Status = "Scheduled"
        };
        _context.appointmentTable.AddRange(appt1, appt2, apptOther);
        await _context.SaveChangesAsync();

        // Act
        var found = await _repo.GetByCustomerIdAsync(c1.CustomerId);

        // Assert
        found.Should().HaveCount(2);
        found.Should().OnlyContain(a => a.CustomerId == c1.CustomerId);
    }

    [Fact]
    public async Task AddAppointment_WhenDuplicateExists_ShouldReturnExistingAppointment()
    {
        // Arrange
        var c = new CustomerModel { Username = "cust1", Name = "n1", ContactInfo = "c1" };
        var s = new ServiceModel { ServiceName = "s1", ServicePrice = 10 };
        var b = new BarberModel { Username = "b1", Name = "B1", Specialty = "sp1", ContactInfo = "b1" };
        _context.customerTable.Add(c);
        _context.serviceTable.Add(s);
        _context.barberTable.Add(b);
        await _context.SaveChangesAsync();

        var existing = new AppointmentModel
        {
            CustomerId = c.CustomerId,
            ServiceId = s.ServiceId,
            BarberId = b.BarberId,
            AppointmentDate = DateTime.UtcNow.Date,
            Status = "Scheduled"
        };
        _context.appointmentTable.Add(existing);
        await _context.SaveChangesAsync();

        var toAdd = new AppointmentModel
        {
            AppointmentId = existing.AppointmentId,
            CustomerId = c.CustomerId,
            ServiceId = s.ServiceId,
            BarberId = b.BarberId,
            AppointmentDate = DateTime.UtcNow.Date,
            Status = "Scheduled"
        }
            ;

        // Act
        var result = await _repo.AddAsync(toAdd);

        // Assert
        result.Should().NotBeNull();
        result!.AppointmentId.Should().Be(existing.AppointmentId);
    }

    [Fact]
    public async Task AddAppointment_WhenNew_ShouldAddAndBeRetrievableAfterSave()
    {
        // Arrange
        var c = new CustomerModel { Username = "cust5", Name = "n5", ContactInfo = "c5" };
        var s = new ServiceModel { ServiceName = "s5", ServicePrice = 50 };
        var b = new BarberModel { Username = "b5", Name = "B5", Specialty = "sp5", ContactInfo = "b5" };
        _context.customerTable.Add(c);
        _context.serviceTable.Add(s);
        _context.barberTable.Add(b);
        await _context.SaveChangesAsync();

        var toAdd = new AppointmentModel
        {
            CustomerId = c.CustomerId,
            ServiceId = s.ServiceId,
            BarberId = b.BarberId,
            AppointmentDate = DateTime.UtcNow.Date,
            Status = "Scheduled"
        };

        // Act
        var added = await _repo.AddAsync(toAdd);
        await _context.SaveChangesAsync();
        var found = await _repo.GetByIdAsync(added.AppointmentId);

        // Assert
        added.Should().NotBeNull();
        found.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateAppointment_WhenAppointmentExists_ShouldReturnUpdatedAppointment()
    {
        // Arrange
        var c = new CustomerModel { Username = "cust3", Name = "n3", ContactInfo = "c3" };
        var s1 = new ServiceModel { ServiceName = "s3", ServicePrice = 30 };
        var s2 = new ServiceModel { ServiceName = "s4", ServicePrice = 40 };
        var b1 = new BarberModel { Username = "b3", Name = "B3", Specialty = "sp3", ContactInfo = "b3" };
        var b2 = new BarberModel { Username = "b4", Name = "B4", Specialty = "sp4", ContactInfo = "b4" };
        _context.customerTable.Add(c);
        _context.serviceTable.AddRange(s1, s2);
        _context.barberTable.AddRange(b1, b2);
        await _context.SaveChangesAsync();

        var existing = new AppointmentModel
        {
            CustomerId = c.CustomerId,
            ServiceId = s1.ServiceId,
            BarberId = b1.BarberId,
            AppointmentDate = DateTime.UtcNow.Date,
            Status = "Scheduled"
        };
        _context.appointmentTable.Add(existing);
        await _context.SaveChangesAsync();

        var updated = new AppointmentModel
        {
            AppointmentId = existing.AppointmentId,
            CustomerId = c.CustomerId,
            ServiceId = s2.ServiceId,
            BarberId = b2.BarberId,
            AppointmentDate = DateTime.UtcNow.Date.AddDays(1),
            Status = "Scheduled"
        };

        // Act
        var result = await _repo.UpdateAsync(existing.AppointmentId, updated);

        // Assert
        result.Should().NotBeNull();
        result!.AppointmentId.Should().Be(existing.AppointmentId);
        result.ServiceId.Should().Be(s2.ServiceId);
    }

    [Fact]
    public async Task UpdateAppointment_WhenAppointmentDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var c = new CustomerModel { Username = "nope", Name = "none", ContactInfo = "none" };
        var s = new ServiceModel { ServiceName = "sx", ServicePrice = 99 };
        var b = new BarberModel { Username = "bx", Name = "Bx", Specialty = "spx", ContactInfo = "bx" };
        _context.customerTable.Add(c);
        _context.serviceTable.Add(s);
        _context.barberTable.Add(b);
        await _context.SaveChangesAsync();

        var updated = new AppointmentModel
        {
            AppointmentId = 999,
            CustomerId = c.CustomerId,
            ServiceId = s.ServiceId,
            BarberId = b.BarberId,
            AppointmentDate = DateTime.UtcNow.Date,
            Status = "Scheduled"
        };

        // Act
        var result = await _repo.UpdateAsync(999, updated);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteApptById_WhenAppointmentExists_ShouldReturnDeletedAppointment()
    {
        // Arrange
        var c = new CustomerModel { Username = "cust7", Name = "n7", ContactInfo = "c7" };
        var s = new ServiceModel { ServiceName = "s7", ServicePrice = 70 };
        var b = new BarberModel { Username = "b7", Name = "B7", Specialty = "sp7", ContactInfo = "b7" };
        _context.customerTable.Add(c);
        _context.serviceTable.Add(s);
        _context.barberTable.Add(b);
        await _context.SaveChangesAsync();

        var existing = new AppointmentModel
        {
            CustomerId = c.CustomerId,
            ServiceId = s.ServiceId,
            BarberId = b.BarberId,
            AppointmentDate = DateTime.UtcNow.Date,
            Status = "Scheduled"
        };
        _context.appointmentTable.Add(existing);
        await _context.SaveChangesAsync();

        // Act
        var deleted = await _repo.RemoveByIdAsync(existing.AppointmentId);

        // Assert
        deleted.Should().NotBeNull();
        deleted!.AppointmentId.Should().Be(existing.AppointmentId);
    }

    [Fact]
    public async Task DeleteApptById_WhenAppointmentDoesNotExist_ShouldReturnNull()
    {
        // Act
        var deleted = await _repo.RemoveByIdAsync(999);
        // Assert
        deleted.Should().BeNull();
    }
}
