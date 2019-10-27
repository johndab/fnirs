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
// tslint:disable-next-line
const { ipcRenderer } = require("electron");

export default {
  data: () => ({
    pending: false,
    localCheckbox: false,
  }),
  computed: {
    ...mapGetters(['isConnected']),
  },
  created() {
    this.register();
    ipcRenderer.send("isConnected");
  },
  methods: {
    update() {
      this.pending = true;
      if(!this.isConnected) {
        ipcRenderer.send("hardwareConnect");
      } else {
        ipcRenderer.send("hardwareDisconnect");
      }
    },
    register() {
      ipcRenderer.on('isConnected', (event, arg) => {
        this.localCheckbox = !this.localCheckbox;
        this.$nextTick(() => {
          this.localCheckbox = arg;
        });

        this.$store.commit('setConnected', arg);
        this.pending = false;
      });
    },
  },
}
</script>

<style>

</style>