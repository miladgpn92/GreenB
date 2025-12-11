import { Drawer } from "flowbite";
import type { DrawerOptions, DrawerInterface } from "flowbite";
let drawer: DrawerInterface
// options with default values
const options: DrawerOptions = {
    placement: 'bottom',
    backdrop: true,
    bodyScrolling: false,
    edge: false,
    edgeOffset: '',
    onHide: () => {
    },
    onShow: () => {
    },
    onToggle: () => {
    }
};

const initDrawer = (drawerContainer: HTMLElement): void => {

    drawer = new Drawer(drawerContainer, options);

}
const showDrawer = () => {
    drawer.show()
}
const hideDrawer = () => {
    drawer.hide();
}


export { initDrawer, showDrawer, hideDrawer }