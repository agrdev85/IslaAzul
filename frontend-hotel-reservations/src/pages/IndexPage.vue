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
          narrow-indicator
        >
          <q-tab name="clientes" label="Clientes" />
          <q-tab name="amasDeLlaves" label="Amas de Llaves" />
          <q-tab name="habitaciones" label="Habitaciones" />
          <q-tab name="reservas" label="Reservas" />
          <q-tab name="trazas" label="Trazas" />
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
                    val => (val && val.length <= 25 && /^[a-zA-Z\s]+$/.test(val)) || 'Máximo 25 caracteres, solo letras'
                  ]"
                />
              </div>
              <div class="col-12">
                <q-input
                  v-model="clienteForm.CI"
                  filled
                  label="CI (11 dígitos, único)"
                  color="red-7"
                  :rules="[
                    val => !!val || 'Campo requerido',
                    val => (val && val.length === 11) || 'Debe tener 11 caracteres',
                    val => /^\d+$/.test(val) || 'Solo números permitidos'
                  ]"
                >
                  <template v-slot:append>
                    <q-icon name="clear" v-if="clienteForm.CI" @click="clienteForm.CI = ''" class="cursor-pointer" />
                  </template>
                </q-input>
              </div>
              <div class="col-12">
                <q-input
                  v-model="clienteForm.NumeroTelefonico"
                  filled
                  label="Número Telefónico"
                  color="red-7"
                  :rules="[
                    val => !!val || 'Campo requerido',
                    val => (val && /^\d+$/.test(val)) || 'Solo números permitidos'
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
                    val => (val && val.length <= 25 && /^[a-zA-Z\s]+$/.test(val)) || 'Máximo 25 caracteres, solo letras'
                  ]"
                />
              </div>
              <div class="col-12">
                <q-input
                  v-model="amaForm.CI"
                  filled
                  label="CI (11 dígitos, único)"
                  color="red-7"
                  :rules="[
                    val => !!val || 'Campo requerido',
                    val => (val && val.length === 11) || 'Debe tener 11 caracteres',
                    val => /^\d+$/.test(val) || 'Solo números permitidos'
                  ]"
                >
                  <template v-slot:append>
                    <q-icon name="clear" v-if="amaForm.CI" @click="amaForm.CI = ''" class="cursor-pointer" />
                  </template>
                </q-input>
              </div>
              <div class="col-12">
                <q-input
                  v-model="amaForm.NumeroTelefonico"
                  filled
                  label="Número Telefónico"
                  color="red-7"
                  :rules="[
                    val => !!val || 'Campo requerido',
                    val => (val && /^\d+$/.test(val)) || 'Solo números permitidos'
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
                  use-input
                  use-chips
                  hide-selected
                  fill-input
                  :options="filteredAmas"
                  option-label="label"
                  option-value="value"
                  @filter="filterFnAmas"
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
              <!-- Formulario para Crear/Modificar Reserva -->
              <div class="col-12 col-md-6">
                <q-card flat bordered class="q-pa-md">
                  <q-card-section>
                    <div class="text-h6">Gestión de Reservas</div>
                  </q-card-section>
                  <q-card-section>
                    <q-input
                      v-model="reservaForm.FechaEntrada"
                      filled
                      label="Fecha de Entrada"
                      type="date"
                      color="red-7"
                      :rules="[
                        val => !!val || 'Campo requerido',
                        val => isValidDate(val) || 'Fecha inválida',
                        val => val >= DateTime.now().toISODate() || 'No puede ser anterior a hoy'
                      ]"
                      @update:model-value="validateReservaForm"
                    />
                    <q-input
                      v-model="reservaForm.FechaSalida"
                      filled
                      label="Fecha de Salida"
                      type="date"
                      color="red-7"
                      :rules="[
                        val => !!val || 'Campo requerido',
                        val => isValidDate(val) || 'Fecha inválida',
                        val => val > reservaForm.FechaEntrada || 'Debe ser posterior a la fecha de entrada',
                        val => isMinimumStayValid(val) || 'La reserva debe ser de al menos 3 días'
                      ]"
                      @update:model-value="validateReservaForm"
                    />
                    <q-select
                      v-model="reservaForm.ClienteId"
                      filled
                      label="Seleccionar Cliente"
                      color="red-7"
                      use-input
                      use-chips
                      hide-selected
                      fill-input
                      :options="filteredClientes"
                      option-label="label"
                      option-value="value"
                      @filter="filterFnClientes"
                      :rules="[val => !!val || 'Campo requerido']"
                    />
                    <q-input
                      v-model="reservaForm.HabitacionNumero"
                      filled
                      label="Número de Habitación (0XY)"
                      color="red-7"
                      :rules="[
                        val => !!val || 'Campo requerido',
                        val => /^0[1-3][1-5]$/.test(val) || 'Formato inválido (0XY)'
                      ]"
                      @update:model-value="validateReservaForm"
                    />
                    <div class="q-mt-md">
                      <q-btn
                        label="Crear Reserva"
                        color="red-10"
                        @click="crearReserva"
                        :disable="reservaFormDisabled || !isRoomAvailable"
                      />
                      <q-btn
                        v-if="selectedReservaId"
                        label="Modificar Reserva"
                        color="red-10"
                        class="q-ml-md"
                        @click="modificarReserva"
                        :disable="reservaFormDisabled || !canModifyReserva || !isRoomAvailable"
                      />
                      <q-btn
                        label="Limpiar Formulario"
                        color="grey"
                        class="q-ml-md"
                        @click="resetReservaForm"
                      />
                    </div>
                  </q-card-section>
                </q-card>
              </div>

              <!-- Acciones sobre Reservas -->
              <div class="col-12 col-md-6">
                <q-card flat bordered class="q-pa-md">
                  <q-card-section>
                    <div class="text-h6">Acciones de Reservas</div>
                  </q-card-section>
                  <q-card-section>
                    <q-select
                      v-model="reservaId"
                      filled
                      label="Seleccionar Reserva"
                      color="red-7"
                      use-input
                      use-chips
                      hide-selected
                      fill-input
                      :options="filteredReservas"
                      option-label="label"
                      option-value="value"
                      @filter="filterFnReservas"
                      :rules="[val => !!val || 'Campo requerido']"
                      @update:model-value="updateReservaStatus"
                    />
                    <q-input
                      v-model="motivoCancelacion"
                      filled
                      label="Motivo de Cancelación"
                      color="red-7"
                      :rules="[val => !!val || 'Campo requerido']"
                      :disable="!canCancelReserva"
                    />
                    <q-btn
                      label="Cancelar Reserva"
                      color="red-10"
                      class="q-mt-md"
                      @click="cancelarReserva"
                      :disable="!canCancelReserva || !motivoCancelacion"
                    />
                    <q-input
                      v-model="nuevoNumeroHabitacion"
                      filled
                      label="Nuevo Número de Habitación (0XY)"
                      color="red-7"
                      :rules="[
                        val => !!val || 'Campo requerido',
                        val => /^0[1-3][1-5]$/.test(val) || 'Formato inválido (0XY)'
                      ]"
                      :disable="!canChangeRoom"
                    />
                    <q-btn
                      label="Cambiar Habitación"
                      color="red-10"
                      class="q-mt-md"
                      @click="cambiarHabitacion"
                      :disable="!canChangeRoom || !isNewRoomAvailable || !nuevoNumeroHabitacion"
                    />
                    <q-btn
                      label="Registrar Llegada"
                      color="red-10"
                      class="q-mt-md q-ml-md"
                      @click="registrarLlegada"
                      :disable="!canRegisterArrival"
                    />
                  </q-card-section>
                </q-card>
              </div>

              <!-- Tabla de Reservas -->
              <div class="col-12 q-mt-md">
                <q-card flat bordered class="q-pa-md">
                  <q-card-section>
                    <div class="text-h6">Reservas</div>
                  </q-card-section>
                  <q-card-section>
                    <q-input
                      v-model="fechaActivas"
                      filled
                      label="Filtrar Reservas Activas por Fecha"
                      type="date"
                      color="red-7"
                    />
                    <q-btn
                      label="Ver Reservas Activas"
                      color="red-10"
                      class="q-ml-md"
                      @click="fetchReservasActivas"
                    />
                    <q-btn
                      label="Mostrar Todas"
                      color="red-10"
                      class="q-ml-md"
                      @click="fetchAllReservas"
                    />
                    <q-btn
                      label="Actualizar Reservas Expiradas"
                      color="red-10"
                      class="q-ml-md"
                      @click="updateExpiredReservations"
                    />
                    <q-table
                      :rows="reservasActivas"
                      :columns="reservasColumns"
                      row-key="Id"
                      color="red-7"
                      flat
                      bordered
                      :loading="reservasActivasLoading"
                      :pagination="pagination"
                      @request="onRequestReservas"
                    >
                      <template v-slot:body-cell-actions="props">
                        <q-td :props="props">
                          <q-btn
                            flat
                            round
                            color="red-10"
                            icon="edit"
                            @click="selectReserva(props.row.Id)"
                          />
                        </q-td>
                      </template>
                    </q-table>
                  </q-card-section>
                </q-card>
              </div>

              <!-- Clientes Activos -->
              <div class="col-12 q-mt-md">
                <q-card flat bordered class="q-pa-md">
                  <q-card-section>
                    <div class="text-h6">Clientes Activos</div>
                  </q-card-section>
                  <q-card-section>
                    <q-input
                      v-model="fechaClientesActivos"
                      filled
                      label="Fecha para Clientes Activos"
                      type="date"
                      color="red-7"
                    />
                    <q-btn
                      label="Ver Clientes Activos"
                      color="red-10"
                      class="q-ml-md"
                      @click="fetchClientesActivos"
                    />
                    <q-table
                      :rows="clientesActivos"
                      :columns="clientesActivosColumns"
                      row-key="Id"
                      color="red-7"
                      flat
                      bordered
                      :loading="reservasActivasLoading"
                    />
                  </q-card-section>
                </q-card>
              </div>

              <!-- Habitaciones Disponibles -->
              <div class="col-12 q-mt-md">
                <q-card flat bordered class="q-pa-md">
                  <q-card-section>
                    <div class="text-h6">Habitaciones Disponibles</div>
                  </q-card-section>
                  <q-card-section>
                    <q-input
                      v-model="fechaInicioDisponibilidad"
                      filled
                      label="Fecha Inicio Disponibilidad"
                      type="date"
                      color="red-7"
                    />
                    <q-input
                      v-model="fechaFinDisponibilidad"
                      filled
                      label="Fecha Fin Disponibilidad"
                      type="date"
                      color="red-7"
                      class="q-ml-md"
                    />
                    <q-btn
                      label="Ver Habitaciones Disponibles"
                      color="red-10"
                      class="q-ml-md"
                      @click="fetchHabitacionesDisponibles"
                    />
                    <q-table
                      :rows="habitacionesDisponibles"
                      :columns="habitacionesColumns"
                      row-key="Id"
                      color="red-7"
                      flat
                      bordered
                    />
                  </q-card-section>
                </q-card>
              </div>
            </div>
          </q-tab-panel>

          <!-- Pestaña Trazas -->
          <q-tab-panel name="trazas">
            <div class="row q-col-gutter-md">
              <div class="col-12">
                <q-input
                  v-model="busquedaTraza"
                  filled
                  label="Búsqueda (Operación, Tabla, Detalles)"
                  color="red-7"
                />
                <q-btn
                  label="Actualizar Lista"
                  color="red-10"
                  class="q-ml-md"
                  @click="fetchTrazas"
                />
                <q-table
                  :rows="trazas"
                  :columns="trazasColumns"
                  row-key="Id"
                  color="red-7"
                  flat
                  bordered
                  :loading="trazasLoading"
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
import { DateTime } from 'luxon'

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
      ClienteId: null,
      HabitacionNumero: ''
    })
    const asignacionForm = ref({
      AmaDeLlavesId: null,
      HabitacionNumero: '',
      Turno: ''
    })

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
    const fechaClientesActivos = ref('')
    const fechaInicioDisponibilidad = ref('')
    const fechaFinDisponibilidad = ref('')
    const busquedaTraza = ref('')

    const clientes = ref([])
    const amasDeLlaves = ref([])
    const habitaciones = ref([])
    const reservasActivas = ref([])
    const habitacionesAma = ref([])
    const clientesActivos = ref([])
    const habitacionesDisponibles = ref([])
    const trazas = ref([])
    const clientesLoading = ref(true)
    const amasLoading = ref(true)
    const habitacionesLoading = ref(true)
    const reservasActivasLoading = ref(false)
    const trazasLoading = ref(false)

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
      { name: 'EsVip', label: 'VIP', field: row => row.EsVip ? 'Sí' : 'No', sortable: true },
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
      { name: 'ClienteName', label: 'Nombre del Cliente', field: 'ClienteName', sortable: true },
      { name: 'HabitacionNumero', label: 'Habitación', field: 'HabitacionNumero', sortable: true },
      { name: 'EstaElClienteEnHostal', label: 'Cliente en Hostal', field: 'EstaElClienteEnHostal', sortable: true },
      { name: 'EstaCancelada', label: 'Cancelada', field: 'EstaCancelada', sortable: true },
      { name: 'FechaCancelacion', label: 'Fecha Cancelación', field: 'FechaCancelacion', sortable: true },
      { name: 'MotivoCancelacion', label: 'Motivo Cancelación', field: 'MotivoCancelacion', sortable: true },
      { name: 'actions', label: 'Acciones', field: 'actions', sortable: false }
    ]

    const clientesActivosColumns = [
      { name: 'Id', label: 'ID', field: 'Id', sortable: true },
      { name: 'ClienteName', label: 'Nombre del Cliente', field: 'ClienteName', sortable: true },
      { name: 'HabitacionNumero', label: 'Habitación', field: 'HabitacionNumero', sortable: true }
    ]

    const habitacionesAmaColumns = [
      { name: 'HabitacionNumero', label: 'Habitación', field: 'HabitacionNumero', sortable: true },
      { name: 'Turno', label: 'Turno', field: 'Turno', sortable: true }
    ]

    const trazasColumns = [
      { name: 'Id', label: 'ID', field: 'Id', sortable: true },
      { name: 'Operacion', label: 'Operación', field: 'Operacion', sortable: true },
      { name: 'TablaAfectada', label: 'Tabla', field: 'TablaAfectada', sortable: true },
      { name: 'RegistroId', label: 'Registro ID', field: 'RegistroId', sortable: true },
      { name: 'Fecha', label: 'Fecha', field: 'Fecha', sortable: true },
      { name: 'Detalles', label: 'Detalles', field: 'Detalles', sortable: true }
    ]

    const apiUrl = 'http://localhost:5014/api'

    const toggleDarkMode = () => {
      darkMode.value = !darkMode.value
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
      return !clienteForm.value.NombreApellidos ||
             !clienteForm.value.CI ||
             !clienteForm.value.NumeroTelefonico ||
             !/^[a-zA-Z\s]{1,25}$/.test(clienteForm.value.NombreApellidos) ||
             !/^\d{11}$/.test(clienteForm.value.CI) ||
             !/^\d+$/.test(clienteForm.value.NumeroTelefonico) ||
             clientes.value.some(c => c.CI === clienteForm.value.CI && c.Id !== selectedClienteId.value)
    })

    const amaFormDisabled = computed(() => {
      return !amaForm.value.NombreApellidos ||
             !amaForm.value.CI ||
             !amaForm.value.NumeroTelefonico ||
             !/^[a-zA-Z\s]{1,25}$/.test(amaForm.value.NombreApellidos) ||
             !/^\d{11}$/.test(amaForm.value.CI) ||
             !/^\d+$/.test(amaForm.value.NumeroTelefonico) ||
             amasDeLlaves.value.some(a => a.CI === amaForm.value.CI && a.Id !== selectedAmaId.value)
    })

    const habitacionFormDisabled = computed(() => {
      return !habitacionForm.value.Numero || !/^0[1-3][1-5]$/.test(habitacionForm.value.Numero)
    })

    const reservaFormDisabled = computed(() => {
      return !reservaForm.value.FechaEntrada ||
             !reservaForm.value.FechaSalida ||
             !reservaForm.value.ClienteId ||
             !reservaForm.value.HabitacionNumero ||
             !/^0[1-3][1-5]$/.test(reservaForm.value.HabitacionNumero)
    })

    const asignacionFormDisabled = computed(() => {
      return !asignacionForm.value.AmaDeLlavesId ||
             !asignacionForm.value.HabitacionNumero ||
             !asignacionForm.value.Turno ||
             !/^0[1-3][1-5]$/.test(asignacionForm.value.HabitacionNumero)
    })

    const canModifyReserva = computed(() => {
      if (!selectedReservaId.value) return false
      const reserva = reservasActivas.value.find(r => r.Id === selectedReservaId.value.toString())
      const today = DateTime.now().toISODate()
      return reserva && !reserva.EstaElClienteEnHostal && !reserva.EstaCancelada && today <= reserva.FechaEntrada
    })

    const canCancelReserva = computed(() => {
      if (!reservaId.value) return false
      const reserva = reservasActivas.value.find(r => r.Id === (reservaId.value.value || reservaId.value).toString())
      return reserva && !reserva.EstaElClienteEnHostal && !reserva.EstaCancelada
    })

    const canChangeRoom = computed(() => {
      if (!reservaId.value) return false
      const reserva = reservasActivas.value.find(r => r.Id === (reservaId.value.value || reservaId.value).toString())
      return reserva && !reserva.EstaElClienteEnHostal && !reserva.EstaCancelada
    })

    const canRegisterArrival = computed(() => {
      if (!reservaId.value) return false
      const reserva = reservasActivas.value.find(r => r.Id === (reservaId.value.value || reservaId.value).toString())
      const today = DateTime.now().toISODate()
      return reserva && !reserva.EstaElClienteEnHostal && !reserva.EstaCancelada && today === reserva.FechaEntrada
    })

    const isRoomAvailable = computed(() => {
      if (!reservaForm.value.FechaEntrada || !reservaForm.value.FechaSalida || !reservaForm.value.HabitacionNumero) return false
      const conflict = reservasActivas.value.some(r =>
        r.HabitacionNumero === reservaForm.value.HabitacionNumero &&
        !r.EstaCancelada &&
        r.EstaElClienteEnHostal &&
        r.FechaEntrada <= reservaForm.value.FechaSalida &&
        r.FechaSalida >= reservaForm.value.FechaEntrada
      )
      return !conflict
    })

    const isNewRoomAvailable = computed(() => {
      if (!reservaId.value || !nuevoNumeroHabitacion.value) return false
      const reserva = reservasActivas.value.find(r => r.Id === (reservaId.value.value || reservaId.value).toString())
      if (!reserva) return false
      const conflict = reservasActivas.value.some(r =>
        r.HabitacionNumero === nuevoNumeroHabitacion.value &&
        r.Id !== (reservaId.value.value || reservaId.value).toString() &&
        !r.EstaCancelada &&
        r.EstaElClienteEnHostal &&
        r.FechaEntrada <= reserva.FechaSalida &&
        r.FechaSalida >= reserva.FechaEntrada
      )
      return !conflict
    })

    const filteredAmas = ref([])
    const filteredClientes = ref([])
    const filteredReservas = ref([])

    const filterFnAmas = (val, update) => {
      update(() => {
        const needle = val.toLowerCase()
        filteredAmas.value = amasDeLlaves.value
          .filter(v => v.NombreApellidos.toLowerCase().includes(needle))
          .map(v => ({ label: v.NombreApellidos, value: v.Id }))
      })
    }

    const filterFnClientes = (val, update) => {
      update(() => {
        const needle = val.toLowerCase()
        filteredClientes.value = clientes.value
          .filter(v => `${v.NombreApellidos} (CI: ${v.CI})`.toLowerCase().includes(needle))
          .map(v => ({ label: `${v.NombreApellidos} (CI: ${v.CI})`, value: v.Id }))
      })
    }

    const filterFnReservas = (val, update) => {
      update(() => {
        const needle = val.toLowerCase()
        filteredReservas.value = reservasActivas.value
          .filter(r => !r.EstaCancelada)
          .filter(v => `Reserva #${v.Id} - ${v.ClienteName}`.toLowerCase().includes(needle))
          .map(v => ({ label: `Reserva #${v.Id} - ${v.ClienteName}`, value: v.Id }))
      })
    }

    const isValidDate = (date) => {
      return DateTime.fromISO(date).isValid
    }

    const isMinimumStayValid = (fechaSalida) => {
      if (!reservaForm.value.FechaEntrada || !fechaSalida) return false
      const entrada = DateTime.fromISO(reservaForm.value.FechaEntrada)
      const salida = DateTime.fromISO(fechaSalida)
      return salida.diff(entrada, 'days').days >= 2 // Mínimo 3 días (2 noches)
    }

    const validateReservaForm = () => {
      // Validación manejada por los :rules en el template
    }

    const resetReservaForm = () => {
      reservaForm.value = { FechaEntrada: '', FechaSalida: '', ClienteId: null, HabitacionNumero: '' }
      selectedReservaId.value = null
      reservaId.value = null
      motivoCancelacion.value = ''
      nuevoNumeroHabitacion.value = ''
    }

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
        showNotification(error.response?.data?.message || 'Error al cargar clientes', 'negative')
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
        showNotification(error.response?.data?.message || 'Error al cargar amas de llaves', 'negative')
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
        showNotification(error.response?.data?.message || 'Error al cargar habitaciones', 'negative')
      } finally {
        habitacionesLoading.value = false
      }
    }

    const onRequestReservas = async ({ pagination: { page, rowsPerPage } }) => {
      reservasActivasLoading.value = true
      try {
        const params = fechaActivas.value ? { fecha: fechaActivas.value, pagina: page, tamanoPagina: rowsPerPage } : { pagina: page, tamanoPagina: rowsPerPage }
        const response = await axios.get(`${apiUrl}/Reservas/activas`, { params })
        reservasActivas.value = response.data.data.map(reserva => ({
          Id: reserva.Id?.toString() || 'N/A',
          FechaReservacion: reserva.FechaReservacion ? DateTime.fromISO(reserva.FechaReservacion).toFormat('yyyy-MM-dd') : 'N/A',
          FechaEntrada: reserva.FechaEntrada ? DateTime.fromISO(reserva.FechaEntrada).toFormat('yyyy-MM-dd') : 'N/A',
          FechaSalida: reserva.FechaSalida ? DateTime.fromISO(reserva.FechaSalida).toFormat('yyyy-MM-dd') : 'N/A',
          Importe: reserva.Importe?.toFixed(2) || '0.00',
          ClienteName: reserva.ClienteNombre || 'Desconocido',
          HabitacionNumero: reserva.HabitacionNumero || 'N/A',
          EstaElClienteEnHostal: reserva.EstaElClienteEnHostal ? 'Sí' : 'No',
          EstaCancelada: reserva.EstaCancelada ? 'Sí' : 'No',
          FechaCancelacion: reserva.FechaCancelacion ? DateTime.fromISO(reserva.FechaCancelacion).toFormat('yyyy-MM-dd') : 'N/A',
          MotivoCancelacion: reserva.MotivoCancelacion || 'N/A'
        }))
        pagination.value.page = page
        pagination.value.rowsPerPage = rowsPerPage
        filterFnReservas('', () => {})
      } catch (error) {
        showNotification(error.response?.data?.message || 'Error al cargar reservas activas', 'negative')
        console.error('Error fetching reservas activas:', error.response?.data || error)
      } finally {
        reservasActivasLoading.value = false
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
        await onRequestClientes({ pagination: pagination.value })
      } catch (error) {
        showNotification(error.response?.data?.message || 'Error al crear cliente', 'negative')
        console.error('Error detallado:', error.response?.data || error)
      }
    }

    const selectCliente = async (id) => {
      try {
        const response = await axios.get(`${apiUrl}/Clientes/${id}`)
        clienteForm.value = { ...response.data }
        selectedClienteId.value = id
      } catch (error) {
        showNotification(error.response?.data?.message || 'Error al obtener cliente', 'negative')
      }
    }

    const modificarCliente = async () => {
      if (!selectedClienteId.value) return
      try {
        await axios.put(`${apiUrl}/Clientes/${selectedClienteId.value}`, clienteForm.value)
        showNotification('Cliente modificado con éxito')
        clienteForm.value = { NombreApellidos: '', CI: '', NumeroTelefonico: '', EsVip: false }
        selectedClienteId.value = null
        await onRequestClientes({ pagination: pagination.value })
      } catch (error) {
        showNotification(error.response?.data?.message || 'Error al modificar cliente', 'negative')
      }
    }

    const eliminarCliente = async (id) => {
      if (confirm(`¿Eliminar cliente con ID: ${id}?`)) {
        try {
          await axios.delete(`${apiUrl}/Clientes/${id}`)
          showNotification('Cliente eliminado')
          await onRequestClientes({ pagination: pagination.value })
        } catch (error) {
          showNotification(error.response?.data?.message || 'Error al eliminar cliente', 'negative')
        }
      }
    }

    const crearAmaDeLlaves = async () => {
      try {
        const response = await axios.post(`${apiUrl}/AmasDeLlaves`, amaForm.value)
        showNotification('Ama de llaves creada con éxito')
        amaForm.value = { NombreApellidos: '', CI: '', NumeroTelefonico: '' }
        selectedAmaId.value = null
        await onRequestAmas({ pagination: pagination.value })
      } catch (error) {
        showNotification(error.response?.data?.message || 'Error al crear ama de llaves', 'negative')
        console.error('Error detallado:', error.response?.data || error)
      }
    }

    const selectAma = async (id) => {
      try {
        const response = await axios.get(`${apiUrl}/AmasDeLlaves/${id}`)
        amaForm.value = { ...response.data }
        selectedAmaId.value = id
      } catch (error) {
        showNotification(error.response?.data?.message || 'Error al obtener ama de llaves', 'negative')
      }
    }

    const modificarAmaDeLlaves = async () => {
      if (!selectedAmaId.value) return
      try {
        await axios.put(`${apiUrl}/AmasDeLlaves/${selectedAmaId.value}`, amaForm.value)
        showNotification('Ama de llaves modificada con éxito')
        amaForm.value = { NombreApellidos: '', CI: '', NumeroTelefonico: '' }
        selectedAmaId.value = null
        await onRequestAmas({ pagination: pagination.value })
      } catch (error) {
        showNotification(error.response?.data?.message || 'Error al modificar ama de llaves', 'negative')
      }
    }

    const eliminarAmaDeLlaves = async (id) => {
      if (confirm(`¿Eliminar ama de llaves con ID: ${id}?`)) {
        try {
          await axios.delete(`${apiUrl}/AmasDeLlaves/${id}`)
          showNotification('Ama de llaves eliminada')
          await onRequestAmas({ pagination: pagination.value })
        } catch (error) {
          showNotification(error.response?.data?.message || 'Error al eliminar ama de llaves', 'negative')
        }
      }
    }

    const asignarHabitacion = async () => {
      if (asignacionFormDisabled.value) {
        showNotification('Formulario incompleto, verifica los campos', 'negative')
        return
      }
      const amaDeLlavesId = asignacionForm.value.AmaDeLlavesId?.value || asignacionForm.value.AmaDeLlavesId
      if (!amaDeLlavesId || isNaN(Number(amaDeLlavesId))) {
        showNotification('Debe proporcionar un ID de Ama de Llaves válido', 'negative')
        return
      }
      try {
        await axios.post(`${apiUrl}/AmasDeLlaves/asignar-habitacion-ama`, {
          AmaDeLlavesId: Number(amaDeLlavesId),
          HabitacionNumero: asignacionForm.value.HabitacionNumero,
          Turno: asignacionForm.value.Turno
        })
        showNotification('Habitación asignada con éxito')
        await fetchHabitacionesPorAma()
        asignacionForm.value = { AmaDeLlavesId: null, HabitacionNumero: '', Turno: '' }
      } catch (error) {
        showNotification(error.response?.data?.message || 'Error al asignar habitación', 'negative')
        console.error('Error detallado:', error.response?.data || error)
      }
    }

    const desasignarHabitacion = async () => {
      if (asignacionFormDisabled.value) {
        showNotification('Formulario incompleto, verifica los campos', 'negative')
        return
      }
      const amaDeLlavesId = asignacionForm.value.AmaDeLlavesId?.value || asignacionForm.value.AmaDeLlavesId
      if (!amaDeLlavesId || isNaN(Number(amaDeLlavesId))) {
        showNotification('Debe proporcionar un ID de Ama de Llaves válido', 'negative')
        return
      }
      try {
        await axios.post(`${apiUrl}/AmasDeLlaves/desasignar-habitacion`, {
          AmaDeLlavesId: Number(amaDeLlavesId),
          HabitacionNumero: asignacionForm.value.HabitacionNumero
        })
        showNotification('Habitación desasignada con éxito')
        await fetchHabitacionesPorAma()
        asignacionForm.value = { AmaDeLlavesId: null, HabitacionNumero: '', Turno: '' }
      } catch (error) {
        showNotification(error.response?.data?.message || 'Error al desasignar habitación', 'negative')
        console.error('Error detallado:', error.response?.data || error)
      }
    }

    const fetchHabitacionesPorAma = async () => {
      const amaDeLlavesId = asignacionForm.value.AmaDeLlavesId?.value || asignacionForm.value.AmaDeLlavesId
      if (!amaDeLlavesId) {
        habitacionesAma.value = []
        showNotification('Seleccione una ama de llaves', 'negative')
        return
      }
      try {
        const response = await axios.get(`${apiUrl}/AmasDeLlaves/habitaciones-por-ama-de-llaves/${amaDeLlavesId}`)
        habitacionesAma.value = response.data.map(h => ({
          HabitacionNumero: h.Habitacion?.Numero || 'Sin asignar',
          Turno: h.Turno || 'N/A'
        }))
      } catch (error) {
        console.error('Error fetching habitaciones por ama:', error)
        showNotification(error.response?.data?.message || 'Error al cargar habitaciones asignadas', 'negative')
        habitacionesAma.value = []
      }
    }

    const crearHabitacion = async () => {
      if (habitacionFormDisabled.value) {
        showNotification('Formulario incompleto, verifica los campos', 'negative')
        return
      }
      try {
        await axios.post(`${apiUrl}/Habitaciones`, habitacionForm.value)
        showNotification('Habitación creada con éxito')
        habitacionForm.value = { Numero: '', EstaFueraDeServicio: false }
        selectedHabitacionId.value = null
        await onRequestHabitaciones({ pagination: pagination.value })
      } catch (error) {
        showNotification(error.response?.data?.message || 'Error al crear habitación', 'negative')
      }
    }

    const selectHabitacion = async (id) => {
      try {
        const response = await axios.get(`${apiUrl}/Habitaciones/${id}`)
        habitacionForm.value = { ...response.data }
        selectedHabitacionId.value = id
      } catch (error) {
        showNotification(error.response?.data?.message || 'Error al obtener habitación', 'negative')
      }
    }

    const modificarHabitacion = async () => {
      if (!selectedHabitacionId.value) return
      try {
        await axios.put(`${apiUrl}/Habitaciones/${selectedHabitacionId.value}`, habitacionForm.value)
        showNotification('Habitación modificada con éxito')
        habitacionForm.value = { Numero: '', EstaFueraDeServicio: false }
        selectedHabitacionId.value = null
        await onRequestHabitaciones({ pagination: pagination.value })
      } catch (error) {
        showNotification(error.response?.data?.message || 'Error al modificar habitación', 'negative')
      }
    }

    const eliminarHabitacion = async (id) => {
      if (confirm(`¿Eliminar habitación con ID: ${id}?`)) {
        try {
          await axios.delete(`${apiUrl}/Habitaciones/${id}`)
          showNotification('Habitación eliminada')
          await onRequestHabitaciones({ pagination: pagination.value })
        } catch (error) {
          showNotification(error.response?.data?.message || 'Error al eliminar habitación', 'negative')
        }
      }
    }

    const crearReserva = async () => {
      if (reservaFormDisabled.value || !isRoomAvailable.value) {
        showNotification('Formulario incompleto o habitación no disponible', 'negative')
        return
      }
      try {
        const payload = {
          FechaEntrada: reservaForm.value.FechaEntrada,
          FechaSalida: reservaForm.value.FechaSalida,
          ClienteId: Number(reservaForm.value.ClienteId?.value || reservaForm.value.ClienteId),
          HabitacionNumero: reservaForm.value.HabitacionNumero
        }
        const response = await axios.post(`${apiUrl}/Reservas`, payload)
        showNotification(`Reserva creada con éxito. ID: ${response.data.Id}`)
        resetReservaForm()
        await fetchReservasActivas()
      } catch (error) {
        showNotification(error.response?.data?.message || 'Error al crear reserva', 'negative')
        console.error('Error detallado en crearReserva:', error.response?.data || error)
      }
    }

    const selectReserva = async (id) => {
      try {
        const response = await axios.get(`${apiUrl}/Reservas/${id}`)
        const reserva = response.data
        reservaForm.value = {
          FechaEntrada: DateTime.fromISO(reserva.FechaEntrada).toFormat('yyyy-MM-dd'),
          FechaSalida: DateTime.fromISO(reserva.FechaSalida).toFormat('yyyy-MM-dd'),
          ClienteId: clientes.value.find(c => c.Id === reserva.ClienteId) ? {
            label: clientes.value.find(c => c.Id === reserva.ClienteId).NombreApellidos,
            value: reserva.ClienteId
          } : null,
          HabitacionNumero: reserva.HabitacionNumero
        }
        selectedReservaId.value = id
        reservaId.value = { value: id }
        validateReservaForm()
      } catch (error) {
        showNotification(error.response?.data?.message || 'Error al obtener reserva', 'negative')
        console.error('Error detallado:', error.response?.data || error)
      }
    }

    const modificarReserva = async () => {
      if (!selectedReservaId.value || !canModifyReserva.value || !isRoomAvailable.value) {
        showNotification('No se puede modificar la reserva', 'negative')
        return
      }
      try {
        const payload = {
          FechaEntrada: reservaForm.value.FechaEntrada,
          FechaSalida: reservaForm.value.FechaSalida,
          ClienteId: Number(reservaForm.value.ClienteId?.value || reservaForm.value.ClienteId),
          HabitacionNumero: reservaForm.value.HabitacionNumero
        }
        await axios.put(`${apiUrl}/Reservas/${selectedReservaId.value}`, payload)
        showNotification('Reserva modificada con éxito')
        resetReservaForm()
        await fetchReservasActivas()
      } catch (error) {
        showNotification(error.response?.data?.message || 'Error al modificar reserva', 'negative')
        console.error('Error detallado:', error.response?.data || error)
      }
    }

    const cancelarReserva = async () => {
      if (!reservaId.value || !motivoCancelacion.value || !canCancelReserva.value) {
        showNotification('Formulario incompleto o reserva no puede ser cancelada', 'negative')
        return
      }
      try {
        const id = reservaId.value.value || reservaId.value
        await axios.put(`${apiUrl}/Reservas/${id}/cancelar`, { motivoCancelacion: motivoCancelacion.value })
        showNotification('Reserva cancelada con éxito')
        resetReservaForm()
        await fetchReservasActivas()
      } catch (error) {
        showNotification(error.response?.data?.message || 'Error al cancelar reserva', 'negative')
        console.error('Error detallado:', error.response?.data || error)
      }
    }

    const cambiarHabitacion = async () => {
      if (!reservaId.value || !nuevoNumeroHabitacion.value || !canChangeRoom.value || !isNewRoomAvailable.value) {
        showNotification('Formulario incompleto o habitación no disponible', 'negative')
        return
      }
      try {
        const id = reservaId.value.value || reservaId.value
        await axios.put(`${apiUrl}/Reservas/${id}/cambiar-habitacion`, { nuevoNumeroHabitacion: nuevoNumeroHabitacion.value })
        showNotification('Habitación cambiada con éxito')
        resetReservaForm()
        await fetchReservasActivas()
      } catch (error) {
        showNotification(error.response?.data?.message || 'Error al cambiar habitación', 'negative')
        console.error('Error detallado:', error.response?.data || error)
      }
    }

    const registrarLlegada = async () => {
      if (!reservaId.value || !canRegisterArrival.value) {
        showNotification('No se puede registrar la llegada', 'negative')
        return
      }
      try {
        const id = reservaId.value.value || reservaId.value
        await axios.put(`${apiUrl}/Reservas/${id}/registrar-llegada`)
        showNotification('Llegada registrada con éxito')
        resetReservaForm()
        await fetchReservasActivas()
      } catch (error) {
        showNotification(error.response?.data?.message || 'Error al registrar llegada', 'negative')
        console.error('Error detallado:', error.response?.data || error)
      }
    }

    const fetchReservasActivas = async () => {
      await onRequestReservas({ pagination: pagination.value })
    }

    const fetchAllReservas = async () => {
      reservasActivasLoading.value = true
      try {
        const response = await axios.get(`${apiUrl}/Reservas`)
        reservasActivas.value = response.data.map(reserva => ({
          Id: reserva.Id?.toString() || 'N/A',
          FechaReservacion: reserva.FechaReservacion ? DateTime.fromISO(reserva.FechaReservacion).toFormat('yyyy-MM-dd') : 'N/A',
          FechaEntrada: reserva.FechaEntrada ? DateTime.fromISO(reserva.FechaEntrada).toFormat('yyyy-MM-dd') : 'N/A',
          FechaSalida: reserva.FechaSalida ? DateTime.fromISO(reserva.FechaSalida).toFormat('yyyy-MM-dd') : 'N/A',
          Importe: reserva.Importe?.toFixed(2) || '0.00',
          ClienteName: reserva.ClienteNombre || 'Desconocido',
          HabitacionNumero: reserva.HabitacionNumero || 'N/A',
          EstaElClienteEnHostal: reserva.EstaElClienteEnHostal ? 'Sí' : 'No',
          EstaCancelada: reserva.EstaCancelada ? 'Sí' : 'No',
          FechaCancelacion: reserva.FechaCancelacion ? DateTime.fromISO(reserva.FechaCancelacion).toFormat('yyyy-MM-dd') : 'N/A',
          MotivoCancelacion: reserva.MotivoCancelacion || 'N/A'
        }))
        filterFnReservas('', () => {})
      } catch (error) {
        showNotification(error.response?.data?.message || 'Error al cargar todas las reservas', 'negative')
        console.error('Error fetching all reservas:', error.response?.data || error)
      } finally {
        reservasActivasLoading.value = false
      }
    }

    const updateExpiredReservations = async () => {
      try {
        const response = await axios.post(`${apiUrl}/Reservas/actualizar-expiradas`)
        showNotification(response.data.message || 'Reservas expiradas actualizadas manualmente')
        await fetchReservasActivas()
      } catch (error) {
        showNotification(error.response?.data?.message || 'Error al actualizar reservas expiradas', 'negative')
        console.error('Error updating expired reservations:', error.response?.data || error)
      }
    }

    const fetchClientesActivos = async () => {
      if (!fechaClientesActivos.value) {
        showNotification('Por favor, seleccione una fecha', 'negative')
        return
      }
      try {
        const response = await axios.get(`${apiUrl}/Reservas/activas`, { params: { fecha: fechaClientesActivos.value } })
        clientesActivos.value = response.data.data.map(reserva => ({
          Id: reserva.Id?.toString() || 'N/A',
          ClienteName: reserva.ClienteNombre || 'Desconocido',
          HabitacionNumero: reserva.HabitacionNumero || 'N/A'
        }))
      } catch (error) {
        showNotification(error.response?.data?.message || 'Error al cargar clientes activos', 'negative')
        console.error('Error fetching clientes activos:', error.response?.data || error)
      }
    }

    const fetchHabitacionesDisponibles = async () => {
      if (!fechaInicioDisponibilidad.value || !fechaFinDisponibilidad.value) {
        showNotification('Por favor, seleccione ambas fechas', 'negative')
        return
      }
      try {
        const response = await axios.get(`${apiUrl}/Habitaciones/disponibles`, {
          params: {
            fechaInicio: fechaInicioDisponibilidad.value,
            fechaFin: fechaFinDisponibilidad.value
          }
        })
        habitacionesDisponibles.value = response.data.map(h => ({
          Id: h.Id,
          Numero: h.Numero,
          Estado: h.EstaFueraDeServicio ? 'Fuera de Servicio' : 'Disponible'
        }))
      } catch (error) {
        console.error('Error fetching habitaciones disponibles:', error)
        showNotification(error.response?.data?.message || 'Error al cargar habitaciones disponibles', 'negative')
      }
    }

    const fetchTrazas = async () => {
      trazasLoading.value = true
      try {
        const response = await axios.get(`${apiUrl}/Trazas`, {
          params: { busqueda: busquedaTraza.value }
        })
        trazas.value = response.data.map(t => ({
          Id: t.Id,
          Operacion: t.Operacion,
          TablaAfectada: t.TablaAfectada,
          RegistroId: t.RegistroId,
          Fecha: DateTime.fromISO(t.Fecha).toFormat('yyyy-MM-dd'),
          Detalles: t.Detalles
        }))
      } catch (error) {
        console.error('Error fetching trazas:', error)
        showNotification(error.response?.data?.message || 'Error al cargar trazas', 'negative')
      } finally {
        trazasLoading.value = false
      }
    }

    const updateReservaStatus = () => {
      if (!reservaId.value) return
      const reserva = reservasActivas.value.find(r => r.Id === (reservaId.value.value || reservaId.value).toString())
      if (reserva) {
        canCancelReserva.value = !reserva.EstaElClienteEnHostal && !reserva.EstaCancelada
        canChangeRoom.value = !reserva.EstaElClienteEnHostal && !reserva.EstaCancelada
        canRegisterArrival.value = !reserva.EstaElClienteEnHostal && !reserva.EstaCancelada &&
          DateTime.now().toISODate() === reserva.FechaEntrada
      }
    }

    onMounted(() => {
      onRequestClientes({ pagination: pagination.value })
      onRequestAmas({ pagination: pagination.value })
      onRequestHabitaciones({ pagination: pagination.value })
    })

    return {
      tab,
      darkMode,
      toggleDarkMode,
      clienteForm,
      amaForm,
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
      fechaClientesActivos,
      fechaInicioDisponibilidad,
      fechaFinDisponibilidad,
      busquedaTraza,
      clientes,
      amasDeLlaves,
      habitaciones,
      reservasActivas,
      habitacionesAma,
      clientesActivos,
      habitacionesDisponibles,
      trazas,
      clientesColumns,
      amasColumns,
      habitacionesColumns,
      reservasColumns,
      clientesActivosColumns,
      habitacionesAmaColumns,
      trazasColumns,
      pagination,
      clienteFormDisabled,
      amaFormDisabled,
      habitacionFormDisabled,
      reservaFormDisabled,
      asignacionFormDisabled,
      canModifyReserva,
      canCancelReserva,
      canChangeRoom,
      canRegisterArrival,
      isRoomAvailable,
      isNewRoomAvailable,
      filteredAmas,
      filteredClientes,
      filteredReservas,
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
      crearReserva,
      selectReserva,
      modificarReserva,
      cancelarReserva,
      cambiarHabitacion,
      registrarLlegada,
      fetchReservasActivas,
      fetchAllReservas,
      updateExpiredReservations,
      fetchClientesActivos,
      fetchHabitacionesDisponibles,
      fetchTrazas,
      onRequestClientes,
      onRequestAmas,
      onRequestHabitaciones,
      onRequestReservas,
      filterFnAmas,
      filterFnClientes,
      filterFnReservas,
      isValidDate,
      isMinimumStayValid,
      validateReservaForm,
      resetReservaForm,
      clientesLoading,
      amasLoading,
      habitacionesLoading,
      reservasActivasLoading,
      trazasLoading,
      updateReservaStatus
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