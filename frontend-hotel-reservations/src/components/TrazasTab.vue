<template>
  <q-card class="shadow-2">
    <q-card-section class="bg-deep-purple-1">
      <div class="text-h6 text-deep-purple-10 flex items-center justify-between">
        <div class="flex items-center">
          <q-icon name="history" class="q-mr-sm" />
          Registro de Trazas del Sistema
        </div>
        <q-chip color="deep-purple-10" text-color="white" icon="info">
          {{ trazas.length }} registros
        </q-chip>
      </div>
    </q-card-section>
    
    <q-card-section>
      <!-- Filtros -->
      <div class="row q-col-gutter-md q-mb-md">
        <div class="col-12 col-sm-6">
          <q-input
            v-model="filtros.busqueda"
            filled
            label="Buscar en trazas (Operación, Tabla, Detalles)..."
            color="deep-purple-7"
            debounce="500"
            @update:model-value="fetchTrazas"
          >
            <template v-slot:prepend>
              <q-icon name="search" />
            </template>
            <template v-slot:append>
              <q-icon 
                name="clear" 
                @click="filtros.busqueda = ''; fetchTrazas()" 
                class="cursor-pointer"
                v-if="filtros.busqueda"
              />
            </template>
          </q-input>
        </div>
        <div class="col-12 col-sm-6">
          <q-btn
            label="Actualizar Registro de Trazas"
            color="deep-purple-10"
            icon="refresh"
            @click="fetchTrazas"
            :loading="loading"
            class="full-width"
          />
        </div>
      </div>
      
      <!-- Tabla de trazas -->
      <q-table
        :rows="trazas"
        :columns="TABLE_COLUMNS.trazas"
        row-key="Id"
        :loading="loading"
        :pagination="pagination"
        @request="fetchTrazas"
        class="shadow-1"
      >
        <template v-slot:body-cell-Operacion="props">
          <q-td :props="props">
            <q-chip 
              :color="getOperacionColor(props.value)" 
              text-color="white"
              size="sm"
            >
              {{ props.value }}
            </q-chip>
          </q-td>
        </template>
        
        <template v-slot:body-cell-TablaAfectada="props">
          <q-td :props="props">
            <q-chip 
              color="grey-6" 
              text-color="white"
              size="sm"
            >
              {{ props.value }}
            </q-chip>
          </q-td>
        </template>
        
        <template v-slot:body-cell-Fecha="props">
          <q-td :props="props">
            <div class="text-caption">{{ formatFecha(props.value) }}</div>
          </q-td>
        </template>
        
        <template v-slot:body-cell-Detalles="props">
          <q-td :props="props">
            <div class="text-caption" style="max-width: 200px; overflow: hidden; text-overflow: ellipsis;">
              {{ props.value }}
            </div>
            <q-tooltip v-if="props.value && props.value.length > 50">
              {{ props.value }}
            </q-tooltip>
          </q-td>
        </template>
      </q-table>
    </q-card-section>
  </q-card>
</template>

<script setup>
import { useTrazas } from 'src/composables/useTrazas'
import { TABLE_COLUMNS } from 'src/utils/constants'
import { onMounted } from 'vue'

const {
  trazas,
  loading,
  pagination,
  filtros,
  fetchTrazas,
  getOperacionColor,
  formatFecha
} = useTrazas()

onMounted(() => {
  fetchTrazas()
})
</script>
