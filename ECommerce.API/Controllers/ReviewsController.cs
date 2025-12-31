using ECommerce.Business.DTOs.Errors;
using ECommerce.Business.DTOs.Reviews.Requests;
using ECommerce.Business.DTOs.Reviews.Responses;
using ECommerce.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Tags("Reviews Management")]
    public class ReviewsController(IReviewService reviews) : ControllerBase
    {
        private readonly IReviewService _reviews = reviews;

        [Authorize]
        [HttpGet]
        [EndpointSummary("Retrieves logged in user all reviews.")]
        [ProducesResponseType(typeof(IEnumerable<ReviewSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllReviews([FromQuery] string? rating)
        {
            var reviews = await _reviews.GetAllReviewsAsync(rating);
            return Ok(reviews);
        }

        [HttpGet("{productId:int}")]
        [EndpointSummary("Retrieves all reviews of a product.")]
        [ProducesResponseType(typeof(IEnumerable<ReviewProductSummaryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllProductReviews([FromRoute] int productId)
        {
            var reviews = await _reviews.GetAllProductReviewsAsync(productId);
            return Ok(reviews);
        }

        [Authorize]
        [HttpPut("{reviewId:int}/helpful")]
        [EndpointSummary("Sets another user's review as helpful.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MarkHelpful([FromRoute] int reviewId)
        {
            await _reviews.MarkHelpfulAsync(reviewId);
            return Ok();
        }

        [Authorize]
        [HttpPost("{productId:int}")]
        [EndpointSummary("Posts a review on a product.")]
        [ProducesResponseType(typeof(ReviewProductSummaryDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddReview([FromRoute] int productId, [FromBody] AddReviewRequest request)
        {
            var createdReview = await _reviews.AddReviewAsync(productId, request);
            return StatusCode(StatusCodes.Status201Created, createdReview);
        }

        [Authorize]
        [HttpPut("{reviewId:int}")]
        [EndpointSummary("Updates a posted review on a product.")]
        [ProducesResponseType(typeof(ReviewSummaryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateReview([FromRoute] int reviewId, [FromBody] UpdateReviewRequest request)
        {
            var updatedReview = await _reviews.UpdateReviewAsync(reviewId, request);
            return Ok(updatedReview);
        }

        [Authorize]
        [HttpDelete("{reviewId:int}")]
        [EndpointSummary("Deletes a posted review.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteReview([FromRoute] int reviewId)
        {
            await _reviews.DeleteReviewAsync(reviewId);
            return NoContent();
        }

    }
}
