using HomeBankingMinHub.DTOs;
using HomeBankingMinHub.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HomeBankingMinHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private IAccountRepository _accountRepository;

        //constructor
        public AccountsController(IAccountRepository accountRepository)

        {

            _accountRepository = accountRepository;

        }

        [HttpGet("{id}")]

        public IActionResult FindAccountById(long id)
        {
            try
            {
                var account = _accountRepository.FindAccountById(id);
                var accountDTO = new AccountDTO(account);
                return Ok(accountDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]

        public IActionResult GetAllAccounts()
        {
            try
            {
                var account = _accountRepository.GetAllAccounts();
                var accountDTO = account.Select(c =>  new AccountDTO(c)).ToList();
                return Ok(accountDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
