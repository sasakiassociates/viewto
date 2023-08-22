import { useState } from 'react';
import { Button } from '@strategies/ui';
import { FiArrowRight } from 'react-icons/fi';

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
                        {steps.map((step, i) => (
                            <li key={step.title} onClick={() => setStep(i)}>
                                {step.title}
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
                        <Button className="secondary">
                            Previous
                        </Button>
                    )}

                    <StartupProgress 
                        current={step+1}
                        steps={steps.length}
                        onClick={step => setStep(step - 1)}
                    />

                    {isAtFinalStep || (
                        <Button>
                            Next
                            <FiArrowRight />
                        </Button>
                    )}
                </footer>
            </div>
        </div>
    );
}
