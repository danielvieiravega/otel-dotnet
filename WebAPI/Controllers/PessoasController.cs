using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PessoasController: ControllerBase
{
    private readonly ILogger<PessoasController> _logger;
    private readonly IBus _bus;
    private readonly PessoaContext _context;

    public PessoasController(ILogger<PessoasController> logger, IBus bus, PessoaContext context)
    {
        _logger = logger;
        _bus = bus;
        _context = context;
    }

    [HttpPost]
    public IActionResult AddName([FromBody] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest("Não pode mandar vazio ou null!");
        }
        
        _bus.Publish(new Mensagem { Name = name });

        return Ok($"O nome {name} foi adicionado na fila de processamento!");
    }

    [HttpGet]
    public async Task<ActionResult> Get(int take = 10, int skip = 0)
    {
        return Ok(await _context.Pessoas.OrderBy(p => p.Nome).Skip(skip).Take(take).ToListAsync());
    }
}
