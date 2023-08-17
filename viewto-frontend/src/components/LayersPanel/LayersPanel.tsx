import { observer } from 'mobx-react';
import { Panel, Title, Body, Select } from '@strategies/ui';
import { Input } from '@strategies/react-form';
import { useStores } from "@strategies/stores";
import Stores from "../../stores/Stores";
import { FiInfo } from 'react-icons/fi';

const sampleData = ['data1', 'data2', 'data3', 'data4']; //todo: This needs to be replaced with the actual data
export default observer(function LayersPanel() {

    const { view } = useStores<Stores>();
    return (
        <Panel
            className="Layers"
            active={true}
        >
            <Title>Layers</Title>
            <Body>
                <div className="Widget">
                    <div className="_View">
                        <div className="SelectContainer">
                            <div className="Title">
                                {"View Focus"} <FiInfo/>
                            </div>
                            <Select
                                placeholder={'Select view focus ...'}
                                isCreatable={true}
                                isMulti={true}
                                options={sampleData}
                                value={view.viewFocus}
                                onChange={(values: string[]) => view.setViewFocus(values)}
                                menuPortalTarget={document.body}
                                menuShouldScrollIntoView={false}
                                styles={{
                                    menuPortal: (base) => ({
                                        ...base,
                                    })
                                }}
                                menuPosition={'absolute'}
                                menuPlacement={'auto'}
                            />
                        </div>
                        <div className="SelectContainer">
                            <div className="Title">
                                {"View Condition"} <FiInfo/>
                            </div>
                            <Select
                                placeholder={'Select view condition ...'}
                                isCreatable={true}
                                options={sampleData}
                                value={view.viewCondition}
                                onChange={(value: string) => view.setViewCondition(value)}
                                menuPortalTarget={document.body}
                                menuShouldScrollIntoView={false}
                                styles={{
                                    menuPortal: (base) => ({
                                        ...base,
                                    })
                                }}
                                menuPosition={'absolute'}
                                menuPlacement={'auto'}
                            />
                        </div>

                    </div>
                    <div className="_Points">
                        <div className="InputContainer">
                            <div className="Title">
                                {"Total points"}
                            </div>
                            <Input
                                className="points-input"
                                type="number"
                                readonly={true}
                                value={view.totalPoints}
                                onChange={(value: string|number) => {
                                    view.setTotalPoints(value as number)
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
                                value={view.visiblePoints}
                                onChange={(value: string|number) => {
                                    view.setVisiblePoints(value as number)
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
                                value={view.goodViewPoints}
                                onChange={(value: string|number) => {
                                    view.setGoodViewPoints(value as number)
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
                                value={view.noViewPoints}
                                onChange={(value: string|number) => {
                                    view.setNoViewPoints(value as number)
                                }}
                            />
                        </div>
                    </div>
                </div>
            </Body>
        </Panel>
    );
});
