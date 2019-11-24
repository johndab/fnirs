<template>
  <div class="mt-1">
    Switch
    <span
      v-if="pending"
      class="pl-2"
    >
      <BSpinner small />
    </span>

    <BFormSelect 
      v-model="swMode"
      :options="options"
      @change="update"
    />
  </div>
</template>

<script>
import { mapGetters } from 'vuex';

export default {
  data: () => ({
    pending: false,
    swMode: 8,
  }),
  computed: {
    options() {
      return [
        {
          value: 8,
          text: 'Switch-8',
        },
        {
          value: 16,
          text: 'Switch-16',
        },
        {
          value: 32,
          text: 'Switch-32',
        }
      ]
    },
  },
  created() {
    this.$ipc.on('SetSwitch', this.setSwitch);
  },
  methods: {
    update() {
      this.pending = true;
      var val = parseInt(this.swMode, 10);
      this.$ipc.send('SetSwitch', val);
    },
    setSwitch(arg) {
      this.pending = false;
      this.$store.commit('setSwitch', arg);
    },
  },
}
</script>

<style>

</style>