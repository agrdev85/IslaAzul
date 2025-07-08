<template>
  <q-layout view="hHh lpR fFf">
    <!-- Encabezado -->
    <q-header elevated class="bg-red-9 text-white">
      <q-toolbar>
        <q-toolbar-title class="flex items-center">
          <q-icon name="hotel" size="2rem" class="q-mr-md" />
          <div>
            <div class="text-h5 text-weight-bold">Hostal Isla Azul</div>
            <div class="text-caption opacity-80">Sistema de Gestión</div>
          </div>
        </q-toolbar-title>
        
        <q-space />
        
        <q-btn 
          flat 
          round 
          icon="notifications" 
          class="q-mr-sm"
        >
          <q-badge color="orange" floating>3</q-badge>
        </q-btn>
        
        <q-btn 
          flat 
          round 
          :icon="$q.dark.isActive ? 'light_mode' : 'dark_mode'" 
          @click="$q.dark.toggle()"
        />
        
        <q-btn 
          flat 
          round 
          icon="account_circle" 
          class="q-ml-sm"
        />
      </q-toolbar>
    </q-header>

    <!-- Contenido principal -->
    <q-page-container>
      <q-page class="q-pa-md bg-grey-1">
        
        <!-- Dashboard Stats (Nuevo) -->
        <div class="row q-col-gutter-md q-mb-lg">
          <div class="col-12 col-sm-6 col-md-3">
            <q-card class="bg-blue-6 text-white">
              <q-card-section class="text-center">
                <q-icon name="people" size="2rem" class="q-mb-sm" />
                <div class="text-h4 text-weight-bold">{{ stats.totalClientes }}</div>
                <div class="text-body2">Clientes Totales</div>
              </q-card-section>
            </q-card>
          </div>
          
          <div class="col-12 col-sm-6 col-md-3">
            <q-card class="bg-green-6 text-white">
              <q-card-section class="text-center">
                <q-icon name="event_available" size="2rem" class="q-mb-sm" />
                <div class="text-h4 text-weight-bold">{{ stats.reservasActivas }}</div>
                <div class="text-body2">Reservas Activas</div>
              </q-card-section>
            </q-card>
          </div>
          
          <div class="col-12 col-sm-6 col-md-3">
            <q-card class="bg-orange-6 text-white">
              <q-card-section class="text-center">
                <q-icon name="hotel" size="2rem" class="q-mb-sm" />
                <div class="text-h4 text-weight-bold">{{ stats.habitacionesDisponibles }}</div>
                <div class="text-body2">Habitaciones Libres</div>
              </q-card-section>
            </q-card>
          </div>
          
          <div class="col-12 col-sm-6 col-md-3">
            <q-card class="bg-purple-6 text-white">
              <q-card-section class="text-center">
                <q-icon name="attach_money" size="2rem" class="q-mb-sm" />
                <div class="text-h4 text-weight-bold">${{ stats.ingresosMes }}</div>
                <div class="text-body2">Ingresos del Mes</div>
              </q-card-section>
            </q-card>
          </div>
        </div>

        <!-- Pestañas principales -->
        <q-card class="shadow-2">
          <q-tabs
            v-model="tab"
            dense
            class="text-grey-8 bg-white"
            active-color="red-10"
            indicator-color="red-7"
            align="justify"
            narrow-indicator
          >
            <q-tab name="clientes" icon="people" label="Clientes" />
            <q-tab name="reservas" icon="event" label="Reservas" />
            <q-tab name="habitaciones" icon="hotel" label="Habitaciones" />
            <q-tab name="amas" icon="cleaning_services" label="Amas de Llaves" />
            <q-tab name="reportes" icon="analytics" label="Reportes" />
          </q-tabs>
          
          <q-separator />

          <!-- Contenido de las pestañas -->
          <q-tab-panels v-model="tab" animated class="bg-grey-1">
            
            <!-- Pestaña Clientes -->
            <q-tab-panel name="clientes" class="q-pa-md">
              <div class="row q-col-gutter-lg">
                
                <!-- Formulario de Cliente -->
                <div class="col-12 col-lg-5">
                  <q-card class="shadow-1">
                    <q-card-section class="bg-red-1">
                      <div class="text-h6 text-red-10 flex items-center">
                        <q-icon name="person_add" class="q-mr-sm" />
                        {{ editingClient ? 'Modificar Cliente' : 'Nuevo Cliente' }}
                      </div>
                    </q-card-section>
                    
                    <q-card-section class="q-gutter-md">
                      <q-input
                        v-model="clientForm.nombre"
                        filled
                        label="Nombre y Apellidos"
                        color="red-7"
                        :rules="[val => !!val || 'Campo requerido']"
                      >
                        <template v-slot:prepend>
                          <q-icon name="person" />
                        </template>
                      </q-input>
                      
                      <q-input
                        v-model="clientForm.ci"
                        filled
                        label="Cédula de Identidad"
                        color="red-7"
                        mask="###########"
                        :rules="[val => val?.length === 11 || 'Debe tener 11 dígitos']"
                      >
                        <template v-slot:prepend>
                          <q-icon name="badge" />
                        </template>
                      </q-input>
                      
                      <q-input
                        v-model="clientForm.telefono"
                        filled
                        label="Teléfono"
                        color="red-7"
                        type="tel"
                      >
                        <template v-slot:prepend>
                          <q-icon name="phone" />
                        </template>
                      </q-input>
                      
                      <q-toggle
                        v-model="clientForm.esVip"
                        label="Cliente VIP"
                        color="red-7"
                        icon="star"
                      />
                      
                      <div class="row q-gutter-sm">
                        <q-btn
                          :label="editingClient ? 'Actualizar' : 'Crear Cliente'"
                          color="red-10"
                          icon="save"
                          @click="saveClient"
                          :loading="loading"
                          class="col"
                        />
                        <q-btn
                          label="Limpiar"
                          color="grey-6"
                          icon="clear"
                          @click="clearForm"
                          outline
                          class="col-auto"
                        />
                      </div>
                    </q-card-section>
                  </q-card>
                </div>
                
                <!-- Lista de Clientes -->
                <div class="col-12 col-lg-7">
                  <q-card class="shadow-1">
                    <q-card-section class="bg-blue-1">
                      <div class="text-h6 text-blue-10 flex items-center justify-between">
                        <div class="flex items-center">
                          <q-icon name="people" class="q-mr-sm" />
                          Lista de Clientes
                        </div>
                        <q-chip color="blue-10" text-color="white" icon="info">
                          {{ clientes.length }} clientes
                        </q-chip>
                      </div>
                    </q-card-section>
                    
                    <q-card-section>
                      <!-- Filtros -->
                      <div class="row q-col-gutter-md q-mb-md">
                        <div class="col-12 col-sm-6">
                          <q-input
                            v-model="searchClient"
                            filled
                            label="Buscar cliente..."
                            color="blue-7"
                            debounce="300"
                          >
                            <template v-slot:prepend>
                              <q-icon name="search" />
                            </template>
                            <template v-slot:append>
                              <q-icon 
                                name="clear" 
                                @click="searchClient = ''" 
                                class="cursor-pointer"
                                v-if="searchClient"
                              />
                            </template>
                          </q-input>
                        </div>
                        <div class="col-12 col-sm-6">
                          <q-select
                            v-model="filterVip"
                            filled
                            label="Filtrar por tipo"
                            color="blue-7"
                            :options="[
                              { label: 'Todos', value: null },
                              { label: 'Solo VIP', value: true },
                              { label: 'Solo Regulares', value: false }
                            ]"
                            option-label="label"
                            option-value="value"
                            emit-value
                            map-options
                          />
                        </div>
                      </div>
                      
                      <!-- Tabla de clientes -->
                      <q-table
                        :rows="filteredClientes"
                        :columns="clientColumns"
                        row-key="id"
                        :loading="loading"
                        :pagination="{ rowsPerPage: 5 }"
                        class="shadow-1"
                      >
                        <template v-slot:body-cell-vip="props">
                          <q-td :props="props">
                            <q-chip 
                              :color="props.value ? 'orange' : 'grey-5'" 
                              :text-color="props.value ? 'white' : 'grey-8'"
                              :icon="props.value ? 'star' : 'person'"
                              size="sm"
                            >
                              {{ props.value ? 'VIP' : 'Regular' }}
                            </q-chip>
                          </q-td>
                        </template>
                        
                        <template v-slot:body-cell-actions="props">
                          <q-td :props="props">
                            <q-btn
                              flat
                              round
                              color="blue"
                              icon="edit"
                              @click="editClient(props.row)"
                              size="sm"
                            >
                              <q-tooltip>Editar</q-tooltip>
                            </q-btn>
                            <q-btn
                              flat
                              round
                              color="red"
                              icon="delete"
                              @click="deleteClient(props.row.id)"
                              size="sm"
                              class="q-ml-xs"
                            >
                              <q-tooltip>Eliminar</q-tooltip>
                            </q-btn>
                          </q-td>
                        </template>
                      </q-table>
                    </q-card-section>
                  </q-card>
                </div>
              </div>
            </q-tab-panel>

            <!-- Pestaña Reservas -->
            <q-tab-panel name="reservas" class="q-pa-md">
              <div class="row q-col-gutter-lg">
                
                <!-- Formulario de Reserva -->
                <div class="col-12 col-lg-6">
                  <q-card class="shadow-1">
                    <q-card-section class="bg-green-1">
                      <div class="text-h6 text-green-10 flex items-center">
                        <q-icon name="event_available" class="q-mr-sm" />
                        Nueva Reserva
                      </div>
                    </q-card-section>
                    
                    <q-card-section class="q-gutter-md">
                      <div class="row q-col-gutter-md">
                        <div class="col-6">
                          <q-input
                            v-model="reservaForm.fechaEntrada"
                            filled
                            label="Fecha de Entrada"
                            type="date"
                            color="green-7"
                          >
                            <template v-slot:prepend>
                              <q-icon name="event" />
                            </template>
                          </q-input>
                        </div>
                        <div class="col-6">
                          <q-input
                            v-model="reservaForm.fechaSalida"
                            filled
                            label="Fecha de Salida"
                            type="date"
                            color="green-7"
                          >
                            <template v-slot:prepend>
                              <q-icon name="event" />
                            </template>
                          </q-input>
                        </div>
                      </div>
                      
                      <q-select
                        v-model="reservaForm.cliente"
                        filled
                        label="Seleccionar Cliente"
                        color="green-7"
                        :options="clienteOptions"
                        option-label="nombre"
                        option-value="id"
                        use-input
                        @filter="filterClientes"
                      >
                        <template v-slot:prepend>
                          <q-icon name="person" />
                        </template>
                        <template v-slot:no-option>
                          <q-item>
                            <q-item-section class="text-grey">
                              No hay clientes disponibles
                            </q-item-section>
                          </q-item>
                        </template>
                      </q-select>
                      
                      <q-select
                        v-model="reservaForm.habitacion"
                        filled
                        label="Habitación"
                        color="green-7"
                        :options="habitacionesDisponibles"
                        option-label="numero"
                        option-value="id"
                      >
                        <template v-slot:prepend>
                          <q-icon name="hotel" />
                        </template>
                      </q-select>
                      
                      <q-input
                        v-model="reservaForm.observaciones"
                        filled
                        label="Observaciones"
                        color="green-7"
                        type="textarea"
                        rows="3"
                      >
                        <template v-slot:prepend>
                          <q-icon name="note" />
                        </template>
                      </q-input>
                      
                      <div class="bg-green-1 q-pa-md rounded-borders">
                        <div class="text-subtitle2 text-green-10 q-mb-sm">Resumen de la Reserva</div>
                        <div class="row">
                          <div class="col-6">
                            <div class="text-caption text-grey-7">Noches:</div>
                            <div class="text-body1 text-weight-medium">{{ calcularNoches }} noches</div>
                          </div>
                          <div class="col-6">
                            <div class="text-caption text-grey-7">Total estimado:</div>
                            <div class="text-h6 text-green-10 text-weight-bold">${{ calcularTotal }}</div>
                          </div>
                        </div>
                      </div>
                      
                      <q-btn
                        label="Crear Reserva"
                        color="green-10"
                        icon="save"
                        @click="createReserva"
                        :loading="loading"
                        class="full-width"
                        size="lg"
                      />
                    </q-card-section>
                  </q-card>
                </div>
                
                <!-- Lista de Reservas -->
                <div class="col-12 col-lg-6">
                  <q-card class="shadow-1">
                    <q-card-section class="bg-orange-1">
                      <div class="text-h6 text-orange-10 flex items-center justify-between">
                        <div class="flex items-center">
                          <q-icon name="event_note" class="q-mr-sm" />
                          Reservas Activas
                        </div>
                        <q-btn
                          flat
                          round
                          icon="refresh"
                          color="orange-10"
                          @click="loadReservas"
                          :loading="loading"
                        />
                      </div>
                    </q-card-section>
                    
                    <q-card-section class="q-pa-none">
                      <q-list separator>
                        <q-item 
                          v-for="reserva in reservasActivas" 
                          :key="reserva.id"
                          class="q-pa-md"
                        >
                          <q-item-section avatar>
                            <q-avatar 
                              :color="getReservaColor(reserva.estado)" 
                              text-color="white" 
                              icon="event"
                            />
                          </q-item-section>
                          
                          <q-item-section>
                            <q-item-label class="text-weight-medium">
                              {{ reserva.clienteNombre }}
                              <q-chip 
                                v-if="reserva.esVip" 
                                color="orange" 
                                text-color="white" 
                                size="xs" 
                                icon="star"
                                class="q-ml-sm"
                              >
                                VIP
                              </q-chip>
                            </q-item-label>
                            <q-item-label caption>
                              Habitación {{ reserva.habitacion }} • {{ reserva.fechaEntrada }} - {{ reserva.fechaSalida }}
                            </q-item-label>
                            <q-item-label caption class="text-green-8">
                              ${{ reserva.total }}
                            </q-item-label>
                          </q-item-section>
                          
                          <q-item-section side>
                            <div class="row q-gutter-xs">
                              <q-btn
                                flat
                                round
                                color="blue"
                                icon="visibility"
                                size="sm"
                              >
                                <q-tooltip>Ver detalles</q-tooltip>
                              </q-btn>
                              <q-btn
                                flat
                                round
                                color="green"
                                icon="check_in"
                                size="sm"
                                v-if="reserva.estado === 'confirmada'"
                              >
                                <q-tooltip>Check-in</q-tooltip>
                              </q-btn>
                              <q-btn
                                flat
                                round
                                color="red"
                                icon="cancel"
                                size="sm"
                              >
                                <q-tooltip>Cancelar</q-tooltip>
                              </q-btn>
                            </div>
                          </q-item-section>
                        </q-item>
                      </q-list>
                    </q-card-section>
                  </q-card>
                </div>
              </div>
            </q-tab-panel>

            <!-- Pestaña Habitaciones -->
            <q-tab-panel name="habitaciones" class="q-pa-md">
              <div class="text-h6 q-mb-md flex items-center">
                <q-icon name="hotel" class="q-mr-sm" />
                Estado de Habitaciones
              </div>
              
              <div class="row q-col-gutter-md">
                <div 
                  v-for="habitacion in habitaciones" 
                  :key="habitacion.numero"
                  class="col-12 col-sm-6 col-md-4 col-lg-3"
                >
                  <q-card 
                    :class="getHabitacionClass(habitacion.estado)"
                    class="shadow-2 cursor-pointer"
                    @click="selectHabitacion(habitacion)"
                  >
                    <q-card-section class="text-center">
                      <q-icon 
                        :name="getHabitacionIcon(habitacion.estado)" 
                        size="3rem" 
                        class="q-mb-sm"
                      />
                      <div class="text-h5 text-weight-bold">{{ habitacion.numero }}</div>
                      <div class="text-caption">{{ habitacion.tipo }}</div>
                      <q-chip 
                        :color="getEstadoColor(habitacion.estado)"
                        text-color="white"
                        size="sm"
                        class="q-mt-sm"
                      >
                        {{ habitacion.estado }}
                      </q-chip>
                    </q-card-section>
                    
                    <q-card-section v-if="habitacion.ocupante" class="q-pt-none">
                      <q-separator class="q-mb-sm" />
                      <div class="text-caption text-grey-7">Ocupante:</div>
                      <div class="text-body2 text-weight-medium">{{ habitacion.ocupante }}</div>
                      <div class="text-caption">Hasta: {{ habitacion.fechaSalida }}</div>
                    </q-card-section>
                  </q-card>
                </div>
              </div>
            </q-tab-panel>

            <!-- Otras pestañas -->
            <q-tab-panel name="amas">
              <div class="text-center q-pa-xl">
                <q-icon name="cleaning_services" size="4rem" color="grey-5" />
                <div class="text-h6 text-grey-7 q-mt-md">Gestión de Amas de Llaves</div>
                <div class="text-body2 text-grey-6">Próximamente...</div>
              </div>
            </q-tab-panel>
            
            <q-tab-panel name="reportes">
              <div class="text-center q-pa-xl">
                <q-icon name="analytics" size="4rem" color="grey-5" />
                <div class="text-h6 text-grey-7 q-mt-md">Reportes y Estadísticas</div>
                <div class="text-body2 text-grey-6">Próximamente...</div>
              </div>
            </q-tab-panel>
          </q-tab-panels>
        </q-card>
      </q-page>
    </q-page-container>
  </q-layout>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useQuasar } from 'quasar'
