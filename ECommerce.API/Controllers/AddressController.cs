using ECommerce.Business.DTOs.Addresses;
using ECommerce.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Tags("Addresses Managment")]
    public class AddressController(IAddressService addresses) : ControllerBase
    {
        private readonly IAddressService _addresses = addresses;

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _addresses.GetAllAsync());

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
            => Ok(await _addresses.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAddressDto dto)
        {
            var createdAddress = await _addresses.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdAddress.Id }, createdAddress);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateAddressDto dto)
        {
            var updatedAddress = await _addresses.UpdateAsync(id, dto);
            return Ok(updatedAddress);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await _addresses.DeleteAsync(id);
            return NoContent();
        }

    }
}
