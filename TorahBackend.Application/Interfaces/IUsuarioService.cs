using TorahBackend.Domain.Entities;


namespace TorahBackend.Application.Interfaces
{
    public interface IUsuarioService
    {
        Task<Usuario> GetUsuario(string? email);
        string GenerateJwtToken(Usuario usuario);
        bool Authenticate(Usuario usuario, string? password);
    }
}
