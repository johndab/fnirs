<template>
  <div>
    Data Stream
    <div>
      <BFormCheckbox
        ref="checkbox"
        :checked="localCheckbox"
        switch
        size="lg"
        :disabled="pending"
        @change="update"
      >
        {{ isStreaming ? 'Started' : 'Stopped' }}
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
    ...mapGetters(['isStreaming']),
  },
  created() {
    // this.register();
    // ipcRenderer.send("isStreaming");
  },
  methods: {
    update() {
      // this.pending = true;
      // if(!this.isStreaming) {
      //   ipcRenderer.send("startStream");
      // } else {
      //   ipcRenderer.send("stopStream");
      // }
    },
    register() {
      ipcRenderer.on('isStreaming', (event, arg) => {
        this.localCheckbox = !this.localCheckbox;
        this.$nextTick(() => {
          this.localCheckbox = arg;
        });

        this.$store.commit('setStreaming', arg);
        this.pending = false;
      });
    },
  },
}
</script>

<style>

</style>