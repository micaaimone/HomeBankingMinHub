using HomeBankingMinHub.DTOs;
using HomeBankingMinHub.Models;
using HomeBankingMinHub.Repositories;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using Microsoft.IdentityModel.Tokens;
using System.Reflection.Metadata;

namespace HomeBankingMinHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        //inyectamos repositorio
        private readonly IClientRepository _clientRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;

        //constructor
        public TransactionsController(IClientRepository clientRepository, IAccountRepository accountRepository, ITransactionRepository transactionRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
        }

        /* 
        - [*] Debe recibir el monto, la descripción, número de cuenta de origen y número de cuenta de destino como parámetros de solicitud
        - [*] Verificar que los parámetros no estén vacíos
        - [*] Verificar que los números de cuenta no sean iguales
        - [*] Verificar que exista la cuenta de origen
        - [*] Verificar que la cuenta de origen pertenezca al cliente autenticado
        - [*] Verificar que exista la cuenta de destino
        - [*] Verificar que la cuenta de origen tenga el monto disponible.
        - [ ] Se deben crear dos transacciones, una con el tipo de transacción “DEBIT” asociada a la cuenta de origen y la otra con el tipo de transacción “CREDIT” asociada a la cuenta de destino.
        - [ ] A la cuenta de origen se le restará el monto indicado en la petición y a la cuenta de destino se le sumará el mismo monto.
        */

        [HttpPost]
        //- [*] metodo que reciba de parametro el transferDTO
        public IActionResult NewTransaction(TransferDTO transferDTO)
        {
            try
            {
                //valido q sea un cliente autenticado 
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email.IsNullOrEmpty())
                    return Forbid();

                Client client = _clientRepository.FindByEmail(email);

                if (client == null)
                {
                    return StatusCode(403, "El cliente no se encontro");
                }
                //Verificar que los parámetros no estén vacíos
                if (transferDTO.ToAccountNumber == null || transferDTO.FromAccountNumber == null || transferDTO.Amount <= 0 || transferDTO.Description == null)
                {
                    return StatusCode(403, "Alguno de los datos esta mal ingresado");
                }
                // Verificar que los números de cuenta no sean iguales
                if (transferDTO.FromAccountNumber == transferDTO.ToAccountNumber)
                {
                    return StatusCode(403, "Las cuentas son iguales");
                }
                //Verificar que exista la cuenta de origen
                var fromAccount = _accountRepository.GetAccountByNumber(transferDTO.FromAccountNumber);
                if(fromAccount == null)
                {
                    return StatusCode(403, "La cuenta de origen no existe");
                }
                //Verificar que la cuenta de origen pertenezca al cliente autenticado
                if (client.Id != fromAccount.ClientId)
                {
                    return StatusCode(403, "La cuenta de origen no pertenece al cliente autenticado");
                }
                //Verificar que exista la cuenta de destino
                var toAccount = _accountRepository.GetAccountByNumber(transferDTO.ToAccountNumber);
                if (toAccount == null)
                {
                    return StatusCode(403, "La cuenta de destino no existe");
                }
                //Verificar que la cuenta de origen tenga el monto disponible.
                if (fromAccount.Balance<transferDTO.Amount)
                {
                    return StatusCode(403, "La cuenta de origen no cuenta con el monto necesario");
                }

                //Se deben crear dos transacciones,
                //una con el tipo de transacción “DEBIT” asociada a la cuenta de origen 
                //la otra con el tipo de transacción “CREDIT” asociada a la cuenta de destino.

                Transaction debitTransaction = new Transaction
                {
                    AccountId = fromAccount.Id,
                    Type = TransactionType.DEBIT,
                    Amount = -transferDTO.Amount,
                    Description = transferDTO.Description,
                    Date = DateTime.Now
                };

                Transaction creditTransaction = new Transaction
                {
                    AccountId = toAccount.Id,
                    Type = TransactionType.CREDIT,
                    Amount = transferDTO.Amount,
                    Description = transferDTO.Description,
                    Date = DateTime.Now
                };

                //guardo las transacciones 
                _transactionRepository.Save(debitTransaction);
                _transactionRepository.Save(creditTransaction);

                //actualizar balance de cuenta
                toAccount.Balance += transferDTO.Amount;
                fromAccount.Balance -= transferDTO.Amount;

                _accountRepository.UpdateAccount(toAccount);
                _accountRepository.UpdateAccount(fromAccount);
                

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
