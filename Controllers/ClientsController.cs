using HomeBankingMinHub.DTOs;
using HomeBankingMinHub.Models;
using HomeBankingMinHub.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace HomeBankingMinHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        //inyectamos repositorio
        private readonly IClientRepository _clientRepository;
        //constructor
        public ClientsController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        [HttpGet]

        public IActionResult GetAllClients()
        {
            try
            {
                var clients = _clientRepository.GetAllClients();
                //creo clientes dto para evitar recursividad
                var clientsDTO = clients.Select(c => new ClientDTO(c)).ToList();

                return Ok(clientsDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetClientsById(long id)
        {
            try
            {
                var client = _clientRepository.FindById(id);
                if (client == null)
                {
                    return Forbid();
                } 
                var clientDTO = new ClientDTO(client);
                return Ok(clientDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("current")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult GetCurrent()
        {
            try
            {
                //if ternario 
                //User es un objeto con metodo para poder sacar un claim, si es != null entonces agarro el valor 
                //si no encuentra nada devuelve nulo
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email.IsNullOrEmpty())
                    return Forbid();

                Client client = _clientRepository.FindByEmail(email);

                if (client == null)
                    return Forbid();
                var clientDTO = new ClientDTO(client);

                return Ok(clientDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] NewClientDTO newClientDTO)
        {
            try
            {
                //validamos datos antes
                if (String.IsNullOrEmpty(newClientDTO.Email) || String.IsNullOrEmpty(newClientDTO.Password) || String.IsNullOrEmpty(newClientDTO.FirstName) || String.IsNullOrEmpty(newClientDTO.LastName))
                    return StatusCode(403, "datos inválidos");

                //buscamos si ya existe el usuario
                Client user = _clientRepository.FindByEmail(newClientDTO.Email);
                if (user != null)
                    return StatusCode(403, "Email está en uso");

                Client newClient = new Client
                {
                    Email = newClientDTO.Email,
                    Password = newClientDTO.Password,
                    FirstName = newClientDTO.FirstName,
                    LastName = newClientDTO.LastName,
                };

                _clientRepository.Save(newClient);
                return StatusCode(201, newClient);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
