import { observer } from 'mobx-react';
import { Panel, Title, Body, Select } from '@strategies/ui';
import { stores, useStores } from "@strategies/stores";
import UIStore from "../../stores/UIStore";
import Stores from "../../stores/Stores";
import { FiInfo } from 'react-icons/fi';

export default observer(function LayersPanel() {
    const sampleData = ['hello', 'test'];
    const { view } = useStores<Stores>();
    return (
        <Panel
            className="Layers"
            active={true}
        >
            <Title>Layers</Title>
            <Body>
                <div className="ViewParamsWidget">
                    <div className="ViewWidget">
                        <div className={'title'}>
                            {"View Focus"} <FiInfo/>
                        </div>
                        <Select
                            placeholder={'Select view focus ...'}
                            isCreatable={true}
                            isMulti={true}
                            options={sampleData}
                            value={view.viewFocus}
                            onChange={(values: string[]) => view.setViewFocus(values)}
                        />
                    </div>
                    <div className="ViewWidget">
                        <div className={'title'}>
                            {"View Focus"} <FiInfo/>
                        </div>
                        <Select
                            placeholder={'Select view focus ...'}
                            isCreatable={true}
                            isMulti={true}
                            options={sampleData}
                            value={view.viewFocus}
                            onChange={(values: string[]) => view.setViewFocus(values)}
                        />
                    </div>

                </div>
            </Body>
        </Panel>
    );
});
