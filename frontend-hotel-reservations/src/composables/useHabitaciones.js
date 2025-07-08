import { ref, computed } from "vue"
import { useQuasar } from "quasar"
import axios from "axios"

const apiUrl = "http://localhost:5014/api"

export function useHabitaciones() {
  const $q = useQuasar()

  const habitaciones = ref([])
  const habitacionesDisponibles = ref([])
  const loading = ref(false)
  const selectedHabitacionId = ref(null)

  const habitacionForm = ref({
    Numero: "",
    EstaFueraDeServicio: false,
  })

  const filtros = ref({
    busqueda: "",
    estado: null, // Aseguramos que sea null para mostrar todas las habitaciones por defecto
    fechaInicio: "",
    fechaFin: "",
  })

  const estadoOptions = [
    { label: "Todas", value: null },
    { label: "Disponibles", value: false },
    { label: "Fuera de Servicio", value: true },
  ]

  const filteredHabitaciones = computed(() => {
    let filtered = habitaciones.value

    if (filtros.value.busqueda) {
      filtered = filtered.filter((habitacion) =>
        habitacion.Numero.toLowerCase().includes(filtros.value.busqueda.toLowerCase())
      )
    }

    if (filtros.value.estado !== null) {
      filtered = filtered.filter((habitacion) => habitacion.EstaFueraDeServicio === filtros.value.estado)
    }

    return filtered
  })

  const formDisabled = computed(() => {
    return (
      !habitacionForm.value.Numero ||
      !/^0[1-3][1-5]$/.test(habitacionForm.value.Numero) ||
      habitaciones.value.some((h) => h.Numero === habitacionForm.value.Numero && h.Id !== selectedHabitacionId.value)
    )
  })

  const showNotification = (message, type = "positive") => {
    $q.notify({
      message,
      color: type === "positive" ? "green" : "red",
      position: "top",
      timeout: 3000,
      icon: type === "positive" ? "check_circle" : "error",
    })
  }

  const fetchHabitaciones = async () => {
    loading.value = true
    try {
      const response = await axios.get(`${apiUrl}/Habitaciones`, {
        params: {
          pagina: 1,
          tamanoPagina: 100, // Solicitamos hasta 100 habitaciones para asegurar que se carguen todas
        }
      })
      habitaciones.value = Array.isArray(response.data) ? response.data.map(h => ({
        Id: h.Id,
        Numero: h.Numero || "N/A",
        EstaFueraDeServicio: h.EstaFueraDeServicio || false
      })) : []
      console.log("Habitaciones cargadas:", {
        count: habitaciones.value.length,
        habitaciones: habitaciones.value
      })
      showNotification(`${habitaciones.value.length} habitaciones cargadas`)
    } catch (error) {
      console.error("Error loading habitaciones:", {
        message: error.message,
        status: error.response?.status,
        data: error.response?.data
      })
      showNotification(`Error al cargar habitaciones: ${error.response?.data?.message || error.message}`, "negative")
      habitaciones.value = []
    } finally {
      loading.value = false
    }
  }

  const buscarHabitacionesDisponibles = async (fechaInicio, fechaFin) => {
    if (!fechaInicio || !fechaFin) {
      showNotification("Debe seleccionar ambas fechas", "negative")
      return []
    }

    loading.value = true
    try {
      const response = await axios.get(`${apiUrl}/Habitaciones/habitaciones-disponibles`, {
        params: {
          fechaInicio,
          fechaFin,
        },
      })
      habitacionesDisponibles.value = Array.isArray(response.data) ? response.data.map(h => ({
        Numero: h.Numero || "N/A",
        EstaFueraDeServicio: h.EstaFueraDeServicio || false
      })) : []
      console.log("Habitaciones disponibles:", {
        count: habitacionesDisponibles.value.length,
        habitaciones: habitacionesDisponibles.value
      })
      showNotification(`${habitacionesDisponibles.value.length} habitaciones disponibles encontradas`)
      return habitacionesDisponibles.value
    } catch (error) {
      console.error("Error searching available rooms:", {
        message: error.message,
        status: error.response?.status,
        data: error.response?.data
      })
      showNotification(`Error al buscar habitaciones disponibles: ${error.response?.data?.message || error.message}`, "negative")
      habitacionesDisponibles.value = []
      return []
    } finally {
      loading.value = false
    }
  }

  const crearHabitacion = async () => {
    loading.value = true
    try {
      await axios.post(`${apiUrl}/Habitaciones`, habitacionForm.value)
      showNotification("Habitación creada exitosamente")
      resetForm()
      await fetchHabitaciones()
    } catch (error) {
      console.error("Error creating habitacion:", {
        message: error.message,
        status: error.response?.status,
        data: error.response?.data
      })
      showNotification(error.response?.data?.message || "Error al crear habitación", "negative")
    } finally {
      loading.value = false
    }
  }

  const modificarHabitacion = async () => {
    if (!selectedHabitacionId.value) return
    loading.value = true
    try {
      await axios.put(`${apiUrl}/Habitaciones/${selectedHabitacionId.value}`, habitacionForm.value)
      showNotification("Habitación modificada exitosamente")
      resetForm()
      await fetchHabitaciones()
    } catch (error) {
      console.error("Error modifying habitacion:", {
        message: error.message,
        status: error.response?.status,
        data: error.response?.data
      })
      showNotification(error.response?.data?.message || "Error al modificar habitación", "negative")
    } finally {
      loading.value = false
    }
  }

    const deleteHabitacion = async (id) => {
    loading.value = true
    try {
      await $q.dialog({
        title: "Confirmar eliminación",
        message: "¿Está seguro de que desea eliminar esta habitación? Esto no se puede deshacer.",
        cancel: true,
        persistent: true,
      }).onOk(async () => {
        console.log("Attempting to delete habitacion:", { id })
        await axios.delete(`${apiUrl}/Habitaciones/${id}`)
        showNotification("Habitación eliminada exitosamente")
        await fetchHabitaciones()
      })
    } catch (error) {
      console.error("Error deleting habitacion:", {
        message: error.message,
        status: error.response?.status,
        data: error.response?.data
      })
      let errorMessage = "Error al eliminar habitación";
      if (error.response?.status === 400) {
        errorMessage = error.response?.data?.message || "No se puede eliminar la habitación porque tiene reservas o asignaciones asociadas";
      } else if (error.response?.status === 404) {
        errorMessage = "La habitación no existe";
      } else {
        errorMessage = error.response?.data?.message || error.message;
      }
      showNotification(errorMessage, "negative")
    } finally {
      loading.value = false
    }
  }
   
  const selectHabitacion = async (id) => {
    try {
      const response = await axios.get(`${apiUrl}/Habitaciones/${id}`)
      habitacionForm.value = { ...response.data }
      selectedHabitacionId.value = id
      console.log("Habitación seleccionada:", habitacionForm.value)
      showNotification("Habitación seleccionada para edición")
    } catch (error) {
      console.error("Error selecting habitacion:", {
        message: error.message,
        status: error.response?.status,
        data: error.response?.data
      })
      showNotification(`Error al obtener habitación: ${error.response?.data?.message || error.message}`, "negative")
    }
  }

  const toggleServicio = async (habitacion) => {
    try {
      if (habitacion.EstaFueraDeServicio) {
        await axios.put(`${apiUrl}/Habitaciones/${habitacion.Id}`, {
          ...habitacion,
          EstaFueraDeServicio: false,
        })
        showNotification("Habitación puesta en servicio")
      } else {
        await axios.post(`${apiUrl}/Habitaciones/${habitacion.Id}/fuera-de-servicio`)
        showNotification("Habitación puesta fuera de servicio")
      }
      await fetchHabitaciones()
    } catch (error) {
      console.error("Error toggling servicio:", {
        message: error.message,
        status: error.response?.status,
        data: error.response?.data
      })
      showNotification(`Error al cambiar estado de habitación: ${error.response?.data?.message || error.message}`, "negative")
    }
  }

  const resetForm = () => {
    habitacionForm.value = { Numero: "", EstaFueraDeServicio: false }
    selectedHabitacionId.value = null
  }

  const getHabitacionClass = (fueraDeServicio) => {
    return fueraDeServicio ? "bg-red-1 text-red-10" : "bg-green-1 text-green-10"
  }

  const getHabitacionIcon = (fueraDeServicio) => {
    return fueraDeServicio ? "build" : "hotel"
  }

  const getEstadoColor = (fueraDeServicio) => {
    return fueraDeServicio ? "red" : "green"
  }

  return {
    habitaciones,
    habitacionesDisponibles,
    loading,
    selectedHabitacionId,
    habitacionForm,
    filtros,
    filteredHabitaciones,
    estadoOptions,
    formDisabled,
    fetchHabitaciones,
    buscarHabitacionesDisponibles,
    crearHabitacion,
    modificarHabitacion,
    deleteHabitacion,
    selectHabitacion,
    toggleServicio,
    resetForm,
    getHabitacionClass,
    getHabitacionIcon,
    getEstadoColor,
  }
}