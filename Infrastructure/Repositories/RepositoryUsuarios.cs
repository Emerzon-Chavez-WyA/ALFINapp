using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Infrastructure.Repositories
{
    public class RepositoryUsuarios : ALFINapp.Domain.Interfaces.IRepositoryUsuarios
    {
        private readonly MDbContext _context;
        public RepositoryUsuarios(MDbContext context)
        {
            _context = context;
        }
        public async Task<bool> RegisterEmail(string? email, int idUsuario)
        {
            try
            {
                var registerEmail = await _context
                    .Database
                    .ExecuteSqlAsync(
                        $"EXECUTE dbo.sp_usuario_modificacion_existente @IdUsuario={idUsuario}, @Correo={email}");
                if (registerEmail == 0)
                {
                    return false;
                }
                return true;
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public Task<bool> RegisterPassword(string password, int idUsuario)
        {
            throw new NotImplementedException();
        }
    }
}