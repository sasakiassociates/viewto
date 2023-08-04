import { observer } from 'mobx-react';
import { Panel, Title, Body } from '@strategies/ui';


export default observer(function PointViewPanel() {
    return (
        <Panel
            className="PointView"
            active={true}
        >
            <Title>Points and Views Explorer</Title>
            <Body>
            </Body>
        </Panel>
    );
});