import { DateTime } from 'luxon'

const $q = useQuasar()
const tab = ref('clientes')
const loading = ref(false)

// Estados y datos de ejemplo
const stats = ref({
  totalClientes: 127,
  reservasActivas: 23,
  habitacionesDisponibles: 8,
  ingresosMes: '15,420'
})

const clientForm = ref({
  nombre: '',
  ci: '',
  telefono: '',
  esVip: false
})

const reservaForm = ref({
  fechaEntrada: '',
  fechaSalida: '',
  cliente: null,
  habitacion: null,
  observaciones: ''
})

const editingClient = ref(false)
const searchClient = ref('')
const filterVip = ref(null)

// Datos de ejemplo
const clientes = ref([
  { id: 1, nombre: 'Juan Pérez García', ci: '85041512345', telefono: '53123456', esVip: true },
  { id: 2, nombre: 'María González López', ci: '90052612346', telefono: '53234567', esVip: false },
  { id: 3, nombre: 'Carlos Rodríguez Díaz', ci: '88031412347', telefono: '53345678', esVip: true },
  { id: 4, nombre: 'Ana Martínez Ruiz', ci: '92041812348', telefono: '53456789', esVip: false },
  { id: 5, nombre: 'Luis Fernández Castro', ci: '87052212349', telefono: '53567890', esVip: false }
])

