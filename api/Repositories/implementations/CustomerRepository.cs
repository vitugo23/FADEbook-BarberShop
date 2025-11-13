using Microsoft.EntityFrameworkCore;
using Fadebook.Models;
using Fadebook.DB;

namespace Fadebook.Repositories;

public class CustomerRepository(
    FadebookDbContext _fadebookDbContext
    ) : ICustomerRepository
{
    //find customer by id
    public async Task<CustomerModel?> GetByIdAsync(int id)
    {
        return await _fadebookDbContext.customerTable.FindAsync(id);
    }

    //get all customers
    public async Task<IEnumerable<CustomerModel>> GetAllAsync()
    {
        return await _fadebookDbContext.customerTable.ToListAsync();
    }

    // TODO: Add Customer

    //find customer by username
    public async Task<CustomerModel?> GetByUsernameAsync(string username)
    {
        return await _fadebookDbContext.customerTable.Where(c => c.Username == username).FirstOrDefaultAsync();
    }

    public async Task<CustomerModel> UpdateAsync(int customerId, CustomerModel customer)
    {
        var foundCustomerModel = await GetByIdAsync(customerId);
        if (foundCustomerModel is null)
            return null!;
        if (customer.Username != null && foundCustomerModel.Username != customer.Username)
        {
            var usernameCustomerModel = await GetByUsernameAsync(customer.Username);
            if (usernameCustomerModel != null)
                return null!; // conflict
        }
        // foundCustomerModel.Update(customer);
        foundCustomerModel.Username = customer.Username;
        foundCustomerModel.Name = customer.Name;
        foundCustomerModel.ContactInfo = customer.ContactInfo;
        _fadebookDbContext.customerTable.Update(foundCustomerModel);
        return foundCustomerModel;
    }

    public async Task<CustomerModel> AddAsync(CustomerModel customer)
    {
        var usernameCustomerModel = await GetByUsernameAsync(customer.Username);
        if (usernameCustomerModel != null)
            return null!; // conflict
        await _fadebookDbContext.customerTable.AddAsync(customer);
        return customer;
    }
}
