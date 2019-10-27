<template>
  <div class="board d-flex flex-column">
    <div class="d-flex justify-content-between">
      <div>
        <button 
          class="btn btn-secondary btn-sm"
          style="width: 120px"
          @click="edit"
        >
          Edit layout
        </button>
      </div>

      <div />
    </div>

    <Layout 
      class="flex-grow-1"
      :layout="layout"
    />
  </div>
</template>

<script>
import { mapGetters } from 'vuex';
import Layout from './Layout';
// tslint:disable-next-line
const { ipcRenderer } = require("electron");

export default {
  components: {
    Layout,
  },
  data: () => ({

  }),
  computed: {
    ...mapGetters(['layout']),
  },
  created() {
    this.register();
    ipcRenderer.send('getConfig');
  },
  methods: {
    register() {
      ipcRenderer.on('getConfig', (event, arg) => {
        this.pending = false;
        const layout = arg ? JSON.parse(arg) : '';
        this.$store.commit('setLayout', layout);
      });
    },
    edit() {
      this.$router.push('/edit');
    },
  },
}
</script>

<style scoped>
  .board {
    padding-top: 10px;
    padding-left: 10px;
    padding-right: 10px;
  }
</style>