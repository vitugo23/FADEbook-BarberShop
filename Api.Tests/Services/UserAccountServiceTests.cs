using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using Fadebook.Models;
using Fadebook.Repositories;
using Fadebook.Services;
using Fadebook.Exceptions;

namespace Api.Tests.Services;

public class UserAccountServiceTests
{
    private readonly Mock<IDbTransactionContext> _mockDbTransactionContext;
    private readonly Mock<ICustomerRepository> _mockCustomerRepository;
    private readonly UserAccountService _service;

    public UserAccountServiceTests()
    {
        _mockDbTransactionContext = new Mock<IDbTransactionContext>();
        _mockCustomerRepository = new Mock<ICustomerRepository>();
        _service = new UserAccountService(
            _mockDbTransactionContext.Object,
            _mockCustomerRepository.Object
        );
    }

    [Fact]
    public async Task LoginAsync_ReturnsCustomer_WhenUsernameExists()
    {
        // Arrange
        var customer = new CustomerModel
        {
            CustomerId = 1,
            Username = "customer1",
            Name = "John Customer",
            ContactInfo = "555-0201"
        };
        _mockCustomerRepository.Setup(r => r.GetByUsernameAsync("customer1")).ReturnsAsync(customer);

        // Act
        var result = await _service.LoginAsync("customer1");

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(customer);
        _mockCustomerRepository.Verify(r => r.GetByUsernameAsync("customer1"), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_ThrowsNotFoundException_WhenUsernameDoesNotExist()
    {
        // Arrange
        _mockCustomerRepository.Setup(r => r.GetByUsernameAsync("nonexistent")).ReturnsAsync((CustomerModel)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.LoginAsync("nonexistent"));
    }

    [Fact]
    public async Task LoginAsync_ThrowsBadRequestException_WhenUsernameBlank()
    {
        await Assert.ThrowsAsync<BadRequestException>(() => _service.LoginAsync(" "));
        await Assert.ThrowsAsync<BadRequestException>(() => _service.LoginAsync(string.Empty));
    }

    [Fact]
    public async Task CheckIfUsernameExistsAsync_ReturnsTrue_WhenUsernameExists()
    {
        // Arrange
        var customer = new CustomerModel { CustomerId = 1, Username = "customer1" };
        _mockCustomerRepository.Setup(r => r.GetByUsernameAsync("customer1")).ReturnsAsync(customer);

        // Act
        var result = await _service.CheckIfUsernameExistsAsync("customer1");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CheckIfUsernameExistsAsync_ReturnsFalse_WhenUsernameDoesNotExist()
    {
        // Arrange
        _mockCustomerRepository.Setup(r => r.GetByUsernameAsync("nonexistent")).ReturnsAsync((CustomerModel)null);

        // Act
        var result = await _service.CheckIfUsernameExistsAsync("nonexistent");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CheckIfUsernameExistsAsync_ThrowsBadRequestException_WhenUsernameBlank()
    {
        await Assert.ThrowsAsync<BadRequestException>(() => _service.CheckIfUsernameExistsAsync(""));
    }

    [Fact]
    public async Task SignUpCustomerAsync_ThrowsValidationException_WhenCustomerIsIncomplete()
    {
        // Arrange
        var incompleteCustomer = new CustomerModel
        {
            Username = "customer2"
            // Missing Name and ContactInfo
        };

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _service.SignUpCustomerAsync(incompleteCustomer));
    }

    [Fact]
    public async Task GetCustomerByIdAsync_ReturnsCustomer_WhenCustomerExists()
    {
        // Arrange
        var customer = new CustomerModel
        {
            CustomerId = 1,
            Username = "customer1",
            Name = "John Customer",
            ContactInfo = "555-0201"
        };
        _mockCustomerRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(customer);

        // Act
        var result = await _service.GetCustomerByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(customer);
    }

    [Fact]
    public async Task GetCustomerByIdAsync_ThrowsNotFoundException_WhenCustomerDoesNotExist()
    {
        // Arrange
        _mockCustomerRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((CustomerModel)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetCustomerByIdAsync(1));
    }
}
