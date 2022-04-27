using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
namespace WebApplication1.Controllers;

[ApiController]
[Route("[controller]")]

public class TransactionController : ControllerBase
{
    private readonly DataContext _dataContext;
    

    public TransactionController(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
    
    [HttpGet]
    public async Task<ActionResult<List<Transaction>>> Get()
    {
        return Ok(await _dataContext.Transactions.ToListAsync());
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<Transaction>> Get(int id)
    {
        var transaction = await _dataContext.Transactions.FindAsync(id);
        if (transaction == null)
            return NotFound("Transaction not found"); 
        return Ok(transaction);
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult<Transaction>> Delete(int id)
    {
        var transaction = await _dataContext.Transactions.FindAsync(id);
        if (transaction == null)
            return NotFound("Transaction not found");

        _dataContext.Transactions.Remove(transaction);
        await _dataContext.SaveChangesAsync();
        return Ok("Transaction has been succesfully removed");
    }
    
    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddTransaction(Transaction transaction)
    {

        var errMsg = ValidateTransaction(transaction);
        if (errMsg == "{}")
        { 
            _dataContext.Transactions.Add(transaction);
            await _dataContext.SaveChangesAsync();
        }
        else
        {
            return BadRequest(errMsg);
        }
        

        return Ok("Transaction has successfully created");
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTransaction(int id, Transaction request)
    {
        var transaction = await _dataContext.Transactions.FindAsync(id);
        if (transaction == null)
            return NotFound("Transaction not found"); 
        
        var errMsg = ValidateTransaction(request);
        if (errMsg == "{}")
        {
            transaction.Amount = request.Amount;
            transaction.Month = request.Month;
            transaction.Year = request.Year;
            transaction.CardNumber = request.CardNumber;
            transaction.PayerName = request.PayerName;
            transaction.PaymentDate = request.PaymentDate;
            transaction.CVC = request.CVC;

            await _dataContext.SaveChangesAsync();
        }
        else
        {
            return BadRequest(errMsg);
        }
        

        return Ok("Transaction has successfully updated");
    }
    
    public static string ValidateTransaction(Transaction transaction, bool notForUpdate = false)
    {
        var errorsDict = new Dictionary<string, string>();
        if (transaction.ID < 0)
        {
            errorsDict.Add("ID", "ID must be more than zero");
        }

        var payerName = transaction.PayerName;
        if (!(payerName.Length > 3 && payerName.Length < 15 && Char.IsUpper(payerName, 0)))
        {
            errorsDict.Add("PayerName", "payer name length must be more than 3 and less than 15, also it must start" +
                                        "from capital letter");
        }

        var cardNumber = transaction.CardNumber;
        if (!(cardNumber.ToString().Length == 16))
        {
            errorsDict.Add("CardNumber", "card number must be 16 digits number");
        }

        var month = transaction.Month;
        if (!(month.Length <= 2 && month.Length > 0 && Int32.Parse(month) > 0 && Int32.Parse(month) < 13))
        {
            errorsDict.Add("Month", "month must be a number from 01 to 12");
        }

        var year = transaction.Year;
        if (!((year.Length == 4 && Int32.Parse(year) >= 2022 && Int32.Parse(year) <= 2032)))
        {
            errorsDict.Add("Year", "Year must be more than 2022 and less than 2032");
        }

        var cvc = transaction.CVC;
        if (!(cvc > 99 && cvc <= 9999))
        {
            errorsDict.Add("CVC", "CVC must be in a format ### or ####");
        }

        var value = transaction.PaymentDate;
        if (!(value.Year <= 2022 && value.Year >= 2015 && (value.Year != 2022 || value.Month < DateTime.Today.Month 
              || (value.Month == DateTime.Today.Month && value.Day <= DateTime.Today.Day))))
        {
            errorsDict.Add("PaymentDate", "Payment date is ivalid");
        }

        var amount = transaction.Amount;
        if (amount <= 0)
        {
            errorsDict.Add("Amount", "Amount must be more than zero");
        }

        return JsonConvert.SerializeObject( errorsDict, Formatting.Indented );
    }
}


