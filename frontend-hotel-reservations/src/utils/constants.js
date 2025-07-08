export const TABLE_COLUMNS = {
  clientes: [
    { name: "Id", label: "ID", field: "Id", sortable: true },
    { name: "NombreApellidos", label: "Nombre", field: "NombreApellidos", sortable: true },
    { name: "CI", label: "CI", field: "CI", sortable: true },
    { name: "NumeroTelefonico", label: "Teléfono", field: "NumeroTelefonico", sortable: true },
    { name: "EsVip", label: "VIP", field: "EsVip", sortable: true },
    { name: "actions", label: "Acciones", field: "actions", sortable: false },
  ],

  amas: [
    { name: "Id", label: "ID", field: "Id", sortable: true },
    { name: "NombreApellidos", label: "Nombre", field: "NombreApellidos", sortable: true },
    { name: "CI", label: "CI", field: "CI", sortable: true },
    { name: "NumeroTelefonico", label: "Teléfono", field: "NumeroTelefonico", sortable: true },
    { name: "actions", label: "Acciones", field: "actions", sortable: false },
  ],

  trazas: [
    { name: "Id", label: "ID", field: "Id", sortable: true },
    { name: "Operacion", label: "Operación", field: "Operacion", sortable: true },
    { name: "TablaAfectada", label: "Tabla", field: "TablaAfectada", sortable: true },
    { name: "RegistroId", label: "Registro ID", field: "RegistroId", sortable: true },
    { name: "Fecha", label: "Fecha", field: "Fecha", sortable: true },
    { name: "Detalles", label: "Detalles", field: "Detalles", sortable: false },
  ],
}
