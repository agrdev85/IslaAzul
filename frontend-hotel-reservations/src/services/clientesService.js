import ApiService from "./apiService"

class ClientesService {
  constructor() {
    this.endpoint = "/Clientes"
  }

  async getAll(filters = {}) {
    return ApiService.getPaginated(this.endpoint, {
      busqueda: filters.busqueda || "",
      ciFiltro: filters.ciFiltro || "",
      ...filters,
    })
  }

  async getById(id) {
    return ApiService.get(`${this.endpoint}/${id}`)
  }

  async create(cliente) {
    return ApiService.post(this.endpoint, cliente)
  }

  async update(id, cliente) {
    return ApiService.put(`${this.endpoint}/${id}`, cliente)
  }

  async delete(id) {
    return ApiService.delete(`${this.endpoint}/${id}`)
  }

  // Validaciones específicas
  validateCliente(cliente) {
    const errors = []

    if (!cliente.NombreApellidos || !/^[a-zA-Z\s]{1,25}$/.test(cliente.NombreApellidos)) {
      errors.push("Nombre debe tener máximo 25 caracteres y solo letras")
    }

    if (!cliente.CI || !/^\d{11}$/.test(cliente.CI)) {
      errors.push("CI debe tener exactamente 11 dígitos")
    }

    if (!cliente.NumeroTelefonico || !/^\d+$/.test(cliente.NumeroTelefonico)) {
      errors.push("Número telefónico debe contener solo números")
    }

    return errors
  }
}

export default new ClientesService()
