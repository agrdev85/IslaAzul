<template>
  <div class="row q-col-gutter-lg">
    <!-- Formulario de Reserva -->
    <div class="col-12 col-lg-6">
      <q-card class="shadow-2">
        <q-card-section class="bg-green-1">
          <div class="text-h6 text-green-10 flex items-center">
            <q-icon name="event_available" class="q-mr-sm" />
            {{ selectedReservaId ? "Modificar Reserva" : "Nueva Reserva" }}
          </div>
        </q-card-section>

        <q-card-section class="q-gutter-md">
          <div class="row q-col-gutter-md">
            <div class="col-6">
              <q-input
                v-model="reservaForm.FechaEntrada"
                filled
                label="Fecha de Entrada"
                type="date"
                color="green-7"
                :rules="validationRules.fechaEntrada"
              >
                <template v-slot:prepend>
                  <q-icon name="event" color="green-7" />
                </template>
              </q-input>
            </div>
            <div class="col-6">
              <q-input
                v-model="reservaForm.FechaSalida"
                filled
                label="Fecha de Salida"
                type="date"
                color="green-7"
                :rules="[(val) => validationRules.fechaSalida(val, reservaForm.FechaEntrada)]"
              >
                <template v-slot:prepend>
                  <q-icon name="event" color="green-7" />
                </template>
              </q-input>
            </div>
          </div>

          <q-select
            v-model="reservaForm.ClienteId"
            filled
            label="Seleccionar Cliente"
            color="green-7"
            use-input
            use-chips
            hide-selected
            fill-input
            :options="clienteOptions"
            option-label="label"
            option-value="value"
            :rules="[validationRules.required]"
            :loading="loading"
          >
            <template v-slot:prepend>
              <q-icon name="person" color="green-7" />
            </template>
            <template v-slot:hint v-if="!clienteOptions.length && !loading">
              No hay clientes disponibles
            </template>
          </q-select>

          <q-select
            v-model="reservaForm.HabitacionNumero"
            filled
            label="Seleccionar Habitación"
            color="green-7"
            :options="habitacionOptions"
            option-label="label"
            option-value="value"
            :rules="[(val) => validationRules.habitacion(val?.value || val)]"
            :loading="loading"
            hint="Seleccione una habitación disponible para las fechas indicadas"
            :error="!reservaForm.HabitacionNumero && habitacionOptions.length > 0"
            error-message="Por favor, seleccione una habitación"
          >
            <template v-slot:prepend>
              <q-icon name="hotel" color="green-7" />
            </template>
            <template v-slot:hint v-if="!habitacionOptions.length && !loading">
              No hay habitaciones disponibles
            </template>
          </q-select>

          <div class="bg-green-1 q-pa-md rounded-borders" v-if="calcularNoches >= 3">
            <div class="text-subtitle2 text-green-10 q-mb-sm">Resumen de la Reserva</div>
            <div class="row">
              <div class="col-4">
                <div class="text-caption text-grey-7">Días:</div>
                <div class="text-body1 text-weight-medium">{{ calcularNoches }} días</div>
              </div>
              <div class="col-4">
                <div class="text-caption text-grey-7">Precio/día:</div>
                <div class="text-body1 text-weight-medium">$10 USD</div>
              </div>
              <div class="col-4">
                <div class="text-caption text-grey-7">Total:</div>
                <div class="text-h6 text-green-10 text-weight-bold">${{ calcularTotal }}</div>
              </div>
            </div>
            <div v-if="clienteEsVip" class="text-caption text-amber-8 q-mt-sm">
              <q-icon name="star" /> Cliente VIP - 10% descuento aplicado
            </div>
          </div>

          <q-btn
            :label="selectedReservaId ? 'Modificar Reserva' : 'Crear Reserva'"
            color="green-10"
            icon="save"
            @click="selectedReservaId ? modificarReserva() : crearReserva()"
            :loading="loading"
            :disable="formDisabled"
            class="full-width"
            size="lg"
          />

          <q-btn
            label="Limpiar Formulario"
            color="grey"
            icon="clear"
            @click="resetForm"
            outline
            class="full-width"
          />
        </q-card-section>
      </q-card>
    </div>

      <q-card class="shadow-2 q-mb-md">
        <q-card-section class="bg-blue-1">
          <div class="col-12 col-lg-6 text-h6 text-blue-10 flex items-center">
            <q-icon name="settings" class="q-mr-sm" />
            Acciones de Reservas
          </div>
        </q-card-section>

        <q-card-section class="q-gutter-md">
          <q-select
            v-model="selectedReservaAccion"
            filled
            label="Seleccionar Reserva para Acciones"
            color="blue-7"
            :options="reservaOptions"
            option-label="label"
            option-value="value"
            clearable
            :loading="loading"
          >
            <template v-slot:prepend>
              <q-icon name="event_note" color="blue-7" />
            </template>
          </q-select>

          <div class="row q-col-gutter-sm">
            <div class="col-6">
              <q-btn
                label="Registrar Llegada"
                color="green-10"
                icon="check_in"
                @click="registrarLlegada"
                :disable="!selectedReservaAccion || !canRegisterArrival"
                class="full-width"
              />
            </div>
            <div class="col-6">
              <q-btn
                label="Cambiar Habitación"
                color="blue-10"
                icon="swap_horiz"
                @click="showCambiarHabitacion = true"
                :disable="!selectedReservaAccion || !canChangeRoom"
                class="full-width"
              />
            </div>
          </div>

          <q-input
            v-model="motivoCancelacion"
            filled
            label="Motivo de Cancelación"
            color="blue-7"
            type="textarea"
            rows="2"
            :disable="!selectedReservaAccion || !canCancelReserva"
            :rules="[validationRules.required]"
          />

          <q-btn
            label="Cancelar Reserva"
            color="red-10"
            icon="cancel"
            @click="cancelarReserva"
            :disable="!selectedReservaAccion || !motivoCancelacion || !canCancelReserva"
            class="full-width"
          />
        </q-card-section>
      </q-card>

      <!-- Filtros de Búsqueda y Acciones -->
    <div class="col-12 col-lg-6">
      <q-card class="shadow-2 q-mb-md">
        <q-card-section class="bg-orange-1">
          <div class="text-h6 text-orange-10 flex items-center">
            <q-icon name="filter_list" class="q-mr-sm" />
            Filtros de Búsqueda
          </div>
        </q-card-section>
        <q-card-section class="q-gutter-md">
          <div class="row q-col-gutter-md">
            <div class="col-4">
              <q-input
                v-model="fechaInicioFiltro"
                filled
                label="Fecha de Reservación (Inicio)"
                type="date"
                color="orange-7"
              >
                <template v-slot:prepend>
                  <q-icon name="event" color="orange-7" />
                </template>
              </q-input>
            </div>
            <div class="col-4">
              <q-input
                v-model="fechaFinFiltro"
                filled
                label="Fecha de Reservación (Fin)"
                type="date"
                color="orange-7"
              >
                <template v-slot:prepend>
                  <q-icon name="event" color="orange-7" />
                </template>
              </q-input>
            </div>
            <div class="col-4">
              <q-select
                v-model="estadoFiltro"
                filled
                label="Estado"
                color="orange-7"
                :options="estadoOptions"
                option-label="label"
                option-value="value"
                clearable
              >
                <template v-slot:prepend>
                  <q-icon name="filter_alt" color="orange-7" />
                </template>
              </q-select>
            </div>
          </div>
          <q-btn
            label="Aplicar Filtros"
            color="orange-10"
            icon="search"
            @click="fetchReservas"
            :loading="loading"
            class="full-width"
          />
          <q-btn
            label="Limpiar Filtros"
            color="grey"
            icon="clear"
            @click="fechaInicioFiltro = ''; fechaFinFiltro = ''; estadoFiltro = null; fetchReservas()"
            outline
            class="full-width"
          />
        </q-card-section>
      </q-card>
       
      <!-- Lista de Reservas -->
      <q-card class="shadow-2">
        <q-card-section class="bg-orange-1">
          <div class="text-h6 text-orange-10 flex items-center justify-between">
            <div class="flex items-center">
              <q-icon name="event_note" class="q-mr-sm" />
              Todas las Reservas
            </div>
            <q-btn
              flat
              round
              icon="refresh"
              color="orange-10"
              @click="fetchReservas"
              :loading="loading"
            />
          </div>
        </q-card-section>

        <q-card-section class="q-pa-none">
          <q-table
            :rows="reservas || []"
            :columns="columns"
            row-key="Id"
            :loading="loading"
            :pagination="{ rowsPerPage: 10, sortBy: 'FechaEntrada', descending: false }"
            @row-click="onRowClick"
          >
            <template v-slot:body-cell-Estado="props">
              <q-td :props="props">
                <q-chip :color="getReservaChipColor(props.row)" text-color="white" dense>
                  {{ getReservaStatus(props.row) }}
                </q-chip>
              </q-td>
            </template>
            <template v-slot:body-cell-actions="props">
              <q-td :props="props">
                <q-btn
                  flat
                  round
                  color="red"
                  icon="edit"
                  size="sm"
                  @click.stop="selectReserva(props.row.Id)"
                >
                  <q-tooltip>Editar Reserva</q-tooltip>
                </q-btn>
              </q-td>
            </template>
          </q-table>
          <div v-if="!reservas?.length && !loading" class="text-center q-pa-lg text-grey-6">
            <q-icon name="event_busy" size="3rem" />
            <div class="q-mt-md">No hay reservas</div>
          </div>
        </q-card-section>
      </q-card>
    </div>
  </div>

  <!-- Diálogo Cambiar Habitación -->
  <q-dialog v-model="showCambiarHabitacion" persistent>
    <q-card style="min-width: 350px">
      <q-card-section class="bg-blue-1">
        <div class="text-h6 text-blue-10">Cambiar Habitación de Reserva</div>
      </q-card-section>

      <q-card-section class="q-gutter-md">
        <q-input
          v-model="nuevaHabitacion"
          filled
          label="Nueva Habitación (0XY)"
          color="blue-7"
          :rules="[validationRules.habitacion]"
          hint="Formato: 0XY donde X es el piso (1-3) y Y es la habitación (1-5)"
          :disable="loading"
        >
          <template v-slot:prepend>
            <q-icon name="hotel" color="blue-7" />
          </template>
        </q-input>
      </q-card-section>

      <q-card-actions align="right">
        <q-btn flat label="Cancelar" color="grey" @click="showCambiarHabitacion = false; nuevaHabitacion = ''" />
        <q-btn
          label="Cambiar Habitación"
          color="blue-10"
          @click="cambiarHabitacion"
          :disable="!nuevaHabitacion || !validationRules.habitacion(nuevaHabitacion)"
        />
      </q-card-actions>
    </q-card>
  </q-dialog>
