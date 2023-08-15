import { observer } from 'mobx-react';
import { MenuBar, Title, Body } from '@strategies/ui';


export default observer(function ViewToMenuBar() {
    return (
        <MenuBar drawer={false}>
            <Title>View Analysis Dashboard</Title>
            <Body>
            </Body>
        </MenuBar>
    );
});
