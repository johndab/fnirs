<template>
  <div class="mt-1">
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

export default {
  data: () => ({
    pending: false,
    localCheckbox: false,
  }),
  computed: {
    ...mapGetters(['isStreaming']),
  },
  created() {
    this.$ipc.on('IsStreaming', this.onIsStreaming);
    this.$ipc.send("IsStreaming"); 
  },
  destroyed() {
    this.$ipc.removeListener('IsStreaming', this.onIsStreaming);
  },
  methods: {
    update() {
      this.pending = true;
      if(!this.isStreaming) {
        this.$ipc.send("StartDataStream");
      } else {
        this.$ipc.send("StopDataStream");
      }
    },
    onIsStreaming(arg) {
      this.localCheckbox = !this.localCheckbox;
      this.$nextTick(() => {
        this.localCheckbox = arg;
      });

      this.$store.commit('setStreaming', arg);
      this.pending = false;
    }
  },
}
</script>

<style>

</style>