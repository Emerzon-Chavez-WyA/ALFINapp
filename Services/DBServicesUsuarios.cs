using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Services
{
    public class DBServicesUsuarios
    {
        private readonly MDbContext _context;
        public DBServicesUsuarios(MDbContext context)
        {
            _context = context;
        }

        public async Task<(bool IsSuccess, string Message)> ModificarUsuario(Usuario usuario, int IdUsuarioAccion)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@IdUsuario", usuario.IdUsuario),
                    new SqlParameter("@Dni", usuario.Dni != null ? usuario.Dni : (object)DBNull.Value),
                    new SqlParameter("@NombresCompletos", usuario.NombresCompletos != null ? usuario.NombresCompletos : (object)DBNull.Value),
                    new SqlParameter("@Rol", usuario.Rol != null ? usuario.Rol : (object)DBNull.Value),
                    new SqlParameter("@Departamento", usuario.Departamento != null ? usuario.Departamento : (object)DBNull.Value),
                    new SqlParameter("@Provincia", usuario.Provincia != null ? usuario.Provincia : (object)DBNull.Value),
                    new SqlParameter("@Distrito", usuario.Distrito != null ? usuario.Distrito : (object)DBNull.Value),
                    new SqlParameter("@Telefono", usuario.Telefono != null ? usuario.Telefono : (object)DBNull.Value),
                    new SqlParameter("@Estado", usuario.Estado != null ? usuario.Estado : "ACTIVO"),
                    new SqlParameter("@IDUSUARIOSUP", usuario.IDUSUARIOSUP != null ? usuario.IDUSUARIOSUP : (object)DBNull.Value),
                    new SqlParameter("@RESPONSABLESUP", usuario.RESPONSABLESUP != null ? usuario.RESPONSABLESUP : (object)DBNull.Value),
                    new SqlParameter("@REGION", usuario.REGION != null ? usuario.REGION : (object)DBNull.Value),
                    new SqlParameter("@NOMBRECAMPAÑA", usuario.NOMBRECAMPAÑA != null ? usuario.NOMBRECAMPAÑA : (object)DBNull.Value),
                    new SqlParameter("@IdRol", usuario.IdRol != null ? usuario.IdRol : (object)DBNull.Value),
                    new SqlParameter("@id_usuario_accion", IdUsuarioAccion)
                };

                await _context.Database.ExecuteSqlRawAsync("EXEC sp_usuario_modificacion_existente @IdUsuario, @Dni, @NombresCompletos, @Rol, @Departamento, @Provincia, @Distrito, @Telefono, @Estado, @IDUSUARIOSUP, @RESPONSABLESUP, @REGION, @NOMBRECAMPAÑA, @IdRol, @id_usuario_accion", 
                    parameters);

                return (true, "Datos actualizados correctamente");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, string Message)> DesactivarUsuario(int usuarioId, int idAccion)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@IdUsuario", usuarioId),
                    new SqlParameter("@id_usuario_accion", idAccion)
                };
                await _context.Database.ExecuteSqlRawAsync("EXEC sp_usuario_desactivar @IdUsuario, @id_usuario_accion", parameters);
                return (true, "Usuario desactivado correctamente");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, string Message)> ActivarUsuario(int usuarioId, int idAccion)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@IdUsuario", usuarioId),
                    new SqlParameter("@id_usuario_accion", idAccion)
                };
                await _context.Database.ExecuteSqlRawAsync("EXEC sp_usuario_activar @IdUsuario, @id_usuario_accion", parameters);
                return (true, "Usuario activado correctamente");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, string Message)> CrearUsuario(Usuario usuario, int IdUsuarioAccion)
        {
            try
            {
                if (usuario.IdRol == null)
                {
                    return (false, "El rol del usuario es obligatorio");
                }
                if (usuario.Dni == null)
                {
                    return (false, "El DNI del usuario es obligatorio");
                }
                if (usuario.NombresCompletos == null)
                {
                    return (false, "El nombre del usuario es obligatorio");
                }
                var getUsuario = new Usuario();
                if (usuario.IDUSUARIOSUP != null)
                {
                    getUsuario = await _context.usuarios.FirstOrDefaultAsync(x => x.IdUsuario == usuario.IDUSUARIOSUP);
                    if (getUsuario == null)
                    {
                        return (false, "El usuario supervisor no existe");
                    }
                }
                var rolConseguir = await _context.roles.FirstOrDefaultAsync(x => x.IdRol == usuario.IdRol);
                var parameters = new[]
                {
                    new SqlParameter("@Dni", usuario.Dni),
                    new SqlParameter("@NombresCompletos", usuario.NombresCompletos),
                    new SqlParameter("@Rol", rolConseguir != null ? rolConseguir.Rol : "ASESOR"),
                    new SqlParameter("@Departamento", usuario.Departamento != null ? usuario.Departamento : (object)DBNull.Value),
                    new SqlParameter("@Provincia", usuario.Provincia != null ? usuario.Provincia : (object)DBNull.Value),
                    new SqlParameter("@Distrito", usuario.Distrito != null ? usuario.Distrito : (object)DBNull.Value),
                    new SqlParameter("@Telefono", usuario.Telefono != null ? usuario.Telefono : (object)DBNull.Value),
                    new SqlParameter("@Estado", usuario.Estado != null ? usuario.Estado : "ACTIVO"),
                    new SqlParameter("@IDUSUARIOSUP", usuario.IDUSUARIOSUP != null ? usuario.IDUSUARIOSUP : (object)DBNull.Value),
                    new SqlParameter("@RESPONSABLESUP", getUsuario?.NombresCompletos ?? (object)DBNull.Value),
                    new SqlParameter("@REGION", usuario.REGION != null ? usuario.REGION : (object)DBNull.Value),
                    new SqlParameter("@NOMBRECAMPAÑA", usuario.NOMBRECAMPAÑA != null ? usuario.NOMBRECAMPAÑA : (object)DBNull.Value),
                    new SqlParameter("@IdRol", usuario.IdRol),
                    new SqlParameter("@id_usuario_accion", IdUsuarioAccion)
                };
                await _context.Database.ExecuteSqlRawAsync("EXEC sp_usuario_crear_nuevo @Dni, @NombresCompletos, @Rol, @Departamento, @Provincia, @Distrito, @Telefono, @Estado, @IDUSUARIOSUP, @RESPONSABLESUP, @REGION, @NOMBRECAMPAÑA, @IdRol, @id_usuario_accion",
                    parameters);
                return (true, "Usuario creado correctamente");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, string Message)> UpdateUsuarioXCampo(int usuarioId, string campo, string nuevoValor)
        {
            try
            {
                var usuario = await _context.usuarios.FirstOrDefaultAsync(x => x.IdUsuario == usuarioId);
                if (usuario == null)
                {
                    return (false, "El usuario no existe");
                }
                var parameters = new[]
                {
                    new SqlParameter("@IdUsuario", usuarioId),
                    new SqlParameter("@Campo", campo),
                    new SqlParameter("@NuevoValor", nuevoValor ?? (object)DBNull.Value) // Si es null, lo manda como NULL a SQL
                };
                await _context.Database.ExecuteSqlRawAsync("EXEC sp_usuario_actualizar_campo @IdUsuario, @Campo, @NuevoValor", parameters);
                return (true, "Campo actualizado correctamente");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}