import { observer } from 'mobx-react';
import { Panel, Title, Body } from '@strategies/ui';


export default observer(function SelectionPanel() {
    return (
        <Panel
            className="Selection"
            active={true}
        >
            <Title>Selection</Title>
            <Body>
            </Body>
        </Panel>
    );
});
