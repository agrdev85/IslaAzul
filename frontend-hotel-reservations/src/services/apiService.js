import { api } from "boot/axios"

class ApiService {
  // Métodos genéricos para CRUD
  async get(endpoint, params = {}) {
    const response = await api.get(endpoint, { params })
    return response.data
  }

  async post(endpoint, data) {
    const response = await api.post(endpoint, data)
    return response.data
  }

  async put(endpoint, data) {
    const response = await api.put(endpoint, data)
    return response.data
  }

  async delete(endpoint) {
    const response = await api.delete(endpoint)
    return response.data
  }

  // Métodos específicos para paginación
  async getPaginated(endpoint, { page = 1, rowsPerPage = 10, sortBy = "Id", descending = false, ...filters } = {}) {
    const params = {
      pagina: page,
      tamanoPagina: rowsPerPage,
      ordenarPor: sortBy,
      descendente: descending,
      ...filters,
    }

    return this.get(endpoint, params)
  }
}

export default new ApiService()
