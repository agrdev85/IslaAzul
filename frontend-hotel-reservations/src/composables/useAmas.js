import { ref, computed } from "vue"
import { useQuasar } from "quasar"
import axios from "axios"

const apiUrl = "http://localhost:5014/api"

export function useAmas() {
  const $q = useQuasar()

  const amas = ref([])
  const habitacionesAma = ref([])
  const loading = ref(false)
  const selectedAmaId = ref(null)

  const amaForm = ref({
    NombreApellidos: "",
    CI: "",
    NumeroTelefonico: "",
  })

  const asignacionForm = ref({
    AmaDeLlavesId: null,
    HabitacionNumero: "",
    Turno: null,
  })

  const filtros = ref({
    busqueda: "",
    ciFiltro: "",
  })

  const turnosOptions = ["Mañana", "Tarde", "Noche"]

  const habitacionesAmaColumns = [
    { name: "HabitacionNumero", label: "Habitación", field: "HabitacionNumero", sortable: true },
    { name: "Turno", label: "Turno", field: "Turno", sortable: true },
  ]

  const amaOptions = computed(() => {
    return amas.value.map((ama) => ({
      label: `${ama.NombreApellidos} (CI: ${ama.CI})`,
      value: ama.Id,
    }))
  })

  const filteredAmas = computed(() => {
    let filtered = amas.value

    if (filtros.value.busqueda) {
      const search = filtros.value.busqueda.toLowerCase()
      filtered = filtered.filter(
        (ama) =>
          ama.NombreApellidos.toLowerCase().includes(search) ||
          ama.CI.includes(search) ||
          ama.NumeroTelefonico.includes(search),
      )
    }

    if (filtros.value.ciFiltro) {
      filtered = filtered.filter((ama) => ama.CI.includes(filtros.value.ciFiltro))
    }

    return filtered
  })

  const formDisabled = computed(() => {
    return (
      !amaForm.value.NombreApellidos ||
      !amaForm.value.CI ||
      !amaForm.value.NumeroTelefonico ||
      !/^[a-zA-Z\s]{1,25}$/.test(amaForm.value.NombreApellidos) ||
      !/^\d{11}$/.test(amaForm.value.CI) ||
      !/^\d+$/.test(amaForm.value.NumeroTelefonico)
    )
  })

  const asignacionFormDisabled = computed(() => {
    return (
      !asignacionForm.value.AmaDeLlavesId ||
      !asignacionForm.value.HabitacionNumero ||
      !asignacionForm.value.Turno ||
      !/^0[1-3][1-5]$/.test(asignacionForm.value.HabitacionNumero)
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

  const fetchAmas = async () => {
    loading.value = true
    try {
      const response = await axios.get(`${apiUrl}/AmasDeLlaves`)
      amas.value = Array.isArray(response.data) ? response.data : []
      showNotification(`${amas.value.length} amas de llaves cargadas`)
    } catch (error) {
      console.error("Error loading amas:", error)
      showNotification("Error al cargar amas de llaves", "negative")
    } finally {
      loading.value = false
    }
  }

  const crearAma = async () => {
    loading.value = true
    try {
      await axios.post(`${apiUrl}/AmasDeLlaves`, amaForm.value)
      showNotification("Ama de llaves creada exitosamente")
      resetForm()
      await fetchAmas()
    } catch (error) {
      showNotification(error.response?.data?.message || "Error al crear ama de llaves", "negative")
    } finally {
      loading.value = false
    }
  }

  const modificarAma = async () => {
    if (!selectedAmaId.value) return
    loading.value = true
    try {
      await axios.put(`${apiUrl}/AmasDeLlaves/${selectedAmaId.value}`, amaForm.value)
      showNotification("Ama de llaves modificada exitosamente")
      resetForm()
      await fetchAmas()
    } catch (error) {
      showNotification(error.response?.data?.message || "Error al modificar ama de llaves", "negative")
    } finally {
      loading.value = false
    }
  }

  const deleteAma = async (id) => {
    try {
      await $q.dialog({
        title: "Confirmar eliminación",
        message: "¿Está seguro de que desea eliminar esta ama de llaves?",
        cancel: true,
        persistent: true,
      })

      await axios.delete(`${apiUrl}/AmasDeLlaves/${id}`)
      showNotification("Ama de llaves eliminada exitosamente")
      await fetchAmas()
    } catch (error) {
      if (error !== undefined) {
        showNotification(error.response?.data?.message || "Error al eliminar ama de llaves", "negative")
      }
    }
  }

  const selectAma = async (id) => {
    try {
      const response = await axios.get(`${apiUrl}/AmasDeLlaves/${id}`)
      amaForm.value = { ...response.data }
      selectedAmaId.value = id
      showNotification("Ama de llaves seleccionada para edición")
    } catch (error) {
      showNotification("Error al obtener ama de llaves", "negative")
    }
  }

  const asignarHabitacion = async () => {
    if (asignacionFormDisabled.value) return

    try {
      await axios.post(`${apiUrl}/AmasDeLlaves/asignar-habitacion-ama`, {
        AmaDeLlavesId: asignacionForm.value.AmaDeLlavesId.value || asignacionForm.value.AmaDeLlavesId,
        HabitacionNumero: asignacionForm.value.HabitacionNumero,
        Turno: asignacionForm.value.Turno,
      })
      showNotification("Habitación asignada exitosamente")
      asignacionForm.value = { AmaDeLlavesId: null, HabitacionNumero: "", Turno: null }
      await verHabitacionesAma()
    } catch (error) {
      showNotification(error.response?.data?.message || "Error al asignar habitación", "negative")
    }
  }

  const desasignarHabitacion = async () => {
    try {
      await axios.post(`${apiUrl}/AmasDeLlaves/desasignar-habitacion`, {
        AmaDeLlavesId: asignacionForm.value.AmaDeLlavesId.value || asignacionForm.value.AmaDeLlavesId,
        HabitacionNumero: asignacionForm.value.HabitacionNumero,
      })
      showNotification("Habitación desasignada exitosamente")
      asignacionForm.value = { AmaDeLlavesId: null, HabitacionNumero: "", Turno: null }
      await verHabitacionesAma()
    } catch (error) {
      showNotification(error.response?.data?.message || "Error al desasignar habitación", "negative")
    }
  }

  const verHabitacionesAma = async () => {
    const amaId = asignacionForm.value.AmaDeLlavesId?.value || asignacionForm.value.AmaDeLlavesId
    if (!amaId) return

    try {
      const response = await axios.get(`${apiUrl}/AmasDeLlaves/habitaciones-por-ama-de-llaves/${amaId}`)
      const habitacionesData = Array.isArray(response.data) ? response.data : []

      habitacionesAma.value = habitacionesData.map((h) => ({
        HabitacionNumero: h.Habitacion?.Numero || h.HabitacionNumero || "Sin asignar",
        Turno: h.Turno || "N/A",
      }))

      showNotification(`${habitacionesAma.value.length} habitaciones asignadas`)
    } catch (error) {
      console.error("Error loading habitaciones por ama:", error)
      showNotification("Error al cargar habitaciones asignadas", "negative")
      habitacionesAma.value = []
    }
  }

  const verHabitacionesAmaDirecto = async (amaId) => {
    asignacionForm.value.AmaDeLlavesId = amaOptions.value.find((opt) => opt.value === amaId)
    await verHabitacionesAma()
  }

  const resetForm = () => {
    amaForm.value = { NombreApellidos: "", CI: "", NumeroTelefonico: "" }
    selectedAmaId.value = null
  }

  return {
    amas,
    habitacionesAma,
    loading,
    selectedAmaId,
    amaForm,
    asignacionForm,
    filtros,
    filteredAmas,
    amaOptions,
    turnosOptions,
    habitacionesAmaColumns,
    formDisabled,
    asignacionFormDisabled,
    fetchAmas,
    crearAma,
    modificarAma,
    deleteAma,
    selectAma,
    asignarHabitacion,
    desasignarHabitacion,
    verHabitacionesAma,
    verHabitacionesAmaDirecto,
    resetForm,
  }
}
