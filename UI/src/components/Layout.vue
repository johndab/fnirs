<template>
  <div 
    ref="gridContainer"
    class="position-relative"
    :class="editable ? 'editable' : ''"
    @mousedown="mouseDown"
  >
    <GridLayout
      v-if="internalLayout && internalLayout.length > 0"
      ref="layout"
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
        class="grid-item d-flex flex-column"
        :class="{ 
          'justify-content-center': editable,
          selected: selected[item.i], 
          [item.type]: true,
          edited: edited === item.i,
        }"
        :data-item="item.i"
        @move="itemMove"
        @dblclick.native="edit(item)"
      >
        <div class="node">
          {{ item.type.slice(0,1).toUpperCase() }}
          {{ item.address }}
        </div>
        <div 
          :ref="`${item.type}${item.address}`"
          class="value"
        />
      </GridItem>
    </GridLayout>

    <div 
      id="dragArea"
      ref="dragArea"
    />
    <Connections 
      v-if="box.width"
      ref="connections"
      :graph="graph"
      :container="box"
      :col-num="colNum"
      :row-height="rowHeight"
      :layout="layout"
      :size="size"
      :use-connections="useConnections"
      :values="values"
    />
  </div>
</template>

<script>
import VueGridLayout from 'vue-grid-layout';
import tinycolor from 'tinycolor2';
import Connections from './graph/Connections';

