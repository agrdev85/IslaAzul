import { boot } from 'quasar/wrappers'
import { Quasar, Dialog, Notify } from 'quasar'
import '@quasar/extras/material-icons/material-icons.css'
import 'quasar/src/css/index.sass'

export default boot(({ app }) => {
  app.use(Quasar, {
    plugins: {
      Dialog, 
      Notify 
    },
    config: {
      notify: {
        position: 'top',
        timeout: 3000
      }
    }
  })
})