<template>
  <div class="row q-col-gutter-lg">
    
    <!-- Formulario de Ama de Llaves -->
    <div class="col-12 col-lg-6">
      <q-card class="shadow-2">
        <q-card-section class="bg-purple-1">
          <div class="text-h6 text-purple-10 flex items-center">
            <q-icon name="cleaning_services" class="q-mr-sm" />
            {{ selectedAmaId ? 'Modificar Ama de Llaves' : 'Nueva Ama de Llaves' }}
          </div>
        </q-card-section>
        
        <q-card-section class="q-gutter-md">
          <q-input
            v-model="amaForm.NombreApellidos"
            filled
            label="Nombre y Apellidos"
            color="purple-7"
            :rules="validationRules.nombre"
          >
            <template v-slot:prepend>
              <q-icon name="person" color="purple-7" />
            </template>
          </q-input>
          
          <q-input
            v-model="amaForm.CI"
            filled
            label="Cédula de Identidad (11 dígitos)"
            color="purple-7"
            mask="###########"
            :rules="validationRules.ci"
          >
            <template v-slot:prepend>
              <q-icon name="badge" color="purple-7" />
            </template>
          </q-input>
          
          <q-input
            v-model="amaForm.NumeroTelefonico"
            filled
            label="Número Telefónico"
            color="purple-7"
            :rules="validationRules.telefono"
          >
            <template v-slot:prepend>
              <q-icon name="phone" color="purple-7" />
            </template>
          </q-input>
          
          <div class="row q-gutter-sm">
            <q-btn
              :label="selectedAmaId ? 'Actualizar Ama de Llaves' : 'Crear Ama de Llaves'"
              color="purple-10"
              icon="save"
              @click="selectedAmaId ? modificarAma() : crearAma()"
              :loading="loading"
              :disable="formDisabled"
              class="col"
            />
            <q-btn
              label="Limpiar"
              color="grey-6"
              icon="clear"
              @click="resetForm"
              outline
              class="col-auto"
            />
          </div>
        </q-card-section>
      </q-card>
      
      <!-- Asignación de Habitaciones -->
      <q-card class="shadow-2 q-mt-md">
        <q-card-section class="bg-teal-1">
          <div class="text-h6 text-teal-10 flex items-center">
            <q-icon name="assignment" class="q-mr-sm" />
            Asignar Habitaciones
          </div>
        </q-card-section>
        
        <q-card-section class="q-gutter-md">
          <q-select
            v-model="asignacionForm.AmaDeLlavesId"
            filled
            label="Seleccionar Ama de Llaves"
            color="teal-7"
            :options="amaOptions"
            option-label="label"
            option-value="value"
          />
          
          <q-input
            v-model="asignacionForm.HabitacionNumero"
            filled
            label="Número de Habitación (0XY)"
            color="teal-7"
            :rules="validationRules.habitacion"
          />
          
          <q-select
            v-model="asignacionForm.Turno"
            filled
            label="Turno"
            color="teal-7"
            :options="turnosOptions"
          />
          
          <div class="row q-gutter-sm">
            <q-btn
              label="Asignar Habitación"
              color="teal-10"
              icon="add_task"
              @click="asignarHabitacion"
              :disable="asignacionFormDisabled"
              class="col"
            />
            <q-btn
              label="Desasignar"
              color="red-10"
              icon="remove_circle"
              @click="desasignarHabitacion"
              :disable="!asignacionForm.AmaDeLlavesId || !asignacionForm.HabitacionNumero"
              class="col"
            />
          </div>
          
          <q-btn
            label="Ver Habitaciones Asignadas"
            color="indigo-10"
            icon="visibility"
            @click="verHabitacionesAma"
            :disable="!asignacionForm.AmaDeLlavesId"
            class="full-width"
          />
          
          <!-- Tabla de habitaciones asignadas -->
          <div v-if="habitacionesAma.length > 0" class="q-mt-md">
            <div class="text-subtitle2 q-mb-sm">Habitaciones Asignadas ({{ habitacionesAma.length }}):</div>
            <q-table
              :rows="habitacionesAma"
              :columns="habitacionesAmaColumns"
              row-key="HabitacionNumero"
              :pagination="{ rowsPerPage: 5 }"
              class="shadow-1"
            />
          </div>
        </q-card-section>
      </q-card>
    </div>
    
    <!-- Lista de Amas de Llaves -->
    <div class="col-12 col-lg-6">
      <q-card class="shadow-2">
        <q-card-section class="bg-indigo-1">
          <div class="text-h6 text-indigo-10 flex items-center justify-between">
            <div class="flex items-center">
              <q-icon name="people" class="q-mr-sm" />
              Amas de Llaves
            </div>
            <q-chip color="indigo-10" text-color="white" icon="info">
              {{ filteredAmas.length }} amas
            </q-chip>
          </div>
        </q-card-section>
        
        <q-card-section>
          <!-- Filtros para Amas de Llaves -->
          <div class="row q-col-gutter-md q-mb-md">
            <div class="col-12 col-sm-6">
              <q-input
                v-model="filtros.busqueda"
                filled
                label="Buscar por nombre, CI o teléfono..."
                color="indigo-7"
                debounce="500"
              >
                <template v-slot:prepend>
                  <q-icon name="search" />
                </template>
                <template v-slot:append>
                  <q-icon 
                    name="clear" 
                    @click="filtros.busqueda = ''" 
                    class="cursor-pointer"
                    v-if="filtros.busqueda"
                  />
                </template>
              </q-input>
            </div>
            <div class="col-12 col-sm-6">
              <q-input
                v-model="filtros.ciFiltro"
                filled
                label="Filtro específico por CI"
                color="indigo-7"
                debounce="500"
              >
                <template v-slot:prepend>
                  <q-icon name="badge" />
                </template>
              </q-input>
            </div>
            <div class="col-12">
              <q-btn
                label="Actualizar Lista de Amas"
                color="indigo-10"
                icon="refresh"
                @click="fetchAmas"
                :loading="loading"
              />
            </div>
          </div>
          
          <q-table
            :rows="filteredAmas"
            :columns="TABLE_COLUMNS.amas"
            row-key="Id"
            :loading="loading"
            :pagination="{ rowsPerPage: 5 }"
            class="shadow-1"
          >
            <template v-slot:body-cell-actions="props">
              <q-td :props="props">
                <q-btn
                  flat
                  round
                  color="purple"
                  icon="edit"
                  @click="selectAma(props.row.Id)"
                  size="sm"
                >
                  <q-tooltip>Editar Ama de Llaves</q-tooltip>
                </q-btn>
                <q-btn
                  flat
                  round
                  color="teal"
                  icon="assignment"
                  @click="verHabitacionesAmaDirecto(props.row.Id)"
                  size="sm"
                  class="q-ml-xs"
                >
                  <q-tooltip>Ver habitaciones asignadas</q-tooltip>
                </q-btn>
                <q-btn
                  flat
                  round
                  color="red"
                  icon="delete"
                  @click="deleteAma(props.row.Id)"
                  size="sm"
                  class="q-ml-xs"
                >
                  <q-tooltip>Eliminar Ama de Llaves</q-tooltip>
                </q-btn>
              </q-td>
            </template>
          </q-table>
        </q-card-section>
      </q-card>
    </div>
  </div>
</template>

<script setup>
import { useAmas } from 'src/composables/useAmas'
import { validationRules } from 'src/utils/validators'
import { TABLE_COLUMNS } from 'src/utils/constants'
import { onMounted } from 'vue'

const {
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
  resetForm
} = useAmas()

onMounted(() => {
  fetchAmas()
})
</script>
