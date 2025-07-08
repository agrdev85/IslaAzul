import { ref } from "vue"
import axios from "axios"

const apiUrl = "http://localhost:5014/api"

export function useStats() {
  const stats = ref({
    totalClientes: 0,
    reservasActivas: 0,
    habitacionesDisponibles: 0,
    ingresosMes: "0",
  })

  const loadStats = async () => {
    try {
      const [clientesRes, reservasRes, habitacionesRes] = await Promise.all([
        axios.get(`${apiUrl}/Clientes`),
        axios.get(`${apiUrl}/Reservas/activas`),
        axios.get(`${apiUrl}/Habitaciones`),
      ])

      const clientesData = Array.isArray(clientesRes.data) ? clientesRes.data : []
      const reservasData = Array.isArray(reservasRes.data) ? reservasRes.data : []
      const habitacionesData = Array.isArray(habitacionesRes.data) ? habitacionesRes.data : []

      stats.value = {
        totalClientes: clientesData.length,
        reservasActivas: reservasData.length,
        habitacionesDisponibles: habitacionesData.filter((h) => !h.EstaFueraDeServicio).length,
        ingresosMes: "0.00",
      }
    } catch (error) {
      console.error("Error loading stats:", error)
      throw error
    }
  }

  return {
    stats,
    loadStats,
  }
}
