<template>
  <div>
    Hardware Connection
    <div>
      <BFormCheckbox
        ref="checkbox"
        :checked="localCheckbox"
        switch
        size="lg"
        :disabled="pending"
        @change="update"
      >
        {{ isConnected ? 'Connected' : 'Disconnected' }}
        <BSpinner
          v-if="pending"
          small
        />
      </BFormCheckbox>
    </div>
  </div>
</template>

<script>
import { mapGetters } from 'vuex';

export default {
  data: () => ({
    pending: false,
    localCheckbox: false,
  }),
  computed: {
    ...mapGetters(['isConnected']),
  },
  created() {
    this.$ipc.on('isConnected', this.onIsConnected);
    this.$ipc.send("isConnected");
  },
  destroyed() {
    this.$ipc.removeListener('isConnected', this.onIsConnected);
  },
  methods: {
    update() {
      this.pending = true;
      if(!this.isConnected) {
        this.$ipc.send("hardwareConnect");
      } else {
        this.$ipc.send("hardwareDisconnect");
      }
    },
    onIsConnected(event, arg) {
      this.localCheckbox = !this.localCheckbox;
      this.$nextTick(() => {
        this.localCheckbox = arg;
      });

      this.$store.commit('setConnected', arg);
      this.pending = false;
    }
  },
}
</script>

<style>

</style>