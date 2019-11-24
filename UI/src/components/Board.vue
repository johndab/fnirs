<template>
  <div class="board d-flex flex-column">
    <div class="d-flex justify-content-between">
      <div>
        <button 
          class="btn btn-secondary btn-sm"
          style="width: 120px"
          @click="edit"
        >
          Edit layout
        </button>
      </div>

      <div />
    </div>

    <Layout 
      ref="layout"
      class="flex-grow-1"
      :layout="layout"
    />
  </div>
</template>

<script>
import { mapGetters } from 'vuex';
import Layout from './Layout';

export default {
  components: {
    Layout,
  },
  computed: {
    ...mapGetters(['layout']),
  },
  created() {
    this.$ipc.on('NewDataPacket', this.onNewData);
  },
  destroyed() {
    this.$ipc.removeListener('NewDataPacket', this.onNewData);
  },
  methods: {
    edit() {
      this.$router.push('/edit');
    },
    onNewData(arg) {
      if(this.$refs.layout) {
        this.$refs.layout.setData(arg);
       }
    }
  },
}
</script>

<style scoped>
  .board {
    padding-top: 10px;
    padding-left: 10px;
    padding-right: 10px;
  }
</style>