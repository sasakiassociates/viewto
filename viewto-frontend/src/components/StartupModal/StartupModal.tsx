import { observer } from 'mobx-react';
import { useStores } from '@strategies/stores';
import { Modal, Body, Button } from '@strategies/ui';

import Stores from '../../stores/Stores';


export default observer(function StartupModal() {
    const { ui } = useStores<Stores>();

    return (
        <Modal active={ui.startupModalIsOpen}>
            <Body>
            </Body>
        </Modal>
    );
});
