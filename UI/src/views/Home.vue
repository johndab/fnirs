<template>
  <div class="full-height">

    <div class="columns full-height" >

      <div class="column is-three-quarters" >
        <Board />
      </div>
      <div class="column is-paddingless" >
        <SideBar 
        />
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