# 📋 Documentación del Frontend - Sistema Hostal Isla Azul

## 🏗️ Arquitectura Modular

El frontend ha sido refactorizado de un componente monolítico a una arquitectura modular con las siguientes ventajas:

### ✅ **Beneficios de la Modularización:**
- **Mantenibilidad**: Cada módulo es independiente y fácil de mantener
- **Reutilización**: Los composables pueden ser reutilizados en otros componentes
- **Testabilidad**: Cada función puede ser probada de forma aislada
- **Escalabilidad**: Fácil agregar nuevas funcionalidades
- **Separación de responsabilidades**: Lógica separada de la presentación

---

## 📁 Estructura del Proyecto

\`\`\`
src/
├── pages/
│   └── IndexPage.vue          # Página principal con pestañas
├── components/
│   ├── ClientesTab.vue        # Gestión de clientes
│   ├── ReservasTab.vue        # Gestión de reservas
│   ├── HabitacionesTab.vue    # Gestión de habitaciones
│   ├── AmasTab.vue            # Gestión de amas de llaves
│   └── TrazasTab.vue          # Visualización de trazas
├── composables/
│   ├── useStats.js            # Estadísticas del dashboard
│   ├── useClientes.js         # Lógica de clientes
│   ├── useReservas.js         # Lógica de reservas
│   ├── useHabitaciones.js     # Lógica de habitaciones
│   ├── useAmas.js             # Lógica de amas de llaves
│   └── useTrazas.js           # Lógica de trazas
└── utils/
    ├── validators.js          # Reglas de validación
    └── constants.js           # Constantes y configuraciones
\`\`\`

---

## 🔌 Funciones del Frontend por Endpoint

### 📊 **Dashboard y Estadísticas**

#### `useStats.js`
\`\`\`javascript
// GET /api/Clientes, /api/Reservas/activas, /api/Habitaciones
const loadStats = async () => {
  // Carga estadísticas del dashboard
  // - Total de clientes
  // - Reservas activas
  // - Habitaciones disponibles
  // - Ingresos del mes
}
\`\`\`

---

### 👥 **Gestión de Clientes**

#### `useClientes.js`
\`\`\`javascript
// GET /api/Clientes
const fetchClientes = async () => {
  // Obtiene lista de clientes con filtros
  // Parámetros: busqueda, ciFiltro
}

// POST /api/Clientes
const crearCliente = async () => {
  // Crea un nuevo cliente
  // Validaciones: nombre (25 chars), CI (11 dígitos), teléfono
}

// PUT /api/Clientes/{id}
const modificarCliente = async () => {
  // Modifica un cliente existente
  // Requiere: selectedClienteId
}

// GET /api/Clientes/{id}
const selectCliente = async (id) => {
  // Obtiene datos de un cliente específico para edición
}

// DELETE /api/Clientes/{id}
const deleteCliente = async (id) => {
  // Elimina un cliente con confirmación
  // Muestra diálogo de confirmación antes de eliminar
}
\`\`\`

**🔧 Validaciones:**
- Nombre: Máximo 25 caracteres, solo letras y espacios
- CI: Exactamente 11 dígitos numéricos, único
- Teléfono: Solo números
- VIP: Boolean para descuentos del 10%

---

### 🏨 **Gestión de Reservas**

#### `useReservas.js`
\`\`\`javascript
// GET /api/Reservas/activas
const fetchReservasActivas = async () => {
  // Obtiene reservas activas
  // Formatea fechas con Luxon
}

// POST /api/Reservas
const crearReserva = async () => {
  // Crea nueva reserva
  // Calcula automáticamente: noches, total, descuento VIP
}

// PUT /api/Reservas/{id}
const modificarReserva = async () => {
  // Modifica reserva existente
  // Solo si no está cancelada y cliente no está en hostal
}

// PUT /api/Reservas/{id}/cancelar
const cancelarReserva = async () => {
  // Cancela una reserva con motivo
  // Requiere: motivoCancelacion
}

// PUT /api/Reservas/{id}/registrar-llegada
const registrarLlegada = async () => {
  // Registra llegada del cliente
  // Solo disponible en fecha de entrada
}

// PUT /api/Reservas/{id}/cambiar-habitacion
const cambiarHabitacion = async () => {
  // Cambia habitación de una reserva
  // Valida disponibilidad de nueva habitación
}
\`\`\`

**🔧 Validaciones:**
- Fecha entrada: No anterior a hoy
- Fecha salida: Posterior a entrada, mínimo 3 días
- Habitación: Formato 0XY (X=1-3, Y=1-5)
- Cliente: Debe existir en el sistema

**💰 Cálculos Automáticos:**
- Noches = (Fecha salida - Fecha entrada) + 1
- Total = Noches × $10 USD
- Descuento VIP = Total × 0.9 (10% descuento)

---

### 🏠 **Gestión de Habitaciones**

#### `useHabitaciones.js`
\`\`\`javascript
// GET /api/Habitaciones
const fetchHabitaciones = async () => {
  // Obtiene lista de habitaciones
  // Filtros: búsqueda por número, estado (disponible/fuera de servicio)
}

// GET /api/Habitaciones/habitaciones-disponibles
const buscarHabitacionesDisponibles = async () => {
  // Busca habitaciones disponibles por rango de fechas
  // Parámetros: fechaInicio, fechaFin
}

// POST /api/Habitaciones
const crearHabitacion = async () => {
  // Crea nueva habitación
  // Validación: formato 0XY único
}

// PUT /api/Habitaciones/{id}
const modificarHabitacion = async () => {
  // Modifica habitación existente
}

// DELETE /api/Habitaciones/{id}
const deleteHabitacion = async (id) => {
  // Elimina habitación con confirmación
}

// POST /api/Habitaciones/{id}/fuera-de-servicio
const toggleServicio = async (habitacion) => {
  // Cambia estado de servicio de habitación
  // Alterna entre disponible/fuera de servicio
}
\`\`\`

**🔧 Validaciones:**
- Número: Formato 0XY (X=piso 1-3, Y=habitación 1-5)
- Unicidad: No puede haber números duplicados
- Estado: Boolean para fuera de servicio

---

### 🧹 **Gestión de Amas de Llaves**

#### `useAmas.js`
\`\`\`javascript
// GET /api/AmasDeLlaves
const fetchAmas = async () => {
  // Obtiene lista de amas de llaves
  // Filtros: búsqueda por nombre, CI, teléfono
}

// POST /api/AmasDeLlaves
const crearAma = async () => {
  // Crea nueva ama de llaves
  // Validaciones similares a clientes
}

// PUT /api/AmasDeLlaves/{id}
const modificarAma = async () => {
  // Modifica ama de llaves existente
}

// DELETE /api/AmasDeLlaves/{id}
const deleteAma = async (id) => {
  // Elimina ama de llaves con confirmación
}

// POST /api/AmasDeLlaves/asignar-habitacion-ama
const asignarHabitacion = async () => {
  // Asigna habitación a ama de llaves
  // Parámetros: AmaDeLlavesId, HabitacionNumero, Turno
}

// POST /api/AmasDeLlaves/desasignar-habitacion
const desasignarHabitacion = async () => {
  // Desasigna habitación de ama de llaves
  // Parámetros: AmaDeLlavesId, HabitacionNumero
}

// GET /api/AmasDeLlaves/habitaciones-por-ama-de-llaves/{id}
const verHabitacionesAma = async () => {
  // Obtiene habitaciones asignadas a un ama de llaves
  // Muestra: número de habitación y turno
}
\`\`\`

**🔧 Validaciones:**
- Nombre: Máximo 25 caracteres, solo letras
- CI: 11 dígitos únicos
- Teléfono: Solo números
- Habitación: Formato 0XY
- Turno: Mañana, Tarde, Noche

---

### 📋 **Sistema de Trazas**

#### `useTrazas.js`
\`\`\`javascript
// GET /api/Trazas
const fetchTrazas = async () => {
  // Obtiene registro de trazas del sistema
  // Parámetros: busqueda, paginación, ordenamiento
  // Muestra: operación, tabla afectada, fecha, detalles
}
\`\`\`

**🎨 Colores por Operación:**
- **Verde**: CREATE/Crear
- **Azul**: UPDATE/Modificar  
- **Rojo**: DELETE/Eliminar
- **Púrpura**: Otras operaciones

---

## 🎨 **Mejoras de Interfaz (Versión 12)**

### 🌈 **Paleta de Colores Mejorada:**
- **Rojo**: Elementos principales y botones primarios
- **Naranja**: Listas y información secundaria
- **Verde**: Reservas y elementos de confirmación
- **Azul**: Habitaciones y elementos informativos
- **Púrpura**: Amas de llaves y elementos especiales
- **Índigo**: Trazas y elementos del sistema

### 📱 **Componentes Mejorados:**
- **Cards con sombras**: Mejor separación visual
- **Iconos descriptivos**: Cada sección tiene iconos representativos
- **Chips informativos**: Estados visuales claros
- **Tooltips**: Ayuda contextual en botones
- **Animaciones suaves**: Transiciones en hover y cambios

### 📊 **Dashboard Estadísticas:**
- **Cards coloridas**: Cada métrica con color distintivo
- **Iconos grandes**: Mejor identificación visual
- **Números prominentes**: Fácil lectura de estadísticas
- **Badges de notificación**: Alertas visuales

---

## 🔧 **Funcionalidades Técnicas**

### ⚡ **Optimizaciones:**
- **Debounce en búsquedas**: 500ms para evitar llamadas excesivas
- **Loading states**: Indicadores de carga en todas las operaciones
- **Error handling**: Manejo robusto de errores con notificaciones
- **Validación reactiva**: Validación en tiempo real de formularios

### 🔄 **Estados Reactivos:**
- **Computed properties**: Cálculos automáticos y filtros
- **Watchers**: Reactividad en cambios de datos
- **Refs reactivos**: Estado local de componentes
- **Composables reutilizables**: Lógica compartida

### 📱 **Responsividad:**
- **Grid system**: Layout adaptativo con Quasar
- **Breakpoints**: col-12 col-sm-6 col-md-4 col-lg-3
- **Mobile-first**: Diseño optimizado para móviles

---

## 🚀 **Instalación y Uso**

### 📦 **Dependencias:**
\`\`\`bash
npm install quasar axios luxon
\`\`\`

### 🔧 **Configuración:**
1. Configurar URL de API en composables
2. Importar componentes en páginas
3. Configurar rutas en router
4. Configurar Quasar plugins

### 🎯 **Uso de Composables:**
\`\`\`javascript
// En cualquier componente
import { useClientes } from 'src/composables/useClientes'

const {
  clientes,
  loading,
  fetchClientes,
  crearCliente
} = useClientes()
\`\`\`

---

## 📈 **Próximas Mejoras**

### 🔮 **Funcionalidades Futuras:**
- [ ] Reportes avanzados con gráficos
- [ ] Exportación a PDF/Excel
- [ ] Sistema de notificaciones push
- [ ] Calendario de reservas visual
- [ ] Dashboard en tiempo real
- [ ] Sistema de roles y permisos
- [ ] Backup automático de datos
- [ ] Integración con sistemas de pago

### 🛠️ **Mejoras Técnicas:**
- [ ] Tests unitarios con Vitest
- [ ] Documentación con Storybook
- [ ] PWA (Progressive Web App)
- [ ] Optimización de bundle
- [ ] Lazy loading de componentes
- [ ] Service Workers para offline
- [ ] Internacionalización (i18n)

---

## 📞 **Soporte**

Para dudas o problemas:
- 📧 Email: soporte@hostalislazul.com
- 📱 WhatsApp: +53 5555-5555
- 🌐 Web: www.hostalislazul.com

---

*Documentación generada para el Sistema de Gestión Hotelera - Hostal Isla Azul v2.0*
