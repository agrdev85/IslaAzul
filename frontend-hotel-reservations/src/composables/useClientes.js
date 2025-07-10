import { ref, computed } from "vue"
import { useQuasar } from "quasar"
import axios from "axios"

const apiUrl = "http://localhost:5014/api";

export function useClientes() {
  const $q = useQuasar()

  const clientes = ref([])
  const loading = ref(false)
  const selectedClienteId = ref(null)

  const clienteForm = ref({
    NombreApellidos: "",
    CI: "",
    NumeroTelefonico: "",
    EsVip: false,
  })

  const filters = ref({
    busqueda: "",
    ciFiltro: "",
  })

  const pagination = ref({
    sortBy: "Id",
    descending: false,
    page: 1,
    rowsPerPage: 10,
    rowsNumber: 0,
  })

  const formDisabled = computed(() => {
    return (
      !clienteForm.value.NombreApellidos ||
      !clienteForm.value.CI ||
      !clienteForm.value.NumeroTelefonico ||
      !/^[a-zA-Z\s]{1,25}$/.test(clienteForm.value.NombreApellidos) ||
      !/^\d{11}$/.test(clienteForm.value.CI) ||
      !/^\d+$/.test(clienteForm.value.NumeroTelefonico)
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

  const fetchClientes = async () => {
    loading.value = true
    try {
      const response = await axios.get(`${apiUrl}/Clientes`, {
        params: {
          busqueda: filters.value.busqueda,
          ciFiltro: filters.value.ciFiltro,
        },
      })
      clientes.value = Array.isArray(response.data) ? response.data : []
      showNotification(`${clientes.value.length} clientes cargados`)
    } catch (error) {
      console.error("Error loading clientes:", error)
      showNotification("Error al cargar clientes", "negative")
    } finally {
      loading.value = false
    }
  }

  const crearCliente = async () => {
    loading.value = true
    try {
      await axios.post(`${apiUrl}/Clientes`, clienteForm.value)
      showNotification("Cliente creado exitosamente")
      resetForm()
      await fetchClientes()
    } catch (error) {
      showNotification(error.response?.data?.message || "Error al crear cliente", "negative")
    } finally {
      loading.value = false
    }
  }

  const modificarCliente = async () => {
    if (!selectedClienteId.value) return
    loading.value = true
    try {
      await axios.put(`${apiUrl}/Clientes/${selectedClienteId.value}`, clienteForm.value)
      showNotification("Cliente modificado exitosamente")
      resetForm()
      await fetchClientes()
    } catch (error) {
      showNotification(error.response?.data?.message || "Error al modificar cliente", "negative")
    } finally {
      loading.value = false
    }
  }

    const deleteCliente = async (id) => {
    loading.value = true
    console.log('$q:', $q) // Verificar si $q está disponible
    try {
      await $q.dialog({
        title: "Confirmar eliminación",
        message: "¿Está seguro de que desea eliminar este cliente?",
        cancel: true,
        persistent: true,
      }).onOk(async () => {
        console.log("Attempting to delete cliente:", { id })
        const response = await axios.delete(`${apiUrl}/Clientes/${id}`)
        showNotification("Cliente eliminado exitosamente")
        await fetchClientes()
      })
    } catch (error) {
      console.error("Error deleting cliente:", {
        message: error.message,
        status: error.response?.status,
        data: error.response?.data
      })
      let errorMessage = "Error al eliminar cliente"
      if (error.response?.status === 400) {
        errorMessage = error.response.data?.message || "No se puede eliminar el cliente debido a restricciones."
      } else if (error.response?.status === 404) {
        errorMessage = "El cliente no existe"
      } else {
        errorMessage = error.response?.data?.message || error.message
      }
      showNotification(errorMessage, "negative")
    } finally {
      loading.value = false
    }
  }

  const selectCliente = async (id) => {
    try {
      const response = await axios.get(`${apiUrl}/Clientes/${id}`)
      clienteForm.value = { ...response.data }
      selectedClienteId.value = id
      showNotification("Cliente seleccionado para edición")
    } catch (error) {
      showNotification("Error al obtener cliente", "negative")
    }
  }

  const resetForm = () => {
    clienteForm.value = { NombreApellidos: "", CI: "", NumeroTelefonico: "", EsVip: false }
    selectedClienteId.value = null
  }

  return {
    clientes,
    loading,
    selectedClienteId,
    pagination,
    clienteForm,
    filters,
    formDisabled,
    fetchClientes,
    crearCliente,
    modificarCliente,
    deleteCliente,
    selectCliente,
    resetForm,
  }
}
