let currentPage = 1;
let rowsPerPage = 20;
let rows = document.querySelectorAll("tbody tr");
let totalPages = Math.ceil(rows.length / rowsPerPage);

document.addEventListener("DOMContentLoaded", function () {
    var informationTabla = document.getElementById("total-actual");
    var tables = {
        "tablaDerivacionesGestion": document.getElementById("tablaDerivacionesGestion"),
        "tablaGeneralGestion": document.getElementById("tablaGeneralGestion"),
        "tablaGeneralSistema": document.getElementById("tablaGeneralSistema") // La tabla que aparece primero
    };
    function updateTableInfo() {
        let activeTable = Object.values(tables).find(table => isTableVisible(table));
        if (!activeTable) {
            informationTabla.textContent = `Mostrando 0 registros`;
            return;
        }
        let tbody = activeTable.querySelector("tbody");
        let rows = tbody ? tbody.getElementsByTagName("tr") : [];
        let visibleRows = Array.from(rows).filter(row => row.offsetParent !== null); // Detectar filas visibles
        let rowCount = visibleRows.length;
        if (activeTable.id === "tablaGeneralSistema") {
            rowCount = Math.max(rowCount - 1, 0); // Restar 1 solo en este caso
        }
        informationTabla.textContent = `Mostrando ${rowCount} registros`;
    }
    function isTableVisible(table) {
        if (!table) return false;
        let style = window.getComputedStyle(table);
        return style.display !== "none" && style.visibility !== "hidden" && style.opacity !== "0";
    }
    // Observador para detectar cambios en las tablas
    var observer = new MutationObserver(updateTableInfo);
    Object.values(tables).forEach(table => {
        if (table) {
            observer.observe(table, { childList: true, subtree: true, attributes: true });
        }
    });
    // Ejecutar la función una vez al inicio
    setTimeout(updateTableInfo, 10000);
});


document.addEventListener("DOMContentLoaded", function () {
    activatePagination(0);
});

function cargarDerivacionesXAsesorSistema(DniAsesor) {
    const tablaGeneralSistema = document.getElementById("tablaGeneralSistema");
    const tablaDerivacionesGestion = document.getElementById("tablaDerivacionesGestion");
    const tablaGeneralGestion = document.getElementById("tablaGeneralGestion");

    if (!tablaGeneralSistema) return;

    const tabla = tablaGeneralSistema.querySelector("table");
    if (!tabla) return;

    const filas = Array.from(tabla.getElementsByTagName("tr")).slice(3); // Excluye encabezados

    if (DniAsesor === "") {
        filas.forEach(fila => (fila.style.display = "")); // Muestra todas las filas
        tablaGeneralSistema.style.display = "block";
        tablaDerivacionesGestion.style.display = "none";
        tablaDerivacionesGestion.loadedfield = "false";
        tablaGeneralGestion.style.display = "none";
        activatePagination(0);
        return;
    }

    let tieneCoincidencias = false;

    filas.forEach(fila => {
        const columnaDni = fila.cells[4]; // Columna 5 (DNI Asesor, índice 4 basado en 0)
        if (!columnaDni) return;

        const dniFila = columnaDni.textContent.trim();
        const coincide = dniFila === DniAsesor;

        fila.style.display = coincide ? "" : "none";
        tieneCoincidencias ||= coincide;
    });

    // Si no hay coincidencias, podrías ocultar la tabla o mostrar un mensaje
    if (!tieneCoincidencias) {
        console.warn("No se encontraron registros para el DNI:", DniAsesor);
    }
}

