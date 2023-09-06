import { observer } from 'mobx-react';
import { MenuBar, Title, Body, IconButton } from '@strategies/ui';
import { FiFolder, FiHelpCircle, FiSave, FiSettings, FiShare } from 'react-icons/fi'
import { useStores } from '@strategies/stores';
import { RadioSlider } from "@strategies/react-form";

import Stores from '../../stores/Stores';
import { UIMode } from '../../stores/UIStore';



export default observer(function ViewToMenuBar() {
    const { scenario, ui } = useStores<Stores>()


    return (
        <MenuBar drawer={false}>
            <Title>View Analysis Dashboard</Title>
            <Body>
                <div className='menu-icons' >
                    <IconButton>
                        <FiHelpCircle />
                    </IconButton>
                    <IconButton>
                        <FiSettings />
                    </IconButton>
                    <IconButton>
                        <FiShare />
                    </IconButton>
                    <IconButton>
                        <FiFolder />
                    </IconButton>
                </div>
                <div className="scenario-name-wrapper">
                    <div className="scenario-name">
                        <label htmlFor="scenario">Name</label>
                        <input id="scenario" placeholder="Name ..." value={scenario.study?.name} readOnly
                        // onChange={} 
                        />
                    </div>
                </div>
                <div className='menu-icons' >
                    <IconButton>
                        <FiSave />
                    </IconButton>
                </div>
                
            </Body>
        </MenuBar>
    );
});
