<template>
  <div>
    Demodulation Freq.
    <span
      v-if="pending"
      class="pl-2"
    >
      <BSpinner small />
    </span>

    <BFormSelect 
      v-model="frequency"
      :options="options"
      @change="update"
    />
  </div>
</template>

<script>
import { mapGetters } from 'vuex';
// tslint:disable-next-line
const { ipcRenderer } = require("electron");

export default {
  data: () => ({
    frequencies: [],
    frequency: 1,
    pending: false,
  }),
  computed: {
    ...mapGetters(['isConnected']),
    options() {
      return this.frequencies.map((f) => ({
        value: f.index,
        text: f.value,
      }))
    },
  },
  watch: {
    isConnected(c) {
      if(c) {
        this.getFrequencies();
      }
    }
  },
  created() {
    this.register();
    if(this.isConnected) {
      this.getFrequencies();
    }
  },
  methods: {
    getFrequencies() {
      ipcRenderer.send("getFrequencies");
      ipcRenderer.send("getFrequency");
    },
    update() {
      this.pending = true;
      ipcRenderer.send('setFrequency', this.frequency);
    },
    register() {
      ipcRenderer.on('getFrequencies', (event, arg) => {
        this.frequencies = arg;
      });
      ipcRenderer.on('getFrequency', (event, arg) => {
        console.debug(arg);
        this.pending = false;
        this.frequency = arg;
      });
    },
    setFrequency(f) {
      ipcRenderer.send('setFrequency', f);
    }
  },
}
</script>

<style>

</style>