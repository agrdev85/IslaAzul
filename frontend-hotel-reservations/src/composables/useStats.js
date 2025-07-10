import { ref, watch } from "vue";
import axios from "axios";
import { useReservas } from "./useReservas";

const apiUrl = "http://localhost:5014/api";

export function useStats() {
  const stats = ref({
    totalClientes: 0,
    reservasActivas: 0,
    habitacionesDisponibles: 0,
    ingresosMes: "0.00",
  });

  const { reservasActivas, fetchReservasActivas } = useReservas();

  const loadStats = async () => {
    try {
      const [clientesRes, habitacionesRes] = await Promise.all([
        axios.get(`${apiUrl}/Clientes`),
        axios.get(`${apiUrl}/Habitaciones`),
      ]);

      const clientesData = Array.isArray(clientesRes.data) ? clientesRes.data : [];
      const habitacionesData = Array.isArray(habitacionesRes.data) ? habitacionesRes.data : [];

      // Carga las reservas activas
      await fetchReservasActivas();

      const currentDate = new Date(); // 09:38 PM BST, 09/07/2025
      const currentMonth = currentDate.getMonth() + 1; // Julio = 7
      const currentYear = currentDate.getFullYear(); // 2025
      const monthlyReservations = reservasActivas.value.filter((reserva) => {
        const reservaDate = new Date(reserva.FechaEntrada);
        return (
          reservaDate.getMonth() + 1 === currentMonth &&
          reservaDate.getFullYear() === currentYear &&
          !isNaN(reservaDate.getTime()) // Evita fechas inválidas
        );
      });

      stats.value = {
        totalClientes: clientesData.length,
        reservasActivas: reservasActivas.value.length,
        habitacionesDisponibles: habitacionesData.filter((h) => !h.EstaFueraDeServicio).length,
        ingresosMes: monthlyReservations
          .reduce((sum, reserva) => sum + (parseFloat(reserva.Importe) || 0), 0)
          .toFixed(2),
      };
      console.log("Stats loaded at", new Date().toLocaleTimeString(), ":", stats.value);
    } catch (error) {
      console.error("Error loading stats at", new Date().toLocaleTimeString(), ":", error.response?.data || error.message);
      stats.value = {
        totalClientes: 0,
        reservasActivas: 0,
        habitacionesDisponibles: 0,
        ingresosMes: "0.00",
      };
    }
  };

  // Actualiza reactivamente cuando cambien las reservas activas
  watch(reservasActivas, () => {
    const currentDate = new Date();
    const currentMonth = currentDate.getMonth() + 1;
    const currentYear = currentDate.getFullYear();
    const monthlyReservations = reservasActivas.value.filter((reserva) => {
      const reservaDate = new Date(reserva.FechaEntrada);
      return (
        reservaDate.getMonth() + 1 === currentMonth &&
        reservaDate.getFullYear() === currentYear &&
        !isNaN(reservaDate.getTime())
      );
    });
    stats.value.reservasActivas = reservasActivas.value.length;
    stats.value.ingresosMes = monthlyReservations
      .reduce((sum, reserva) => sum + (parseFloat(reserva.Importe) || 0), 0)
      .toFixed(2);
    console.log("Stats updated at", new Date().toLocaleTimeString(), ":", stats.value);
  });

  return {
    stats,
    loadStats,
  };
}