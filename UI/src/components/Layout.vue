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
      @layout-updated="updateLayout"
    >
      <GridItem
        v-for="item in internalLayout"
        :key="item.i"
        :x="item.x"
        :y="item.y"
        :w="item.w"
        :h="item.h"
        :i="item.i"
        class="grid-item d-flex flex-column justify-content-center"
        :class="item.type"
      >
        <div class="node">
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
    layout() {
      this.refreshLayout();
    }
  },
  mounted() {
    const handleResize = () => {
      if (!this.$refs.gridContainer) return;
      this.box = this.$refs.gridContainer.getBoundingClientRect();
      this.rowHeight = this.box.width / this.colNum;
    }
    handleResize();
    window.addEventListener('resize', handleResize);
  },
  methods: {
    updateLayout(l) {
      const newLayout = l.map(n => ({
        address: n.address,
        i: n.i,
        type: n.type,
        x: n.x,
        y: n.y,
      }))
      this.$emit('update:layout', newLayout);
    },
    refreshLayout() {
      this.internalLayout = this.layout
        .map(el => ({
          ...el,
          w: this.size,
          h: this.size,
        }));
    }
  },
}
</script>

<style lang="scss" scoped>
  .grid-item {
    font-size: 10px;
    border: 1px solid rgba(100, 100, 100, 0.5);

    &.detector {
      border-radius: 50%;
    }
    // .node {
    //   width: 100%;
    //   height: 100%;
    //   transform: rotate(45deg);
    // }
  }
</style>