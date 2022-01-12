using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
    [Route("v1")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<dynamic>> Get(
            [FromServices] DataContext context
        )
        {
            var employee = new User
            {
                Id = 1,
                Username = "robin",
                Role = "employee",
                Password = "1234567"
            };

            var manager = new User
            {
                Id = 2,
                Username = "batman",
                Role = "manager",
                Password = "1234567"
            };

            var category = new Category
            {
                Id = 1,
                Title = "Categoria do produto"
            };

            var product = new Product
            {
                Id = 1,
                Title = "Título do produto",
                Price = 299,
                Category = category,
                Description = "Uma descrição do produto"
            };

            try
            {
                context.Users.Add(employee);
                context.Users.Add(manager);
                context.Categories.Add(category);
                context.Products.Add(product);

                await context.SaveChangesAsync();

                return Ok(new { message = "Dados inseridos com sucesso!" });
            }
            catch (System.Exception)
            {
                return BadRequest();
            }
        }

    }
}