import { useState } from 'react';
import { Button } from '@strategies/ui';
import { FiArrowRight } from 'react-icons/fi';
import { PiArrowCircleRight } from 'react-icons/pi';

import Logo from '../../../assets/ViewTo.svg';
import StartupProgress from '../StartupProgress/StartupProgress';
import steps from './steps';


export default function UserGuide() {
    const [step, setStep] = useState<number>(0);
    const isAtFinalStep = step === steps.length - 1;

    return (
        <div className="UserGuide">
            <nav>
                <header>
                    <img src={Logo} alt="ViewTo logo" />
                </header>

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
                    <Button className={isAtFinalStep ? '' : 'secondary'}>
                        {isAtFinalStep ? 'Load Project' : 'Skip & Load Project'}
                    </Button>
                </footer>
            </nav>

            <div className="content">
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
            </div>
        </div>
    );
}
