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
        <div class="col-12 col-sm-4">
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
        <div class="col-12 col-sm-4">
          <q-select
            v-model="filtros.tablaAfectada"
            :options="tablaOptions"
            label="Filtrar por tabla"
            filled
            color="deep-purple-7"
            @update:model-value="fetchTrazas"
          >
            <template v-slot:prepend>
              <q-icon name="filter_list" />
            </template>
          </q-select>
        </div>
        <div class="col-12 col-sm-4">
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

      <!-- Mensaje cuando no hay trazas -->
      <div v-if="!loading && trazas.length === 0" class="q-pa-md text-center text-grey-7">
        No se encontraron trazas para los filtros seleccionados.
      </div>

      <!-- Tabla de trazas -->
      <q-table
        v-else
        :rows="trazas"
        :columns="TABLE_COLUMNS.trazas"
        row-key="Id"
        :loading="loading"
        :pagination="pagination"
        @request="onRequest"
        class="shadow-1"
        :no-data-label="'No hay trazas disponibles'"
      >
        <template v-slot:loading>
          <q-inner-loading showing>
            <q-spinner color="deep-purple-7" size="50px" />
          </q-inner-loading>
        </template>

        <template v-slot:body-cell-Operacion="props">
          <q-td :props="props">
            <q-chip
              :color="getOperacionColor(props.value)"
              text-color="white"
              size="sm"
            >
              {{ props.value || 'N/A' }}
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
              {{ props.value || 'N/A' }}
            </q-chip>
          </q-td>
        </template>

        <template v-slot:body-cell-RegistroId="props">
          <q-td :props="props">
            <div class="text-caption">{{ props.value || 'N/A' }}</div>
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
              {{ props.value || 'N/A' }}
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
import { onMounted } from 'vue'

const TABLE_COLUMNS = {
  trazas: [
    { name: 'Id', label: 'ID', field: 'Id', sortable: true, align: 'left' },
    { name: 'Operacion', label: 'Operación', field: 'Operacion', sortable: true, align: 'left' },
    { name: 'TablaAfectada', label: 'Tabla Afectada', field: 'TablaAfectada', sortable: true, align: 'left' },
    { name: 'RegistroId', label: 'ID Registro', field: 'RegistroId', sortable: true, align: 'left' },
    { name: 'Fecha', label: 'Fecha', field: 'Fecha', sortable: true, align: 'left' },
    { name: 'Detalles', label: 'Detalles', field: 'Detalles', sortable: true, align: 'left' }
  ]
}

const tablaOptions = [
  { label: 'Todas', value: '' },
  { label: 'Cliente', value: 'Cliente' },
  { label: 'Habitación', value: 'Habitacion' },
  { label: 'Reserva', value: 'Reserva' },
  { label: 'Ama', value: 'Ama' }
]

const {
  trazas,
  loading,
  pagination,
  filtros,
  fetchTrazas,
  getOperacionColor,
  formatFecha
} = useTrazas()

const onRequest = (props) => {
  pagination.value = {
    ...props.pagination,
    sortBy: props.pagination.sortBy || 'Id',
    descending: props.pagination.descending ?? true
  }
  fetchTrazas()
}

onMounted(() => {
  console.log('Componente TrazasTab montado')
  fetchTrazas()
})
</script>

<style scoped>
.q-table {
  min-height: 300px;
}
</style>