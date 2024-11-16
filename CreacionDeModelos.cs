public class Cliente
{
    public int Id { get; set; }
    public string Nombres { get; set; }
    public string Apellidos { get; set; }
    public string Genero { get; set; }
    public DateTime FechaNacimiento { get; set; }
    public string Estado { get; set; }
}

public class InformacionGeneralCliente
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string TipoInformacion { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaActualizacion { get; set; }
    public string UsuarioCreador { get; set; }
    public string EstadoInformacion { get; set; }

    public Cliente Cliente { get; set; } // Relación con Cliente
}


 // Instalacion de los paquetes
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Tools

//Configuracion del servicio de DbContext para usar PostgreSQL:
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

//CREACION DE BASE DE DATOS Y MIGRACIONES

public class ApplicationDbContext : DbContext
{
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<InformacionGeneralCliente> InformacionesGenerales { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
}
//ejecucion de la migración y se genera la base de datos
dotnet ef migrations add InitialCreate
dotnet ef database update


//Implementación de los Endpoints 

[Route("api/[controller]")]
[ApiController]
public class ClienteController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ClienteController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Cliente
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes()
    {
        return await _context.Clientes.ToListAsync();
    }

    // POST: api/Cliente
    [HttpPost]
    public async Task<ActionResult<Cliente>> PostCliente(Cliente cliente)
    {
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();
        return CreatedAtAction("GetCliente", new { id = cliente.Id }, cliente);
    }

}

[Route("api/[controller]")]
[ApiController]
public class InformacionGeneralController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public InformacionGeneralController(ApplicationDbContext context)
    {
        _context = context;
    }

        // GET: api/InformacionGeneral
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InformacionGeneralCliente>>> GetInformacionGeneral()
    {
        return await _context.InformacionesGenerales
                             .OrderBy(i => i.FechaCreacion)
                             .ThenBy(i => i.Cliente.Apellidos)
                             .ToListAsync();
    }

        // POST: api/InformacionGeneral
    [HttpPost]
    public async Task<ActionResult<InformacionGeneralCliente>> PostInformacionGeneral(InformacionGeneralCliente info)
    {
        _context.InformacionesGenerales.Add(info);
        await _context.SaveChangesAsync();
        return CreatedAtAction("GetInformacionGeneral", new { id = info.Id }, info);
    }
    }



//Endpoint de ListadoGeneral
    [HttpGet("ListadoGeneral")]
public async Task<ActionResult<IEnumerable<InformacionGeneralCliente>>> GetListadoGeneral()
{
    return await _context.InformacionesGenerales
                         .OrderBy(i => i.FechaCreacion)
                         .ThenBy(i => i.Cliente.Apellidos)
                         .ToListAsync();
}


//Scaffolding (Reverse Engineering):

dotnet ef dbcontext scaffold "Host=localhost;Database=ClienteDB;Username=postgres;Password=yourpassword" Npgsql.EntityFrameworkCore.PostgreSQL --output-dir Models
