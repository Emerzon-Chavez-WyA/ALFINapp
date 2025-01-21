function showMyModal(apiName, controllerName, type, templateTitle) {
    TemplateModalRenderer = document.getElementById('GeneralTemplateModal');
    ContentModalRenderer = document.getElementById('modalContentGeneralTemplate');
    TitleModalRenderer = document.getElementById('GeneralTemplateTitleModalLabel');

    if (controllerName === null) {
        Swal.fire({
            title: 'Error al mostrar la vista',
            text: `El elemento enviado: ${controllerName} no existe, o no es valido`,
            icon: 'warning',
            confirmButtonText: 'Aceptar'
        });
        return;
    }

    if (apiName === null) {
        Swal.fire({
            title: 'Error al mostrar la vista',
            text: `El elemento enviado: ${apiName} no existe, o no es valido`,
            icon: 'warning',
            confirmButtonText: 'Aceptar'
        });
        return;
    }

    if (type === null || type === undefined) {
        Swal.fire({
            title: 'Error al mostrar la vista',
            text: `El elemento enviado: ${type} no existe, o no es valido`,
            icon: 'warning',
            confirmButtonText: 'Aceptar'
        });
        return;
    }

    if (templateTitle === undefined) {
        Swal.fire({
            title: 'Error al mostrar la vista',
            text: `El elemento enviado: ${templateTitle} no existe, o no es valido`,
            icon: 'warning',
            confirmButtonText: 'Aceptar'
        });
        return;
    }

    $.ajax({
        url: `/${apiName}/${controllerName}`,
        type: `${type}`,
        success: function (response) {
            if (response.success === false) {
                Swal.fire({
                    title: 'Error al mostrar la vista',
                    text: response.message,
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });
            } else {
                $('#GeneralTemplateModal').modal('show'); // Muestra el modal
                $('#modalContentGeneralTemplate').html(response); // Insertar la vista parcial en el modal
                TitleModalRenderer.innerText = templateTitle; // Asigna el título al modal
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error('Error al realizar la solicitud AJAX:', textStatus, errorThrown);
            Swal.fire({
                title: 'Error',
                text: 'No se pudo cargar la vista.',
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
        }
    });
}