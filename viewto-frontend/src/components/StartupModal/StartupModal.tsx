import { ReactNode } from 'react';
import { observer } from 'mobx-react';
import { useStores } from '@strategies/stores';
import { Modal, Body as ModalBody } from '@strategies/ui';

import Stores from '../../stores/Stores';
import { Startup } from '../../stores/UIStore';
import Logo from '../../assets/ViewTo.svg';
import UserGuide from './UserGuide/UserGuide';
import LoadProject from './LoadProject/LoadProject';



export default observer(function StartupModal() {
    const { ui } = useStores<Stores>();

    return (
        <Modal 
            className="StartupModal" 
            active={ui.startupModalIsOpen}
            mount={ui.appRef}
        >
            <ModalBody>
                {ui.startup === Startup.USER_GUIDE && <UserGuide />}
                {ui.startup === Startup.LOAD_PROJECT && <LoadProject />}
            </ModalBody>
        </Modal>
    );
});


export function Nav({ children }: { children?: ReactNode }) {
    return (
        <nav> 
            <header>
                <img src={Logo} alt="ViewTo logo" />
            </header>

            {children}
        </nav>
    );
}
 
export function Body({ children }: { children: ReactNode }) {
    return (
        <div className="Body">
            {children}
        </div>
    );
}
