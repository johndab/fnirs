<template>
  <div 
    ref="gridContainer"
    :class="editable ? 'editable' : ''"
  >
    <GridLayout
      :layout.sync="internalLayout"
      :col-num="colNum"
      :row-height="rowHeight"
      :is-draggable="editable"
      :is-resizable="false"
      :is-mirrored="false"
      :vertical-compact="false"
      :prevent-collision="true"
      :margin="[0, 0]"
      :use-css-transforms="true"
    >
      <GridItem
        v-for="item in layout"
        :key="item.i"
        :x="item.x"
        :y="item.y"
        :w="item.w"
        :h="item.h"
        :i="item.i"
        class="grid-item d-flex flex-column justify-content-center"
      >
        <div>
          {{ item.address }}
        </div>
      </GridItem>
    </GridLayout>
  </div>
</template>

<script>
import VueGridLayout from 'vue-grid-layout';

export default {
  components: {
    GridLayout: VueGridLayout.GridLayout,
    GridItem: VueGridLayout.GridItem
  },
  props: {
    layout: {
      type: Array,
      default: () => [],
    },
    values: {
      type: Array,
      default: () => [],
    },
    editable: Boolean,
  },
  data: () => ({
    internalLayout: [],
    colNum: 40,
    rowHeight: 30,
    size: 2,
    box: {},
  }),
  watch: {
    internalLayout(l) {
      if(this.editable) {
        this.$emit('update:layout', l);
      }
    },
    layout(l) {
      this.internalLayout = l;
    },
  },
  created() {
    const handleResize = () => {
      this.box = this.$refs.gridContainer.getBoundingClientRect();
      this.rowHeight = this.box.width / this.colNum;
    }
    window.addEventListener('resize', handleResize);
  }
}
</script>

<style lang="scss" scoped>
  .grid-item {
    border: 1px solid rgba(100, 100, 100, 0.5);
    font-size: 10px;
  }
</style>