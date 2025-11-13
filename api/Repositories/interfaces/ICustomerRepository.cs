
using Fadebook.DB;
using Fadebook.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fadebook.Repositories;

public interface ICustomerRepository
{
    Task<CustomerModel?> GetByIdAsync(int id);
    Task<IEnumerable<CustomerModel>> GetAllAsync();
    Task<CustomerModel?> GetByUsernameAsync(string username);
    Task<CustomerModel> UpdateAsync(int customerId, CustomerModel customer);
    Task<CustomerModel> AddAsync(CustomerModel customer);
}