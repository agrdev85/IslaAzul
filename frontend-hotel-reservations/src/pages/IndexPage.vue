<template>
  <q-layout view="hHh lpR fFf">
    <!-- Encabezado mejorado -->
    <q-header elevated class="bg-red-9 text-white">
      <q-toolbar>
        <q-toolbar-title class="flex items-center">
          <q-icon name="hotel" size="2rem" class="q-mr-md" />
          <div>
            <div class="text-h5 text-weight-bold">Hostal Isla Azul</div>
            <div class="text-caption opacity-80">Sistema de Gestión Hotelera</div>
          </div>
        </q-toolbar-title>
        
        <q-space />
        
        <q-btn 
          flat 
          round 
          icon="notifications" 
          class="q-mr-sm"
        >
          <q-badge color="amber" floating>{{ stats.reservasActivas }}</q-badge>
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
        
        <!-- Spinner global -->
        <q-inner-loading :showing="globalLoading">
          <q-spinner size="50px" color="red-7" />
        </q-inner-loading>
        
        <!-- Dashboard Stats -->
        <div class="row q-col-gutter-md q-mb-lg">
          <div class="col-12 col-sm-6 col-md-3">
            <q-card class="bg-red-6 text-white shadow-3">
              <q-card-section class="text-center">
                <q-icon name="people" size="2rem" class="q-mb-sm" />
                <div class="text-h4 text-weight-bold">{{ stats.totalClientes }}</div>
                <div class="text-body2">Clientes Totales</div>
              </q-card-section>
            </q-card>
          </div>
          
          <div class="col-12 col-sm-6 col-md-3">
            <q-card class="bg-orange-6 text-white shadow-3">
              <q-card-section class="text-center">
                <q-icon name="event_available" size="2rem" class="q-mb-sm" />
                <div class="text-h4 text-weight-bold">{{ stats.reservasActivas }}</div>
                <div class="text-body2">Reservas Activas</div>
              </q-card-section>
            </q-card>
          </div>
          
          <div class="col-12 col-sm-6 col-md-3">
            <q-card class="bg-green-6 text-white shadow-3">
              <q-card-section class="text-center">
                <q-icon name="hotel" size="2rem" class="q-mb-sm" />
                <div class="text-h4 text-weight-bold">{{ stats.habitacionesDisponibles }}</div>
                <div class="text-body2">Habitaciones Libres</div>
              </q-card-section>
            </q-card>
          </div>
          
          <div class="col-12 col-sm-6 col-md-3">
            <q-card class="bg-blue-6 text-white shadow-3">
              <q-card-section class="text-center">
                <q-icon name="attach_money" size="2rem" class="q-mb-sm" />
                <div class="text-h4 text-weight-bold">${{ stats.ingresosMes }}</div>
                <div class="text-body2">Ingresos del Mes</div>
              </q-card-section>
            </q-card>
          </div>
        </div>

        <!-- Pestañas principales -->
        <q-card class="shadow-4">
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
            <q-tab name="trazas" icon="history" label="Trazas" />
          </q-tabs>
          
          <q-separator />

          <!-- Contenido de las pestañas -->
          <q-tab-panels v-model="tab" animated class="bg-grey-1">
            <q-tab-panel name="clientes" class="q-pa-md">
              <ClientesTab />
            </q-tab-panel>
            <q-tab-panel name="reservas" class="q-pa-md">
              <ReservasTab />
            </q-tab-panel>
            <q-tab-panel name="habitaciones" class="q-pa-md">
              <HabitacionesTab />
            </q-tab-panel>
            <q-tab-panel name="amas" class="q-pa-md">
              <AmasTab />
            </q-tab-panel>
            <q-tab-panel name="trazas" class="q-pa-md">
              <TrazasTab />
            </q-tab-panel>
          </q-tab-panels>
        </q-card>
      </q-page>
    </q-page-container>
  </q-layout>
</template>

<script setup>
import '@quasar/extras/material-icons/material-icons.css';
import { ref, onMounted } from 'vue'
import { useQuasar } from 'quasar'
import ClientesTab from 'src/components/ClientesTab.vue'
import ReservasTab from 'src/components/ReservasTab.vue'
import HabitacionesTab from 'src/components/HabitacionesTab.vue'
import AmasTab from 'src/components/AmasTab.vue'
import TrazasTab from 'src/components/TrazasTab.vue'
import { useStats } from 'src/composables/useStats'

const $q = useQuasar()
const tab = ref('clientes')
const globalLoading = ref(false)

const { stats, loadStats } = useStats()

onMounted(async () => {
  globalLoading.value = true
  try {
    await loadStats()
  } catch (error) {
    console.error('Error loading stats:', error)
  } finally {
    globalLoading.value = false
  }
})
</script>

<style scoped>
.q-card {
  border-radius: 12px;
}

.shadow-3 {
  box-shadow: 0 5px 5px -3px rgba(0, 0, 0, 0.2), 0 8px 10px 1px rgba(0, 0, 0, 0.14), 0 3px 14px 2px rgba(0, 0, 0, 0.12);
}

.shadow-4 {
  box-shadow: 0 7px 8px -4px rgba(0, 0, 0, 0.2), 0 12px 17px 2px rgba(0, 0, 0, 0.14), 0 5px 22px 4px rgba(0, 0, 0, 0.12);
}
</style>