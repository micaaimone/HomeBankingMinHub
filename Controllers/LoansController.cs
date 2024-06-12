using HomeBankingMinHub.DTOs;
using HomeBankingMinHub.Models;
using HomeBankingMinHub.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace HomeBanking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;
        private readonly IClientLoanRepository _clientLoanRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;

        public LoansController(
            IClientRepository clientRepository,
            IClientLoanRepository clientLoanRepository,
            ILoanRepository loanRepository,
            IAccountRepository accountRepository,
            ITransactionRepository transactionRepository
        )
        {
            _clientRepository = clientRepository;
            _clientLoanRepository = clientLoanRepository;
            _loanRepository = loanRepository;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
        }
        private Client GetCurrentClient()
        {
            try
            {
                // Obtener el claim de email del usuario
                string email = User.FindFirst("Client")?.Value ?? string.Empty;
                if (string.IsNullOrEmpty(email))
                {
                    throw new UnauthorizedAccessException("Usuario no encontrado o no autenticado");
                }

                // Buscar el cliente por email
                Client client = _clientRepository.FindByEmail(email);
                if (client == null)
                {
                    throw new UnauthorizedAccessException("Cliente no encontrado");
                }

                return client;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el cliente: " + ex.Message);
            }
        }


    

        // GET /api/loans
        [HttpGet]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult GetLoan()
        {
            try
            {
                //verificamos cliente
                Client clientCurrent = GetCurrentClient();
                //obtenemos sus loans
                var loans = _loanRepository.GetAll();
                var loansDTO = loans.Select(l => new LoanDTO(l)).ToList();
                return Ok(loansDTO);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        // POST /api/loans
        [HttpPost]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult CreateLoan(LoanApplicationDTO loanApplicationDTO)
        {
            Client client = GetCurrentClient();
            try
            {
                // Verificar que el préstamo seleccionado exista
                var loanFinded = _loanRepository.GetLoanById(loanApplicationDTO.loanId);
                if (loanFinded == null)
                {
                    return StatusCode(403, "El préstamo no existe");
                }

                // Que el monto NO sea 0 y que no sobrepase el máximo autorizado
                if (loanApplicationDTO.Amount <= 0 || loanApplicationDTO.Amount > loanFinded.MaxAmount)
                {
                    return StatusCode(403, "El monto es 0 o supera el máximo permitido");
                }

                // Que los payments no lleguen vacíos
                List<int> listPayments = loanFinded.Payments.Split(",").Select(int.Parse).ToList();
                if (loanApplicationDTO.Payments.IsNullOrEmpty() || !listPayments.Contains(int.Parse(loanApplicationDTO.Payments)))
                {
                    return StatusCode(403, "El payment fue mal asignado");
                }

                // Que exista la cuenta de destino
                var accountFinded = _accountRepository.GetAccountByNumber(loanApplicationDTO.toAccountNumber);
                if (accountFinded == null)
                {
                    return StatusCode(403, "La cuenta seleccionada no existe");
                }

                // Que la cuenta de destino pertenezca al Cliente autentificado
                if (accountFinded.ClientId != client.Id)
                {
                    return StatusCode(403, "La cuenta seleccionada no pertenece al mismo cliente");
                }

                // Cuando guardes clientLoan el monto debes multiplicarlo por el 20 %
                var newAmount = loanApplicationDTO.Amount * 1.20;
                ClientLoan clientLoan = new ClientLoan()
                {
                    Amount = newAmount,
                    ClientId = client.Id,
                    LoanId = loanApplicationDTO.loanId,
                    Payments = loanApplicationDTO.Payments
                };
                _clientLoanRepository.SaveClientLoan(clientLoan);

                // Guardar la transacción
                Transaction newTransaction = new Transaction()
                {
                    Type = TransactionType.CREDIT,
                    Amount = loanApplicationDTO.Amount,
                    Description = loanFinded.Name + "- Loan aprobada",
                    Date = DateTime.Now,
                    AccountId = accountFinded.Id
                };
                _transactionRepository.Save(newTransaction);

                // Actualizar el Balance de la cuenta sumando el monto del préstamo
                accountFinded.Balance += loanApplicationDTO.Amount;
                _accountRepository.UpdateAccount(accountFinded);

                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}