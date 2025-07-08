export const validationRules = {
  required: (val) => !!val || "Campo requerido",

  nombre: [
    (val) => !!val || "El nombre es requerido",
    (val) => (val && val.length <= 25 && /^[a-zA-Z\s]+$/.test(val)) || "Máximo 25 caracteres, solo letras y espacios",
  ],

  ci: [
    (val) => !!val || "La cédula es requerida",
    (val) => (val && val.length === 11) || "La cédula debe tener exactamente 11 dígitos",
    (val) => /^\d+$/.test(val) || "La cédula solo puede contener números",
  ],

  telefono: [
    (val) => !!val || "El teléfono es requerido",
    (val) => (val && /^\d+$/.test(val)) || "El teléfono solo puede contener números",
  ],

  habitacion: [
    (val) => !!val || "El número de habitación es requerido",
    (val) => /^0[1-3][1-5]$/.test(val) || "Formato inválido (0XY: X=1-3, Y=1-5)",
  ],

  fechaEntrada: [
    (val) => !!val || "La fecha de entrada es requerida",
    (val) => {
      const fecha = new Date(val)
      const hoy = new Date()
      hoy.setHours(0, 0, 0, 0)
      return fecha >= hoy || "La fecha no puede ser anterior a hoy"
    },
  ],

  fechaSalida: (val, fechaEntrada) => {
    if (!val) return "La fecha de salida es requerida"
    if (!fechaEntrada) return true

    const entrada = new Date(fechaEntrada)
    const salida = new Date(val)
    const diffTime = salida - entrada
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24))

    if (salida <= entrada) return "Debe ser posterior a la fecha de entrada"
    if (diffDays < 2) return "La reserva debe ser de al menos 3 días"

    return true
  },
}