function cargarDerivacionesGestion(DniAsesor) {
    tablaGeneralGestion = document.getElementById("tablaGeneralGestion")
    tablaGeneralSistema = document.getElementById("tablaGeneralSistema")

    tablaDerivacionesGestion = document.getElementById("tablaDerivacionesGestion")
    if (tablaGeneralGestion.loadedfield === "true") {
        tablaGeneralGestion.style = "display: block;"
        tablaGeneralSistema.style = "display: none;"
        tablaDerivacionesGestion.style = "display: none;"
        return;
    }
    $.ajax({
        type: 'GET',
        url: '/Derivacion/ObtenerDerivacionesGestion',
        data: {
            DniAsesor: DniAsesor
        },
        success: function (response) {
            if (response.success === false) {
                Swal.fire({
                    title: 'Error al cargar las derivaciones',
                    text: response.message,
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });
                return;
            } else {
                $('#tablaGeneralGestion').html(response);
                tablaGeneralGestion.style = "display: block;"
                tablaGeneralGestion.loadedfield = "true"
                tablaGeneralSistema.style = "display: none;"
                tablaDerivacionesGestion.style = "display: none;"
                return;
            }
        },
        error: function (error) {
            Swal.fire({
                title: 'Error al cargar las derivaciones',
                text: 'Hubo un error inesperado al cargar las derivaciones.',
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
        }
    });
}

function cargarDerivacionesSistema() {

    tablaDerivacionesGestion = document.getElementById("tablaDerivacionesGestion")
    tablaGeneralSistema = document.getElementById("tablaGeneralSistema")
    tablaGeneralGestion = document.getElementById("tablaGeneralGestion")
    tablaGeneralSistema.style = "display: block;"
    tablaGeneralGestion.style = "display: none;"
    tablaDerivacionesGestion.style = "display: none;"
}

function cargarDerivacionesGestionSupervisor(idAsesor) {

    tablaDerivacionesGestion = document.getElementById("tablaDerivacionesGestion")
    tablaGeneralSistema = document.getElementById("tablaGeneralSistema")
    tablaGeneralGestion = document.getElementById("tablaGeneralGestion")
    asesorDni = document.getElementById(idAsesor).value.toString();
    if (asesorDni === "") {
        if (tablaGeneralGestion.innerHTML.trim() !== "") {
            tablaGeneralGestion.style = "display: block;"
            tablaGeneralSistema.style = "display: none;"
            tablaDerivacionesGestion.style = "display: none;"
            return;
        }
        else {
            $.ajax({
                type: 'GET',
                url: '/Derivacion/ObtenerDerivacionesGestionSupervisor',
                data: {
                },
                success: function (response) {
                    if (response.success === false) {
                        Swal.fire({
                            title: 'Error al cargar las derivaciones',
                            text: response.message,
                            icon: 'error',
                            confirmButtonText: 'Aceptar'
                        });
                        return;
                    } else {
                        $('#tablaGeneralGestion').html(response);
                        tablaGeneralGestion.style = "display: block;"
                        tablaGeneralSistema.style = "display: none;"
                        tablaDerivacionesGestion.style = "display: none;"
                        return;
                    }
                },
                error: function (error) {
                    Swal.fire({
                        title: 'Error al cargar las derivaciones',
                        text: 'Hubo un error inesperado al cargar las derivaciones.',
                        icon: 'error',
                        confirmButtonText: 'Aceptar'
                    });
                }
            });
        }
    }
    else {
        $.ajax({
            type: 'GET',
            url: '/Derivacion/ObtenerDerivacionesGestionSupervisor',
            data: {
                DniAsesor: asesorDni
            },
            success: function (response) {
                if (response.success === false) {
                    Swal.fire({
                        title: 'Error al cargar las derivaciones',
                        text: response.message,
                        icon: 'error',
                        confirmButtonText: 'Aceptar'
                    });
                    return;
                } else {
                    $('#tablaDerivacionesGestion').html(response);
                    tablaGeneralGestion.style = "display: none;"
                    tablaGeneralSistema.style = "display: none;"
                    tablaDerivacionesGestion.style = "display: block;"
                    return;
                }
            },
            error: function (error) {
                Swal.fire({
                    title: 'Error al cargar las derivaciones',
                    text: 'Hubo un error inesperado al cargar las derivaciones.',
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });
            }
        });
    }
}

function cargarDerivacionesSistemaSupervisor(idAsesor) {

    tablaDerivacionesGestion = document.getElementById("tablaDerivacionesGestion")
    tablaGeneralSistema = document.getElementById("tablaGeneralSistema")
    tablaGeneralGestion = document.getElementById("tablaGeneralGestion")
    asesorDni = document.getElementById(idAsesor).value.toString();
    if (asesorDni === "") {
        tablaGeneralSistema.style = "display: block;"

        tablaGeneralGestion.style = "display: none;"
        tablaDerivacionesGestion.style = "display: none;"
        return;
    } else {
        tablaGeneralSistema.style = "display: none;"
        tablaGeneralGestion.style = "display: none;"
        tablaDerivacionesGestion.style = "display: none;"
        return;
    }
}


function sortTableDerivaciones(idTabla, numCol, type) {
    const table = document.getElementById(idTabla);
    if (!table) return;

    const rows = Array.from(table.rows).slice(3); // Ya verificaste que es correcto

    const isAscending = table.dataset.sortOrder !== 'asc';
    table.dataset.sortOrder = isAscending ? 'asc' : 'desc';

    rows.sort((a, b) => {
        const getCellValue = (row) => {
            const cell = row.cells[numCol];
            return cell ? cell.textContent.trim() : '';
        };

        let aText = getCellValue(a);
        let bText = getCellValue(b);

        if (type === 'number') {
            return isAscending
                ? (parseFloat(aText) || 0) - (parseFloat(bText) || 0)
                : (parseFloat(bText) || 0) - (parseFloat(aText) || 0);
        }

        if (type === 'date') {
            const parseDate = (dateStr) => {
                if (!dateStr || dateStr.trim() === '') return new Date(0);

                const regex = /^(\d{1,2})\/(\d{1,2})\/(\d{4})\s(\d{1,2}):(\d{1,2}):(\d{1,2})$/;
                const match = dateStr.match(regex);

                if (!match) return new Date(0);

                const [, day, month, year, hours, minutes, seconds] = match.map(Number);
                return new Date(year, month - 1, day, hours, minutes, seconds);
            };

            const aDate = parseDate(aText);
            const bDate = parseDate(bText);
            return isAscending ? aDate - bDate : bDate - aDate;
        }

        return isAscending ? aText.localeCompare(bText) : bText.localeCompare(aText);
    });

    rows.forEach(row => table.tBodies[0].appendChild(row));
}

function cargarTipoFiltro(filtro) {
    const filtrosTabla = document.getElementById("filtrosTabla");
    const filtroDni = document.getElementById("filtroDNI");
    const filtroFecha = document.getElementById("filtroFecha");
    const tablaSistema = document.getElementById("tablaDerivacionesSistema");
    const tablaGestion = document.getElementById("tablaDerivacionesGestion");

    // Reset input values
    if (filtroDni.querySelector('input')) {
        filtroDni.querySelector('input').value = '';
    }
    if (filtroFecha.querySelector('input')) {
        filtroFecha.querySelector('input').value = '';
    }

    // Reset table filters
    if (tablaSistema) {
        filtrarTabla("tablaDerivacionesSistema", "", 0, "text");
    }
    if (tablaGestion) {
        filtrarTabla("tablaDerivacionesGestion", "", 0, "text");
    }

    if (filtro === "dni") {
        filtrosTabla.style = "display: block;";
        filtroDni.style = "display: block;";
        filtroFecha.style = "display: none;";
    } else if (filtro === "fecha") {
        filtrosTabla.style = "display: block;";
        filtroDni.style = "display: none;";
        filtroFecha.style = "display: block;";
    } else {
        filtrosTabla.style = "display: none;";
        filtroDni.style = "display: none;";
        filtroFecha.style = "display: none;";
    }
}

function filtrarTabla(idTabla, value, colIndex, type) {
    const table = document.getElementById(idTabla);
    if (!table) return;

    const rows = Array.from(table.getElementsByTagName("tr")).slice(3);

    rows.forEach(row => {
        const cell = row.cells[colIndex];
        if (!cell) return;

        let cellValue = cell.textContent.trim();
        let showRow = false;

        if (type === "text") {
            if (value === "") {
                showRow = true; // Show all rows when text filter is empty
                activatePagination(0);
            }
            showRow = cellValue.toLowerCase().includes(value.toLowerCase());
        } else if (type === "date") {
            if (value === "") {
                showRow = true; // Show all rows when date filter is empty
                activatePagination(0);
            } else {
                const match = cellValue.match(/(\d{1,2})\/(\d{1,2})\/(\d{4})/);
                if (match) {
                    const formattedDate = `${match[3]}-${match[2].padStart(2, '0')}-${match[1].padStart(2, '0')}`;
                    showRow = formattedDate === value;
                }
            }
        }
        row.style.display = showRow ? "" : "none";
    });
}

function activatePagination(direction) {
    if (direction === -1 && currentPage > 1) {
        currentPage--;
    } else if (direction === 1 && currentPage < totalPages) {
        currentPage++;
    }
    let start = (currentPage - 1) * rowsPerPage;
    let end = start + rowsPerPage;
    rows.forEach((row, index) => {
        // Ensure the first row (header) is always visible
        if (index === 0) {
            row.style.display = "";
        } else {
            row.style.display = (index > start && index <= end) ? "" : "none";
        }
    });
    document.getElementById("page-indicator").innerText = `Página ${currentPage}`;
}
