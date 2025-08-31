using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using webApi.Models; // Add this for AppDbContext and User
using webApi.Data;
using Microsoft.EntityFrameworkCore; // For DbContext
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public UserController(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] webApi.Models.LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = _db.Users.Include(u => u.Roles).FirstOrDefault(u => u.Username == request.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            return Unauthorized(new { message = "Usuario o contraseña incorrectos" });

        // Crear claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString())
        };
        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Name));
        }

        // Configuración JWT
        var key = _config["Jwt:Key"] ?? "EstaEsUnaClaveSuperSeguraDe32Caracteres!!";
        var issuer = _config["Jwt:Issuer"] ?? "secureCodeApi";
        var audience = _config["Jwt:Audience"] ?? "secureCodeApiUsers";
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: credentials
        );

    var jwt = new JwtSecurityTokenHandler().WriteToken(token);
    // Incluir los roles del usuario en la respuesta
    var roles = user.Roles.Select(r => r.Name).ToList();
    return Ok(new { token = jwt, roles });
    }



    [HttpPost("submit")]
    public IActionResult Submit([FromBody] User input)
    {

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        input.Sanitize();

        // Leer el rol del body (si viene de un form, usar Request.Form)
        string? selectedRole = null;
        if (Request.HasFormContentType && Request.Form.ContainsKey("role"))
        {
            selectedRole = Request.Form["role"];
        }
        else if (Request.ContentType != null && Request.ContentType.Contains("application/json"))
        {
            // Si el rol viene en el JSON, usar reflection para obtenerlo
            var roleProp = input.GetType().GetProperty("Role");
            if (roleProp != null)
                selectedRole = roleProp.GetValue(input)?.ToString();
        }
        Console.WriteLine("Rol: " + selectedRole);
        if (string.IsNullOrWhiteSpace(selectedRole))
            selectedRole = "User";

        // Buscar o crear el rol seleccionado
        Console.WriteLine("Rol: " + selectedRole);
        var role = _db.Roles.FirstOrDefault(r => r.Name == selectedRole);
        if (role == null)
        {
            role = new Role { Name = selectedRole };
            _db.Roles.Add(role);
            _db.SaveChanges();
        }

        // Hashear la contraseña antes de guardar
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(input.Password);

        // Crear usuario y asignar rol
        var user = new User
        {
            Username = input.Username,
            Email = input.Email,
            Password = hashedPassword,
            Roles = new List<Role> { role }
        };

        _db.Users.Add(user);
        _db.SaveChanges();

        return Ok(new { message = $"User registered successfully with role {role.Name}." });
    }

    [HttpGet("all")]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "admin")]
    public IActionResult GetAllUsers()
    {
        var users = _db.Users.ToList();
        return Ok(users);
    }
}