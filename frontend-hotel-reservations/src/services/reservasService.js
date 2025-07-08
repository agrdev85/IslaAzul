import ApiService from "./apiService"
import { DateTime } from "luxon"

class ReservasService {
  constructor() {
    this.endpoint = "/Reservas"
  }

  async getAll(filters = {}) {
    return ApiService.getPaginated(this.endpoint, filters)
  }

  async getActivas(fecha = null) {
    const params = fecha ? { fecha } : {}
    return ApiService.get(`${this.endpoint}/activas`, params)
  }

  async getById(id) {
    return ApiService.get(`${this.endpoint}/${id}`)
  }

  async create(reserva) {
    return ApiService.post(this.endpoint, reserva)
  }

  async update(id, reserva) {
    return ApiService.put(`${this.endpoint}/${id}`, reserva)
  }

  async cancelar(id, motivoCancelacion) {
    return ApiService.put(`${this.endpoint}/${id}/cancelar`, { motivoCancelacion })
  }

  async cambiarHabitacion(id, nuevoNumeroHabitacion) {
    return ApiService.put(`${this.endpoint}/${id}/cambiar-habitacion`, { nuevoNumeroHabitacion })
  }

  async registrarLlegada(id) {
    return ApiService.put(`${this.endpoint}/${id}/registrar-llegada`)
  }

  async actualizarExpiradas() {
    return ApiService.post(`${this.endpoint}/actualizar-expiradas`)
  }

  // Validaciones específicas
  validateReserva(reserva) {
    const errors = []

    if (!reserva.FechaEntrada || !DateTime.fromISO(reserva.FechaEntrada).isValid) {
      errors.push("Fecha de entrada inválida")
    }

    if (!reserva.FechaSalida || !DateTime.fromISO(reserva.FechaSalida).isValid) {
      errors.push("Fecha de salida inválida")
    }

    if (reserva.FechaEntrada && reserva.FechaSalida) {
      const entrada = DateTime.fromISO(reserva.FechaEntrada)
      const salida = DateTime.fromISO(reserva.FechaSalida)
      const hoy = DateTime.now()

      if (entrada < hoy.startOf("day")) {
        errors.push("La fecha de entrada no puede ser anterior a hoy")
      }

      if (salida <= entrada) {
        errors.push("La fecha de salida debe ser posterior a la fecha de entrada")
      }

      if (salida.diff(entrada, "days").days < 2) {
        errors.push("La reserva debe ser de al menos 3 días")
      }
    }

    if (!reserva.ClienteId) {
      errors.push("Debe seleccionar un cliente")
    }

    if (!reserva.HabitacionNumero || !/^0[1-3][1-5]$/.test(reserva.HabitacionNumero)) {
      errors.push("Número de habitación inválido (formato: 0XY)")
    }

    return errors
  }
}

export default new ReservasService()
