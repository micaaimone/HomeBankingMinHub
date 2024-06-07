

using HomeBankingMinHub.DTOs;
using HomeBankingMinHub.Models;
using HomeBankingMinHub.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace HomeBankingMinHub.Controllers
{
    //es como un mensajero, recibe una peticion y responde(devuelve algo o no)
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        //inyectamos repositorio
        private readonly IClientRepository _clientRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICardRepository _cardRepository;

        //constructor
        public ClientsController(IClientRepository clientRepository, IAccountRepository accountRepository, ICardRepository cardRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _cardRepository = cardRepository;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]

        //traer todos los clientes 
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
        [Authorize(Policy = "AdminOnly")]

        //traer a un cliente mediante su id
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
        //validar datos de un cliente 
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
        //crear un nuevo cliente
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

                //para crear su cuenta creo un user2 
                Client user2 = _clientRepository.FindByEmail(newClientDTO.Email);

                string numberRandom = "";
                //condicion de corte es cuando el nrRandom no exista
                do
                {
                    numberRandom = "VIN-" + RandomNumberGenerator.GetInt32(0, 99999999);
                } while (_accountRepository.GetAccountByNumber(numberRandom) != null);

                Account newAccount = new Account
                {
                    Balance = 0,
                    Number = numberRandom,
                    CreationDate = DateTime.Now,
                    ClientId = user2.Id
                };

                _accountRepository.Save(newAccount);

                return StatusCode(201, newClient);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        
        //esto dsps va a service
        private Client GetCurrentClient()
        {
            string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
            if (email.IsNullOrEmpty())
                throw new Exception("User not found");

            return _clientRepository.FindByEmail(email);
        }
        

        [HttpPost("current/accounts")]
        [Authorize(Policy = "ClientOnly")]
        //crearle una cuenta a un cliente autenticado
        public IActionResult CreateAccountToClientAuthenticated()
        {
            try
            {
                //traigo al cliente autenticado 
                Client clientAuthenticated = GetCurrentClient();

                //verificar que mi cliente tenga menos de 3 cuentas 


                //esto dsps va a service
                var accountsClient = clientAuthenticated.Accounts.Count();

                if (accountsClient < 3)
                {
                    string numberRandom = "";
                    //condicion de corte es cuando el nrRandom no exista
                    do
                    {
                        numberRandom = "VIN-" + RandomNumberGenerator.GetInt32(0, 99999999);
                    } while (_accountRepository.GetAccountByNumber(numberRandom) != null);

                    Account newAccount = new Account
                    {
                        Balance = 0,
                        Number = numberRandom,
                        CreationDate = DateTime.Now,
                        ClientId = clientAuthenticated.Id
                    };

                    _accountRepository.Save(newAccount);
                    return StatusCode(201, "Account created");
                } else
                {
                    return StatusCode(403, "prohibido");
                }

            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //Traer todas las cuentas del cliente autenticado - JSON con las cuentas de un cliente
        [HttpGet("current/accounts")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult GetAllAccountsByClient()
        {
            try
            {
                
                Client clientAuthenticated = GetCurrentClient();

                var accountsByClient = _accountRepository.GetAccountsByClient(clientAuthenticated.Id);

                var accountsDTO = accountsByClient.Select(a => new AccountClientDTO(a)).ToList();

                return Ok(accountsDTO);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        
        //esto dsps se va a service 
        private string CreateRandomNumberCard(Client client)
        {
            string newNumberCard = "";
            do
            {
                for (int i = 0; i < 4; i++)
                {
                    newNumberCard = newNumberCard + RandomNumberGenerator.GetInt32(1000, 9999);

                    if (i<3)
                    {
                        newNumberCard += "-";
                    }
                }
            } while (_cardRepository.GetCardByNumber(client.Id, newNumberCard) != null);
            
            return newNumberCard;
        }

        //esto dsps se va a service 
        private int CreateRandomCVV()
        {
            return RandomNumberGenerator.GetInt32(100, 999);
        }
        

        [HttpPost("current/cards")]
        [Authorize(Policy = "ClientOnly")]

        //crear una nueva tarjeta para un cliente autenticado
        public IActionResult CreateANewCardForClientAuthenticated([FromBody] NewCardDTO newCardDTO)
        {
            try
            {
                //traigo al cliente autenticado 
                Client clientAuthenticated = GetCurrentClient();

                var card = _cardRepository.GetAllCardsByType(clientAuthenticated.Id, newCardDTO.Type);

                if (card.Count() < 3)
                {
                    if (card.Any(c => c.Color == newCardDTO.Color))
                    {
                        return StatusCode(403, "prohibido, ya tiene ese color de tarjeta");
                    } else
                    {

                        Card newCard = new Card
                        {
                            ClientId = clientAuthenticated.Id,
                            CardHolder = clientAuthenticated.FirstName + " " + clientAuthenticated.LastName,
                            Type = newCardDTO.Type,
                            Color = newCardDTO.Color,
                            Number = CreateRandomNumberCard(clientAuthenticated),
                            Cvv = CreateRandomCVV(),
                            FromDate = DateTime.Now,
                            ThruDate = DateTime.Now.AddYears(5),
                        };
                        _cardRepository.AddCard(newCard);
                    }
                    return StatusCode(201, "Card created");
                }
                else
                {
                    return StatusCode(403, "prohibido, ya tiene la maxima cantidad de tarjetas");
                }
            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message);

            }

        }

        [HttpGet("current/cards")]
        [Authorize(Policy = "ClientOnly")]
        //traer todas las cards de mi cliente 

        public IActionResult GetAllCardsClient()
        {
            try
            {
                Client clientCurrent = GetCurrentClient();

                var cardsByClient = _cardRepository.GetAllCardsByClient(clientCurrent.Id);

                var cardDTO = cardsByClient.Select(c => new CardDTO(c)).ToList();

                return Ok(cardDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);

            }

        }
    }
}