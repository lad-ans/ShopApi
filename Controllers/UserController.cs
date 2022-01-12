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
    [Route("v1/users")]
    public class UserController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<List<User>>> Get(
            [FromServices] DataContext context
        )
        {

            try
            {
                var users = await context.Users.AsNoTracking().ToListAsync();

                if (users == null)
                    return NotFound(new { message = "Usuários não encontrados" });


                return Ok(users);
            }
            catch (System.Exception)
            {
                return BadRequest(new { message = "Não foi possível obter usuários" });
            }
        }

        [HttpPost]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<User>> Post(
            [FromBody] User model,
            [FromServices] DataContext context
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // force user to be "employee"
                model.Role = "employee";

                context.Users.Add(model);
                await context.SaveChangesAsync();

                // hide password
                model.Password = "";

                return Ok(model);
            }
            catch (System.Exception)
            {
                return BadRequest(new { message = "Não foi possível criar o usuário" });
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<dynamic>> Authenticate(
            [FromBody] User model,
            [FromServices] DataContext context
        )
        {

            try
            {
                var user = await context.Users
                    .AsNoTracking()
                    .Where(x => x.Username == model.Username && x.Password == model.Password)
                    .FirstOrDefaultAsync();

                if (user == null)
                    return NotFound(new { message = "Usuário ou senha inválidos" });

                var token = Services.TokenService.GenerateToken(user);

                // hide password
                user.Password = "";

                return Ok(
                    new
                    {
                        user = user,
                        token = token
                    }
                );
            }
            catch (System.Exception)
            {
                return BadRequest(new { message = "Não foi possível criar o usuário" });
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Put(
            [FromBody] User model,
            int id,
            [FromServices] DataContext context
        )
        {

            if (!ModelState.IsValid)
                return NotFound(ModelState);

            if (id != model.Id)
                return NotFound(new { message = "Usuário não encontrado" });

            try
            {
                var user = context.Entry(model).State = EntityState.Modified;
                await context.SaveChangesAsync();

                return Ok(model);
            }
            catch (System.Exception)
            {
                return BadRequest(new { message = "Não foi possível atualizar o usuário" });
            }
        }
    }

}