<template>
  <div class="sidebar text-left">
    <div class="px-2">
      <Connection />
      <Streaming />
      <Collect />
      <CyclesNum />
      <SwitchMode />
      <Frequency />
    </div>
  </div>
</template>

<script>
import Frequency from './sidebar/Frequency';
import Connection from './sidebar/Connection';
import Collect from './sidebar/Collect';
import Streaming from './sidebar/Streaming';
import CyclesNum from './sidebar/CyclesNum';
import SwitchMode from './sidebar/SwitchMode';

export default {
  components: {
    Frequency,
    Connection,
    Collect,
    Streaming,
    CyclesNum,
    SwitchMode,
  },
}
</script>

<style scoped>
  .sidebar {
    background-color: rgba(100, 100, 100, 0.1);
    height: 100%;
    width: 100%;
  }
</style>