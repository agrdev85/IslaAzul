import { boot } from "quasar/wrappers"
import axios from "axios"
import { Notify } from "quasar"

// Configuración base de la API
const api = axios.create({
  baseURL: process.env.VUE_APP_API_URL || "https://localhost:5014/api",
  timeout: 10000,
  headers: {
    "Content-Type": "application/json",
    Accept: "application/json",
  },
})

// Interceptor para requests
api.interceptors.request.use(
  (config) => {
    // Aquí puedes agregar tokens de autenticación si los necesitas
    // const token = localStorage.getItem('token')
    // if (token) {
    //   config.headers.Authorization = `Bearer ${token}`
    // }

    console.log(`🚀 ${config.method?.toUpperCase()} ${config.url}`)
    return config
  },
  (error) => {
    console.error("❌ Request Error:", error)
    return Promise.reject(error)
  },
)

// Interceptor para responses
api.interceptors.response.use(
  (response) => {
    console.log(`✅ ${response.config.method?.toUpperCase()} ${response.config.url} - ${response.status}`)
    return response
  },
  (error) => {
    console.error("❌ Response Error:", error)

    // Manejo centralizado de errores
    const message =
      error.response?.data?.message ||
      error.response?.data?.title ||
      error.message ||
      "Error de conexión con el servidor"

    Notify.create({
      type: "negative",
      message: message,
      position: "top",
      timeout: 5000,
      actions: [
        {
          icon: "close",
          color: "white",
          round: true,
          handler: () => {},
        },
      ],
    })

    return Promise.reject(error)
  },
)

export default boot(({ app }) => {
  app.config.globalProperties.$axios = axios
  app.config.globalProperties.$api = api
})

export { api }
