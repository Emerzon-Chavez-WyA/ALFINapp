function CambiarEstadoUsuario(accion, idUsuario) {
    $.ajax({
        url: "/Usuarios/CambiarEstadoUsuario",
        type: "POST",
        data: {
            IdUsuario: idUsuario,
            accion: accion
        },
        success: function (response) {
            if (response.success === false) {
                Swal.fire({
                    title: 'Error al cambiar el estado del Usuario. ',
                    text: response.message,
                    icon: 'error',
                    confirmButtonText: 'OK'
                });
            } else {
                Swal.fire({
                    title: 'El Estado del asesor fue cambiado correctamente. Se recargara la pagina en 3 segundos para visualizar los cambios. ',
                    text: response.message,
                    icon: 'success',
                    confirmButtonText: 'OK'
                });
                setTimeout(function() {
                    location.reload();
                }, 3000);
            }
        },
        error: function (error) {
            Swal.fire({
                title: 'Error al actualizar los datos',
                text: error,
                icon: 'error',
                confirmButtonText: 'OK'
            });
        }
    });
}

function CargarModalModificarUsuario(idUsuario) {
    $.ajax({
        url: "/Usuarios/ModificarUsuarioVista",
        type: "GET",
        data: {
            IdUsuario: idUsuario
        },
        success: function (response) {
            if (response.success === false) {
                Swal.fire({
                    title: 'Error al obtener los datos del Usuario',
                    text: response.message,
                    icon: 'error',
                    confirmButtonText: 'OK'
                });
            } else {
                $('#modalContentGeneralTemplate').html(response); // Inserta el contenido de la vista en el modal
                $('#GeneralTemplateModal').modal('show'); // Muestra el modal
                $('#GeneralTemplateTitleModalLabel').text("Modificar Derivacion"); // Inserta el contenido de la vista en el modal
            }
        },
        error: function (error) {
            Swal.fire({
                title: 'Error al obtener los datos del Usuario',
                text: error,
                icon: 'error',
                confirmButtonText: 'OK'
            });
        }
    });
}