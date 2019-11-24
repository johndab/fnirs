<template>
  <div class="full-height">
    <div class="d-flex full-height">
      <div class="flex-grow-1">
        <RouterView v-if="!configPending" />
        <div
          v-else
          class="mt-4"
        >
          <BSpinner />
        </div>
      </div>
      <div style="width: 250px">
        <SideBar />
      </div>
    </div>
  </div>
</template>

<script>
import SideBar from '@/components/SideBar.vue';
import { mapGetters } from 'vuex';

export default {
  components: {
    SideBar,
  },
  data: () => ({
    configPending: false,
  }),
  created() {
    this.configPending = true;
    this.$ipc.on('GetLayout', this.configReceived);
    this.$ipc.send('GetLayout');
  },
  destroyed() {
    this.$ipc.removeListener('getConfig', this.configReceived);
  },
  methods: {
    configReceived(arg) {
      this.configPending = false;
      const layout = arg ? JSON.parse(arg) : [];
      this.$store.commit('setLayout', layout);
    },
  },
};
</script>


<style scoped>

  .full-height {
    height: 100vh;
  }
</style>