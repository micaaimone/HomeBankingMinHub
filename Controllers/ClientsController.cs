using HomeBankingMinHub.DTOs;
using HomeBankingMinHub.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeBankingMinHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;

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
    }
}
