import { ref, computed, watch } from "vue";
import { useQuasar } from "quasar";
import { DateTime } from "luxon";
import axios from "axios";

const apiUrl = "http://localhost:5014/api";

export function useReservas() {
  const $q = useQuasar();

  // Estado
  const reservas = ref([]);
  const reservasActivas = ref([]);
  const loading = ref(false);
  const selectedReservaId = ref(null);
  const selectedReservaAccion = ref(null);
  const motivoCancelacion = ref("");
  const clienteOptions = ref([]);
  const habitacionOptions = ref([]);
  const nuevaHabitacion = ref("");
  const fechaInicioFiltro = ref("");
  const fechaFinFiltro = ref("");
  const estadoFiltro = ref(null);

  // Opciones de estado para el filtro
  const estadoOptions = [
    { label: "Confirmada", value: "Confirmada" },
    { label: "En Hostal", value: "En Hostal" },
    { label: "Cancelada", value: "Cancelada" },
  ];

  // Formulario de reserva
  const reservaForm = ref({
    FechaEntrada: "",
    FechaSalida: "",
    ClienteId: null,
    HabitacionNumero: null, // Objeto para q-select { label, value }
  });

  // Propiedades computadas
  const reservaOptions = computed(() => {
    const options = reservas.value
      .filter((r) => !r.EstaCancelada)
      .map((r) => ({
        label: `Reserva #${r.Id} - ${r.ClienteNombre || "Desconocido"} - Hab. ${
          r.HabitacionNumero || "N/A"
        }`,
        value: r.Id,
        reserva: r,
      }));
    console.log("reservaOptions:", options);
    return options;
  });

  const formDisabled = computed(() => {
    const habitacionValue =
      typeof reservaForm.value.HabitacionNumero === "object" &&
      reservaForm.value.HabitacionNumero?.value
        ? reservaForm.value.HabitacionNumero.value
        : reservaForm.value.HabitacionNumero;
    const clienteIdValue =
      typeof reservaForm.value.ClienteId === "object" &&
      reservaForm.value.ClienteId?.value
        ? reservaForm.value.ClienteId.value
        : reservaForm.value.ClienteId;
    const isInvalid =
      !reservaForm.value.FechaEntrada ||
      !reservaForm.value.FechaSalida ||
      !clienteIdValue ||
      !habitacionValue ||
      !/^0[1-3][1-5]$/.test(habitacionValue) ||
      calcularNoches.value < 3;
    console.log("formDisabled:", {
      isInvalid,
      form: reservaForm.value,
      noches: calcularNoches.value,
      habitacionValue,
      clienteId: clienteIdValue,
    });
    return isInvalid;
  });

  const canRegisterArrival = computed(() => {
    if (!selectedReservaAccion.value?.value) return false;
    const reserva = reservas.value.find((r) => r.Id === selectedReservaAccion.value.value);
    const today = DateTime.now().toISODate();
    const result =
      reserva &&
      !reserva.EstaElClienteEnHostal &&
      !reserva.EstaCancelada &&
      today >= DateTime.fromISO(reserva.FechaEntrada).toISODate();
    console.log("canRegisterArrival:", {
      reservaId: selectedReservaAccion.value?.value,
      today,
      fechaEntrada: reserva?.FechaEntrada,
      result,
    });
    return result;
  });

  const canCancelReserva = computed(() => {
    if (!selectedReservaAccion.value?.value) return false;
    const reserva = reservas.value.find((r) => r.Id === selectedReservaAccion.value.value);
    const today = DateTime.now().toISODate();
    const result =
      reserva &&
      !reserva.EstaElClienteEnHostal &&
      !reserva.EstaCancelada &&
      today <= DateTime.fromISO(reserva.FechaEntrada).toISODate();
    console.log("canCancelReserva:", {
      reservaId: selectedReservaAccion.value?.value,
      today,
      fechaEntrada: reserva?.FechaEntrada,
      result,
    });
    return result;
  });

  const canChangeRoom = computed(() => {
    if (!selectedReservaAccion.value?.value) return false;
    const reserva = reservas.value.find((r) => r.Id === selectedReservaAccion.value.value);
    const today = DateTime.now().toISODate();
    const result =
      reserva &&
      !reserva.EstaElClienteEnHostal &&
      !reserva.EstaCancelada &&
      today <= DateTime.fromISO(reserva.FechaEntrada).toISODate();
    console.log("canChangeRoom:", {
      reservaId: selectedReservaAccion.value?.value,
      today,
      fechaEntrada: reserva?.FechaEntrada,
      result,
    });
    return result;
  });

  const calcularNoches = computed(() => {
    if (!reservaForm.value.FechaEntrada || !reservaForm.value.FechaSalida) return 0;
    const entrada = DateTime.fromISO(reservaForm.value.FechaEntrada);
    const salida = DateTime.fromISO(reservaForm.value.FechaSalida);
    const noches = Math.max(0, salida.diff(entrada, "days").days + 1);
    console.log("calcularNoches:", {
      entrada: reservaForm.value.FechaEntrada,
      salida: reservaForm.value.FechaSalida,
      noches,
    });
    return noches;
  });

  const clienteEsVip = computed(() => {
    if (!reservaForm.value.ClienteId) return false;
    const clienteId =
      typeof reservaForm.value.ClienteId === "object"
        ? reservaForm.value.ClienteId.value
        : reservaForm.value.ClienteId;
    const cliente = clienteOptions.value.find((c) => c.value === clienteId);
    console.log("clienteEsVip:", { clienteId, cliente, isVip: cliente?.EsVip });
    return cliente?.EsVip || false;
  });

  const calcularTotal = computed(() => {
    const noches = calcularNoches.value;
    const precioPorNoche = 10;
    let total = noches * precioPorNoche;
    if (clienteEsVip.value) {
      total *= 0.9; // 10% descuento VIP
    }
    console.log("calcularTotal:", { noches, total: total.toFixed(2) });
    return total.toFixed(2);
  });

  const showNotification = (message, type = "positive") => {
    $q.notify({
      message,
      color: type === "positive" ? "green" : "red",
      position: "top",
      timeout: 3000,
      icon: type === "positive" ? "check_circle" : "error",
    });
  };

  const fetchReservasActivas = async () => {
    loading.value = true;
    try {
      const response = await axios.get(`${apiUrl}/Reservas/activas`);
      console.log("Reservas activas API response:", response.data);
      reservasActivas.value = Array.isArray(response.data.data)
        ? response.data.data.map((reserva) => ({
            Id: reserva.Id?.toString() || "N/A",
            FechaReservacion: reserva.FechaReservacion
              ? DateTime.fromISO(reserva.FechaReservacion).toFormat("yyyy-MM-dd")
              : "N/A",
            FechaEntrada: reserva.FechaEntrada
              ? DateTime.fromISO(reserva.FechaEntrada).toFormat("yyyy-MM-dd")
              : "N/A",
            FechaSalida: reserva.FechaSalida
              ? DateTime.fromISO(reserva.FechaSalida).toFormat("yyyy-MM-dd")
              : "N/A",
            Importe: reserva.Importe?.toFixed(2) || "0.00",
            ClienteNombre: reserva.ClienteNombre || "Desconocido",
            ClienteId: reserva.ClienteId || null,
            HabitacionNumero: reserva.HabitacionNumero || "N/A",
            EstaElClienteEnHostal: reserva.EstaElClienteEnHostal || false,
            EstaCancelada: reserva.EstaCancelada || false,
            FechaCancelacion: reserva.FechaCancelacion
              ? DateTime.fromISO(reserva.FechaCancelacion).toFormat("yyyy-MM-dd")
              : "N/A",
            MotivoCancelacion: reserva.MotivoCancelacion || "N/A",
          }))
        : [];
      showNotification(`${reservasActivas.value.length} reservas activas cargadas`);
    } catch (error) {
      console.error("Error loading reservas activas:", error.response?.data || error.message);
      reservasActivas.value = [];
      showNotification(
        "Error al cargar reservas activas: " + (error.response?.data?.message || error.message),
        "negative"
      );
    } finally {
      loading.value = false;
    }
  };

      const revertirCheckIn = async (reservaId) => {
      loading.value = true;
      try {
        // Obtener los detalles de la reserva para validar la fecha
        const reserva = await axios.get(`${apiUrl}/Reservas/${reservaId}`);
        const fechaEntrada = DateTime.fromISO(reserva.data.data.FechaEntrada).toJSDate();
        const hoy = DateTime.now().toJSDate();

        // Validar si la fecha de entrada ya pasó
        if (fechaEntrada < hoy) {
          showNotification(
            "No se puede revertir el check-in porque la fecha de entrada ya pasó.",
            "negative"
          );
          return;
        }

        // Mostrar diálogo de confirmación si la validación pasa
        await $q.dialog({
          title: "Confirmar Reversión",
          message: "¿Está seguro de revertir el registro de llegada de esta reserva?",
          cancel: true,
          persistent: true,
        }).onOk(async () => {
          const response = await axios.put(`${apiUrl}/Reservas/${reservaId}/revertir-checkin`);
          console.log("Revertir check-in response:", response.data);
          await fetchReservasActivas(); // Recargar las reservas
          showNotification("Check-in revertido exitosamente");
        });
      } catch (error) {
        console.error("Error reverting check-in:", error.response?.data || error.message);
        showNotification(
          "Error al revertir el check-in: " + (error.response?.data?.message || error.message),
          "negative"
        );
      } finally {
        loading.value = false;
      }
    };

  const actualizarExpiradas = async () => {
    loading.value = true;
    try {
      const response = await axios.post(`${apiUrl}/Reservas/actualizar-expiradas`);
      console.log("Actualizar expiradas response:", response.data);
      await fetchReservasActivas(); // Recargar las reservas
      showNotification("Reservas expiradas actualizadas exitosamente");
    } catch (error) {
      console.error("Error updating expired reservations:", error.response?.data || error.message);
      showNotification(
        "Error al actualizar reservas expiradas: " + (error.response?.data?.message || error.message),
        "negative"
      );
    } finally {
      loading.value = false;
    }
  };

  // Funciones de API
  const fetchClientes = async () => {
    try {
      const response = await axios.get(`${apiUrl}/clientes`);
      console.log("Clientes API response:", response.data);
      clienteOptions.value = Array.isArray(response.data)
        ? response.data.map((cliente) => ({
            label: `${cliente.NombreApellidos} (CI: ${cliente.CI || "N/A"})`.trim(),
            value: cliente.Id,
            EsVip: cliente.EsVip || false,
          }))
        : [];
      if (clienteOptions.value.length === 0) {
        showNotification("No se encontraron clientes", "warning");
      } else {
        showNotification(`${clienteOptions.value.length} clientes cargados`);
      }
    } catch (error) {
      console.error("Error loading clientes:", error.response?.data || error.message);
      clienteOptions.value = [];
      showNotification(
        "Error al cargar clientes: " + (error.response?.data?.message || error.message),
        "negative"
      );
    }
  };

  const fetchHabitacionesDisponibles = async () => {
    if (!reservaForm.value.FechaEntrada || !reservaForm.value.FechaSalida) {
      habitacionOptions.value = [];
      showNotification(
        "Seleccione fechas de entrada y salida para cargar habitaciones disponibles",
        "warning"
      );
      return;
    }
    try {
      const response = await axios.get(`${apiUrl}/Habitaciones/habitaciones-disponibles`, {
        params: {
          fechaInicio: reservaForm.value.FechaEntrada,
          fechaFin: reservaForm.value.FechaSalida,
        },
      });
      console.log("Habitaciones disponibles API response:", response.data);
      habitacionOptions.value = Array.isArray(response.data)
        ? response.data.map((num) => ({
            label: `Habitación ${num}`,
            value: num,
          }))
        : [];
      if (habitacionOptions.value.length === 0) {
        showNotification("No hay habitaciones disponibles para las fechas seleccionadas", "warning");
      }
    } catch (error) {
      console.error("Error loading habitaciones disponibles:", error.response?.data || error.message);
      habitacionOptions.value = [];
      showNotification(
        "Error al cargar habitaciones disponibles: " + (error.response?.data?.message || error.message),
        "negative"
      );
    }
  };

  const fetchReservas = async () => {
    loading.value = true;
    try {
      const params = {};
      if (fechaInicioFiltro.value) params.fechaInicio = fechaInicioFiltro.value;
      if (fechaFinFiltro.value) params.fechaFin = fechaFinFiltro.value;
      if (estadoFiltro.value) params.estado = estadoFiltro.value;
      const response = await axios.get(`${apiUrl}/Reservas`, { params });
      console.log("Reservas API response:", response.data);
      reservas.value = Array.isArray(response.data.data)
        ? response.data.data.map((reserva) => ({
            Id: reserva.Id?.toString() || "N/A",
            FechaReservacion: reserva.FechaReservacion
              ? DateTime.fromISO(reserva.FechaReservacion).toFormat("yyyy-MM-dd")
              : "N/A",
            FechaEntrada: reserva.FechaEntrada
              ? DateTime.fromISO(reserva.FechaEntrada).toFormat("yyyy-MM-dd")
              : "N/A",
            FechaSalida: reserva.FechaSalida
              ? DateTime.fromISO(reserva.FechaSalida).toFormat("yyyy-MM-dd")
              : "N/A",
            Importe: reserva.Importe?.toFixed(2) || "0.00",
            ClienteNombre: reserva.ClienteNombre || "Desconocido",
            ClienteId: reserva.ClienteId || null,
            HabitacionNumero: reserva.HabitacionNumero || "N/A",
            EstaElClienteEnHostal: reserva.EstaElClienteEnHostal || false,
            EstaCancelada: reserva.EstaCancelada || false,
            FechaCancelacion: reserva.FechaCancelacion
              ? DateTime.fromISO(reserva.FechaCancelacion).toFormat("yyyy-MM-dd")
              : "N/A",
            MotivoCancelacion: reserva.MotivoCancelacion || "N/A",
          }))
        : [];
      showNotification(`${reservas.value.length} reservas cargadas`);
    } catch (error) {
      console.error("Error loading reservas:", error.response?.data || error.message);
      reservas.value = [];
      showNotification(
        "Error al cargar reservas: " + (error.response?.data?.message || error.message),
        "negative"
      );
    } finally {
      loading.value = false;
    }
  };

  const crearReserva = async () => {
    loading.value = true;
    try {
      const habitacionValue =
        typeof reservaForm.value.HabitacionNumero === "object"
          ? reservaForm.value.HabitacionNumero?.value
          : reservaForm.value.HabitacionNumero;
      const clienteId =
        typeof reservaForm.value.ClienteId === "object"
          ? reservaForm.value.ClienteId?.value
          : reservaForm.value.ClienteId;
      const payload = {
        FechaEntrada: reservaForm.value.FechaEntrada,
        FechaSalida: reservaForm.value.FechaSalida,
        ClienteId: clienteId,
        HabitacionNumero: habitacionValue,
        Importe: parseFloat(calcularTotal.value),
      };
      console.log("Crear reserva payload:", payload);
      const response = await axios.post(`${apiUrl}/Reservas`, payload);
      console.log("Crear reserva response:", response.data);
      showNotification("Reserva creada exitosamente");
      resetForm();
      await Promise.all([fetchReservas(), fetchReservasActivas()]);
      return response.data.data;
    } catch (error) {
      console.error("Error creating reserva:", error.response?.data || error.message);
      showNotification(
        error.response?.data?.message || "Error al crear reserva",
        "negative"
      );
      throw error;
    } finally {
      loading.value = false;
    }
  };

  const modificarReserva = async () => {
    if (!selectedReservaId.value) return;
    loading.value = true;
    try {
      const habitacionValue =
        typeof reservaForm.value.HabitacionNumero === "object"
          ? reservaForm.value.HabitacionNumero?.value
          : reservaForm.value.HabitacionNumero;
      const clienteId =
        typeof reservaForm.value.ClienteId === "object"
          ? reservaForm.value.ClienteId?.value
          : reservaForm.value.ClienteId;
      const payload = {
        FechaEntrada: reservaForm.value.FechaEntrada,
        FechaSalida: reservaForm.value.FechaSalida,
        ClienteId: clienteId,
        HabitacionNumero: habitacionValue,
        Importe: parseFloat(calcularTotal.value),
      };
      console.log("Modificar reserva payload:", payload);
      const response = await axios.put(`${apiUrl}/Reservas/${selectedReservaId.value}`, payload);
      console.log("Modificar reserva response:", response.data);
      showNotification("Reserva modificada exitosamente");
      resetForm();
      await Promise.all([fetchReservas(), fetchReservasActivas()]);
    } catch (error) {
      console.error("Error modifying reserva:", error.response?.data || error.message);
      showNotification(
        error.response?.data?.message || "Error al modificar reserva",
        "negative"
      );
      throw error;
    } finally {
      loading.value = false;
    }
  };

  const cancelarReserva = async () => {
    if (!selectedReservaAccion.value?.value || !motivoCancelacion.value) return;
    try {
      console.log("Cancelling reserva:", {
        id: selectedReservaAccion.value.value,
        motivo: motivoCancelacion.value,
      });
      const response = await axios.put(
        `${apiUrl}/Reservas/${selectedReservaAccion.value.value}/cancelar`,
        `"${motivoCancelacion.value}"`,
        {
          headers: { "Content-Type": "application/json" },
        }
      );
      console.log("Cancelar reserva response:", response.data);
      showNotification("Reserva cancelada exitosamente");
      selectedReservaAccion.value = null;
      motivoCancelacion.value = "";
      await Promise.all([fetchReservas(), fetchReservasActivas()]);
    } catch (error) {
      console.error("Error cancelling reserva:", error.response?.data || error.message);
      showNotification(
        error.response?.data?.message || "Error al cancelar reserva",
        "negative"
      );
      throw error;
    }
  };

  const registrarLlegada = async () => {
    if (!selectedReservaAccion.value?.value) return;
    try {
      console.log("Registering llegada for reserva:", selectedReservaAccion.value.value);
      const response = await axios.put(
        `${apiUrl}/Reservas/${selectedReservaAccion.value.value}/registrar-llegada`
      );
      console.log("Registrar llegada response:", response.data);
      showNotification("Llegada registrada exitosamente");
      selectedReservaAccion.value = null;
      await Promise.all([fetchReservas(), fetchReservasActivas()]);
    } catch (error) {
      console.error("Error registering llegada:", error.response?.data || error.message);
      showNotification(
        error.response?.data?.message || "Error al registrar llegada",
        "negative"
      );
      throw error;
    }
  };

  const cambiarHabitacion = async () => {
    if (
      !selectedReservaAccion.value?.value ||
      !nuevaHabitacion.value ||
      !/^0[1-3][1-5]$/.test(nuevaHabitacion.value)
    ) {
      showNotification("Por favor, ingrese un número de habitación válido (formato 0XY)", "negative");
      return;
    }
    try {
      console.log("Changing habitacion:", {
        reservaId: selectedReservaAccion.value.value,
        nuevaHabitacion: nuevaHabitacion.value,
      });
      const response = await axios.put(
        `${apiUrl}/Reservas/${selectedReservaAccion.value.value}/cambiar-habitacion`,
        `"${nuevaHabitacion.value}"`,
        {
          headers: { "Content-Type": "application/json" },
        }
      );
      console.log("Cambiar habitacion response:", response.data);
      showNotification("Habitación cambiada exitosamente");
      nuevaHabitacion.value = "";
      await Promise.all([fetchReservas(), fetchReservasActivas()]);
    } catch (error) {
      console.error("Error changing habitacion:", error.response?.data || error.message);
      showNotification(
        error.response?.data?.message || "Error al cambiar habitación",
        "negative"
      );
      throw error;
    }
  };

  const selectReserva = async (id) => {
    try {
      console.log("Fetching reserva with ID:", id);
      const response = await axios.get(`${apiUrl}/Reservas/${id}`, {
        headers: { "Accept": "application/json" },
      });
      console.log("Reserva API response:", response.data);
      const reserva = response.data.data;
      const cliente =
        clienteOptions.value.find((c) => c.value === reserva.ClienteId) || {
          label: `${reserva.ClienteNombre} (CI: ${reserva.ClienteId || "N/A"})`.trim(),
          value: reserva.ClienteId,
        };
      reservaForm.value = {
        FechaEntrada: DateTime.fromISO(reserva.FechaEntrada).toFormat("yyyy-MM-dd"),
        FechaSalida: DateTime.fromISO(reserva.FechaSalida).toFormat("yyyy-MM-dd"),
        ClienteId: cliente,
        HabitacionNumero: {
          label: `Habitación ${reserva.HabitacionNumero}`,
          value: reserva.HabitacionNumero,
        },
      };
      selectedReservaId.value = id;
      await fetchHabitacionesDisponibles();
      showNotification("Reserva seleccionada para edición");
    } catch (error) {
      console.error("Error fetching reserva:", error.response?.data || error.message);
      showNotification(
        "Error al obtener reserva: " + (error.response?.data?.message || error.message),
        "negative"
      );
    }
  };

  const selectReservaFromList = (reserva) => {
    selectedReservaAccion.value = {
      label: `Reserva #${reserva.Id} - ${reserva.ClienteNombre || "Desconocido"} - Hab. ${
        reserva.HabitacionNumero || "N/A"
      }`,
      value: reserva.Id,
      reserva,
    };
    console.log("Selected reserva for action:", selectedReservaAccion.value);
    showNotification("Reserva seleccionada para acciones");
  };

  const resetForm = () => {
    reservaForm.value = { FechaEntrada: "", FechaSalida: "", ClienteId: null, HabitacionNumero: null };
    selectedReservaId.value = null;
    selectedReservaAccion.value = null;
    motivoCancelacion.value = "";
    nuevaHabitacion.value = "";
    habitacionOptions.value = [];
    console.log("Form reset:", reservaForm.value);
  };

  const getReservaColor = (cancelada, enHostal) => {
    if (cancelada) return "red";
    if (enHostal) return "green";
    return "blue";
  };

  const getReservaChipColor = (reserva) => {
    if (reserva.EstaCancelada) return "red";
    if (reserva.EstaElClienteEnHostal) return "green";
    return "blue";
  };

  const getReservaStatus = (reserva) => {
    if (reserva.EstaCancelada) return "Cancelada";
    if (reserva.EstaElClienteEnHostal) return "En Hostal";
    return "Confirmada";
  };

  // Observar cambios en las fechas para actualizar habitaciones disponibles
  watch(
    () => [reservaForm.value.FechaEntrada, reservaForm.value.FechaSalida],
    async ([newFechaEntrada, newFechaSalida]) => {
      console.log("Watch fechas:", { newFechaEntrada, newFechaSalida, form: reservaForm.value });
      if (newFechaEntrada && newFechaSalida) {
        await fetchHabitacionesDisponibles();
      } else {
        habitacionOptions.value = [];
        reservaForm.value.HabitacionNumero = null;
      }
    },
    { immediate: false }
  );

  return {
    reservas,
    reservasActivas,
    loading,
    selectedReservaId,
    selectedReservaAccion,
    reservaForm,
    motivoCancelacion,
    clienteOptions,
    reservaOptions,
    nuevaHabitacion,
    habitacionOptions,
    fechaInicioFiltro,
    fechaFinFiltro,
    estadoFiltro,
    estadoOptions,
    formDisabled,
    canRegisterArrival,
    canCancelReserva,
    canChangeRoom,
    calcularNoches,
    calcularTotal,
    clienteEsVip,
    fetchClientes,
    fetchReservas,
    fetchReservasActivas,
    fetchHabitacionesDisponibles,
    crearReserva,
    modificarReserva,
    cancelarReserva,
    registrarLlegada,
    cambiarHabitacion,
    selectReserva,
    selectReservaFromList,
    resetForm,
    getReservaColor,
    getReservaChipColor,
    getReservaStatus,
    revertirCheckIn,
    actualizarExpiradas,
  };
}