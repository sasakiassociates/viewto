import { observer } from 'mobx-react';
import { Panel, Title, Body } from '@strategies/ui';


export default observer(function LayersPanel() {
    return (
        <Panel
            className="Layers"
            active={true}
        >
            <Title>Layers</Title>
            <Body>
            </Body>
        </Panel>
    );
});
