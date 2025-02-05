function cargarDerivacionManual(FechaReferido, AgenciaDerivacion, DniAsesor, Telefono, DniCliente, NombreCliente) {
    if (!DniCliente || !Telefono || !DniAsesor) {
        Swal.fire({
            title: 'Error al derivar',
            text: 'Error al cargar los datos de las derivaciones',
            icon: 'error',
            confirmButtonText: 'Aceptar'
        });
        return;
    }
    $.ajax({
        type: 'GET',
        url: '/Referidos/DatosEnviarDerivacion',
        data: {
            FechaVisitaDerivacion: FechaReferido,
            AgenciaDerivacion: AgenciaDerivacion,
            AsesorDerivacion: DniAsesor,
            DNIAsesorDerivacion: DniAsesor,
            TelefonoDerivacion: Telefono,
            DNIClienteDerivacion: DniCliente,
            NombreClienteDerivacion: NombreCliente,
        },
        success: function (response) {
            if (response.success === false) {
                Swal.fire({
                    title: 'Error al cargar las referencias a derivar',
                    text: response.message,
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });
                return;
            } else {
                $('#modalContentGeneralTemplate').html(response); // Inserta el contenido de la vista en el modal
                $('#GeneralTemplateModal').modal('show'); // Muestra el modal
                $('#GeneralTemplateTitleModalLabel').text("Precaucion se mandara esta Derivacion"); // Inserta el contenido de la vista en el modal
            }
        },
        error: function (error) {
            Swal.fire({
                title: 'Error al cargar las Referencias',
                text: 'Hubo un error inesperado al cargar las Referencias a enviar.',
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
        }
    });
}

function enviarDerivacionPorReferencia(params) {
    console.log(params);
}