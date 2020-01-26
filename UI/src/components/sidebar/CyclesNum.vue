<template>
  <div class="mt-1">
    Cycles per block
    <div class="d-flex">
      <BFormInput 
        v-model="cycles"
        size="sm"
        :disabled="!isConnected"
        class="mr-1"
      />
      <button 
        class="btn btn-secondary btn-sm btn-block ml-1" 
        @click="update"
      >
        <span v-if="!pending">
          Set
        </span>
        <span v-else>
          <BSpinner
            small
          />
        </span>
      </button> 
    </div>
  </div>
</template>

<script>
import { mapGetters } from 'vuex';

export default {
  data: () => ({
    cycles: 0,
    pending: false,
  }),
  computed:{ 
    ...mapGetters(['cyclesNum', 'isConnected']),
  },
  created() {
    this.cycles = this.cyclesNum;
    this.$ipc.on('SetCyclesNum', this.setCyclesNum);
  },
  methods: {
    update() {
      var val = parseInt(this.cycles, 10);
      if(!Number.isNaN(val) && val > 0 && val <= 16) {
        this.pending = true;
        this.$ipc.send('SetCyclesNum', val);
      } 
    },
    setCyclesNum(arg) {
      this.pending = false;
      this.cycles = arg;
      this.$store.commit('setCyclesNum', arg);
    },
  }
}
</script>

<style>

</style>