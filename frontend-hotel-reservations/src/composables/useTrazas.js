import { ref } from "vue"
import { useQuasar } from "quasar"
import { DateTime } from "luxon"
import axios from "axios"

const apiUrl = "http://localhost:5014/api"

export function useTrazas() {
  const $q = useQuasar()

  const trazas = ref([])
  const loading = ref(false)

  const filtros = ref({
    busqueda: "",
    tablaAfectada: ""
  })

  const pagination = ref({
    sortBy: "Id",
    descending: true,
    page: 1,
    rowsPerPage: 15,
    rowsNumber: 0,
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

  const fetchTrazas = async () => {
    loading.value = true
    try {
      const response = await axios.get(`${apiUrl}/Trazas`, {
        params: {
          busqueda: filtros.value.busqueda || undefined,
          tablaAfectada: filtros.value.tablaAfectada || undefined,
          pagina: pagination.value.page,
          tamanoPagina: pagination.value.rowsPerPage,
          ordenarPor: pagination.value.sortBy,
          descendente: pagination.value.descending,
        },
      })

      console.log("Respuesta de la API:", response.data)

      trazas.value = response.data.data || []
      pagination.value.rowsNumber = response.data.total || 0
      console.log("Trazas mapeadas:", trazas.value)
      showNotification(`${trazas.value.length} trazas cargadas`)
    } catch (error) {
      console.error("Error al cargar trazas:", {
        message: error.message,
        status: error.response?.status,
        data: error.response?.data
      })
      showNotification(`Error al cargar trazas: ${error.response?.data?.message || error.message}`, "negative")
      trazas.value = []
    } finally {
      loading.value = false
    }
  }

  const getOperacionColor = (operacion) => {
    switch (operacion?.toLowerCase()) {
      case "create":
      case "crear":
        return "green"
      case "update":
      case "modificar":
        return "blue"
      case "delete":
      case "eliminar":
        return "red"
      default:
        return "purple"
    }
  }

  const formatFecha = (fecha) => {
    return DateTime.fromISO(fecha).toFormat("dd/MM/yyyy HH:mm")
  }

  return {
    trazas,
    loading,
    pagination,
    filtros,
    fetchTrazas,
    getOperacionColor,
    formatFecha,
  }
}