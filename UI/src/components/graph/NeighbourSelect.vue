<template>
  <div class="position-relative">
    <BFormSelect 
      v-model="depth"
      :options="options"
      size="sm"
      type="number"
    />
    <span class="comment">
      neighbour
    </span>
  </div>
</template>

<script>
export default {
  props: {
    value: {
      type: Number,
      default: 1,
    },
  },
  data: () => ({
    depth: 1,
    options: [1,2,3,4,5],
  }),
  watch: {
    depth(v) {
      const n = Number.parseInt(v, 10);
      if(n && n > 0 && n < 10) {
        this.$emit('update:value', n)
      } else {
        this.depth = 1;
      }
    },
  },
  created() {
    this.depth = this.value;
  }
}
</script>

<style scoped>
  .comment {
    position: absolute; 
    top: -7px; 
    right: 3px;
    font-size: 10px;
    background-color: white;
  }
</style>