const reservasActivas = ref([
  {
    id: 1,
    clienteNombre: 'Juan Pérez García',
    habitacion: '011',
    fechaEntrada: '2024-01-15',
    fechaSalida: '2024-01-18',
    total: '450.00',
    estado: 'confirmada',
    esVip: true
  },
  {
    id: 2,
    clienteNombre: 'María González López',
    habitacion: '012',
    fechaEntrada: '2024-01-16',
    fechaSalida: '2024-01-20',
    total: '600.00',
    estado: 'en_curso',
    esVip: false
  },
  {
    id: 3,
    clienteNombre: 'Carlos Rodríguez Díaz',
    habitacion: '021',
    fechaEntrada: '2024-01-17',
    fechaSalida: '2024-01-19',
    total: '300.00',
    estado: 'confirmada',
    esVip: true
  }
])

const habitaciones = ref([
  { numero: '011', tipo: 'Individual', estado: 'ocupada', ocupante: 'Juan Pérez García', fechaSalida: '2024-01-18' },
  { numero: '012', tipo: 'Doble', estado: 'ocupada', ocupante: 'María González López', fechaSalida: '2024-01-20' },
  { numero: '013', tipo: 'Individual', estado: 'disponible' },
  { numero: '014', tipo: 'Doble', estado: 'disponible' },
  { numero: '015', tipo: 'Individual', estado: 'mantenimiento' },
  { numero: '021', tipo: 'Suite', estado: 'ocupada', ocupante: 'Carlos Rodríguez Díaz', fechaSalida: '2024-01-19' },
  { numero: '022', tipo: 'Doble', estado: 'disponible' },
  { numero: '023', tipo: 'Individual', estado: 'disponible' },
  { numero: '024', tipo: 'Doble', estado: 'limpieza' },
  { numero: '025', tipo: 'Suite', estado: 'disponible' },
  { numero: '031', tipo: 'Individual', estado: 'disponible' },
  { numero: '032', tipo: 'Doble', estado: 'disponible' }
])

