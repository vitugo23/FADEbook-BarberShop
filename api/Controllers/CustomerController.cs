using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Fadebook.Models;
using Fadebook.Services;
using Fadebook.DTOs;
using AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace Fadebook.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // /api/customer
    public class CustomerController : ControllerBase
    {
        // Fields
        private readonly ICustomerAppointmentService _customerAppointmentService;
        private readonly IUserAccountService _userAccountService;
        private readonly IMapper _mapper;

        // Constructor
        public CustomerController(ICustomerAppointmentService service, IUserAccountService userAccountService, IMapper mapper)
        {
            _customerAppointmentService = service;
            _userAccountService = userAccountService;
            _mapper = mapper;
        }

    
        // GET: api/customer/customers (for admin)
        [HttpGet("customers")]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAllCustomers()
        {
            var customers = await _userAccountService.GetAllCustomersAsync();
            var dtos = _mapper.Map<IEnumerable<CustomerDto>>(customers);
            return Ok(dtos);
        }

        // GET: /customer/{id}
        [HttpGet("/customer/{id}", Name = "GetCustomerById")]
        public async Task<ActionResult<CustomerDto>> GetById(int id)
        {
            var customer = await _userAccountService.GetCustomerByIdAsync(id);
            return Ok(_mapper.Map<CustomerDto>(customer));
        }

        // GET: api/customer/services
        [HttpGet("services")]
        public async Task<ActionResult<IEnumerable<ServiceDto>>> GetServices()
        {
            var services = await _customerAppointmentService.ListAvailableServicesAsync();
            var dtos = _mapper.Map<IEnumerable<ServiceDto>>(services);
            return Ok(dtos);
        }

        // GET: api/customer/barbers-by-service/{serviceId}
        [HttpGet("barbers-by-service/{serviceId:int}")]
        public async Task<ActionResult<IEnumerable<BarberDto>>> GetBarbersByService([FromRoute] int serviceId)
        {
            var barbers = await _customerAppointmentService.ListAvailableBarbersByServiceAsync(serviceId);
            var dtos = _mapper.Map<IEnumerable<BarberDto>>(barbers);
            return Ok(dtos);
        }

        // POST: api/customer/request-appointment
        [HttpPost("request-appointment")]
        public async Task<ActionResult<AppointmentDto>> RequestAppointment([FromBody] AppointmentRequestDto request)
        {
            var customer = _mapper.Map<CustomerModel>(request.Customer);
            var appointment = _mapper.Map<AppointmentModel>(request.Appointment);
            var created = await _customerAppointmentService.MakeAppointmentAsync(appointment);
            var dto = _mapper.Map<AppointmentDto>(created);
            return Created($"/api/appointment/{created.AppointmentId}", dto);
        }


    }
}