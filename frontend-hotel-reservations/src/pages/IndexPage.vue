<template>
  <q-layout view="hHh lpR fFf">
    <!-- Encabezado -->
    <q-header elevated class="bg-red-9 text-white text-center q-py-xs">
      <q-toolbar>
        <q-toolbar-title>
          <q-icon name="hotel" size="1.5rem" class="q-mr-sm" />
          <span class="text-h6">Hostal Isla Azul - Gestión</span>
        </q-toolbar-title>
      </q-toolbar>
    </q-header>

    <!-- Contenido principal -->
    <q-page-container>
      <q-page class="q-pa-md">
        <!-- Pestañas -->
        <q-tabs
          v-model="tab"
          dense
          class="text-grey-8"
          active-color="red-10"
          indicator-color="red-7"
          align="justify"
          narrow-indicator>
          <q-tab name="clientes" label="Clientes" />
          <q-tab name="amasDeLlaves" label="Amas de Llaves" />
          <q-tab name="habitaciones" label="Habitaciones" />
          <q-tab name="reservas" label="Reservas" />
        </q-tabs>

        <q-separator />

        <!-- Contenido de las pestañas -->
        <q-tab-panels v-model="tab" animated>
          <!-- Pestaña Clientes -->
          <q-tab-panel name="clientes">
            <div class="row q-col-gutter-md">
              <div class="col-12">
                <q-input
                  v-model="clienteForm.NombreApellidos"
                  filled
                  label="Nombre y Apellidos"
                  color="red-7"
                  :rules="[
                    val => !!val || 'Campo requerido',
                    val => (val && val.length <= 25) || 'Máximo 25 caracteres',
                    val => /^[a-zA-Z\s]+$/.test(val) || 'Solo letras permitidas'
                  ]"
                />
              </div>
              <div class="col-12">
                <q-input
                  v-model="clienteForm.CI"
                  filled
                  label="CI (11 caracteres)"
                  color="red-7"
                  :rules="[
                      val => !!val || 'Campo requerido',
                      val => (val && val.length === 11) || 'Debe tener 11 caracteres',
                      val => /^\d+$/.test(val) || 'Solo números permitidos'
                    ]"
                />
              </div>
              <div class="col-12">
                <q-input
                  v-model="clienteForm.NumeroTelefonico"
                  filled
                  label="Número Telefónico"
                  color="red-7"
                  :rules="[
                    val => !!val || 'Campo requerido',
                    val => /^\d+$/.test(val) || 'Solo números permitidos'
                  ]"
                />
              </div>
              <div class="col-12">
                <q-toggle
                  v-model="clienteForm.EsVip"
                  label="Es VIP"
                  color="red-7"
                />
              </div>
              <div class="col-12">
                <q-btn
                  label="Crear Cliente"
                  color="red-10"
                  @click="crearCliente"
                  :disable="clienteFormDisabled"
                />
                <q-btn
                  v-if="selectedClienteId"
                  label="Modificar Cliente"
                  color="red-10"
                  class="q-ml-md"
                  @click="modificarCliente"
                  :disable="clienteFormDisabled"
                />
              </div>
              <div class="col-12 q-mt-md">
                <q-input
                  v-model="busquedaCliente"
                  filled
                  label="Búsqueda (Nombre, CI, Teléfono)"
                  color="red-7"
                />
                <q-input
                  v-model="ciFiltro"
                  filled
                  label="Filtro por CI"
                  color="red-7"
                  class="q-ml-md"
                />
                <q-btn
                  label="Actualizar Lista"
                  color="red-10"
                  class="q-ml-md"
                  @click="refreshClientes"
                />
                <q-table
                  :rows="clientes"
                  :columns="clientesColumns"
                  row-key="Id"
                  color="red-7"
                  flat
                  bordered
                  :loading="clientesLoading"
                  :pagination="pagination"
                  @request="onRequestClientes"
                >
                  <template v-slot:body-cell-actions="props">
                    <q-td :props="props">
                      <q-btn
                        flat
                        round
                        color="red-10"
                        icon="edit"
                        @click="selectCliente(props.row.Id)"
                      />
                      <q-btn
                        flat
                        round
                        color="red-10"
                        icon="delete"
                        @click="eliminarCliente(props.row.Id)"
                        class="q-ml-sm"
                      />
                    </q-td>
                  </template>
                </q-table>
              </div>
            </div>
          </q-tab-panel>

          <!-- Pestaña Amas de Llaves -->
          <q-tab-panel name="amasDeLlaves">
            <div class="row q-col-gutter-md">
              <div class="col-12">
                <q-input
                  v-model="amaForm.NombreApellidos"
                  filled
                  label="Nombre y Apellidos"
                  color="red-7"
                  :rules="[
                    val => !!val || 'Campo requerido',
                    val => (val && val.length <= 25) || 'Máximo 25 caracteres',
                    val => /^[a-zA-Z\s]+$/.test(val) || 'Solo letras permitidas'
                  ]"
                      
                />
              </div>
              <div class="col-12">
                <q-input
                  v-model="amaForm.CI"
                  filled
                  label="CI (11 caracteres)"
                  color="red-7"
                  :rules="[
                      val => !!val || 'Campo requerido',
                      val => (val && val.length === 11) || 'Debe tener 11 caracteres',
                      val => /^\d+$/.test(val) || 'Solo números permitidos'
                    ]"
                />
              </div>
              <div class="col-12">
                <q-input
                  v-model="amaForm.NumeroTelefonico"
                  filled
                  label="Número Telefónico"
                  color="red-7"
                  :rules="[
                    val => !!val || 'Campo requerido',
                    val => /^\d+$/.test(val) || 'Solo números permitidos'
                  ]"
                />
              </div>
              <div class="col-12">
                <q-btn
                  label="Crear Ama de Llaves"
                  color="red-10"
                  @click="crearAmaDeLlaves"
                  :disable="amaFormDisabled"
                />
                <q-btn
                  v-if="selectedAmaId"
                  label="Modificar Ama de Llaves"
                  color="red-10"
                  class="q-ml-md"
                  @click="modificarAmaDeLlaves"
                  :disable="amaFormDisabled"
                />
              </div>
              <div class="col-12 q-mt-md">
                <q-input
                  v-model="busquedaAma"
                  filled
                  label="Búsqueda (Nombre, CI, Teléfono)"
                  color="red-7"
                />
                <q-input
                  v-model="ciFiltroAma"
                  filled
                  label="Filtro por CI"
                  color="red-7"
                  class="q-ml-md"
                />
                <q-btn
                  label="Actualizar Lista"
                  color="red-10"
                  class="q-ml-md"
                  @click="refreshAmas"
                />
                <q-table
                  :rows="amasDeLlaves"
                  :columns="amasColumns"
                  row-key="Id"
                  color="red-7"
                  flat
                  bordered
                  :loading="amasLoading"
                  :pagination="pagination"
                  @request="onRequestAmas"
                >
                  <template v-slot:body-cell-actions="props">
                    <q-td :props="props">
                      <q-btn
                        flat
                        round
                        color="red-10"
                        icon="edit"
                        @click="selectAma(props.row.Id)"
                      />
                      <q-btn
                        flat
                        round
                        color="red-10"
                        icon="delete"
                        @click="eliminarAmaDeLlaves(props.row.Id)"
                        class="q-ml-sm"
                      />
                    </q-td>
                  </template>
                </q-table>
              </div>
              <div class="col-12 q-mt-md">
                <q-select
                  v-model="asignacionForm.AmaDeLlavesId"
                  filled
                  label="Seleccionar Ama de Llaves"
                  color="red-7"
                  :options="amasDeLlavesOpciones"
                  option-label="NombreApellidos"
                  option-value="Id"
                  use-input
                  @filter="filterAmas"
                  :rules="[val => !!val || 'Campo requerido']"
                />
                <q-input
                  v-model="asignacionForm.HabitacionNumero"
                  filled
                  label="Número de Habitación (0XY)"
                  color="red-7"
                  :rules="[
                    val => !!val || 'Campo requerido',
                    val => (val && /^0[1-3][1-5]$/.test(val)) || 'Formato inválido (0XY)'
                  ]"
                />
                <q-select
                  v-model="asignacionForm.Turno"
                  filled
                  label="Turno (Mañana, Tarde, Noche)"
                  color="red-7"
                  :options="['Mañana', 'Tarde', 'Noche']"
                  :rules="[val => !!val || 'Campo requerido']"
                />
                <q-btn
                  label="Asignar Habitación"
                  color="red-10"
                  class="q-ml-md"
                  @click="asignarHabitacion"
                  :disable="asignacionFormDisabled"
                />
                <q-btn
                  label="Desasignar Habitación"
                  color="red-10"
                  class="q-ml-md"
                  @click="desasignarHabitacion"
                  :disable="asignacionFormDisabled"
                />
              </div>
              <div class="col-12 q-mt-md">
                <q-btn
                  label="Ver Habitaciones Asignadas"
                  color="red-10"
                  @click="fetchHabitacionesPorAma"
                />
                <q-table
                  :rows="habitacionesAma"
                  :columns="habitacionesAmaColumns"
                  row-key="HabitacionNumero"
                  color="red-7"
                  flat
                  bordered
                  class="q-mt-md"
                />
              </div>
            </div>
          </q-tab-panel>

          <!-- Pestaña Habitaciones -->
          <q-tab-panel name="habitaciones">
            <div class="row q-col-gutter-md">
              <div class="col-12">
                <q-input
                  v-model="habitacionForm.Numero"
                  filled
                  label="Número de Habitación (0XY)"
                  color="red-7"
                  :rules="[
                    val => !!val || 'Campo requerido',
                    val => (val && /^0[1-3][1-5]$/.test(val)) || 'Formato inválido (0XY)'
                  ]"
                />
                <q-toggle
                  v-model="habitacionForm.EstaFueraDeServicio"
                  label="Fuera de Servicio"
                  color="red-7"
                  class="q-ml-md"
                />
                <q-btn
                  label="Crear Habitación"
                  color="red-10"
                  @click="crearHabitacion"
                  :disable="habitacionFormDisabled"
                />
                <q-btn
                  v-if="selectedHabitacionId"
                  label="Modificar Habitación"
                  color="red-10"
                  class="q-ml-md"
                  @click="modificarHabitacion"
                  :disable="habitacionFormDisabled"
                />
              </div>
              <div class="col-12 q-mt-md">
                <q-input
                  v-model="busquedaHabitacion"
                  filled
                  label="Búsqueda por Número"
                  color="red-7"
                />
                <q-toggle
                  v-model="filtroFueraDeServicio"
                  label="Solo Fuera de Servicio"
                  color="red-7"
                  class="q-ml-md"
                />
                <q-btn
                  label="Actualizar Lista"
                  color="red-10"
                  class="q-ml-md"
                  @click="refreshHabitaciones"
                />
                <q-table
                  :rows="habitaciones"
                  :columns="habitacionesColumns"
                  row-key="Id"
                  color="red-7"
                  flat
                  bordered
                  :loading="habitacionesLoading"
                  :pagination="pagination"
                  @request="onRequestHabitaciones"
                >
                  <template v-slot:body-cell-actions="props">
                    <q-td :props="props">
                      <q-btn
                        flat
                        round
                        color="red-10"
                        icon="edit"
                        @click="selectHabitacion(props.row.Id)"
                      />
                      <q-btn
                        flat
                        round
                        color="red-10"
                        icon="delete"
                        @click="eliminarHabitacion(props.row.Id)"
                        class="q-ml-sm"
                      />
                    </q-td>
                  </template>
                </q-table>
              </div>
            </div>
          </q-tab-panel>

          <!-- Pestaña Reservas -->
          <q-tab-panel name="reservas">
            <div class="row q-col-gutter-md">
              <div class="col-12">
                <q-input
                  v-model="reservaForm.FechaEntrada"
                  filled
                  label="Fecha de Entrada"
                  type="date"
                  color="red-7"
                  :rules="[val => !!val || 'Campo requerido']"
                />
              </div>
              <div class="col-12">
                <q-input
                  v-model="reservaForm.FechaSalida"
                  filled
                  label="Fecha de Salida"
                  type="date"
                  color="red-7"
                  :rules="[val => !!val || 'Campo requerido']"
                />
              </div>
              <div class="col-12">
                <q-select
                  v-model="reservaForm.ClienteId"
                  filled
                  label="Seleccionar Cliente"
                  color="red-7"
                  :options="clientesOpciones"
                  option-label="NombreApellidos"
                  option-value="Id"
                  use-input
                  @filter="filterClientes"
                  :rules="[val => !!val || 'Campo requerido']"
                />
              </div>
              <div class="col-12">
                <q-input
                  v-model="reservaForm.HabitacionNumero"
                  filled
                  label="Número de Habitación (0XY)"
                  color="red-7"
                  :rules="[
                    val => !!val || 'Campo requerido',
                    val => (val && /^0[1-3][1-5]$/.test(val)) || 'Formato inválido (0XY)'
                  ]"
                />
              </div>
              <div class="col-12">
                <q-btn
                  label="Crear Reserva"
                  color="red-10"
                  @click="crearReserva"
                  :disable="reservaFormDisabled"
                />
                <q-btn
                  v-if="selectedReservaId"
                  label="Modificar Reserva"
                  color="red-10"
                  class="q-ml-md"
                  @click="modificarReserva"
                  :disable="reservaFormDisabled"
                />
              </div>
              <div class="col-12 q-mt-md">
                <q-input
                  v-model="reservaId"
                  filled
                  label="ID Reserva"
                  type="number"
                  color="red-7"
                />
                <q-input
                  v-model="motivoCancelacion"
                  filled
                  label="Motivo Cancelación"
                  color="red-7"
                  :rules="[val => !!val || 'Campo requerido']"
                />
                <q-btn
                  label="Cancelar Reserva"
                  color="red-10"
                  class="q-ml-md"
                  @click="cancelarReserva"
                  :disable="!reservaId || !motivoCancelacion"
                />
                <q-input
                  v-model="nuevoNumeroHabitacion"
                  filled
                  label="Nuevo Número Habitación"
                  color="red-7"
                  :rules="[
                    val => !!val || 'Campo requerido',
                    val => (val && /^0[1-3][1-5]$/.test(val)) || 'Formato inválido (0XY)'
                  ]"
                />
                <q-btn
                  label="Cambiar Habitación"
                  color="red-10"
                  class="q-ml-md"
                  @click="cambiarHabitacion"
                  :disable="!reservaId || !nuevoNumeroHabitacion"
                />
                <q-btn
                  label="Registrar Llegada"
                  color="red-10"
                  class="q-ml-md"
                  @click="registrarLlegada"
                  :disable="!reservaId"
                />
              </div>
              <div class="col-12 q-mt-md">
                <q-input
                  v-model="fechaActivas"
                  filled
                  label="Fecha Reservas Activas"
                  type="date"
                  color="red-7"
                />
                <q-btn
                  label="Ver Reservas Activas"
                  color="red-10"
                  class="q-ml-md"
                  @click="fetchReservasActivas"
                />
                <q-table
                  :rows="reservasActivas"
                  :columns="reservasColumns"
                  row-key="Id"
                  color="red-7"
                  flat
                  bordered
                  :loading="reservasActivasLoading"
                  class="q-mt-md"
                />
              </div>
              <div class="col-12 q-mt-md">
                <q-input
                  v-model="fechaInicioDisponibles"
                  filled
                  label="Fecha Inicio Disponibles"
                  type="date"
                  color="red-7"
                  :rules="[val => !!val || 'Campo requerido']"
                />
                <q-input
                  v-model="fechaFinDisponibles"
                  filled
                  label="Fecha Fin Disponibles"
                  type="date"
                  color="red-7"
                  :rules="[val => !!val || 'Campo requerido']"
                />
                <q-btn
                  label="Ver Habitaciones Disponibles"
                  color="red-10"
                  class="q-ml-md"
                  @click="fetchHabitacionesDisponibles"
                />
                <q-table
                  :rows="habitacionesDisponibles"
                  :columns="habitacionesDisponiblesColumns"
                  row-key="Numero"
                  color="red-7"
                  flat
                  bordered
                  class="q-mt-md"
                />
              </div>
            </div>
          </q-tab-panel>
        </q-tab-panels>
      </q-page>
    </q-page-container>
  </q-layout>