export default {
  components: {
    GridLayout: VueGridLayout.GridLayout,
    GridItem: VueGridLayout.GridItem,
    Connections,
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
    edited: {
      type: [Number, String],
      default: -1,
    },
    depth: {
      type: Number,
      default: 1,
    },
    useConnections: Boolean,
  },
  data: () => ({
    internalLayout: [],
    colNum: 40,
    rowHeight: 30,
    size: 2,
    box: {},
    drag: {},
    graph: [],
    selected: {},
  }),
  watch: {
    layout() {
      this.refreshLayout();
    },
    depth(v) {
      this.graph = this.genGraph(this.layout);
      this.$emit('setGraph', this.graph);
    },
    useConnections(v) {
      if(v) {
        this.layout.forEach(({ address, type }) => {
          const boxes = this.$refs[`${type}${address}`];
          if(boxes.length === 0) return;
          const el = boxes[0];

          el.parentElement.style.backgroundColor = 'transparent';
          el.parentElement.style.borderColor = 'rgba(100, 100, 100, 0.2)';
          el.innerHTML = '';
        })
      }
    },
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
  created() {
    this.refreshLayout();
    this.graph = this.genGraph(this.layout);
    this.$emit('setGraph', this.graph);

    if (this.editable) {
      document.addEventListener('mousemove', this.mouseMove);
      document.addEventListener('mouseup', this.mouseUp);
    }
  },
  destroyed() {
    document.removeEventListener('mousemove', this.mouseMove);
    document.removeEventListener('mouseup', this.mouseUp);
  },
  methods: {
    setData(data) {
      if(this.useConnections) {
        this.$refs.connections.setData(data);
      }

      const max = data.acMax;
      const min = data.acMin;

      const updateElement = (el, val) => {
        let ratio = (val - min) / (max - min);
          if(Number.isNaN(ratio)) {
            ratio = 0;
            val = '?';
          }
          if(this.useConnections) {
            el.innerHTML = `${Math.floor(val)}`;
            return;
          }
          const colorRatio = Math.round(ratio * 100); 

          let color = tinycolor({ 
            h: 100 - colorRatio * 0.8, 
            s: 100, 
            l: 50,
            a: colorRatio / 100
          })
          .toRgbString();

          el.parentElement.style.backgroundColor = color;
          el.parentElement.style.borderColor = color;
          el.innerHTML = `${Math.floor(val)}`;
      }

      Object.keys(data.values).forEach(d => {
          const dbox = this.$refs[`detector${d}`];
          if(dbox.length === 0 || data.values[d].length === 0) return;
          const el = dbox[0];
          const sourceValues = Object.values(data.values[d]);
          const avg = sourceValues.reduce((acc, source) => source.ac + acc, 0) 
            / sourceValues.length;
          
          updateElement(el, avg);
      });

      const list = Object.values(data.values);
      this.layout.forEach(({ address, type }) => {
        if(type === 'detector') return;
        const sbox = this.$refs[`source${address}`];
        if(sbox.length === 0) return;
        const el = sbox[0];

        const values = list
          .filter(det => det[address])
          .map(det => det[address]);
        
        const avg = values
          .reduce((acc, curr) => acc + curr.ac, 0) / values.length;

        updateElement(el, avg);
      });
    },
    genGraph(layout) {
      const { depth } = this;
      const distance = (a, b) => Math.pow((a.x - b.x), 2) + Math.pow((a.y - b.y), 2); 
      return layout
        .filter(x => x.type === 'detector')
        .map(x => {
        const distances = layout
          .reduce((acc, y) => { 
            if(y.i == x.i || x.type === y.type) return acc;
            const d = Math.floor(distance(x, y));
            if(acc[d]) {
              acc[d].push(y);
            } else {
              acc[d] = [y];
            }
            return acc;
          }, {});

        const targets = Object.keys(distances)
          .sort((a, b) => a - b);
        const target = targets[depth-1] || [];
        const nearest = distances[target];
        
        return {
          address: x.address,
          type: x.type,
          x: x.x,
          y: x.y,
          nearest,
        }
      });
    },
    updateLayout(l) {
      const newLayout = l.map(n => ({
        address: n.address,
        i: n.i,
        type: n.type,
        x: n.x,
        y: n.y,
      }))
      this.graph = this.genGraph(newLayout);
      this.$emit('setGraph', this.graph);
      this.$emit('update:layout', newLayout);
    },
    edit(item) {
      this.$emit('edit', item);
    },  
    refreshLayout() {
      this.internalLayout = this.layout
        .map(el => ({
          ...el,
          x: el.x === -1 ? (this.colNum - this.size) : el.x,
          y: el.y === -1 ? 0 : el.y,
          w: this.size,
          h: this.size,
        }));
    },
    itemMove(i, newX, newY) {
      const node = this.internalLayout.find(el => el.i === i);
      if (!node) return;
      const dx = node.x - newX;
      const dy = node.y - newY;

      this.internalLayout.forEach((n, index) => {
        if (this.selected[n.i] && n.i !== i) {
          this.internalLayout[index].x = n.x - dx;
          this.internalLayout[index].y = n.y - dy;
        }
      });
    },
    mouseDown(e) {
      if(!this.editable) return;
      // grid item clicked
      const itemEl = e.target.closest('.grid-item');
      if(itemEl) {
        const i = itemEl.getAttribute('data-item');
        this.$set(this.selected, i, !this.selected[i]);
      } else {
        this.selected = {};
        this.drag.start = {
          x: e.pageX,
          y: e.pageY,
        };
        this.updateDragArea(e);
      }
    },
    mouseUp(e) {
      if(!e.ctrlKey) {
        this.selected = {};
      }
      
      if(!this.drag.start) return;      

      const { top, left, width, height } = this.getArea(e);

      const blocks = this.findBlocks(
        { x: left, y: top, }, 
        { x: left + width, y: top + height }
      );

      blocks.forEach((b) => {
        this.$set(this.selected, b.i, true);
      });

      this.drag.start = null;
      this.updateDragArea(e);
    },
    findBlocks(start, end) {
      const nodes = this.$refs.layout.$el.childNodes;
      const yOffset = this.box.top || 0;
      const xOffset = this.box.left || 0;
      
      const items = [...nodes]
        .filter(el => el.classList.contains('grid-item'))
        .map(el => ({
          i: el.getAttribute('data-item'),
          box: el.getBoundingClientRect(),
        }))
        .filter(x => x.box)
        .map(x => ({
          i: x.i,
          box: {
            left: x.box.left - xOffset,
            top: x.box.top - yOffset,
            width: x.box.width,
            height: x.box.height,
          },
        }));

      return items.filter(({ i, box }) => {
        return box.left > start.x && box.top > start.y
          && (box.left + box.width) < end.x 
          && (box.top + box.height) < end.y;
      })
    },
    mouseMove(e) {
      this.updateDragArea(e);
    },
    getArea(e) {
      const yOffset = this.box.top || 0;
      const xOffset = this.box.left || 0;

      const { x, y } = this.drag.start;
      const top = Math.min(e.pageY, y) - yOffset;
      const left = Math.min(e.pageX, x) - xOffset;
      
      const width = Math.abs(e.pageX - x);
      const height = Math.abs(e.pageY - y);
      return {
        top, 
        left,
        width,
        height,
      }
    },
    updateDragArea(e) {
      if(!this.$refs.dragArea) return;
      if(!this.drag.start) {
        this.$refs.dragArea.style.display = 'none';
        return;
      }
      const { top, left, width, height } = this.getArea(e);

      this.$refs.dragArea.style.display = 'block';
      this.$refs.dragArea.style.top = `${top}px`;
      this.$refs.dragArea.style.left = `${left}px`;

      this.$refs.dragArea.style.width = `${width}px`;
      this.$refs.dragArea.style.height = `${height}px`;
    },
  },
}
</script>

<style lang="scss" scoped>
  .grid-item {
    font-size: 10px;
    border: 1px solid rgba(100, 100, 100, 0.2);

    user-select: none;

    &.detector {
      border-radius: 50%;
    }

    &.selected {
      border-color: #333;
      box-shadow: 0 0 4px rgba(#333, 0.3);
    }

    &.edited {
      border-color: rgb(207, 36, 36);
      background-color: rgba(207, 36, 36, 0.1);
      font-weight: 500;
    }
    .node {
      font-weight: bold;
    }
  }

  #dragArea {
    border: 1px solid #00B3B3;
    background-color: rgba(#00E2E2, 0.1);
    position: absolute;
  }
</style>