// Computed
const filteredClientes = computed(() => {
  let filtered = clientes.value

  if (searchClient.value) {
    const search = searchClient.value.toLowerCase()
    filtered = filtered.filter(client => 
      client.nombre.toLowerCase().includes(search) ||
      client.ci.includes(search) ||
      client.telefono.includes(search)
    )
  }

  if (filterVip.value !== null) {
    filtered = filtered.filter(client => client.esVip === filterVip.value)
  }

  return filtered
})

const clienteOptions = computed(() => {
  return clientes.value.map(client => ({
    ...client,
    label: `${client.nombre} (${client.ci})`
  }))
})

const habitacionesDisponibles = computed(() => {
  return habitaciones.value
    .filter(h => h.estado === 'disponible')
    .map(h => ({ id: h.numero, numero: h.numero, tipo: h.tipo }))
})

const calcularNoches = computed(() => {
  if (!reservaForm.value.fechaEntrada || !reservaForm.value.fechaSalida) return 0
  const entrada = DateTime.fromISO(reservaForm.value.fechaEntrada)
  const salida = DateTime.fromISO(reservaForm.value.fechaSalida)
  return Math.max(0, salida.diff(entrada, 'days').days)
})

const calcularTotal = computed(() => {
  const noches = calcularNoches.value
  const precioPorNoche = 150 // Precio base
  return (noches * precioPorNoche).toFixed(2)
})

