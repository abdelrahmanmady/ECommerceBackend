using Microsoft.AspNetCore.Mvc;
using MyApp.API.DTOs.Categories;
using MyApp.API.Interfaces;

namespace MyApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController(ICategoryService categories) : ControllerBase
    {
        private readonly ICategoryService _categories = categories;


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _categories.GetAllAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var category = await _categories.GetByIdAsync(id);
            if (category is null)
                return NotFound();
            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
        {
            var createdCategory = await _categories.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdCategory.Id }, createdCategory);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCategoryDto dto)
        {
            var updatedCategory = await _categories.UpdateAsync(id, dto);
            if (updatedCategory is null)
                return NotFound();
            return Ok(updatedCategory);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var isDeleted = await _categories.DeleteAsync(id);
            if (!isDeleted)
                return NotFound();
            return NoContent();
        }
    }
}
