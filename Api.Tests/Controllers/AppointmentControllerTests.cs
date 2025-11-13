using System;
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

public class AppointmentControllerTests
{
    private readonly Mock<IAppointmentManagementService> _mockService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly AppointmentController _controller;

    public AppointmentControllerTests()
    {
        _mockService = new Mock<IAppointmentManagementService>();
        _mockMapper = new Mock<IMapper>();
        _controller = new AppointmentController(_mockService.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Create_ReturnsCreated_WhenAppointmentIsValid()
    {
        // Arrange
        var appointmentDto = new AppointmentDto
        {
            Status = "Pending",
            CustomerId = 1,
            ServiceId = 1,
            BarberId = 1,
            appointmentDate = DateTime.UtcNow.AddDays(1)
        };

        var appointmentModel = new AppointmentModel
        {
            Status = "Pending",
            CustomerId = 1,
            ServiceId = 1,
            BarberId = 1,
            AppointmentDate = DateTime.UtcNow.AddDays(1)
        };

        var createdModel = new AppointmentModel
        {
            AppointmentId = 1,
            Status = "Pending",
            CustomerId = 1,
            ServiceId = 1,
            BarberId = 1,
            AppointmentDate = DateTime.UtcNow.AddDays(1)
        };

        _mockMapper.Setup(m => m.Map<AppointmentModel>(appointmentDto)).Returns(appointmentModel);
        _mockService.Setup(s => s.AddAppointmentAsync(appointmentModel)).ReturnsAsync(createdModel);
        _mockMapper.Setup(m => m.Map<AppointmentDto>(createdModel)).Returns(appointmentDto);

        // Act
        var result = await _controller.Create(appointmentDto);

        // Assert
        result.Result.Should().BeOfType<CreatedResult>();
        var createdResult = result.Result as CreatedResult;
        createdResult.Location.Should().Be("api/appointment/1");
    }

    [Fact]
    public async Task Create_ThrowsBadRequest_WhenServiceRejects()
    {
        // Arrange
        var appointmentDto = new AppointmentDto
        {
            Status = "Pending",
            CustomerId = 1,
            ServiceId = 1,
            BarberId = 1,
            appointmentDate = DateTime.UtcNow.AddDays(1)
        };

        var appointmentModel = new AppointmentModel();
        _mockMapper.Setup(m => m.Map<AppointmentModel>(appointmentDto)).Returns(appointmentModel);
        _mockService.Setup(s => s.AddAppointmentAsync(appointmentModel)).ThrowsAsync(new BadRequestException("Invalid appointment"));

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _controller.Create(appointmentDto));
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenAppointmentExists()
    {
        // Arrange
        var appointmentModel = new AppointmentModel
        {
            AppointmentId = 1,
            Status = "Pending",
            CustomerId = 1,
            ServiceId = 1,
            BarberId = 1,
            AppointmentDate = DateTime.UtcNow.AddDays(1)
        };

        var appointmentDto = new AppointmentDto
        {
            AppointmentId = 1,
            Status = "Pending",
            CustomerId = 1,
            ServiceId = 1,
            BarberId = 1,
            appointmentDate = DateTime.UtcNow.AddDays(1)
        };

        _mockService.Setup(s => s.GetAppointmentByIdAsync(1)).ReturnsAsync(appointmentModel);
        _mockMapper.Setup(m => m.Map<AppointmentDto>(appointmentModel)).Returns(appointmentDto);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult.Value.Should().BeEquivalentTo(appointmentDto);
    }

    [Fact]
    public async Task GetById_ThrowsNotFound_WhenAppointmentDoesNotExist()
    {
        // Arrange
        _mockService.Setup(s => s.GetAppointmentByIdAsync(1)).ThrowsAsync(new NotFoundException("not found"));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _controller.GetById(1));
    }

    [Fact]
    public async Task Update_ReturnsOk_WhenAppointmentIsUpdated()
    {
        // Arrange
        var appointmentDto = new AppointmentDto
        {
            Status = "Completed",
            CustomerId = 1,
            ServiceId = 1,
            BarberId = 1,
            appointmentDate = DateTime.UtcNow.AddDays(1)
        };

        var appointmentModel = new AppointmentModel
        {
            AppointmentId = 1,
            Status = "Completed",
            CustomerId = 1,
            ServiceId = 1,
            BarberId = 1,
            AppointmentDate = DateTime.UtcNow.AddDays(1)
        };

        _mockMapper.Setup(m => m.Map<AppointmentModel>(appointmentDto)).Returns(appointmentModel);
        _mockService.Setup(s => s.UpdateAppointmentAsync(1, It.IsAny<AppointmentModel>())).ReturnsAsync(appointmentModel);
        _mockMapper.Setup(m => m.Map<AppointmentDto>(appointmentModel)).Returns(appointmentDto);

        // Act
        var result = await _controller.Update(1, appointmentDto);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Update_ThrowsNotFound_WhenAppointmentDoesNotExist()
    {
        // Arrange
        var appointmentDto = new AppointmentDto();
        var appointmentModel = new AppointmentModel();

        _mockMapper.Setup(m => m.Map<AppointmentModel>(appointmentDto)).Returns(appointmentModel);
        _mockService.Setup(s => s.UpdateAppointmentAsync(1, It.IsAny<AppointmentModel>())).ThrowsAsync(new NotFoundException("not found"));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _controller.Update(1, appointmentDto));
    }

    [Fact]
    public async Task GetByDate_ReturnsOk_WithAppointments()
    {
        // Arrange
        var date = DateTime.UtcNow.Date;
        var appointments = new List<AppointmentModel>
        {
            new AppointmentModel { AppointmentId = 1, AppointmentDate = date },
            new AppointmentModel { AppointmentId = 2, AppointmentDate = date }
        };
        var appointmentDtos = new List<AppointmentDto>
        {
            new AppointmentDto { AppointmentId = 1, appointmentDate = date },
            new AppointmentDto { AppointmentId = 2, appointmentDate = date }
        };

        _mockService.Setup(s => s.GetAppointmentsByDateAsync(date)).ReturnsAsync(appointments);
        _mockMapper.Setup(m => m.Map<IEnumerable<AppointmentDto>>(appointments)).Returns(appointmentDtos);

        // Act
        var result = await _controller.GetByDate(date);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult.Value.Should().BeEquivalentTo(appointmentDtos);
    }

    [Fact]
    public async Task GetByUsername_ReturnsOk_WhenUsernameExists()
    {
        // Arrange
        var username = "testuser";
        var appointments = new List<AppointmentModel>
        {
            new AppointmentModel { AppointmentId = 1, CustomerId = 1 }
        };
        var appointmentDtos = new List<AppointmentDto>
        {
            new AppointmentDto { AppointmentId = 1, CustomerId = 1 }
        };

        _mockService.Setup(s => s.LookupAppointmentsByUsernameAsync(username)).ReturnsAsync(appointments);
        _mockMapper.Setup(m => m.Map<IEnumerable<AppointmentDto>>(appointments)).Returns(appointmentDtos);

        // Act
        var result = await _controller.GetByUsername(username);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetByUsername_ReturnsBadRequest_WhenUsernameIsEmpty()
    {
        // Act
        var result = await _controller.GetByUsername("");

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetByUsername_ThrowsNotFound_WhenUsernameDoesNotExist()
    {
        // Arrange
        _mockService.Setup(s => s.LookupAppointmentsByUsernameAsync("nonexistent")).ThrowsAsync(new NotFoundException(""));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _controller.GetByUsername("nonexistent"));
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenAppointmentIsDeleted()
    {
        // Arrange
        var appointmentModel = new AppointmentModel { AppointmentId = 1 };
        _mockService.Setup(s => s.DeleteAppointmentAsync(1)).ReturnsAsync(appointmentModel);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_ThrowsNotFound_WhenAppointmentDoesNotExist()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteAppointmentAsync(1)).ThrowsAsync(new NotFoundException(""));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _controller.Delete(1));
    }
}
