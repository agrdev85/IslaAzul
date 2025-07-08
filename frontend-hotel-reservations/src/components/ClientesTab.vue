<template>
  <div class="row q-col-gutter-lg">
    
    <!-- Formulario de Cliente -->
    <div class="col-12 col-lg-5">
      <q-card class="shadow-2">
        <q-card-section class="bg-red-1">
          <div class="text-h6 text-red-10 flex items-center">
            <q-icon name="person_add" class="q-mr-sm" />
            {{ selectedClienteId ? 'Modificar Cliente' : 'Nuevo Cliente' }}
          </div>
        </q-card-section>
        
        <q-card-section class="q-gutter-md">
          <q-input
            v-model="clienteForm.NombreApellidos"
            filled
            label="Nombre y Apellidos"
            color="red-7"
            :rules="validationRules.nombre"
          >
            <template v-slot:prepend>
              <q-icon name="person" color="red-7" />
            </template>
          </q-input>
          
          <q-input
            v-model="clienteForm.CI"
            filled
            label="Cédula de Identidad (11 dígitos)"
            color="red-7"
            mask="###########"
            :rules="validationRules.ci"
          >
            <template v-slot:prepend>
              <q-icon name="badge" color="red-7" />
            </template>
            <template v-slot:append>
              <q-icon 
                name="clear" 
                v-if="clienteForm.CI" 
                @click="clienteForm.CI = ''" 
                class="cursor-pointer" 
                color="red-7"
              />
            </template>
          </q-input>
          
          <q-input
            v-model="clienteForm.NumeroTelefonico"
            filled
            label="Número Telefónico"
            color="red-7"
            type="tel"
            :rules="validationRules.telefono"
          >
            <template v-slot:prepend>
              <q-icon name="phone" color="red-7" />
            </template>
          </q-input>
          
          <q-toggle
            v-model="clienteForm.EsVip"
            label="Cliente VIP (10% descuento en reservas)"
            color="red-7"
            icon="star"
          />
          
          <div class="row q-gutter-sm">
            <q-btn
              :label="selectedClienteId ? 'Actualizar Cliente' : 'Crear Cliente'"
              color="red-10"
              icon="save"
              @click="selectedClienteId ? modificarCliente() : crearCliente()"
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
    </div>
    
    <!-- Lista de Clientes -->
    <div class="col-12 col-lg-7">
      <q-card class="shadow-2">
        <q-card-section class="bg-orange-1">
          <div class="text-h6 text-orange-10 flex items-center justify-between">
            <div class="flex items-center">
              <q-icon name="people" class="q-mr-sm" />
              Lista de Clientes
            </div>
            <q-chip color="orange-10" text-color="white" icon="info">
              {{ clientes.length }} clientes
            </q-chip>
          </div>
        </q-card-section>
        
        <q-card-section>
          <!-- Filtros -->
          <div class="row q-col-gutter-md q-mb-md">
            <div class="col-12 col-sm-6">
              <q-input
                v-model="filters.busqueda"
                filled
                label="Buscar por nombre, CI o teléfono..."
                color="orange-7"
                debounce="500"
                @update:model-value="fetchClientes"
              >
                <template v-slot:prepend>
                  <q-icon name="search" />
                </template>
                <template v-slot:append>
                  <q-icon 
                    name="clear" 
                    @click="filters.busqueda = ''; fetchClientes()" 
                    class="cursor-pointer"
                    v-if="filters.busqueda"
                  />
                </template>
              </q-input>
            </div>
            <div class="col-12 col-sm-6">
              <q-input
                v-model="filters.ciFiltro"
                filled
                label="Filtro específico por CI"
                color="orange-7"
                debounce="500"
                @update:model-value="fetchClientes"
              >
                <template v-slot:prepend>
                  <q-icon name="badge" />
                </template>
              </q-input>
            </div>
            <div class="col-12">
              <q-btn
                label="Actualizar Lista de Clientes"
                color="orange-10"
                icon="refresh"
                @click="fetchClientes"
                :loading="loading"
              />
            </div>
          </div>
          
          <!-- Tabla de clientes -->
          <q-table
            :rows="clientes"
            :columns="TABLE_COLUMNS.clientes"
            row-key="Id"
            :loading="loading"
            :pagination="pagination"
            @request="fetchClientes"
            class="shadow-1"
          >
            <template v-slot:body-cell-EsVip="props">
              <q-td :props="props">
                <q-chip 
                  :color="props.value ? 'amber' : 'grey-5'" 
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
                  color="red"
                  icon="edit"
                  @click="selectCliente(props.row.Id)"
                  size="sm"
                >
                  <q-tooltip>Editar Cliente</q-tooltip>
                </q-btn>
                <q-btn
                  flat
                  round
                  color="red"
                  icon="delete"
                  @click="deleteCliente(props.row.Id)"
                  size="sm"
                  class="q-ml-xs"
                >
                  <q-tooltip>Eliminar Cliente</q-tooltip>
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
import { useClientes } from 'src/composables/useClientes'
import { validationRules } from 'src/utils/validators'
import { TABLE_COLUMNS } from 'src/utils/constants'
import { onMounted } from 'vue'

const {
  clientes,
  loading,
  selectedClienteId,
  pagination,
  clienteForm,
  filters,
  formDisabled,
  fetchClientes,
  crearCliente,
  modificarCliente,
  deleteCliente,
  selectCliente,
  resetForm
} = useClientes()

onMounted(() => {
  fetchClientes()
})
</script>
