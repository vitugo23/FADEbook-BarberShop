using Microsoft.AspNetCore.Mvc;
using Fadebook.Models;
using Fadebook.Services;
using Fadebook.DTOs;
using AutoMapper;

namespace Fadebook.Controllers;

[ApiController]
[Route("api/[controller]")]
// api/service
public class ServiceController : ControllerBase
{
    private readonly IServiceManagementService _serviceService;
    private readonly IMapper _mapper;

    public ServiceController(
        IServiceManagementService serviceService,
        IMapper mapper)
    {
        _serviceService = serviceService;
        _mapper = mapper;
    }

    // GET: api/service
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ServiceDto>>> GetAll()
    {
        var services = await _serviceService.GetAllAsync();
        var dtos = _mapper.Map<IEnumerable<ServiceDto>>(services);
        return Ok(dtos);
    }

    // GET: api/service/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ServiceDto>> GetById([FromRoute] int id)
    {
        var service = await _serviceService.GetByIdAsync(id);
        var dto = _mapper.Map<ServiceDto>(service);
        return Ok(dto);
    }

    // POST: api/service
    [HttpPost]
    public async Task<ActionResult<ServiceDto>> Create([FromBody] ServiceDto serviceDto)
    {
        var service = _mapper.Map<ServiceModel>(serviceDto);
        var created = await _serviceService.CreateAsync(service);
        var dto = _mapper.Map<ServiceDto>(created);
        return Created($"/api/service/{created.ServiceId}", dto);
    }

    // DELETE: api/service/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        await _serviceService.DeleteAsync(id);
        return NoContent();
    }
}
