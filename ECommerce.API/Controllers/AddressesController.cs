using ECommerce.Business.DTOs.Addresses.Requests;
using ECommerce.Business.DTOs.Addresses.Responses;
using ECommerce.Business.DTOs.Errors;
using ECommerce.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Tags("Addresses Managment")]
    public class AddressesController(IAddressService addresses) : ControllerBase
    {
        private readonly IAddressService _addresses = addresses;

        [HttpGet]
        [EndpointSummary("Get all saved addresses of current logged in user.")]
        [ProducesResponseType(typeof(IEnumerable<AddressSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllAddresses()
        {
            var addresses = await _addresses.GetAllAddressesAsync();
            return Ok(addresses);
        }

        [HttpPost]
        [EndpointSummary("Adds a new saved address.")]
        [ProducesResponseType(typeof(AddressSummaryDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateAddress([FromBody] CreateAddressRequest createAddressRequest)
        {
            var addressCreated = await _addresses.CreateAddressAsync(createAddressRequest);
            return StatusCode(StatusCodes.Status201Created, addressCreated);
        }

        [HttpPut("{addressId:int}")]
        [EndpointSummary("Updates an existing address details.")]
        [ProducesResponseType(typeof(AddressSummaryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAddress([FromRoute] int addressId, [FromBody] UpdateAddressRequest updateAddressRequest)
        {
            var addressUpdated = await _addresses.UpdateAddressAsync(addressId, updateAddressRequest);
            return Ok(addressUpdated);
        }

        [HttpPut("{addressId:int}/set-default")]
        [EndpointSummary("Sets new address as default one.")]
        [ProducesResponseType(typeof(IEnumerable<AddressSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetDefault([FromRoute] int addressId)
        {
            var addresses = await _addresses.SetDefaultAsync(addressId);
            return Ok(addresses);
        }

        [HttpDelete("{addressId:int}")]
        [EndpointSummary("Deletes existing address if not set as default.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteAddress([FromRoute] int addressId)
        {
            await _addresses.DeleteAddressAsync(addressId);
            return NoContent();
        }
    }
}
