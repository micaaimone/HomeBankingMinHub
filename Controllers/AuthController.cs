using HomeBankingMinHub.Models;
using HomeBankingMinHub.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HomeBankingMinHub.DTOs;

namespace HomeBankingMinHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        //inyectamos repositorio
        private readonly IClientRepository _clientRepository;
        //constructor
        public AuthController(IClientRepository clientRepository)

        {

            _clientRepository = clientRepository;

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] ClientLoginDTO clientLoginDTO)
        {
            try
            {
                //verifica que exista el usuario
                Client user = _clientRepository.FindByEmail(clientLoginDTO.Email);
                if (user == null)
                    return StatusCode(403, "User not found");
                if(!String.Equals(user.Password, clientLoginDTO.Password))
                    return StatusCode(403, "Invalid credentials");

                

                //creo la claim (informacion extra que puedo agregar dentro de una cookie)
                var claims = new List<Claim>
                {
                    //clave, valor
                    new Claim("Client", user.Email),
                };

                if (user.Email == "mica@gmail.com")
                {
                    claims.Add(new Claim("Admin", "true"));
                }

                //definimos una identidad de usuario, esta es la identidad y las reclamaciones 
                var claimsIdentity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme
                    );

                //retorno la cookie al navegador
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                return Ok();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
