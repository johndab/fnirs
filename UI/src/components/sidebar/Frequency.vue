<template>
  <div class="mt-1">
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
    this.$ipc.removeListener("GetFrequencies", this.getFrequencies);
    this.$ipc.removeListener("GetFrequency", this.getFrequency);
  },
  methods: {
    requestFrequencies() {
      this.$ipc.send("GetFrequencies");
      this.$ipc.send("GetFrequency");
    },
    update() {
      this.pending = true;
      this.$ipc.send('SetFrequency', this.frequency);
    },
    register() {
      this.$ipc.on('GetFrequencies', this.getFrequencies);
      this.$ipc.on('GetFrequency', this.getFrequency);
    },
    getFrequencies(arg) {
      this.frequencies = arg || [];
    },
    getFrequency(arg) {
      this.pending = false;
      this.frequency = arg;
    },
    setFrequency(f) {
      this.$ipc.send('SetFrequency', f);
    }
  },
}
</script>

<style>

</style>