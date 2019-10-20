<template>
  <div id="app">
    <RouterView />
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';

// tslint:disable-next-line
// const { ipcRenderer } = require("electron");

@Component({
})
export default class App extends Vue {}
</script>

<style lang="scss">
#app {
  font-family: 'Avenir', Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
}

body {
  margin-top: 10px;
}
</style>
