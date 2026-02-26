using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TeamLink.Core.Entities; // Kendi AppUser modelinin olduğu yer

namespace TeamLink.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<AppUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        // 1. KAYIT OLMA (POST: api/auth/register)
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var userExists = await _userManager.FindByEmailAsync(dto.Email);
            if (userExists != null)
                return BadRequest(new { message = "Bu email zaten kullanımda." });

            var user = new AppUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName // Eğer modelinde FullName yoksa bu satırı silebilirsin
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return BadRequest(new { message = "Kayıt başarısız. Şifreniz yeterince güçlü olmayabilir." });

            return Ok(new { message = "Kayıt başarılı! Lütfen giriş yapın." });
        }

        // 2. GİRİŞ YAPMA VE TOKEN ÜRETME (POST: api/auth/login)
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            // Kullanıcı var mı ve şifresi doğru mu kontrol et
            if (user != null && await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                // Kimlik kartının (Token) içindeki bilgiler
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                // appsettings.json dosyasından gizli şifreyi alıyoruz
                // Eğer orada yoksa geçici olarak güvenlik amaçlı uzun bir string kullanıyoruz
                var jwtKey = _configuration["Jwt:Key"] ?? "cok_gizli_ve_uzun_bir_sifre_anahtari_olustur_123456789";
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    expires: DateTime.Now.AddDays(7), // Token 7 gün geçerli
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                // Frontend'e üretilen Token'ı gönderiyoruz
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }

            return Unauthorized(new { message = "Email veya şifre hatalı." });
        }
    }

    // --- DTO'LAR (Veri Taşıma Objeleri) ---
    public class RegisterDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
    }

    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}