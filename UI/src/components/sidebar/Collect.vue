<template>
  <div class="mt-1">
    Collect
    <div>
      <BFormCheckbox
        ref="checkbox"
        :checked="localCheckbox"
        switch
        size="lg"
        :disabled="pending || !isStreaming"
        @change="update"
      >
        {{ isCollecting ? 'Collecting' : 'Stopped' }}
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
    ...mapGetters(['isStreaming', 'isCollecting']),
  },
  created() {
    this.$ipc.on('IsCollecting', this.onIsCollecting);
    this.$ipc.send("IsCollecting"); 
  },
  destroyed() {
    this.$ipc.removeListener('IsCollecting', this.onIsCollecting);
  },
  methods: {
    update() {
      this.pending = true;
      if(!this.isCollecting) {
        this.$ipc.send("CollectStart");
      } else {
        this.$ipc.send("CollectStop");
      }
    },
    onIsCollecting(arg) {
      this.localCheckbox = !this.localCheckbox;
      this.$nextTick(() => {
        this.localCheckbox = arg;
      });

      this.$store.commit('setCollecting', arg);
      this.pending = false;
    }
  },
}
</script>

<style>

</style>