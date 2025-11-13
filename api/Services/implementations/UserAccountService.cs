
using Fadebook.DB;
using Fadebook.Models;
using Fadebook.Repositories;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using Fadebook.Exceptions;

namespace Fadebook.Services;

public class UserAccountService(
    IDbTransactionContext _dbTransactionContext,
    ICustomerRepository _customerRepository
    ) : IUserAccountService
{
    public async Task<CustomerModel> LoginAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new BadRequestException("Username is required.");
        var foundCustomer = await _customerRepository.GetByUsernameAsync(username);
        if (foundCustomer is null)
            throw new NotFoundException($"Customer with username {username} does not exist");
        return foundCustomer;
    }

    public async Task<bool> CheckIfUsernameExistsAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new BadRequestException("Username is required.");
        var foundCustomer = await _customerRepository.GetByUsernameAsync(username);
        return (foundCustomer is not null);
    }

    public async Task<CustomerModel> SignUpCustomerAsync(CustomerModel customerModel)
    {
        if (!customerModel.AreAllValuesNotNull(ignorePrimaryKey: true))
            throw new ValidationException($"Given customer model is incomplete: \n {customerModel.ToJson()}");
        var foundCustomer = await _customerRepository.GetByUsernameAsync(customerModel.Username);
        if (foundCustomer is not null)
            throw new InvalidOperationException($"Customer with username \"{customerModel.Username}\" already exists");
        try
        {
            await _customerRepository.AddAsync(customerModel);
            await _dbTransactionContext.SaveChangesAsync();
            return customerModel;
        }
        catch
        {
            throw;
        }
    }

    public async Task<CustomerModel> GetCustomerByIdAsync(int customerId)
    {
        var customer = await _customerRepository.GetByIdAsync(customerId);
        if (customer is null)
            throw new NotFoundException($"Customer with id \"{customerId}\" does not exists");
        return customer;
    }

    public async Task<IEnumerable<CustomerModel>> GetAllCustomersAsync()
    {
        return await _customerRepository.GetAllAsync();
    }
}