</template>

<script setup>
import { ref, onMounted } from "vue"
import { useReservas } from "src/composables/useReservas"
import { validationRules } from "src/utils/validators"

const {
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
  getReservaChipColor,
  getReservaStatus,
} = useReservas()

const showCambiarHabitacion = ref(false)

const columns = [
  { name: "FechaReservacion", label: "Fecha de Reservación", field: "FechaReservacion", align: "center", sortable: true },
  { name: "ClienteNombre", label: "Cliente", field: "ClienteNombre", align: "left", sortable: true },
  { name: "HabitacionNumero", label: "Habitación", field: "HabitacionNumero", align: "center", sortable: true },
  { name: "FechaEntrada", label: "Entrada", field: "FechaEntrada", align: "center", sortable: true },
  { name: "FechaSalida", label: "Salida", field: "FechaSalida", align: "center", sortable: true },
  { name: "Importe", label: "Importe", field: "Importe", align: "center", format: (val) => `$${val}`, sortable: true },
  { name: "Estado", label: "Estado", field: (row) => getReservaStatus(row), align: "center", sortable: true },
  { name: "actions", label: "Acciones", field: "actions", align: "center" },
]

const onRowClick = (evt, row) => {
  selectReservaFromList(row)
}

onMounted(async () => {
  await Promise.all([fetchClientes(), fetchReservas(), fetchReservasActivas()])
})
</script>