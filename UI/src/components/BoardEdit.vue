<template>
  <div class="board d-flex flex-column">
    <div class="d-flex justify-content-between">
      <div class="d-flex">
        <div style="width: 220px">
          <button 
            class="btn btn-secondary btn-sm"
            style="width: 100px; margin: 0 5px"
            @click="cancel"
          >
            Close
          </button>
          <button 
            class="btn btn-success btn-sm"
            style="width: 100px; margin: 0 5px"
            :disabled="layoutPending"
            @click="save"
          >
            Save
          </button>

          <BSpinner
            v-if="layoutPending"
            small
          />
        </div>
        <NeighbourSelect 
          style="width: 100px; margin-left: 5px;"
          :value.sync="depth"
        />
      </div>

      <form @submit.prevent="submit">
        <div class="d-flex">
          <div class="flex-grow-1">
            <BFormInput
              v-model="address" 
              size="sm"
              placeholder="Address.."
            />
          </div>
          <div class="pl-1">
            <button 
              class="btn btn-success btn-sm"
              style="width: 100px"
              type="submit"
            >
              <span v-if="edited">
                Save
              </span>
              <span v-else>
                Add
              </span>
            </button>
          </div>
        </div>
        <div class="mt-1 d-flex justify-content-between">
          <BButtonGroup class="mr-2">
            <BButton 
              class="btn btn-sm"
              :variant="type === 'source' ? 'secondary' : 'outline-secondary'"
              type="button"
              @click="type = 'source'"
            >
              <i 
                class="ion ion-md-square-outline pr-1" 
                style="font-size: 16px; position: relative; top: 1px"
              />
              Source
            </BButton>
            <BButton 
              class="btn btn-sm"
              :variant="type === 'detector' ? 'secondary' : 'outline-secondary'"
              type="button"
              @click="type = 'detector'"
            >
              <i 
                class="ion ion-md-radio-button-off pr-1" 
                style="font-size: 16px; position: relative; top: 1px"
              />
              Detector
            </BButton>
          </BButtonGroup>
          <div 
            v-if="edited"
            class="mr-1"
          >
            <button 
              class="btn btn-danger btn-sm"
              type="type"
              @click="remove"
            >
              Remove
            </button>
          </div>
          <div v-if="edited">
            <button 
              class="btn btn-secondary btn-sm"
              type="type"
              @click="edited = null; address = ''"
            >
              Cancel
            </button>
          </div>
        </div>
      </form>
    </div>
    <Layout 
      class="flex-grow-1"
      :layout.sync="layout"
      :editable="true"
      :edited="edited ? edited.i : null"
      :depth="depth"
      @edit="edit"
      @setGraph="setGraph"
    />
  </div>
</template>

<script>
import Layout from './Layout';
import NeighbourSelect from './graph/NeighbourSelect'
import { mapGetters } from 'vuex';

export default {
  components: {
    NeighbourSelect,
    Layout,
  },
  data: () => ({
    layout: [],
    address: '',
    type: 'source',
    pending: false,
    edited: null,
    depth: 1,
  }),
  computed: {
    ...mapGetters(['layoutPending']),
  },
  watch: {
    layoutPending(v) {
      if(!v) {
        this.$router.push('/');
      }
    }
  },
  created() {
    this.layout = this.$store.getters.layout.slice();
  },
  methods: {
    cancel() {
      this.$router.push('/');
    },
    setGraph(graph) {
      const model = graph.reduce((acc, x) => {
        const detector = x.address.toUpperCase().charCodeAt(0) - 'A'.charCodeAt(0) + 1;
        const key = parseInt(detector, 10);
        if(isNaN(key)) return acc;
        acc[key] = (x.nearest || []).map(x => parseInt(x.address, 10));
        return acc;
      }, {});

      this.$ipc.send('SetGraph', graph);
    },
    edit(item) {
      this.edited = item;
      this.address = this.edited.address;
      this.type = this.edited.type;
    },
    remove() {
      this.layout = this.layout.filter(l => l.i !== this.edited.i);
      this.address = '';
      this.type = 'source';
      this.edited = null;
    },
    submit() {
      if(!this.edited) {
        const i = this.layout.length + 1;
        this.layout.push({
          i: i,
          x: -1,
          y: -1,
          type: this.type,
          address: this.address,
        });
        this.address = '';
      } else {
        const node = this.layout.find(l => l.i === this.edited.i)
        this.layout = this.layout.filter(l => l.i !== this.edited.i);
        this.layout.push({
          i: node.i,
          x: node.x,
          y: node.y,
          address: this.address,
          type: this.type,
        });
        this.edited = null;
        this.address = '';
        this.type = 'source';
      }
    },
    save() {
      this.$store.commit('setLayoutPending', true);
      this.$ipc.send('SaveLayout', JSON.stringify(this.layout));
    },
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