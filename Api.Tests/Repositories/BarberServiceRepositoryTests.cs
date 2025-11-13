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

public class BarberServiceRepositoryTests: RepositoryTestBase
{
    private readonly BarberServiceRepository _repo;

    public BarberServiceRepositoryTests()
    {
        _repo = new BarberServiceRepository(_context);
    }

    [Fact]
    public async Task GetByIdAsync_WhenExists_ShouldReturnEntity()
    {
        // Arrange
        var barber = new BarberModel { Username = "b1", Name = "B1", Specialty = "sp1", ContactInfo = "b1" };
        var service = new ServiceModel { ServiceName = "s1", ServicePrice = 10 };
        _context.barberTable.Add(barber);
        _context.serviceTable.Add(service);
        await _context.SaveChangesAsync();

        var entity = new BarberServiceModel { BarberId = barber.BarberId, ServiceId = service.ServiceId };
        _context.barberServiceTable.Add(entity);
        await _context.SaveChangesAsync();

        // Act
        var found = await _repo.GetByIdAsync(entity.Id);

        // Assert
        found.Should().NotBeNull();
        found!.Id.Should().Be(entity.Id);
        found.BarberId.Should().Be(barber.BarberId);
        found.ServiceId.Should().Be(service.ServiceId);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotExists_ShouldReturnNull()
    {
        // Act
        var found = await _repo.GetByIdAsync(999);
        // Assert
        found.Should().BeNull();
    }

    [Fact]
    public async Task GetBarberServiceByBarberId_ShouldFilterByBarberId()
    {
        // Arrange
        var b10 = new BarberModel { Username = "b10", Name = "B10", Specialty = "sp", ContactInfo = "b10" };
        var b11 = new BarberModel { Username = "b11", Name = "B11", Specialty = "sp", ContactInfo = "b11" };
        var s100 = new ServiceModel { ServiceName = "s100", ServicePrice = 10 };
        var s101 = new ServiceModel { ServiceName = "s101", ServicePrice = 11 };
        _context.barberTable.AddRange(b10, b11);
        _context.serviceTable.AddRange(s100, s101);
        await _context.SaveChangesAsync();

        _context.barberServiceTable.AddRange(
            new BarberServiceModel { BarberId = b10.BarberId, ServiceId = s100.ServiceId },
            new BarberServiceModel { BarberId = b10.BarberId, ServiceId = s101.ServiceId },
            new BarberServiceModel { BarberId = b11.BarberId, ServiceId = s100.ServiceId }
        );
        await _context.SaveChangesAsync();

        // Act
        var found = await _repo.GetByBarberIdAsync(b10.BarberId);

        // Assert
        found.Should().HaveCount(2);
        found.Should().OnlyContain(bsm => bsm.BarberId == b10.BarberId || bsm.BarberId == b11.BarberId);
    }

    [Fact]
    public async Task GetBarberServiceByServiceId_ShouldFilterByServiceId()
    {
        // Arrange
        var barberId1 = 1;
        var barberId2 = 2;
        var barberId3 = 3;
        var serviceId1 = 4;
        var serviceId2 = 5;
        var b10 = new BarberModel { BarberId = barberId1, Username = "b10", Name = "B10", Specialty = "sp", ContactInfo = "b10" };
        var b11 = new BarberModel { BarberId = barberId2, Username = "b11", Name = "B11", Specialty = "sp", ContactInfo = "b11" };
        var b12 = new BarberModel { BarberId = barberId3, Username = "b12", Name = "B12", Specialty = "sp", ContactInfo = "b12" };
        var s100 = new ServiceModel { ServiceId = serviceId1, ServiceName = "s100", ServicePrice = 10 };
        var s101 = new ServiceModel { ServiceId = serviceId2, ServiceName = "s101", ServicePrice = 11 };
        _context.barberTable.AddRange(b10, b11, b12);
        _context.serviceTable.AddRange(s100, s101);
        await _context.SaveChangesAsync();

        _context.barberServiceTable.AddRange(
            new BarberServiceModel { BarberId = barberId1, ServiceId = serviceId1 },
            new BarberServiceModel { BarberId = barberId2, ServiceId = serviceId1 },
            new BarberServiceModel { BarberId = barberId3, ServiceId = serviceId2 }
        );
        await _context.SaveChangesAsync();

        // Act
        var found = await _repo.GetByServiceIdAsync(serviceId1);

        // Assert
        found.Should().HaveCount(2);
        found.Should().OnlyContain(bsm => bsm.BarberId == barberId1 || bsm.BarberId == barberId2);
        found.Should().NotContain(bsm => bsm.BarberId == barberId3);
    }

    [Fact]
    public async Task GetBarberServiceByBarberIdServiceId_WhenExists_ShouldReturnEntity()
    {
        // Arrange
        var barberId = 1;
        var serviceId = 2;
        var b = new BarberModel { BarberId = barberId, Username = "b", Name = "B", Specialty = "sp", ContactInfo = "b" };
        var s = new ServiceModel { ServiceId = serviceId, ServiceName = "s", ServicePrice = 10 };
        _context.barberTable.Add(b);
        _context.serviceTable.Add(s);
        await _context.SaveChangesAsync();

        var entity = new BarberServiceModel { BarberId = b.BarberId, ServiceId = s.ServiceId };
        _context.barberServiceTable.Add(entity);
        await _context.SaveChangesAsync();

        // Act
        var found = await _repo.GetByBarberIdServiceIdAsync(barberId, serviceId);

        // Assert
        found.Should().NotBeNull();
        found.BarberId.Should().Be(barberId);
        found.ServiceId.Should().Be(serviceId);
    }

    [Fact]
    public async Task AddBarberService_WhenDuplicateExists_ShouldReturnExisting()
    {
        // Arrange
        var barberId = 1;
        var serviceId = 2;

        var b = new BarberModel { BarberId = barberId, Username = "b20", Name = "B20", Specialty = "sp", ContactInfo = "b20" };
        var s = new ServiceModel { ServiceId = serviceId, ServiceName = "s200", ServicePrice = 20 };
        _context.barberTable.Add(b);
        _context.serviceTable.Add(s);
        await _context.SaveChangesAsync();


        var entity = new BarberServiceModel { BarberId = barberId, ServiceId = serviceId };
        _context.barberServiceTable.Add(entity);
        await _context.SaveChangesAsync();

        // Act
        var newEntity = new BarberServiceModel { BarberId = barberId, ServiceId = serviceId };
        var returned = await _repo.AddAsync(newEntity);

        // Assert
        returned.Should().NotBeNull();
        returned.BarberId.Should().Be(barberId);
        returned.ServiceId.Should().Be(serviceId);
    }

    [Fact]
    public async Task RemoveBarberServiceById_WhenExists_ShouldReturnRemovedEntity()
    {
        // Arrange
        var barberId = 1;
        var serviceId = 2;
        var b = new BarberModel { Username = "b22", Name = "B22", Specialty = "sp", ContactInfo = "b22" };
        var s = new ServiceModel { ServiceName = "s222", ServicePrice = 22 };
        _context.barberTable.Add(b);
        _context.serviceTable.Add(s);
        await _context.SaveChangesAsync();

        var entity = new BarberServiceModel { BarberId = b.BarberId, ServiceId = s.ServiceId };
        _context.barberServiceTable.Add(entity);
        await _context.SaveChangesAsync();

        // Act
        var removed = await _repo.RemoveByIdAsync(entity.Id);
        await _context.SaveChangesAsync();

        // Assert
        removed.Should().NotBeNull();
        removed!.Id.Should().Be(entity.Id);
    }

    [Fact]
    public async Task RemoveBarberServiceById_WhenNotExists_ShouldReturnNull()
    {
        // Act
        var removed = await _repo.RemoveByIdAsync(999);
        // Assert
        removed.Should().BeNull();
    }

    [Fact]
    public async Task RemoveBarberServiceByBarberIdServiceId_WhenExists_ShouldReturnRemovedEntity()
    {
        // Arrange
        var barberId = 1;
        var serviceId = 2;
        var b = new BarberModel { BarberId = barberId, Username = "b30", Name = "B30", Specialty = "sp", ContactInfo = "b30" };
        var s = new ServiceModel { ServiceId = serviceId, ServiceName = "s300", ServicePrice = 30 };
        _context.barberTable.Add(b);
        _context.serviceTable.Add(s);
        await _context.SaveChangesAsync();

        var entity = new BarberServiceModel { BarberId = barberId, ServiceId = serviceId };
        _context.barberServiceTable.Add(entity);
        await _context.SaveChangesAsync();

        // Act
        var removed = await _repo.RemoveByBarberIdServiceId(barberId, serviceId);

        // Assert
        removed.Should().NotBeNull();
        removed.Id.Should().Be(entity.Id);
        removed.BarberId.Should().Be(barberId);
        removed.ServiceId.Should().Be(serviceId);
    }
}
