<template>
  <div class="board d-flex flex-column">
    <div class="d-flex justify-content-between">
      <div>
        <button 
          class="btn btn-secondary btn-sm mx-1"
          style="width: 120px"
          @click="cancel"
        >
          Close
        </button>
        <button 
          class="btn btn-success btn-sm mx-1"
          style="width: 120px"
          :disabled="pending"
          @click="save"
        >
          Save
        </button>

        <BSpinner
          v-if="pending"
          small
        />
      </div>

      <form @submit.prevent="add">
        <div class="d-flex">
          <div>
            <BFormInput
              v-model="address" 
              size="sm"
              placeholder="Address.."
            />
          </div>
          <div class="pl-1">
            <button 
              class="btn btn-success btn-sm"
              style="width: 100px"
              type="submit"
            >
              Add
            </button>
          </div>
        </div>
      </form>
    </div>
    <Layout 
      class="flex-grow-1"
      :layout.sync="layout"
      :editable="true"
    />
  </div>
</template>

<script>
import Layout from './Layout';
// tslint:disable-next-line
const { ipcRenderer } = require("electron");

export default {
  components: {
    Layout,
  },
  data: () => ({
    layout: [],
    address: '',
    pending: false,
  }),
  mounted() {
    this.register();
    ipcRenderer.send('getConfig');
  },
  methods: {
    register() {
      ipcRenderer.on('getConfig', (event, arg) => {
        this.pending = false;
        this.layout = arg ? JSON.parse(arg) : [];
        this.$store.commit('setLayout', this.layout);
      });
    },
    cancel() {
      this.$router.push('/');
    },
    add() {
      this.layout.push({
        i: this.layout.length,
        x: 10,
        y: 10,
        w: this.size,
        h: this.size,
        address: this.address,
      });
      this.address = '';
    },
    save() {
      this.pending = true;
      ipcRenderer.send('saveConfig', JSON.stringify(this.layout));
    },
  },
}
</script>

<style scoped>
  .board {
    padding-top: 10px;
    padding-left: 10px;
    padding-right: 10px;
    height: 100%;
  }
</style>