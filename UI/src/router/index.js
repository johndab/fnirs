import Vue from 'vue';
import VueRouter from 'vue-router';
import Home from '../views/Home.vue';
import Board from '../components/Board.vue';
import BoardEdit from '../components/BoardEdit.vue';
Vue.use(VueRouter);
const routes = [
    {
        path: '/',
        name: 'home',
        component: Home,
        children: [
            {
                path: '',
                component: Board,
            },
            {
                path: 'edit',
                component: BoardEdit,
            }
        ]
    },
];
const router = new VueRouter({
    mode: 'history',
    base: process.env.BASE_URL,
    routes,
});
export default router;
//# sourceMappingURL=index.js.map