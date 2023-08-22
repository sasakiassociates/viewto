import { observer } from 'mobx-react';
import { useStores } from '@strategies/stores';
import { Modal, Body } from '@strategies/ui';

import Stores from '../../stores/Stores';
import UserGuide from './UserGuide/UserGuide';


export default observer(function StartupModal() {
    const { ui } = useStores<Stores>();

    return (
        <Modal 
            className="StartupModal" 
            active={ui.startupModalIsOpen}
            mount={ui.appRef}
        >
            <Body>
                <UserGuide />
            </Body>
        </Modal>
    );
});
