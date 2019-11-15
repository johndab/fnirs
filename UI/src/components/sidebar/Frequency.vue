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
        this.requestFrequencies();
      }
    }
  },
  created() {
    this.register();
    if(this.isConnected) {
      this.requestFrequencies();
    }
  },
  destroyed() {
    this.$ipc.removeListener("getFrequencies", this.getFrequencies);
    this.$ipc.removeListener("getFrequency", this.getFrequency);
  },
  methods: {
    requestFrequencies() {
      this.$ipc.send("getFrequencies");
      this.$ipc.send("getFrequency");
    },
    update() {
      this.pending = true;
      this.$ipc.send('setFrequency', this.frequency);
    },
    register() {
      this.$ipc.on('getFrequencies', this.getFrequencies);
      this.$ipc.on('getFrequency', this.getFrequency);
    },
    getFrequencies(event, arg) {
      console.debug(arg);
      this.frequencies = arg || [];
    },
    getFrequency(event, arg) {
      this.pending = false;
      this.frequency = arg;
    },
    setFrequency(f) {
      this.$ipc.send('setFrequency', f);
    }
  },
}
</script>

<style>

</style>