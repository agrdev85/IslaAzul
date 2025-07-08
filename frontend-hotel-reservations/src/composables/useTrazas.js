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
          busqueda: filtros.value.busqueda,
          pagina: pagination.value.page,
          tamanoPagina: pagination.value.rowsPerPage,
          ordenarPor: pagination.value.sortBy,
          descendente: pagination.value.descending,
        },
      })

      trazas.value =
        response.data.data?.map((t) => ({
          ...t,
          Fecha: t.Fecha,
        })) || []

      pagination.value.rowsNumber = response.data.total || 0
      showNotification(`${trazas.value.length} trazas cargadas`)
    } catch (error) {
      showNotification("Error al cargar trazas", "negative")
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
