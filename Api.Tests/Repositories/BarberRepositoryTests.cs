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

public class BarberRepositoryTests: RepositoryTestBase
{
   private readonly BarberRepository _repo;

    public BarberRepositoryTests()
    {
        _repo = new BarberRepository(_context);
    }

    [Fact]
    public async Task GetByIdAsync_WhenBarberExists_ShouldReturnBarber()
    {
        // Arrange
        var testId = 1;
        var barber = new BarberModel
        {
            BarberId = testId,
            Username = "john_doe",
            Name = "John Doe",
            Specialty = "Fades",
            ContactInfo = "john@example.com"
        };
        _context.barberTable.Add(barber);
        await _context.SaveChangesAsync();

        // Act
        var found = await _repo.GetByIdAsync(testId);

        // Assert
        found.Should().NotBeNull();
        found.BarberId.Should().Be(testId);
        found.Username.Should().Be("john_doe");
    }

    [Fact]
    public async Task GetByIdAsync_WhenBarberDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var testId = 1;
        var barber = new BarberModel
        {
            BarberId = testId,
            Username = "john_doe",
            Name = "John Doe",
            Specialty = "Fades",
            ContactInfo = "john@example.com"
        };
        _context.barberTable.Add(barber);
        await _context.SaveChangesAsync();

        // Act
        var found = await _repo.GetByIdAsync(testId + 1);

        // Assert
        found.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_WhenBarberExist_ShouldReturnBarber()
    {
        // Arrange
        var testId = 1;
        var barber = new BarberModel
        {
            BarberId = testId,
            Username = "john_doe",
            Name = "John Doe",
            Specialty = "Fades",
            ContactInfo = "john@example.com"
        };
        _context.barberTable.Add(barber);
        await _context.SaveChangesAsync();

        // Act
        var found = await _repo.GetAllAsync();

        // Assert
        found.Should().NotBeNull();
        found.Should().HaveCount(1);
        found.Should().ContainSingle(b => b.Username == "john_doe" && b.Name == "John Doe");
    }

    [Fact]
    public async Task GetAllAsync_WhenMultipleBarbersExist_ShouldReturnBarbers()
    {
    // Arrange
    var testId = 1;
    var barber = new BarberModel
    {
        BarberId = testId,
        Username = "john_doe",
        Name = "John Doe",
        Specialty = "Fades",
        ContactInfo = "john@example.com"
    };
    var barber2 = new BarberModel
    {
        BarberId = testId + 1,
        Username = "jane_doe",
        Name = "Jane Doe",
        Specialty = "Fades",
        ContactInfo = "jane@example.com"
    };
    _context.barberTable.Add(barber);
    _context.barberTable.Add(barber2);
    await _context.SaveChangesAsync();

    // Act
    var found = await _repo.GetAllAsync();

    // Assert
    found.Should().NotBeNull();
    found.Should().HaveCount(2); 
    found.Should().Contain(b => b.Username == "john_doe" && b.Name == "John Doe");
    found.Should().Contain(b => b.Username == "jane_doe" && b.Name == "Jane Doe");
    }

    [Fact]
    public async Task UpdateAsync_WhenBarberExists_ShouldReturnUpdatedBarber()
    {
        // Arrange
        var testId = 1;
        var barber = new BarberModel
        {
            BarberId = testId,
            Username = "john_doe",
            Name = "John Doe",
            Specialty = "Fades",
            ContactInfo = "john@example.com"
        };
        _context.barberTable.Add(barber);
        await _context.SaveChangesAsync();
        var updatedBarber = new BarberModel
        {
            BarberId = testId,
            Username = "tom_doe",
            Name = "Tom Doe",
            Specialty = "Shape Ups",  
            ContactInfo = "tom@example.com"
        };

        // Act
        var found = await _repo.UpdateAsync(testId, updatedBarber);
        await _context.SaveChangesAsync();

        // Assert
        found.Should().NotBeNull();
        found.BarberId.Should().Be(testId);
        found.Username.Should().Be("tom_doe");
        found.Name.Should().Be("Tom Doe");
        found.Specialty.Should().Be("Shape Ups");
        found.ContactInfo.Should().Be("tom@example.com");
    }

    [Fact]
    public async Task UpdateAsync_WhenBarberDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var testId = 1;
        var barber = new BarberModel
        {
            BarberId = testId,
            Username = "john_doe",
            Name = "John Doe",
            Specialty = "Fades",
            ContactInfo = "john@example.com"
        };
        _context.barberTable.Add(barber);
        await _context.SaveChangesAsync();
        var updatedBarber = new BarberModel
        {
            BarberId = testId + 1,
            Username = "tom_doe",
            Name = "Tom Doe",
            Specialty = "Shape Ups",
            ContactInfo = "tom@example.com"
        };

        // Act
        var found = await _repo.UpdateAsync(testId + 1, updatedBarber);
        // Assert
        found.Should().BeNull();
    }

    [Fact]
    public async Task DeleteByIdAsync_WhenBarberExists_ShouldReturnDeletedBarber()
    {
        // Arrange
        var testId = 1;
        var barber = new BarberModel
        {
            BarberId = testId,
            Username = "john_doe",
            Name = "John Doe",
            Specialty = "Fades",
            ContactInfo = "john@example.com"
        };
        _context.barberTable.Add(barber);
        await _context.SaveChangesAsync();

        // Act
        var found = await _repo.RemoveByIdAsync(testId);

        // Assert
        found.Should().NotBeNull();
        found.BarberId.Should().Be(testId);
    }

    [Fact]
    public async Task DeleteByIdAsync_WhenBarberDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var testId = 1;
        var barber = new BarberModel
        {
            BarberId = testId,
            Username = "john_doe",
            Name = "John Doe",
            Specialty = "Fades",
            ContactInfo = "john@example.com"
        };
        _context.barberTable.Add(barber);
        await _context.SaveChangesAsync();

        // Act
        var removed = await _repo.RemoveByIdAsync(testId + 1);
        // Assert
        removed.Should().BeNull();
    }
}

