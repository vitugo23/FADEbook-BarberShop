using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Fadebook.Models;
using Fadebook.Services;
using AutoMapper;
using Fadebook.DTOs;

namespace Fadebook.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // "/students
    public class BarberController : ControllerBase
    {
        // Fields
        private readonly IBarberManagementService _service;
        private readonly IMapper _mapper;

        public BarberController(IBarberManagementService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BarberDto>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<BarberDto>>(result);
            return Ok(dtos);
        }

        // GET: api/barber/{id}
        [HttpGet("{id:int}", Name = "GetBarberById")]
        public async Task<ActionResult<BarberDto>> GetById([FromRoute] int id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(_mapper.Map<BarberDto>(result));
        }

        // POST: api/barber
        [HttpPost]
        public async Task<ActionResult<BarberDto>> Create([FromBody] CreateBarberDto dto)
        {
            var model = _mapper.Map<BarberModel>(dto);
            var created = await _service.AddBarberWithServicesAsync(model, dto.ServiceIds);
            var createdDto = _mapper.Map<BarberDto>(created);
            return Created($"api/barber/{created.BarberId}", createdDto); 
        }

        // PUT: api/barber/{id}
        [HttpPut("{id:int}")]
        public async Task<ActionResult<BarberDto>> Update([FromRoute] int id, [FromBody] BarberDto dto)
        {
            var model = _mapper.Map<BarberModel>(dto);
            var updated = await _service.UpdateAsync(id, model);
            return Ok(_mapper.Map<BarberDto>(updated));
        }

        // DELETE: api/barber/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await _service.DeleteByIdAsync(id);
            return NoContent();
        }

        // GET: api/barber/{id}/services
        [HttpGet("{id:int}/services")]
        public async Task<ActionResult<IEnumerable<ServiceDto>>> GetBarberServices([FromRoute] int id)
        {
            var barber = await _service.GetByIdAsync(id);
            var services = await _service.GetBarberServicesAsync(id);
            var serviceDtos = _mapper.Map<IEnumerable<ServiceDto>>(services);
            return Ok(serviceDtos);
        }

        // PUT: api/barber/{id}/services
        [HttpPut("{id:int}/services")]
        public async Task<IActionResult> UpdateServices([FromRoute] int id, [FromBody] List<int> serviceIds)
        {
            if (serviceIds is null || !serviceIds.Any()) 
                return BadRequest(new { message = "Service IDs are required." });

            await _service.UpdateBarberServicesAsync(id, serviceIds);
            return NoContent();
        }


        
    }
}