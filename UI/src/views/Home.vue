<template>
  <div class="full-height">
    <div class="d-flex full-height">
      <div class="flex-grow-1">
        <RouterView />
      </div>
      <div style="width: 250px">
        <SideBar />
      </div>
    </div>
  </div>
</template>

<script>
import SideBar from '@/components/SideBar.vue';

export default {
  components: {
    SideBar,
  },
};
</script>


<style scoped>

  .full-height {
    height: 100vh;
  }
</style>