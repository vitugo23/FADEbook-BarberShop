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

public class CustomerRepositoryTests: RepositoryTestBase
{
    private readonly CustomerRepository _repo;

    public CustomerRepositoryTests()
    {
        _repo = new CustomerRepository(_context);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCustomerExists_ShouldReturnCustomer()
    {
        // Arrange
        var customer = new CustomerModel
        {
            CustomerId = 1,
            Username = "john_doe",
            Name = "John Doe",
            ContactInfo = "john@example.com"
        };
        _context.customerTable.Add(customer);
        await _context.SaveChangesAsync();

        // Act
        var found = await _repo.GetByIdAsync(1);

        // Assert
        found.Should().NotBeNull();
        found!.CustomerId.Should().Be(1);
        found.Username.Should().Be("john_doe");
    }

    [Fact]
    public async Task GetByIdAsync_WhenCustomerDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var customer = new CustomerModel
        {
            CustomerId = 1,
            Username = "john_doe",
            Name = "John Doe",
            ContactInfo = "john@example.com"
        };
        _context.customerTable.Add(customer);
        await _context.SaveChangesAsync();

        // Act
        var found = await _repo.GetByIdAsync(2);

        // Assert
        found.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_WhenMultipleCustomersExist_ShouldReturnCustomers()
    {
        // Arrange
        var c1 = new CustomerModel { CustomerId = 1, Username = "u1", Name = "n1", ContactInfo = "c1" };
        var c2 = new CustomerModel { CustomerId = 2, Username = "u2", Name = "n2", ContactInfo = "c2" };
        _context.customerTable.AddRange(c1, c2);
        await _context.SaveChangesAsync();

        // Act
        var found = await _repo.GetAllAsync();

        // Assert
        found.Should().NotBeNull();
        found.Should().HaveCount(2);
        found.Should().Contain(c => c.Username == "u1" && c.Name == "n1");
        found.Should().Contain(c => c.Username == "u2" && c.Name == "n2");
    }

    [Fact]
    public async Task GetAllAsync_WhenNoCustomersExist_ShouldReturnEmptyList()
    {
        // Act
        var found = await _repo.GetAllAsync();

        // Assert
        found.Should().NotBeNull();
        found.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByUsernameAsync_WhenCustomerExists_ShouldReturnCustomer()
    {
        // Arrange
        var customer = new CustomerModel { CustomerId = 1, Username = "exists", Name = "n1", ContactInfo = "c1" };
        _context.customerTable.Add(customer);
        await _context.SaveChangesAsync();

        // Act
        var found = await _repo.GetByUsernameAsync("exists");

        // Assert
        found.Should().NotBeNull();
        found!.Username.Should().Be("exists");
        found.CustomerId.Should().Be(1);
    }

    [Fact]
    public async Task UpdateCustomerAsync_WhenCustomerExists_ShouldReturnUpdatedCustomer()
    {
        // Arrange
        var existing = new CustomerModel { CustomerId = 5, Username = "u5", Name = "n5", ContactInfo = "c5" };
        _context.customerTable.Add(existing);
        await _context.SaveChangesAsync();

        var update = new CustomerModel { CustomerId = 5, Username = "u5_new", Name = "n5_new", ContactInfo = "c5_new" };

        // Act
        var result = await _repo.UpdateAsync(5, update);
        await _context.SaveChangesAsync();

        // Assert
        result.Should().NotBeNull();
        result!.CustomerId.Should().Be(5);
        result.Username.Should().Be("u5_new");
        result.Name.Should().Be("n5_new");
        result.ContactInfo.Should().Be("c5_new");
    }

    [Fact]
    public async Task UpdateCustomerAsync_WhenCustomerDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var update = new CustomerModel { CustomerId = 99, Username = "nope", Name = "none", ContactInfo = "none" };

        // Act
        var result = await _repo.UpdateAsync(99, update);
        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AddCustomerAsync_WhenDuplicateUsernameExists_ShouldReturnNull()
    {
        // Arrange
        var existing = new CustomerModel { CustomerId = 7, Username = "dup", Name = "n7", ContactInfo = "c7" };
        _context.customerTable.Add(existing);
        await _context.SaveChangesAsync();

        var toAdd = new CustomerModel { CustomerId = 8, Username = "dup", Name = "n8", ContactInfo = "c8" };

        // Act
        var added = await _repo.AddAsync(toAdd);
        // Assert
        added.Should().BeNull();
    }

    [Fact]
    public async Task AddCustomerAsync_WhenNew_ShouldAddAndBeRetrievableAfterSave()
    {
        // Arrange
        var toAdd = new CustomerModel { CustomerId = 12, Username = "newuser", Name = "new name", ContactInfo = "new contact" };

        // Act
        var added = await _repo.AddAsync(toAdd);
        await _context.SaveChangesAsync();
        var found = await _repo.GetByIdAsync(12);

        // Assert
        added.Should().NotBeNull();
        found.Should().NotBeNull();
        found!.Username.Should().Be("newuser");
    }
}
