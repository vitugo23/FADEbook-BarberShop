using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Fadebook.DB;
using Fadebook.Models;
using Fadebook.Repositories;
using FluentAssertions;
using Fadebook.Api.Tests.TestUtilities;

namespace Api.Tests.Repositories;

public class ServiceRepositoryTests: RepositoryTestBase
{
    private readonly ServiceRepository _repo;

    public ServiceRepositoryTests()
    {
        _repo = new ServiceRepository(_context);
    }

    [Fact]
    public async Task GetByIdAsync_WhenServiceExists_ShouldReturnService()
    {
        // Arrange
        var testId = 1;
        var service = new ServiceModel
        {
            ServiceId = testId,
            ServiceName = "john_doe",
            ServicePrice = 10
        };
        _context.serviceTable.Add(service);
        await _context.SaveChangesAsync();

        // Act
        var found = await _repo.GetByIdAsync(testId);

        // Assert
        found.Should().NotBeNull();
        found.ServiceId.Should().Be(testId);
        found.ServiceName.Should().Be("john_doe");
    }

    [Fact]
    public async Task GetByIdAsync_WhenServiceDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var testId = 1;
        var service = new ServiceModel
        {
            ServiceId = testId,
            ServiceName = "john_doe",
            ServicePrice = 10
        };
        _context.serviceTable.Add(service);
        await _context.SaveChangesAsync();

        // Act
        var found = await _repo.GetByIdAsync(testId + 1);

        // Assert
        found.Should().BeNull();
    }

    [Fact]
    public async Task GetAll_WhenServiceExist_ShouldReturnService()
    {
        // Arrange
        var testId = 1;
        var service = new ServiceModel
        {
            ServiceId = testId,
            ServiceName = "john_doe",
            ServicePrice = 10
        };
        _context.serviceTable.Add(service);
        await _context.SaveChangesAsync();

        // Act
        var found = await _repo.GetAll();

        // Assert
        found.Should().NotBeNull();
        found.Should().HaveCount(1);
        found.Should().Contain(b => b.ServiceName == "john_doe" && b.ServicePrice == 10);
    }

    [Fact]
    public async Task GetAll_WhenMultipleServicesExist_ShouldReturnServices()
    {
        // Arrange
        var testId = 1;
        var service = new ServiceModel
        {
            ServiceId = testId,
            ServiceName = "john_doe",
            ServicePrice = 10
        };
        var testId2 = 2;
        var service2 = new ServiceModel
        {
            ServiceId = testId2,
            ServiceName = "john_doe",
            ServicePrice = 10
        };
        _context.serviceTable.Add(service);
        _context.serviceTable.Add(service2);
        await _context.SaveChangesAsync();

        // Act
        var found = await _repo.GetAll();

        // Assert
        found.Should().NotBeNull();
        found.Should().HaveCount(2);
        found.Should().Contain(b => b.ServiceName == "john_doe" && b.ServicePrice == 10);
        found.Should().Contain(b => b.ServiceName == "john_doe" && b.ServicePrice == 10);
    }

    [Fact]
    public async Task GetAll_WhenNoServicesExist_ShouldReturnEmptyList()
    {
        // Arrange

        // Act
        var found = await _repo.GetAll();

        // Assert
        found.Should().NotBeNull();
        found.Should().BeEmpty();
    }
    
}