// Columnas de tabla
const clientColumns = [
  { name: 'nombre', label: 'Nombre', field: 'nombre', align: 'left', sortable: true },
  { name: 'ci', label: 'CI', field: 'ci', align: 'center', sortable: true },
  { name: 'telefono', label: 'Teléfono', field: 'telefono', align: 'center' },
  { name: 'vip', label: 'Tipo', field: 'esVip', align: 'center' },
  { name: 'actions', label: 'Acciones', field: 'actions', align: 'center' }
]

// Métodos
const saveClient = () => {
  loading.value = true
  setTimeout(() => {
    if (editingClient.value) {
      $q.notify({
        type: 'positive',
        message: 'Cliente actualizado correctamente',
        position: 'top'
      })
    } else {
      clientes.value.push({
        id: Date.now(),
        ...clientForm.value
      })
      $q.notify({
        type: 'positive',
        message: 'Cliente creado correctamente',
        position: 'top'
      })
    }
    clearForm()
    loading.value = false
  }, 1000)
}

const clearForm = () => {
  clientForm.value = {
    nombre: '',
    ci: '',
    telefono: '',
    esVip: false
  }
  editingClient.value = false
}

const editClient = (client) => {
  clientForm.value = { ...client }
  editingClient.value = true
}

const deleteClient = (id) => {
  $q.dialog({
    title: 'Confirmar eliminación',
    message: '¿Está seguro de que desea eliminar este cliente?',
    cancel: true,
    persistent: true
  }).onOk(() => {
    clientes.value = clientes.value.filter(c => c.id !== id)
    $q.notify({
      type: 'positive',
      message: 'Cliente eliminado correctamente',
      position: 'top'
    })
  })
}

