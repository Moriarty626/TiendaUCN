using Mapster;
using TiendaUCN.Domain.Models.User;
using TiendaUCN.src.Application.DTOs.AuthDTO;
using TiendaUCN.src.Domain.Models;
namespace TiendaUCN.src.Application.Mappers
{
    public class UserMapper
    {
        public void ConfigureAllMappings()
        {
            ConfigureAuthMappings();
        }

        private void ConfigureAuthMappings()
        {
            TypeAdapterConfig<RegisterDTO, User>.NewConfig()
                .Map(dest => dest.EmailConfirmed, src => false)
                .Map(dest => dest.PasswordHash, src => BCrypt.Net.BCrypt.HashPassword(src.Password))
                .Map(dest => dest.RoleId, src => 2)
                .Map(dest => dest.DateOfBirth, src => src.BirthDate)
                .Map(dest => dest.IsDeleted, src => false);
        }
    }
}