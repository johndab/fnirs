<template>
  <div class="full-height">
    <div class="d-flex full-height">
      <div class="flex-grow-1">
        <Board />
      </div>
      <div style="width: 250px">
        <SideBar />
      </div>
    </div>
  </div>
</template>

<script>
import Board from '@/components/Board.vue';
import SideBar from '@/components/SideBar.vue';

export default {
  components: {
    Board,
    SideBar,
  },
};
</script>


<style scoped>

  .full-height {
    height: 100vh;
  }
</style>