const createReserva = () => {
  loading.value = true
  setTimeout(() => {
    $q.notify({
      type: 'positive',
      message: 'Reserva creada correctamente',
      position: 'top'
    })
    reservaForm.value = {
      fechaEntrada: '',
      fechaSalida: '',
      cliente: null,
      habitacion: null,
      observaciones: ''
    }
    loading.value = false
  }, 1000)
}

const loadReservas = () => {
  loading.value = true
  setTimeout(() => {
    loading.value = false
  }, 500)
}

const selectHabitacion = (habitacion) => {
  $q.notify({
    type: 'info',
    message: `Habitación ${habitacion.numero} - ${habitacion.estado}`,
    position: 'top'
  })
}

const getReservaColor = (estado) => {
  switch (estado) {
    case 'confirmada': return 'blue'
    case 'en_curso': return 'green'
    case 'cancelada': return 'red'
    default: return 'grey'
  }
}

const getHabitacionClass = (estado) => {
  switch (estado) {
    case 'disponible': return 'bg-green-1 text-green-10'
    case 'ocupada': return 'bg-red-1 text-red-10'
    case 'mantenimiento': return 'bg-orange-1 text-orange-10'
    case 'limpieza': return 'bg-blue-1 text-blue-10'
    default: return 'bg-grey-1 text-grey-10'
  }
}

