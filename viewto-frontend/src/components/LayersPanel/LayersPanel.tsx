import { observer } from 'mobx-react';
import { Panel, Title, Body, Select } from '@strategies/ui';
import { Input } from '@strategies/react-form';
import { useStores } from '@strategies/stores';
import { FiInfo } from 'react-icons/fi';

import Stores from '../../stores/Stores';

const sampleData = ['data1', 'data2', 'data3', 'data4']; //todo: This needs to be replaced with the actual data

export default observer(function LayersPanel() {
    const { focuses, views, obstructors } = useStores<Stores>();
    const view = views.active;

    return (
        <Panel className="Layers" active={true}>
            <Title>Layers</Title>
            <Body>
                <div className="Widget">
                    <div className="_View">
                        <div className="SelectContainer">
                            <div className="Title">
                                {'View Focus'} <FiInfo />
                            </div>
                            <Select
                                placeholder={'Select view focus ...'}
                                isMulti={true}
                                options={focuses.all.map(focus => focus.name)}
                                value={view.focuses.map(focus => focus.name)}
                                onChange={(names: string[]) => {
                                    view.setFocusIds(
                                        names.map(name => focuses.byName[name].sasakiId)
                                    );
                                }}
                                menuPortalTarget={document.body}
                                menuShouldScrollIntoView={false}
                                menuPosition={'absolute'}
                                menuPlacement={'auto'}
                            />
                        </div>
                        <div className="SelectContainer">
                            <div className="Title">
                                {'View Condition'} <FiInfo />
                            </div>
                            <Select
                                placeholder={'Select view condition ...'}
                                isCreatable={true}
                                options={obstructors.all.map(obs => obs.name)}
                                value={obstructors.all.map(obs => obs.name)}
                                onChange={(names: string[]) => {
                                    view.setObstructorIds(
                                        names.map(name => obstructors.byName[name].sasakiId)
                                    );
                                }}
                                menuPortalTarget={document.body}
                                menuShouldScrollIntoView={false}
                                styles={{
                                    menuPortal: base => ({
                                        ...base,
                                    }),
                                }}
                                menuPosition={'absolute'}
                                menuPlacement={'auto'}
                            />
                        </div>
                    </div>
                    {/*}
                    <div className="_Points">
                        <div className="InputContainer">
                            <div className="Title">
                                {"Total points"}
                            </div>
                            <Input
                                className="points-input"
                                type="number"
                                readonly={true}
                                value={views.totalPoints}
                                onChange={(value: string|number) => {
                                    views.setTotalPoints(value as number)
                                }}
                            />
                        </div>
                        <div className="InputContainer">
                            <div className="Title">
                                {"Visible points"}
                            </div>
                            <Input
                                className="points-input"
                                type="number"
                                readonly={true}
                                value={views.visiblePoints}
                                onChange={(value: string|number) => {
                                    views.setVisiblePoints(value as number)
                                }}
                            />
                        </div>
                        <div className="InputContainer">
                            <div className="Title">
                                {"Good view points"}
                            </div>
                            <Input
                                className="points-input"
                                type="number"
                                readonly={true}
                                value={views.goodViewPoints}
                                onChange={(value: string|number) => {
                                    views.setGoodViewPoints(value as number)
                                }}
                            />
                        </div>
                        <div className="InputContainer">
                            <div className="Title">
                                {"No view points"}
                            </div>
                            <Input
                                className="points-input"
                                type="number"
                                readonly={true}
                                value={views.noViewPoints}
                                onChange={(value: string|number) => {
                                    views.setNoViewPoints(value as number)
                                }}
                            />
                        </div>
                    </div>
                      */}
                </div>
            </Body>
        </Panel>
    );
});
