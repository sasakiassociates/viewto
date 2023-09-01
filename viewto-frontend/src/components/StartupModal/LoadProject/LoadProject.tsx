import { observer } from 'mobx-react';
import { Button } from '@strategies/ui';
import { FiPlusCircle } from 'react-icons/fi';
import { PiFolderOpen } from 'react-icons/pi';
import { useStores } from '@strategies/stores';

import Stores from '../../../stores/Stores';
import { Startup } from '../../../stores/UIStore';
import { Nav, Body } from '../StartupModal';


export default observer(function LoadProject() {
    const { ui } = useStores<Stores>();

    return (
        <div className="LoadProject">
            <Nav />
            <Body>
                <header>
                    <h2>Welcome to Dashi Scenario Manager</h2>
                </header>
                <main>
                    <p>
                        To start, either open an existing Scenario file or create a new Scenario
                    </p>
                </main>
                <footer>
                    <Button className="secondary" onClick={() => ui.setStartup(Startup.OPEN_SCENARIO)}>
                        Open Scenario
                        <PiFolderOpen />
                    </Button>
                    <Button onClick={() => ui.setStartup(Startup.CREATE_SCENARIO)}>
                        Create New Scenario
                        <FiPlusCircle />
                    </Button>
                </footer>
            </Body>
        </div>
    );
});
