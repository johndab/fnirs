<template>
  <svg
    :height="container.height"
    :width="container.width"
    style="position: absolute; top: 0; left: 0; z-index: -1"
  >
    <line 
      v-for="line in lines"
      :key="line.id"
      :ref="line.id"
      :x1="line.x1"
      :y1="line.y1"
      :x2="line.x2"
      :y2="line.y2"
      style="stroke: rgba(150, 150, 150, 0.5); stroke-width:2"
    />
  </svg>
</template>

<script>
import tinycolor from 'tinycolor2';

export default {
  props: {
    container: {
      type: [Object, DOMRect],
      required: true,
    },
    graph: {
      type: Array,
      required: true,
    },
    layout: {
      type: Array,
      default: () => [],
    },
    colNum: {
      type: Number,
      required: true,
    },
    rowHeight: {
      type: Number,
      required: true,
    },
    size: {
      type: Number,
      required: true,
    },
    useConnections: Boolean,
    values: {
      type: Array,
      default: () => [],
    },
  },
  computed: {
    lines() {
      if(!this.container || !this.container.width) return [];
      const col = this.container.width / this.colNum;
      const row = this.rowHeight;

      const getReal = ({ x, y }) => ({
        x: col * x + (col * this.size / 2),
        y: col  * y + (row * this.size / 2),
      });

      return this.graph.flatMap(g => {
        const from = getReal(g);
        return (g.nearest || []).map(n => {
          const to = getReal(n);
          return {
            id: `${g.type}${g.address}_${n.address}`,
            x1: from.x,
            y1: from.y,
            x2: to.x,
            y2: to.y,
          }
        })
      });
    }
  },
  watch: {
    useConnections(c) {
      if(!c) {
        this.graph.flatMap(g => {
          return (g.nearest || []).map(n => {
            const key = `${g.type}${g.address}_${n.address}`
            const els = this.$refs[key];
            if(els.length == 0) return;
            const el = els[0];
            el.style.strokeWidth = 2;
            el.style.stroke = 'rgba(150, 150, 150, 0.5)';
          })
        });
      }
    },
  },
  methods: {
    setData(data) {
      const max = data.acMax;
      const min = data.acMin;

      Object.keys(data.values).forEach(num => {
        let d = String.fromCharCode(parseInt(num, 10) + 64);

          Object.keys(data.values[num]).forEach(s => {
            const dline = this.$refs[`detector${d}_${s}`];
            if(dline.length === 0 || !data.values[num][s]) return;
            const val = data.values[num][s].ac;
            // console.debug(val)
            const el = dline[0];

            let ratio = (val - min) / (max - min);
            if(Number.isNaN(ratio)) {
              ratio = 0;
            }
            const colorRatio = Math.round(ratio * 100); 

            let color = tinycolor({ 
              h: 100 - colorRatio * 0.8, 
              s: 100, 
              l: 50,
              a: colorRatio / 100
            })
            .toRgbString();

            el.style.strokeWidth = (colorRatio / 100) * 10;
            el.style.stroke = color;
          });
      });
    }
  },
}
</script>

<style>

</style>