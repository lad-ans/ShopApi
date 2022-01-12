using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

// BaseUrl: https://localhost:5001
// BaseUrl: http://localhost:5000

namespace Shop.Controllers
{
    [Route("v1/categories")]
    public class CategoryController : ControllerBase
    {
        // https://localhost:5001/categories

        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        [ResponseCache(VaryByHeader = "User-Agent", Location = ResponseCacheLocation.Any, Duration = 30)]
        // [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None, Duration = 0)]
        public async Task<ActionResult<List<Category>>> Get(
            [FromServices] DataContext context
        )
        {
            try
            {
                var categories = await context.Categories.AsNoTracking().ToListAsync();

                if (categories == null)
                    return NotFound(new { message = "Nenhuma categoria encontrada" });
                return Ok(categories);
            }
            catch
            {
                return BadRequest(new { message = "Ocorreu um erro ao fazer a requisição" });
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Category>> GetById(
            int id,
            [FromServices] DataContext context
        )
        {
            try
            {
                var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

                if (category == null)
                    return NotFound(new { message = "Nenhuma categoria encontrada" });
                return Ok(category);
            }
            catch
            {
                return BadRequest(new { message = "Ocorreu um erro ao fazer a requisição" });
            }

        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<List<Category>>> Post(
            [FromBody] Category model,
            [FromServices] DataContext context
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Categories.Add(model);
                await context.SaveChangesAsync();

                return Ok(model);
            }
            catch
            {
                return BadRequest(new { message = "Não foi possível criar a categoria" });
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<List<Category>>> Put(
            int id, [FromBody] Category model,
            [FromServices] DataContext context
        )
        {
            if (model.Id != id)
                return NotFound(new { message = "Categoria não encontrada" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Entry<Category>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();

                return Ok(model);
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "Este registro já foi atualizado" });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível atualizar a categoria" });
            }

        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<bool>> Delete(
            int id,
            [FromServices] DataContext context
        )
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

            if (category == null)
                return NotFound(new { message = "Categoria não encontrada" });

            try
            {
                context.Categories.Remove(category);
                await context.SaveChangesAsync();
                return Ok(true);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}