<template>
  <div class="row q-col-gutter-lg">
    
    <!-- Formulario de Habitación -->
    <div class="col-12 col-lg-5">
      <q-card class="shadow-2">
        <q-card-section class="bg-blue-1">
          <div class="text-h6 text-blue-10 flex items-center">
            <q-icon name="hotel" class="q-mr-sm" />
            {{ selectedHabitacionId ? 'Modificar Habitación' : 'Nueva Habitación' }}
          </div>
        </q-card-section>
        
        <q-card-section class="q-gutter-md">
          <q-input
            v-model="habitacionForm.Numero"
            filled
            label="Número de Habitación (0XY)"
            color="blue-7"
            :rules="validationRules.habitacion"
            hint="Formato: 0XY donde X es el piso (1-3) y Y es la habitación (1-5)"
            :disable="loading"
          >
            <template v-slot:prepend>
              <q-icon name="hotel" color="blue-7" />
            </template>
          </q-input>
          
          <q-toggle
            v-model="habitacionForm.EstaFueraDeServicio"
            label="Fuera de Servicio"
            color="blue-7"
            icon="build"
            :disable="loading"
          />
          
          <div class="row q-gutter-sm">
            <q-btn
              :label="selectedHabitacionId ? 'Actualizar Habitación' : 'Crear Habitación'"
              color="blue-10"
              icon="save"
              @click="selectedHabitacionId ? modificarHabitacion() : crearHabitacion()"
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
              :disable="loading"
            />
          </div>
        </q-card-section>
      </q-card>
      
      <!-- Búsqueda de Habitaciones Disponibles -->
      <q-card class="shadow-2 q-mt-md">
        <q-card-section class="bg-teal-1">
          <div class="text-h6 text-teal-10 flex items-center">
            <q-icon name="search" class="q-mr-sm" />
            Habitaciones Disponibles por Fecha
          </div>
        </q-card-section>
        
        <q-card-section class="q-gutter-md">
          <div class="row q-col-gutter-md">
            <div class="col-6">
              <q-input
                v-model="filtros.fechaInicio"
                filled
                label="Fecha Inicio"
                type="date"
                color="teal-7"
                :disable="loading"
              />
            </div>
            <div class="col-6">
              <q-input
                v-model="filtros.fechaFin"
                filled
                label="Fecha Fin"
                type="date"
                color="teal-7"
                :disable="loading"
              />
            </div>
          </div>
          
          <q-btn
            label="Buscar Habitaciones Disponibles"
            color="teal-10"
            icon="search"
            @click="buscarHabitacionesDisponibles(filtros.fechaInicio, filtros.fechaFin)"
            :loading="loading"
            class="full-width"
          />
          
          <div v-if="habitacionesDisponibles.length > 0" class="q-mt-md">
            <div class="text-subtitle2 q-mb-sm">Habitaciones Disponibles ({{ habitacionesDisponibles.length }}):</div>
            <q-list bordered separator>
              <q-item v-for="hab in habitacionesDisponibles" :key="hab.Numero">
                <q-item-section>
                  <q-item-label>Habitación {{ hab.Numero }}</q-item-label>
                  <q-item-label caption>{{ hab.EstaFueraDeServicio ? 'Fuera de Servicio' : 'Disponible' }}</q-item-label>
                </q-item-section>
              </q-item>
            </q-list>
          </div>
          <div v-else-if="filtros.fechaInicio && filtros.fechaFin && !loading" class="q-mt-md text-grey-6">
            No hay habitaciones disponibles para las fechas seleccionadas
          </div>
        </q-card-section>
      </q-card>
    </div>
    
    <!-- Lista de Habitaciones -->
    <div class="col-12 col-lg-7">
      <q-card class="shadow-2">
        <q-card-section class="bg-orange-1">
          <div class="text-h6 text-orange-10 flex items-center justify-between">
            <div class="flex items-center">
              <q-icon name="hotel" class="q-mr-sm" />
              Lista de Habitaciones
            </div>
            <q-chip color="orange-10" text-color="white" icon="info">
              {{ filteredHabitaciones.length }} habitaciones
            </q-chip>
          </div>
        </q-card-section>
        
        <q-card-section>
          <!-- Filtros -->
          <div class="row q-col-gutter-md q-mb-md">
            <div class="col-12 col-sm-4">
              <q-input
                v-model="filtros.busqueda"
                filled
                label="Buscar por número..."
                color="orange-7"
                debounce="500"
                :disable="loading"
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
            <div class="col-12 col-sm-4">
              <q-select
                v-model="filtros.estado"
                filled
                label="Filtrar por estado"
                color="orange-7"
                :options="estadoOptions"
                option-label="label"
                option-value="value"
                emit-value
                map-options
                clearable
                :disable="loading"
              />
            </div>
            <div class="col-12 col-sm-4">
              <q-btn
                label="Actualizar Habitaciones"
                color="orange-10"
                icon="refresh"
                @click="fetchHabitaciones"
                :loading="loading"
                class="full-width"
              />
            </div>
          </div>
          
          <!-- Grid de habitaciones -->
          <div class="row q-col-gutter-md">
            <div 
              v-for="habitacion in filteredHabitaciones" 
              :key="habitacion.Id"
              class="col-12 col-sm-6 col-md-4 col-lg-3"
            >
              <q-card 
                :class="getHabitacionClass(habitacion.EstaFueraDeServicio)"
                class="shadow-3 cursor-pointer habitacion-card"
                @click="selectHabitacion(habitacion.Id)"
              >
                <q-card-section class="text-center">
                  <q-icon 
                    :name="getHabitacionIcon(habitacion.EstaFueraDeServicio)" 
                    size="3rem" 
                    class="q-mb-sm"
                  />
                  <div class="text-h5 text-weight-bold">{{ habitacion.Numero }}</div>
                  <div class="text-caption">Piso {{ habitacion.Numero.charAt(1) }}</div>
                  <q-chip 
                    :color="getEstadoColor(habitacion.EstaFueraDeServicio)"
                    text-color="white"
                    size="sm"
                    class="q-mt-sm"
                  >
                    {{ habitacion.EstaFueraDeServicio ? 'Fuera de Servicio' : 'Disponible' }}
                  </q-chip>
                </q-card-section>
                
                <q-card-actions align="center">
                  <q-btn
                    flat
                    :color="habitacion.EstaFueraDeServicio ? 'green' : 'red'"
                    :icon="habitacion.EstaFueraDeServicio ? 'build_circle' : 'cancel'"
                    @click.stop="toggleServicio(habitacion)"
                    size="sm"
                    :disable="loading"
                  >
                    <q-tooltip>
                      {{ habitacion.EstaFueraDeServicio ? 'Poner en servicio' : 'Poner fuera de servicio' }}
                    </q-tooltip>
                  </q-btn>
                  <q-btn
                    flat
                    color="red"
                    icon="delete"
                    @click.stop="deleteHabitacion(habitacion.Id)"
                    size="sm"
                    :disable="loading"
                  >
                    <q-tooltip>Eliminar Habitación</q-tooltip>
                  </q-btn>
                </q-card-actions>
              </q-card>
            </div>
          </div>
          <div v-if="!filteredHabitaciones.length && !loading" class="text-center q-pa-lg text-grey-6">
            <q-icon name="hotel" size="3rem" />
            <div class="q-mt-md">No hay habitaciones</div>
          </div>
        </q-card-section>
      </q-card>
    </div>
  </div>
</template>

<script setup>
import { useHabitaciones } from 'src/composables/useHabitaciones'
import { validationRules } from 'src/utils/validators'
import { onMounted } from 'vue'

const {
  habitaciones,
  habitacionesDisponibles,
  loading,
  selectedHabitacionId,
  habitacionForm,
  filtros,
  filteredHabitaciones,
  estadoOptions,
  formDisabled,
  fetchHabitaciones,
  buscarHabitacionesDisponibles,
  crearHabitacion,
  modificarHabitacion,
  deleteHabitacion,
  selectHabitacion,
  toggleServicio,
  resetForm,
  getHabitacionClass,
  getHabitacionIcon,
  getEstadoColor
} = useHabitaciones()

onMounted(() => {
  fetchHabitaciones()
})
</script>

<style scoped>
.habitacion-card:hover {
  transform: translateY(-2px);
  transition: transform 0.2s ease;
}

.habitacion-card {
  transition: transform 0.2s ease;
}
</style>