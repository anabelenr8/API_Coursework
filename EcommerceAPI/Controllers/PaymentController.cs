using Microsoft.AspNetCore.Mvc;
using EcommerceAPI.Data;
using EcommerceAPI.Models;
using System.Collections.Generic;
using System.Linq;

[Route("api/[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly EcommerceDbContext _context;

    public PaymentController(EcommerceDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Payment>> GetPayments()
    {
        return Ok(_context.Payments.ToList());
    }

    [HttpGet("{id}")]
    public ActionResult<Payment> GetPayment(int id)
    {
        var payment = _context.Payments.Find(id);
        if (payment == null) return NotFound();
        return Ok(payment);
    }

    [HttpPost]
    public ActionResult<Payment> AddPayment(Payment payment)
    {
        _context.Payments.Add(payment);
        _context.SaveChanges();
        return CreatedAtAction(nameof(GetPayment), new { id = payment.Id }, payment);
    }

    [HttpPut("{id}")]
    public IActionResult UpdatePayment(int id, Payment updatedPayment)
    {
        var payment = _context.Payments.Find(id);
        if (payment == null) return NotFound();

        payment.OrderId = updatedPayment.OrderId;
        payment.Amount = updatedPayment.Amount;
        payment.PaymentMethod = updatedPayment.PaymentMethod;
        payment.Status = updatedPayment.Status;

        _context.SaveChanges();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeletePayment(int id)
    {
        var payment = _context.Payments.Find(id);
        if (payment == null) return NotFound();

        _context.Payments.Remove(payment);
        _context.SaveChanges();
        return NoContent();
    }
}