const getHabitacionIcon = (estado) => {
  switch (estado) {
    case 'disponible': return 'hotel'
    case 'ocupada': return 'person'
    case 'mantenimiento': return 'build'
    case 'limpieza': return 'cleaning_services'
    default: return 'hotel'
  }
}

const getEstadoColor = (estado) => {
  switch (estado) {
    case 'disponible': return 'green'
    case 'ocupada': return 'red'
    case 'mantenimiento': return 'orange'
    case 'limpieza': return 'blue'
    default: return 'grey'
  }
}

const filterClientes = (val, update) => {
  update(() => {
    // Filtrado en tiempo real
  })
}

onMounted(() => {
  // Cargar datos iniciales
})
</script>

<style scoped>
.q-card {
  border-radius: 12px;
}

.shadow-1 {
  box-shadow: 0 1px 5px rgba(0, 0, 0, 0.2), 0 2px 2px rgba(0, 0, 0, 0.14), 0 3px 1px -2px rgba(0, 0, 0, 0.12);
}

.shadow-2 {
  box-shadow: 0 3px 5px -1px rgba(0, 0, 0, 0.2), 0 6px 10px rgba(0, 0, 0, 0.14), 0 1px 18px rgba(0, 0, 0, 0.12);
}

.cursor-pointer {
  cursor: pointer;
}

.cursor-pointer:hover {
  transform: translateY(-2px);
  transition: transform 0.2s ease;
}
</style>
