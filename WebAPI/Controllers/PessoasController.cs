using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PessoasController: ControllerBase
{
    private readonly ILogger<PessoasController> _logger;
    private readonly IBus _bus;
    private readonly PessoaContext _context;
    private readonly Counter<long> _counterPeopleAdded;

    public PessoasController(
        ILogger<PessoasController> logger, 
        IBus bus, 
        PessoaContext context,
        Meter meter )
    {
        _logger = logger;
        _bus = bus;
        _context = context;
        _counterPeopleAdded = meter.CreateCounter<long>("people.added", "people", "Contador de pessoas adicionadas");
    }

    [HttpPost]
    public IActionResult AddName([FromBody] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest("Não pode mandar vazio ou null!");
        }
        
        _bus.Publish(new Mensagem { Name = name });

        _counterPeopleAdded.Add(1);

        return Ok($"O nome {name} foi adicionado na fila de processamento!");
    }

    [HttpGet]
    public async Task<ActionResult> Get(int take = 10, int skip = 0)
    {
        return Ok(await _context.Pessoas.OrderBy(p => p.Nome).Skip(skip).Take(take).ToListAsync());
    }
}
