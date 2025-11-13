using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Fadebook.Controllers;
using Fadebook.Models;
using Fadebook.Services;
using Fadebook.DTOs;
using Fadebook.Exceptions;

namespace Api.Tests.Controllers;

public class CustomerAppointmentControllerTests
{
    private readonly Mock<ICustomerAppointmentService> _mockCustomerAppointmentService;
    private readonly Mock<IUserAccountService> _mockUserAccountService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly CustomerController _controller;

    public CustomerAppointmentControllerTests()
    {
        _mockCustomerAppointmentService = new Mock<ICustomerAppointmentService>();
        _mockUserAccountService = new Mock<IUserAccountService>();
        _mockMapper = new Mock<IMapper>();
        _controller = new CustomerController(
            _mockCustomerAppointmentService.Object,
            _mockUserAccountService.Object,
            _mockMapper.Object
        );
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenCustomerExists()
    {
        // Arrange
        var customerModel = new CustomerModel
        {
            CustomerId = 1,
            Username = "customer1",
            Name = "John Customer",
            ContactInfo = "555-0201"
        };
        var customerDto = new CustomerDto
        {
            CustomerId = 1,
            Username = "customer1",
            Name = "John Customer",
            ContactInfo = "555-0201"
        };

        _mockUserAccountService.Setup(s => s.GetCustomerByIdAsync(1)).ReturnsAsync(customerModel);
        _mockMapper.Setup(m => m.Map<CustomerDto>(customerModel)).Returns(customerDto);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult.Value.Should().BeEquivalentTo(customerDto);
    }

    [Fact]
    public async Task GetById_ThrowsNotFound_WhenCustomerDoesNotExist()
    {
        // Arrange
        _mockUserAccountService.Setup(s => s.GetCustomerByIdAsync(1)).ThrowsAsync(new NotFoundException("not found"));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _controller.GetById(1));
    }

    [Fact]
    public async Task GetServices_ReturnsOk_WithServices()
    {
        // Arrange
        var services = new List<ServiceModel>
        {
            new ServiceModel { ServiceId = 1, ServiceName = "Haircut", ServicePrice = 20 },
            new ServiceModel { ServiceId = 2, ServiceName = "Beard Trim", ServicePrice = 15 }
        };
        var serviceDtos = new List<ServiceDto>
        {
            new ServiceDto { ServiceId = 1, ServiceName = "Haircut", ServicePrice = 20 },
            new ServiceDto { ServiceId = 2, ServiceName = "Beard Trim", ServicePrice = 15 }
        };

        _mockCustomerAppointmentService.Setup(s => s.ListAvailableServicesAsync()).ReturnsAsync(services);
        _mockMapper.Setup(m => m.Map<IEnumerable<ServiceDto>>(services)).Returns(serviceDtos);

        // Act
        var result = await _controller.GetServices();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult.Value.Should().BeEquivalentTo(serviceDtos);
    }

    [Fact]
    public async Task GetBarbersByService_ReturnsOk_WithBarbers()
    {
        // Arrange
        var barbers = new List<BarberModel>
        {
            new BarberModel { BarberId = 1, Username = "barber1", Name = "John Barber" }
        };
        var barberDtos = new List<BarberDto>
        {
            new BarberDto { BarberId = 1, Username = "barber1", Name = "John Barber" }
        };

        _mockCustomerAppointmentService.Setup(s => s.ListAvailableBarbersByServiceAsync(1)).ReturnsAsync(barbers);
        _mockMapper.Setup(m => m.Map<IEnumerable<BarberDto>>(barbers)).Returns(barberDtos);

        // Act
        var result = await _controller.GetBarbersByService(1);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult.Value.Should().BeEquivalentTo(barberDtos);
    }

    [Fact]
    public async Task RequestAppointment_ReturnsCreated_WhenAppointmentIsValid()
    {
        // Arrange
        var request = new AppointmentRequestDto
        {
            Customer = new CustomerDto
            {
                CustomerId = 1,
                Username = "customer1",
                Name = "John Customer",
                ContactInfo = "555-0201"
            },
            Appointment = new AppointmentDto
            {
                AppointmentId = 0,
                Status = "Pending",
                CustomerId = 1,
                ServiceId = 1,
                BarberId = 1
            }
        };

        var customerModel = new CustomerModel { CustomerId = 1, Username = "customer1" };
        var appointmentModel = new AppointmentModel { CustomerId = 1, ServiceId = 1, BarberId = 1 };
        var createdAppointment = new AppointmentModel
        {
            AppointmentId = 1,
            CustomerId = 1,
            ServiceId = 1,
            BarberId = 1,
            Status = "Pending"
        };
        var appointmentDto = new AppointmentDto
        {
            AppointmentId = 1,
            CustomerId = 1,
            ServiceId = 1,
            BarberId = 1,
            Status = "Pending"
        };

        _mockMapper.Setup(m => m.Map<CustomerModel>(request.Customer)).Returns(customerModel);
        _mockMapper.Setup(m => m.Map<AppointmentModel>(request.Appointment)).Returns(appointmentModel);
        _mockCustomerAppointmentService.Setup(s => s.MakeAppointmentAsync(appointmentModel)).ReturnsAsync(createdAppointment);
        _mockMapper.Setup(m => m.Map<AppointmentDto>(createdAppointment)).Returns(appointmentDto);

        // Act
        var result = await _controller.RequestAppointment(request);

        // Assert
        result.Result.Should().BeOfType<CreatedResult>();
        var createdResult = result.Result as CreatedResult;
        createdResult.Location.Should().Be("/api/appointment/1");
    }
}