</template>

<script>
import { defineComponent, ref, computed, onMounted } from 'vue'
import { useQuasar } from 'quasar'
import axios from 'axios'

export default defineComponent({
  name: 'IndexPage',

  setup() {
    const $q = useQuasar()
    const tab = ref('clientes')
    const darkMode = ref(false)

    const clienteForm = ref({
      NombreApellidos: '',
      CI: '',
      NumeroTelefonico: '',
      EsVip: false
    })
    const amaForm = ref({
      NombreApellidos: '',
      CI: '',
      NumeroTelefonico: ''
    })
    const habitacionForm = ref({
      Numero: '',
      EstaFueraDeServicio: false
    })
    const reservaForm = ref({
      FechaEntrada: '',
      FechaSalida: '',
      ClienteId: '',
      HabitacionNumero: ''
    })
    const asignacionForm = ref({
      AmaDeLlavesId: '',
      HabitacionNumero: '',
      Turno: ''
    })

    const amasDeLlavesOpciones = ref([])
    const clientesOpciones = ref([])
    const selectedClienteId = ref(null)
    const selectedAmaId = ref(null)
    const selectedHabitacionId = ref(null)
    const selectedReservaId = ref(null)
    const reservaId = ref(null)
    const motivoCancelacion = ref('')
    const nuevoNumeroHabitacion = ref('')
    const busquedaCliente = ref('')
    const ciFiltro = ref('')
    const busquedaAma = ref('')
    const ciFiltroAma = ref('')
    const busquedaHabitacion = ref('')
    const filtroFueraDeServicio = ref(false)
    const fechaActivas = ref('')
    const fechaInicioDisponibles = ref('')
    const fechaFinDisponibles = ref('')

    const clientes = ref([])
    const amasDeLlaves = ref([])
    const habitaciones = ref([])
    const reservasActivas = ref([])
    const habitacionesDisponibles = ref([])
    const habitacionesAma = ref([])
    const clientesLoading = ref(true)
    const amasLoading = ref(true)
    const habitacionesLoading = ref(true)
    const reservasActivasLoading = ref(false)

    const pagination = ref({
      sortBy: 'desc',
      descending: false,
      page: 1,
      rowsPerPage: 10
    })

    const clientesColumns = [
      { name: 'Id', label: 'ID', field: 'Id', sortable: true },
      { name: 'NombreApellidos', label: 'Nombre', field: 'NombreApellidos', sortable: true },
      { name: 'CI', label: 'CI', field: 'CI', sortable: true },
      { name: 'NumeroTelefonico', label: 'Teléfono', field: 'NumeroTelefonico', sortable: true },
      { name: 'EsVip', label: 'VIP', field: 'EsVip', sortable: true },
      { name: 'actions', label: 'Acciones', field: 'actions', sortable: false }
    ]

    const amasColumns = [
      { name: 'Id', label: 'ID', field: 'Id', sortable: true },
      { name: 'NombreApellidos', label: 'Nombre', field: 'NombreApellidos', sortable: true },
      { name: 'CI', label: 'CI', field: 'CI', sortable: true },
      { name: 'NumeroTelefonico', label: 'Teléfono', field: 'NumeroTelefonico', sortable: true },
      { name: 'actions', label: 'Acciones', field: 'actions', sortable: false }
    ]

    const habitacionesColumns = [
      { name: 'Id', label: 'ID', field: 'Id', sortable: true },
      { name: 'Numero', label: 'Número', field: 'Numero', sortable: true },
      { name: 'Estado', label: 'Estado', field: 'Estado', sortable: true },
      { name: 'actions', label: 'Acciones', field: 'actions', sortable: false }
    ]

    const reservasColumns = [
      { name: 'Id', label: 'ID', field: 'Id', sortable: true },
      { name: 'FechaReservacion', label: 'Fecha Reservación', field: 'FechaReservacion', sortable: true },
      { name: 'FechaEntrada', label: 'Fecha Entrada', field: 'FechaEntrada', sortable: true },
      { name: 'FechaSalida', label: 'Fecha Salida', field: 'FechaSalida', sortable: true },
      { name: 'Importe', label: 'Importe', field: 'Importe', sortable: true },
      { name: 'ClienteNombre', label: 'Nombre del Cliente', field: 'ClienteNombre', sortable: true },
      { name: 'HabitacionNumero', label: 'Habitación', field: 'HabitacionNumero', sortable: true },
      { name: 'EstaElClienteEnHostal', label: 'Cliente en Hostal', field: 'EstaElClienteEnHostal', sortable: true },
      { name: 'EstaCancelada', label: 'Cancelada', field: 'EstaCancelada', sortable: true },
      { name: 'FechaCancelacion', label: 'Fecha Cancelación', field: 'FechaCancelacion', sortable: true },
      { name: 'MotivoCancelacion', label: 'Motivo Cancelación', field: 'MotivoCancelacion', sortable: true }
    ]

    const habitacionesDisponiblesColumns = [
      { name: 'Numero', label: 'Número', field: 'Numero', sortable: true }
    ]

    const habitacionesAmaColumns = [
      { name: 'HabitacionNumero', label: 'Habitación', field: 'HabitacionNumero', sortable: true },
      { name: 'Turno', label: 'Turno', field: 'Turno', sortable: true }
    ]

    const apiUrl = 'http://localhost:5014/api'

    const toggleDarkMode = () => {
      $q.dark.set(darkMode.value)
    }

    const showNotification = (message, type = 'positive') => {
      $q.notify({
        message,
        color: type === 'positive' ? 'green' : 'red',
        position: 'top',
        timeout: 3000
      })
    }

    const clienteFormDisabled = computed(() => {
      return !clienteForm.value.NombreApellidos || !clienteForm.value.CI || !clienteForm.value.NumeroTelefonico
    })

    const amaFormDisabled = computed(() => {
      return !amaForm.value.NombreApellidos || !amaForm.value.CI || !amaForm.value.NumeroTelefonico
    })

    const habitacionFormDisabled = computed(() => {
      return !habitacionForm.value.Numero
    })

    const reservaFormDisabled = computed(() => {
      return !reservaForm.value.FechaEntrada || !reservaForm.value.FechaSalida ||
             !reservaForm.value.ClienteId || !reservaForm.value.HabitacionNumero
    })

    const asignacionFormDisabled = computed(() => {
      return !asignacionForm.value.AmaDeLlavesId || !asignacionForm.value.HabitacionNumero || !asignacionForm.value.Turno
    })

    const onRequestClientes = async ({ pagination: { page, rowsPerPage } }) => {
      clientesLoading.value = true
      try {
        const response = await axios.get(`${apiUrl}/Clientes`, {
          params: { busqueda: busquedaCliente.value, ciFiltro: ciFiltro.value, pagina: page, tamanoPagina: rowsPerPage }
        })
        clientes.value = response.data.map(c => ({
          Id: c.Id,
          NombreApellidos: c.NombreApellidos,
          CI: c.CI,
          NumeroTelefonico: c.NumeroTelefonico,
          EsVip: c.EsVip
        }))
        pagination.value.page = page
        pagination.value.rowsPerPage = rowsPerPage
      } catch (error) {
        console.error('Error fetching clientes:', error)
        showNotification('Error al cargar clientes', 'negative')
      } finally {
        clientesLoading.value = false
      }
    }

    const onRequestAmas = async ({ pagination: { page, rowsPerPage } }) => {
      amasLoading.value = true
      try {
        const response = await axios.get(`${apiUrl}/AmasDeLlaves`, {
          params: { busqueda: busquedaAma.value, ciFiltro: ciFiltroAma.value, pagina: page, tamanoPagina: rowsPerPage }
        })
        amasDeLlaves.value = response.data.map(a => ({
          Id: a.Id,
          NombreApellidos: a.NombreApellidos,
          CI: a.CI,
          NumeroTelefonico: a.NumeroTelefonico
        }))
        pagination.value.page = page
        pagination.value.rowsPerPage = rowsPerPage
      } catch (error) {
        console.error('Error fetching amas de llaves:', error)
        showNotification('Error al cargar amas de llaves', 'negative')
      } finally {
        amasLoading.value = false
      }
    }

    const onRequestHabitaciones = async ({ pagination: { page, rowsPerPage } }) => {
      habitacionesLoading.value = true
      try {
        const response = await axios.get(`${apiUrl}/Habitaciones`, {
          params: { busqueda: busquedaHabitacion.value, estaFueraDeServicio: filtroFueraDeServicio.value, pagina: page, tamanoPagina: rowsPerPage }
        })
        habitaciones.value = response.data.map(h => ({
          Id: h.Id,
          Numero: h.Numero,
          Estado: h.EstaFueraDeServicio ? 'Fuera de Servicio' : 'Disponible'
        }))
        pagination.value.page = page
        pagination.value.rowsPerPage = rowsPerPage
      } catch (error) {
        console.error('Error fetching habitaciones:', error)
        showNotification('Error al cargar habitaciones', 'negative')
      } finally {
        habitacionesLoading.value = false
      }
    }

    const refreshClientes = () => {
      onRequestClientes({ pagination: pagination.value })
    }

    const refreshAmas = () => {
      onRequestAmas({ pagination: pagination.value })
    }

    const refreshHabitaciones = () => {
      onRequestHabitaciones({ pagination: pagination.value })
    }

    const crearCliente = async () => {
      try {
        const response = await axios.post(`${apiUrl}/Clientes`, clienteForm.value)
        showNotification('Cliente creado con éxito')
        clienteForm.value = { NombreApellidos: '', CI: '', NumeroTelefonico: '', EsVip: false }
        selectedClienteId.value = null
        onRequestClientes({ pagination: pagination.value })
      } catch (error) {
        showNotification(error.response?.data || 'Error al crear cliente', 'negative')
      }
    }

    const selectCliente = async (id) => {
      try {
        const response = await axios.get(`${apiUrl}/Clientes/${id}`)
        clienteForm.value = response.data
        selectedClienteId.value = id
      } catch (error) {
        showNotification(error.response?.data || 'Error al obtener cliente', 'negative')
      }
    }

    const modificarCliente = async () => {
      if (!selectedClienteId.value) return
      try {
        await axios.put(`${apiUrl}/Clientes/${selectedClienteId.value}`, clienteForm.value)
        showNotification('Cliente modificado con éxito')
        clienteForm.value = { NombreApellidos: '', CI: '', NumeroTelefonico: '', EsVip: false }
        selectedClienteId.value = null
        onRequestClientes({ pagination: pagination.value })
      } catch (error) {
        showNotification(error.response?.data || 'Error al modificar cliente', 'negative')
      }
    }

    const eliminarCliente = async (id) => {
      if (confirm(`¿Eliminar cliente con ID: ${id}?`)) {
        try {
          await axios.delete(`${apiUrl}/Clientes/${id}`)
          showNotification('Cliente eliminado')
          onRequestClientes({ pagination: pagination.value })
        } catch (error) {
          showNotification(error.response?.data || 'Error al eliminar cliente', 'negative')
        }
      }
    }

    const crearAmaDeLlaves = async () => {
      try {
        const response = await axios.post(`${apiUrl}/AmasDeLlaves`, amaForm.value)
        showNotification('Ama de llaves creada con éxito')
        amaForm.value = { NombreApellidos: '', CI: '', NumeroTelefonico: '' }
        selectedAmaId.value = null
        onRequestAmas({ pagination: pagination.value })
      } catch (error) {
        showNotification(error.response?.data || 'Error al crear ama de llaves', 'negative')
      }
    }

    const selectAma = async (id) => {
      try {
        const response = await axios.get(`${apiUrl}/AmasDeLlaves/${id}`)
        amaForm.value = response.data
        selectedAmaId.value = id
      } catch (error) {
        showNotification(error.response?.data || 'Error al obtener ama de llaves', 'negative')
      }
    }

    const modificarAmaDeLlaves = async () => {
      if (!selectedAmaId.value) return
      try {
        await axios.put(`${apiUrl}/AmasDeLlaves/${selectedAmaId.value}`, amaForm.value)
        showNotification('Ama de llaves modificada con éxito')
        amaForm.value = { NombreApellidos: '', CI: '', NumeroTelefonico: '' }
        selectedAmaId.value = null
        onRequestAmas({ pagination: pagination.value })
      } catch (error) {
        showNotification(error.response?.data || 'Error al modificar ama de llaves', 'negative')
      }
    }

    const eliminarAmaDeLlaves = async (id) => {
      if (confirm(`¿Eliminar ama de llaves con ID: ${id}?`)) {
        try {
          await axios.delete(`${apiUrl}/AmasDeLlaves/${id}`)
          showNotification('Ama de llaves eliminada')
          onRequestAmas({ pagination: pagination.value })
        } catch (error) {
          showNotification(error.response?.data || 'Error al eliminar ama de llaves', 'negative')
        }
      }
    }

    const asignarHabitacion = async () => {
      try {
        await axios.post(`${apiUrl}/Reservas/asignar-habitacion-ama`, asignacionForm.value)
        showNotification('Habitación asignada con éxito')
        fetchHabitacionesPorAma()
        asignacionForm.value = { AmaDeLlavesId: '', HabitacionNumero: '', Turno: '' }
      } catch (error) {
        showNotification(error.response?.data || 'Error al asignar habitación', 'negative')
      }
    }

    const desasignarHabitacion = async () => {
      try {
        await axios.post(`${apiUrl}/AmasDeLlaves/desasignar-habitacion`, {
          AmaDeLlavesId: asignacionForm.value.AmaDeLlavesId,
          HabitacionNumero: asignacionForm.value.HabitacionNumero
        });
        showNotification('Habitación desasignada con éxito');
        fetchHabitacionesPorAma();
        asignacionForm.value = { AmaDeLlavesId: '', HabitacionNumero: '' };
      } catch (error) {
        showNotification(error.response?.data || 'Error al desasignar habitación', 'negative');
      }
    };

    const fetchHabitacionesPorAma = async () => {
      if (!asignacionForm.value.AmaDeLlavesId) return
      try {
        const response = await axios.get(`${apiUrl}/Reservas/habitaciones-por-ama-de-llaves/${asignacionForm.value.AmaDeLlavesId}`)
        habitacionesAma.value = response.data.map(h => ({
          HabitacionNumero: h.HabitacionNumero || 'N/A',
          Turno: h.Turno || 'N/A'
        }))
      } catch (error) {
        console.error('Error fetching habitaciones por ama:', error)
        showNotification('Error al cargar habitaciones asignadas', 'negative')
      }
    }

    const crearHabitacion = async () => {
      try {
        const response = await axios.post(`${apiUrl}/Habitaciones`, habitacionForm.value)
        showNotification('Habitación creada con éxito')
        habitacionForm.value = { Numero: '', EstaFueraDeServicio: false }
        selectedHabitacionId.value = null
        onRequestHabitaciones({ pagination: pagination.value })
      } catch (error) {
        showNotification(error.response?.data || 'Error al crear habitación', 'negative')
      }
    }

    const selectHabitacion = async (id) => {
      try {
        const response = await axios.get(`${apiUrl}/Habitaciones/${id}`)
        habitacionForm.value = response.data
        selectedHabitacionId.value = id
      } catch (error) {
        showNotification(error.response?.data || 'Error al obtener habitación', 'negative')
      }
    }

    const modificarHabitacion = async () => {
      if (!selectedHabitacionId.value) return
      try {
        await axios.put(`${apiUrl}/Habitaciones/${selectedHabitacionId.value}`, habitacionForm.value)
        showNotification('Habitación modificada con éxito')
        habitacionForm.value = { Numero: '', EstaFueraDeServicio: false }
        selectedHabitacionId.value = null
        onRequestHabitaciones({ pagination: pagination.value })
      } catch (error) {
        showNotification(error.response?.data || 'Error al modificar habitación', 'negative')
      }
    }

    const eliminarHabitacion = async (id) => {
      if (confirm(`¿Eliminar habitación con ID: ${id}?`)) {
        try {
          await axios.delete(`${apiUrl}/Habitaciones/${id}`)
          showNotification('Habitación eliminada')
          onRequestHabitaciones({ pagination: pagination.value })
        } catch (error) {
          showNotification(error.response?.data || 'Error al eliminar habitación', 'negative')
        }
      }
    }

    const ponerFueraDeServicio = async () => {
        if (!selectedHabitacionId.value) return;
        try {
          const response = await axios.get(`${apiUrl}/Reservas/activas`);
          const habitacionReservadaConCliente = response.data.some(r =>
            r.HabitacionNumero === habitacionForm.value.Numero && r.EstaElClienteEnHostal === true
          );
          if (habitacionReservadaConCliente) {
            showNotification('No se puede poner fuera de servicio una habitación con cliente presente', 'negative');
            return;
          }
          await axios.post(`${apiUrl}/Habitaciones/${selectedHabitacionId.value}/fuera-de-servicio`);
          showNotification('Habitación puesta fuera de servicio');
          selectedHabitacionId.value = null;
          onRequestHabitaciones({ pagination: pagination.value });
        } catch (error) {
          showNotification(error.response?.data || 'Error al poner fuera de servicio', 'negative');
        }
      };

    const crearReserva = async () => {
      try {
        const response = await axios.post(`${apiUrl}/Reservas`, reservaForm.value)
        showNotification('Reserva creada con éxito')
        reservaForm.value = { FechaEntrada: '', FechaSalida: '', ClienteId: '', HabitacionNumero: '' }
        selectedReservaId.value = null
        fetchReservasActivas()
      } catch (error) {
        showNotification(error.response?.data || 'Error al crear reserva', 'negative')
      }
    }

    const selectReserva = async (id) => {
      try {
        const response = await axios.get(`${apiUrl}/Reservas/${id}`)
        reservaForm.value = response.data
        selectedReservaId.value = id
      } catch (error) {
        showNotification(error.response?.data || 'Error al obtener reserva', 'negative')
      }
    }

    const modificarReserva = async () => {
      if (!selectedReservaId.value) return
      try {
        await axios.put(`${apiUrl}/Reservas/${selectedReservaId.value}`, reservaForm.value)
        showNotification('Reserva modificada con éxito')
        reservaForm.value = { FechaEntrada: '', FechaSalida: '', ClienteId: '', HabitacionNumero: '' }
        selectedReservaId.value = null
        fetchReservasActivas()
      } catch (error) {
        showNotification(error.response?.data || 'Error al modificar reserva', 'negative')
      }
    }

    const cancelarReserva = async () => {
      if (!reservaId.value || !motivoCancelacion.value) return
      try {
        await axios.post(`${apiUrl}/Reservas/${reservaId.value}/cancelar`, { Motivo: motivoCancelacion.value })
        showNotification('Reserva cancelada con éxito')
        reservaId.value = null
        motivoCancelacion.value = ''
        fetchReservasActivas()
      } catch (error) {
        showNotification(error.response?.data || 'Error al cancelar reserva', 'negative')
      }
    }

    const cambiarHabitacion = async () => {
      if (!reservaId.value || !nuevoNumeroHabitacion.value) return
      try {
        await axios.post(`${apiUrl}/Reservas/${reservaId.value}/cambiar-habitacion`, { NuevoNumero: nuevoNumeroHabitacion.value })
        showNotification('Habitación cambiada con éxito')
        reservaId.value = null
        nuevoNumeroHabitacion.value = ''
        fetchReservasActivas()
      } catch (error) {
        showNotification(error.response?.data || 'Error al cambiar habitación', 'negative')
      }
    }

    const registrarLlegada = async () => {
      if (!reservaId.value) return
      try {
        await axios.post(`${apiUrl}/Reservas/${reservaId.value}/registrar-llegada`)
        showNotification('Llegada registrada con éxito')
        reservaId.value = null
        fetchReservasActivas()
      } catch (error) {
        showNotification(error.response?.data || 'Error al registrar llegada', 'negative')
      }
    }

    const fetchReservasActivas = async () => {
      reservasActivasLoading.value = true
      try {
        const params = fechaActivas.value ? { Fecha: fechaActivas.value } : {}
        const response = await axios.get(`${apiUrl}/Reservas/activas`, { params })
        console.log('Respuesta cruda de la API:', response.data)
        reservasActivas.value = response.data.map(reserva => ({
          Id: reserva.Id?.toString() || 'N/A',
          FechaReservacion: reserva.FechaReservacion ? new Date(reserva.FechaReservacion).toISOString().split('T')[0] : 'N/A',
          FechaEntrada: reserva.FechaEntrada ? new Date(reserva.FechaEntrada).toISOString().split('T')[0] : 'N/A',
          FechaSalida: reserva.FechaSalida ? new Date(reserva.FechaSalida).toISOString().split('T')[0] : 'N/A',
          Importe: reserva.Importe?.toString() || 'N/A',
          ClienteNombre: reserva.ClienteNombre || 'Desconocido',
          HabitacionNumero: reserva.HabitacionNumero || 'N/A',
          EstaElClienteEnHostal: reserva.EstaElClienteEnHostal === true ? 'Sí' : 'No',
          EstaCancelada: reserva.EstaCancelada === true ? 'Sí' : 'No',
          FechaCancelacion: reserva.FechaCancelacion ? new Date(reserva.FechaCancelacion).toISOString().split('T')[0] : 'N/A',
          MotivoCancelacion: reserva.MotivoCancelacion || 'N/A'
        }))
        console.log('Reservas activas mapeadas:', reservasActivas.value)
      } catch (error) {
        console.error('Error fetching reservas activas:', error)
        showNotification('Error al cargar reservas activas', 'negative')
      } finally {
        reservasActivasLoading.value = false
      }
    }

    const fetchHabitacionesDisponibles = async () => {
      if (!fechaInicioDisponibles.value || !fechaFinDisponibles.value) return
      try {
        const response = await axios.get(`${apiUrl}/Reservas/habitaciones-disponibles`, {
          params: { FechaInicio: fechaInicioDisponibles.value, FechaFin: fechaFinDisponibles.value }
        })
        habitacionesDisponibles.value = response.data.map(h => ({ Numero: h.Numero || h }))
      } catch (error) {
        console.error('Error fetching habitaciones disponibles:', error)
        showNotification('Error al cargar habitaciones disponibles', 'negative')
      }
    }

    const validarCIUnica = async (ci) => {
      try {
        const response = await axios.get(`${apiUrl}/Clientes/validar-ci/${ci}`);
        return response.data.disponible; // Suponiendo que el backend devuelve { disponible: true/false }
      } catch (error) {
        showNotification('Error al validar CI', 'negative');
        return false;
      }
    };

    const clienteFormCI = computed({
      get: () => clienteForm.value.CI,
      set: async (val) => {
        clienteForm.value.CI = val;
        if (val.length === 11 && /^\d+$/.test(val)) {
          const esUnica = await validarCIUnica(val);
          if (!esUnica) {
            showNotification('CI ya existe', 'negative');
          }
        }
      }
    });
    

    const filterAmas = (val, update) => {
      update(() => {
        const needle = val.toLowerCase();
        amasDeLlavesOpciones.value = amasDeLlaves.value.filter(v => v.NombreApellidos.toLowerCase().indexOf(needle) > -1);
      });
    };

    const filterClientes = (val, update) => {
      update(() => {
        const needle = val.toLowerCase();
        clientesOpciones.value = clientes.value.filter(v => v.NombreApellidos.toLowerCase().indexOf(needle) > -1);
      });
    };

    onMounted(async () => {
      await onRequestClientes({ pagination: pagination.value });
      await onRequestAmas({ pagination: pagination.value });
      await onRequestHabitaciones({ pagination: pagination.value })
      clientesOpciones.value = clientes.value;
      amasDeLlavesOpciones.value = amasDeLlaves.value;
    });

    return {
      tab,
      darkMode,
      toggleDarkMode,
      clienteForm,
      amaForm,
      amasDeLlavesOpciones,
      clientesOpciones,
      habitacionForm,
      reservaForm,
      asignacionForm,
      selectedClienteId,
      selectedAmaId,
      selectedHabitacionId,
      selectedReservaId,
      reservaId,
      motivoCancelacion,
      nuevoNumeroHabitacion,
      busquedaCliente,
      ciFiltro,
      busquedaAma,
      ciFiltroAma,
      busquedaHabitacion,
      filtroFueraDeServicio,
      fechaActivas,
      fechaInicioDisponibles,
      fechaFinDisponibles,
      clientes,
      amasDeLlaves,
      habitaciones,
      reservasActivas,
      habitacionesDisponibles,
      habitacionesAma,
      clientesColumns,
      amasColumns,
      habitacionesColumns,
      reservasColumns,
      habitacionesDisponiblesColumns,
      habitacionesAmaColumns,
      pagination,
      clienteFormDisabled,
      amaFormDisabled,
      habitacionFormDisabled,
      reservaFormDisabled,
      asignacionFormDisabled,
      refreshClientes,
      refreshAmas,
      refreshHabitaciones,
      crearCliente,
      selectCliente,
      modificarCliente,
      eliminarCliente,
      crearAmaDeLlaves,
      selectAma,
      modificarAmaDeLlaves,
      eliminarAmaDeLlaves,
      asignarHabitacion,
      desasignarHabitacion,
      fetchHabitacionesPorAma,
      crearHabitacion,
      selectHabitacion,
      modificarHabitacion,
      eliminarHabitacion,
      ponerFueraDeServicio,
      crearReserva,
      selectReserva,
      modificarReserva,
      cancelarReserva,
      cambiarHabitacion,
      registrarLlegada,
      fetchReservasActivas,
      fetchHabitacionesDisponibles,
      onRequestClientes,
      onRequestAmas,
      onRequestHabitaciones,
      clientesLoading,
      amasLoading,
      habitacionesLoading,
      reservasActivasLoading
    }
  }
})
</script>

<style scoped>
body {
  transition: background-color 0.3s, color 0.3s;
}
body.dark {
  background-color: #1D1D1D;
  color: #FFFFFF;
}
body:not(.dark) {
  background-color: #F5F5F5;
  color: #1D1D1D;
}
.q-tab--active {
  color: #B71C1C !important;
}
.q-tab {
  color: #D32F2F;
}
.q-btn {
  margin-top: 10px;
}
</style>