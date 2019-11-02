<template>
  <div class="board d-flex flex-column">
    <div class="d-flex justify-content-between">
      <div>
        <button 
          class="btn btn-secondary btn-sm mx-1"
          style="width: 120px"
          @click="cancel"
        >
          Close
        </button>
        <button 
          class="btn btn-success btn-sm mx-1"
          style="width: 120px"
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

      <form @submit.prevent="add">
        <div class="d-flex">
          <div>
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
              Add
            </button>
          </div>
        </div>
        <div class="mt-1">
          <BButtonGroup>
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
        </div>
      </form>
    </div>
    <Layout 
      class="flex-grow-1"
      :layout.sync="layout"
      :editable="true"
    />
  </div>
</template>

<script>
import Layout from './Layout';
import { mapGetters } from 'vuex';

export default {
  components: {
    Layout,
  },
  data: () => ({
    layout: [],
    address: '',
    pending: false,
    type: 'source',
  }),
  computed: mapGetters(['layoutPending']),
  watch: {
    layoutPending(v) {
      if(!v) {
        this.$router.push('/');
      }
    }
  },
  methods: {
    cancel() {
      this.$router.push('/');
    },
    add() {
      const i = this.layout.length + 1;
      this.layout.push({
        i: i,
        x: 10,
        y: 10,
        type: this.type,
        address: this.address,
      });
      this.address = '';
    },
    save() {
      this.$store.commit('setLayoutPending', true);
      this.$ipc.send('saveConfig', JSON.stringify(this.layout));
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