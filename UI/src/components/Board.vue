<template>
  <div class="board d-flex flex-column">
    <div class="d-flex justify-content-between">
      <div class="d-flex">
        <button 
          class="btn btn-secondary btn-sm btn-block"
          style="width: 120px"
          @click="edit"
        >
          Edit layout
        </button>
        <NeighbourSelect 
          style="width: 100px; margin-left: 5px;"
          :value.sync="depth"
        />
        <div class="pl-2">
          <BFormCheckbox
            ref="checkbox"
            v-model="showConnect"
            switch
            size="lg"
          >
            {{ showConnect ? 'Edges' : 'Nodes' }}
            <small 
              class="text-secondary"
              style="font-size: 12px"
            >
              / {{ !showConnect ? 'Edges' : 'Nodes' }}
            </small>
          </BFormCheckbox>
        </div>
        <div class="pl-3">
          <BFormCheckbox
            ref="checkbox"
            v-model="showFrequency"
            switch
            size="lg"
          >
            {{ showFrequency ? '690nm' : '830nm' }}
            <small 
              class="text-secondary"
              style="font-size: 12px"
            >
              / {{ !showFrequency ? '690nm' : '830nm' }}
            </small>
          </BFormCheckbox>
        </div>
      </div>

      <div />
    </div>

    <Layout 
      ref="layout"
      class="flex-grow-1"
      :layout="layout"
      :depth="depth"
      :use-connections="showConnect"
      @setGraph="setGraph"
    />
  </div>
</template>

<script>
import { mapGetters } from 'vuex';
import NeighbourSelect from './graph/NeighbourSelect';
import Layout from './Layout';

export default {
  components: {
    NeighbourSelect,
    Layout,
  },
  data: () => ({
    depth: 1,
    showConnect: false,
    showFrequency: false,
  }),
  computed: {
    ...mapGetters(['layout']),
  },
  watch: {
    showFrequency(v) {
      this.$ipc.send('SetSourceFrequency', v ? 0 : 1);
    }
  },
  created() {
    this.$ipc.on('NewDataPacket', this.onNewData);
    this.$ipc.send('SetSourceFrequency', this.showFrequency ? 0 : 1)
    this.$ipc.on('Debug', (y) => {
      console.debug(y);
    });
  },
  destroyed() {
    this.$ipc.removeListener('NewDataPacket', this.onNewData);
  },
  methods: {
    edit() {
      this.$router.push('/edit');
    },
    setGraph(graph) {
      const model = graph.reduce((acc, x) => {
        const detector = x.address.toUpperCase().charCodeAt(0) - 'A'.charCodeAt(0) + 1;
        const key = parseInt(detector, 10);
        if(isNaN(key)) return acc;
        acc[key] = (x.nearest || []).map(x => parseInt(x.address, 10));
        return acc;
      }, {});
      this.$ipc.send('SetGraph', model);
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
    height: 100%;
  }
</style>