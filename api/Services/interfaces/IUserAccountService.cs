
using Fadebook.DB;
using Fadebook.Models;
using Fadebook.Repositories;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace Fadebook.Services;

public interface IUserAccountService
{
    Task<CustomerModel> LoginAsync(string username);
    Task<bool> CheckIfUsernameExistsAsync(string username);
    Task<CustomerModel> SignUpCustomerAsync(CustomerModel customerModel);
    Task<CustomerModel> GetCustomerByIdAsync(int customerId);
    Task<IEnumerable<CustomerModel>> GetAllCustomersAsync();
}
