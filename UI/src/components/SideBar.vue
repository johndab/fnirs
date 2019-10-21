<template>
  <div class="sidebar">
    Sidebar

    <!-- <BFormSelect 
      v-model="frequency"
      :options="frequencies"
    /> -->

    {{ frequencies }}
  </div>
</template>

<script>
// tslint:disable-next-line
const { ipcRenderer } = require("electron");


export default {
  data: () => ({
    frequencies: null,
  }),
  created() {
    this.getFrequencies();
  },
  methods: {
    getFrequencies() {
      this.frequencies = ipcRenderer.sendSync("getFrequencies");
    },
  },
}
</script>

<style scoped>
  .sidebar {
    background-color: rgba(100, 100, 100, 0.1);
    height: 100%;
    width: 100%;
  }
</style>