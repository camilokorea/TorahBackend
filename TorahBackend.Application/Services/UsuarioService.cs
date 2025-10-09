using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TorahBackend.Application.Interfaces;
using TorahBackend.Domain.Entities;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;

namespace TorahBackend.Application.Services
{
    public class UsuarioService: IUsuarioService
    {
        private readonly IDataRepository _dataRepository;
        private readonly JwtConfig _jwtConfig;
        public UsuarioService(IDataRepository dataRepository, JwtConfig jwtConfig) {
            _dataRepository = dataRepository;
            _jwtConfig = jwtConfig;
        }

        public async Task<Usuario> GetUsuario(string? email)
        {
            try { 
                return await _dataRepository.GetUsuario(email);
            }
            catch {
                throw;
            }
        }

        public string GenerateJwtToken(Usuario usuario) 
        {
            try {
                var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, usuario.Email),
                        new Claim(ClaimTypes.NameIdentifier, usuario.Id)
                    }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch { throw; }
        }

        public bool Authenticate(Usuario usuario, string password) 
        {
            try {
                StringBuilder hash = new StringBuilder();

                MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();

                byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(password));

                for (int i = 0; i < bytes.Length; i++)
                {
                    hash.Append(bytes[i].ToString("x2"));
                }

                if (usuario == null || hash.ToString() != usuario.Password)
                    return false;

                return true;
            }
            catch { 
                throw;
            }
        }
    }
}
