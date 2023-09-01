import { useCallback, useState } from 'react';
import { Button } from '@strategies/ui';
import { FiArrowRight } from 'react-icons/fi';
import { PiArrowCircleRight } from 'react-icons/pi';
import { useStores } from '@strategies/stores';

import Stores from '../../../stores/Stores';
import { Startup } from '../../../stores/UIStore';
import StartupProgress from '../StartupProgress/StartupProgress';
import steps from './steps';
import { Nav, Body } from '../StartupModal';


export default function UserGuide() {
    const { ui } = useStores<Stores>();
    const [step, setStep] = useState<number>(0);
    const isAtFinalStep = step === steps.length - 1;

    const next = useCallback(() => ui.setStartup(Startup.LOAD_PROJECT), [ui]);

    return (
        <div className="UserGuide">
            <Nav>
                <main>
                    <h3>Welcome</h3>
                    <ol>
                        {steps.map((_step, i) => (
                            <li 
                                key={_step.title} 
                                className={i === step ? 'active' : ''}
                                onClick={() => setStep(i)}
                            >
                                <PiArrowCircleRight />
                                <span>{_step.title}</span>
                            </li>
                        ))}
                    </ol>
                </main>

                <footer>
                    <Button className={isAtFinalStep ? '' : 'secondary'} onClick={next}>
                        {isAtFinalStep ? 'Load Project' : 'Skip & Load Project'}
                    </Button>
                </footer>
            </Nav>

            <Body>
                <header>
                    <h2>{steps[step].header}</h2>
                </header>

                <main>
                    {steps[step].body}
                </main>

                <footer>
                    {step !== 0 && (
                        <Button className="secondary" onClick={() => setStep(step-1)}>
                            Previous
                        </Button>
                    )}

                    <StartupProgress 
                        current={step}
                        steps={steps.length}
                        onClick={setStep}
                    />

                    {isAtFinalStep || (
                        <Button onClick={() => setStep(step+1)}>
                            Next
                            <FiArrowRight />
                        </Button>
                    )}
                </footer>
            </Body>
        </div>
    );
}
