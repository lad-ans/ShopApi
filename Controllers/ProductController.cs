using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;


namespace Shop.Controllers
{
    [Route("v1/products")]
    public class ProductController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<List<Product>>> Get(
            [FromServices] DataContext context
        )
        {
            try
            {
                var products = await context.Products.Include(x => x.Category).AsNoTracking().ToListAsync();

                if (products == null)
                    return NoContent();

                return Ok(products);
            }
            catch (System.Exception)
            {
                return BadRequest(new { message = "Ocorreu um erro ao obter produtos" });
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<ActionResult<Product>> Get(
            int id,
            [FromServices] DataContext context
        )
        {
            try
            {
                var product = await context.Products
                    .Include(x => x.Category)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (product == null)
                    return NoContent();

                return Ok(product);
            }
            catch (System.Exception)
            {
                return BadRequest(new { message = "Ocorreu um erro ao obter produto" });
            }
        }

        [HttpGet]
        [Route("categories/{id:int}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<List<Product>>> GetByCategory(
            int id,
            [FromServices] DataContext context
        )
        {
            try
            {
                var products = await context.Products
                    .Include(x => x.Category)
                    .AsNoTracking()
                    .Where(x => x.CategoryId == id).ToListAsync();

                if (products == null)
                    return NoContent();

                return Ok(products);
            }
            catch (System.Exception)
            {
                return BadRequest(new { message = "Ocorreu um erro ao obter produto" });
            }
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Product>> Post(
          [FromServices] DataContext context,
          [FromBody] Product model
      )
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                context.Products.Add(model);
                await context.SaveChangesAsync();

                return Ok(model);
            }
            catch (System.Exception)
            {
                return BadRequest(new { message = "Ocorreu um erro ao obter produto" });
            }
        }
    }
}