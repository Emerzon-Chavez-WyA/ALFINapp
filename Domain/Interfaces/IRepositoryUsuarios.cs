using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Application.DTOs;
using ALFINapp.Infrastructure.Persistence.Models;

namespace ALFINapp.Domain.Interfaces
{
    public interface IRepositoryUsuarios
    {
        public Task<bool> RegisterEmail(string? email, int idUsuario);
        public Task<bool> RegisterPassword(string password, int idUsuario);
        public Task<Usuario?> GetUser(int idUsuario);
        public Task<List<DetallesUsuarioDTO>> GetAllUsers();
        public Task<List<DetallesUsuarioDTO>> GetAllAsesores();
        public Task<List<DetallesUsuarioDTO>> GetAllSupervisores();
    }
}