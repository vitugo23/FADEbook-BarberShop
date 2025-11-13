using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using Fadebook.Models;
using Fadebook.Repositories;
using Fadebook.Services;
using Fadebook.Exceptions;

namespace Api.Tests.Services;

public class CustomerAppointmentServiceTests
{
    private readonly Mock<IDbTransactionContext> _mockDbTransactionContext;
    private readonly Mock<IServiceRepository> _mockServiceRepository;
    private readonly Mock<IBarberServiceRepository> _mockBarberServiceRepository;
    private readonly Mock<IBarberRepository> _mockBarberRepository;
    private readonly Mock<IAppointmentRepository> _mockAppointmentRepository;
    private readonly CustomerAppointmentService _service;

    public CustomerAppointmentServiceTests()
    {
        _mockDbTransactionContext = new Mock<IDbTransactionContext>();
        _mockServiceRepository = new Mock<IServiceRepository>();
        _mockBarberServiceRepository = new Mock<IBarberServiceRepository>();
        _mockBarberRepository = new Mock<IBarberRepository>();
        _mockAppointmentRepository = new Mock<IAppointmentRepository>();
        _service = new CustomerAppointmentService(
            _mockDbTransactionContext.Object,
            _mockServiceRepository.Object,
            _mockBarberServiceRepository.Object,
            _mockBarberRepository.Object,
            _mockAppointmentRepository.Object
        );
    }

    [Fact]
    public async Task ListAvailableServicesAsync_ReturnsAllServices()
    {
        // Arrange
        var services = new List<ServiceModel>
        {
            new ServiceModel { ServiceId = 1, ServiceName = "Haircut", ServicePrice = 20 },
            new ServiceModel { ServiceId = 2, ServiceName = "Beard Trim", ServicePrice = 15 }
        };
        _mockServiceRepository.Setup(r => r.GetAll()).ReturnsAsync(services);

        // Act
        var result = await _service.ListAvailableServicesAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(services);
        _mockServiceRepository.Verify(r => r.GetAll(), Times.Once);
    }

    [Fact]
    public async Task ListAvailableBarbersByServiceAsync_ReturnsBarbers()
    {
        // Arrange
        var barber1 = new BarberModel { BarberId = 1, Username = "barber1", Name = "John Barber" };
        var barber2 = new BarberModel { BarberId = 2, Username = "barber2", Name = "Jane Barber" };
        var barberServices = new List<BarberServiceModel>
        {
            new BarberServiceModel { BarberId = 1, ServiceId = 1, Barber = barber1 },
            new BarberServiceModel { BarberId = 2, ServiceId = 1, Barber = barber2 }
        };

        _mockBarberServiceRepository.Setup(r => r.GetByServiceIdAsync(1)).ReturnsAsync(barberServices);

        // Act
        var result = await _service.ListAvailableBarbersByServiceAsync(1);

        // Assert
        var barberList = result.ToList();
        barberList.Should().HaveCount(2);
        barberList.Should().Contain(b => b.BarberId == 1);
        barberList.Should().Contain(b => b.BarberId == 2);
    }

    [Fact]
    public async Task GetAppointmentsByCustomerIdAsync_ReturnsCustomerAppointments()
    {
        // Arrange
        var appointments = new List<AppointmentModel>
        {
            new AppointmentModel { AppointmentId = 1, CustomerId = 1, Status = "Pending" },
            new AppointmentModel { AppointmentId = 2, CustomerId = 1, Status = "Completed" }
        };
        _mockAppointmentRepository.Setup(r => r.GetByCustomerIdAsync(1)).ReturnsAsync(appointments);

        // Act
        var result = await _service.GetAppointmentsByCustomerIdAsync(1);

        // Assert
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(appointments);
    }

    [Fact]
    public async Task MakeAppointmentAsync_CreatesAppointment_WhenValid()
    {
        // Arrange
        var appointment = new AppointmentModel
        {
            CustomerId = 1,
            ServiceId = 1,
            BarberId = 1,
            AppointmentDate = DateTime.UtcNow.AddDays(1),
            Status = "Pending"
        };
        var createdAppointment = new AppointmentModel
        {
            AppointmentId = 1,
            CustomerId = 1,
            ServiceId = 1,
            BarberId = 1,
            AppointmentDate = appointment.AppointmentDate,
            Status = "Pending"
        };

        _mockAppointmentRepository.Setup(r => r.AddAsync(It.IsAny<AppointmentModel>())).ReturnsAsync(createdAppointment);
        _mockDbTransactionContext.Setup(d => d.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _service.MakeAppointmentAsync(appointment);

        // Assert
        result.Should().NotBeNull();
        result.AppointmentId.Should().Be(1);
        _mockAppointmentRepository.Verify(r => r.AddAsync(It.IsAny<AppointmentModel>()), Times.Once);
        _mockDbTransactionContext.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MakeAppointmentAsync_ThrowsBadRequest_WhenAppointmentIsIncomplete()
    {
        // Arrange
        var incompleteAppointment = new AppointmentModel
        {
            CustomerId = 1,
            // Missing ServiceId, BarberId, etc.
        };

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _service.MakeAppointmentAsync(incompleteAppointment));
    }
}
