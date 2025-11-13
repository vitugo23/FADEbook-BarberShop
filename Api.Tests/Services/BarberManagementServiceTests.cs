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

public class BarberManagementServiceTests
{
    private readonly Mock<IDbTransactionContext> _mockDbTransactionContext;
    private readonly Mock<IBarberRepository> _mockBarberRepository;
    private readonly Mock<IBarberServiceRepository> _mockBarberServiceRepository;
    private readonly BarberManagementService _service;

    public BarberManagementServiceTests()
    {
        _mockDbTransactionContext = new Mock<IDbTransactionContext>();
        _mockBarberRepository = new Mock<IBarberRepository>();
        _mockBarberServiceRepository = new Mock<IBarberServiceRepository>();
        _service = new BarberManagementService(
            _mockDbTransactionContext.Object,
            _mockBarberRepository.Object,
            _mockBarberServiceRepository.Object
        );
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsBarber_WhenBarberExists()
    {
        // Arrange
        var barber = new BarberModel { BarberId = 1, Username = "barber1", Name = "John Barber" };
        _mockBarberRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(barber);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(barber);
        _mockBarberRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ThrowsNotFound_WhenBarberDoesNotExist()
    {
        // Arrange
        _mockBarberRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((BarberModel)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(1));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllBarbers()
    {
        // Arrange
        var barbers = new List<BarberModel>
        {
            new BarberModel { BarberId = 1, Username = "barber1", Name = "John Barber" },
            new BarberModel { BarberId = 2, Username = "barber2", Name = "Jane Barber" }
        };
        _mockBarberRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(barbers);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(barbers);
    }

    [Fact]
    public async Task AddAsync_AddsBarber_AndSavesChanges()
    {
        // Arrange
        var barber = new BarberModel { Username = "barber3", Name = "Bob Barber" };
        var addedBarber = new BarberModel { BarberId = 3, Username = "barber3", Name = "Bob Barber" };

        _mockBarberRepository.Setup(r => r.AddAsync(barber)).ReturnsAsync(addedBarber);
        _mockDbTransactionContext.Setup(d => d.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _service.AddAsync(barber);

        // Assert
        result.Should().NotBeNull();
        result.BarberId.Should().Be(3);
        _mockBarberRepository.Verify(r => r.AddAsync(barber), Times.Once);
        _mockDbTransactionContext.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesBarber_AndSavesChanges()
    {
        // Arrange
        var barber = new BarberModel { BarberId = 1, Username = "barber1", Name = "John Updated" };
        _mockBarberRepository.Setup(r => r.UpdateAsync(1, barber)).ReturnsAsync(barber);
        _mockDbTransactionContext.Setup(d => d.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _service.UpdateAsync(1, barber);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("John Updated");
        _mockBarberRepository.Verify(r => r.UpdateAsync(1, barber), Times.Once);
        _mockDbTransactionContext.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteByIdAsync_DeletesBarber_AndSavesChanges()
    {
        // Arrange
        var barber = new BarberModel { BarberId = 1, Username = "barber1", Name = "John Barber" };
        _mockBarberRepository.Setup(r => r.RemoveByIdAsync(1)).ReturnsAsync(barber);
        _mockDbTransactionContext.Setup(d => d.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _service.DeleteByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(barber);
        _mockBarberRepository.Verify(r => r.RemoveByIdAsync(1), Times.Once);
        _mockDbTransactionContext.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateBarberServicesAsync_AddsNewServices()
    {
        // Arrange
        var barberId = 1;
        var existingServices = new List<BarberServiceModel>
        {
            new BarberServiceModel { BarberId = 1, ServiceId = 1 }
        };
        var selectedServiceIds = new List<int> { 1, 2, 3 };
        var updatedServices = new List<BarberServiceModel>
        {
            new BarberServiceModel { BarberId = 1, ServiceId = 1 },
            new BarberServiceModel { BarberId = 1, ServiceId = 2 },
            new BarberServiceModel { BarberId = 1, ServiceId = 3 }
        };

        _mockBarberServiceRepository.SetupSequence(r => r.GetByBarberIdAsync(barberId))
            .ReturnsAsync(existingServices)
            .ReturnsAsync(updatedServices);
        _mockBarberServiceRepository.Setup(r => r.AddAsync(It.IsAny<BarberServiceModel>())).ReturnsAsync((BarberServiceModel bs) => bs);
        _mockDbTransactionContext.Setup(d => d.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _service.UpdateBarberServicesAsync(barberId, selectedServiceIds);

        // Assert
        _mockBarberServiceRepository.Verify(r => r.AddAsync(It.Is<BarberServiceModel>(bs => bs.ServiceId == 2)), Times.Once);
        _mockBarberServiceRepository.Verify(r => r.AddAsync(It.Is<BarberServiceModel>(bs => bs.ServiceId == 3)), Times.Once);
        _mockDbTransactionContext.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateBarberServicesAsync_RemovesOldServices()
    {
        // Arrange
        var barberId = 1;
        var existingServices = new List<BarberServiceModel>
        {
            new BarberServiceModel { BarberId = 1, ServiceId = 1 },
            new BarberServiceModel { BarberId = 1, ServiceId = 2 },
            new BarberServiceModel { BarberId = 1, ServiceId = 3 }
        };
        var selectedServiceIds = new List<int> { 1 };
        var updatedServices = new List<BarberServiceModel>
        {
            new BarberServiceModel { BarberId = 1, ServiceId = 1 }
        };

        _mockBarberServiceRepository.SetupSequence(r => r.GetByBarberIdAsync(barberId))
            .ReturnsAsync(existingServices)
            .ReturnsAsync(updatedServices);
        _mockBarberServiceRepository.Setup(r => r.RemoveByBarberIdServiceId(barberId, It.IsAny<int>()))
            .ReturnsAsync((int bId, int sId) => new BarberServiceModel { BarberId = bId, ServiceId = sId });
        _mockDbTransactionContext.Setup(d => d.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _service.UpdateBarberServicesAsync(barberId, selectedServiceIds);

        // Assert
        _mockBarberServiceRepository.Verify(r => r.RemoveByBarberIdServiceId(barberId, 2), Times.Once);
        _mockBarberServiceRepository.Verify(r => r.RemoveByBarberIdServiceId(barberId, 3), Times.Once);
        _mockDbTransactionContext.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
