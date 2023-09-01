import { noop } from '../../../util';


export type StartupProgressProps = {
    current: number;
    steps: number;
    onClick?: (step: number) => any;
};

export default function StartupProgress({ current, steps, onClick }: StartupProgressProps) {
    return (
        <div className="StartupProgress">
            {Array.from(Array(steps).keys()).map(step => (
                <span 
                    className={step === current ? 'current' : ''} 
                    onClick={onClick ? () => onClick(step) : noop}
                />
            ))} 
        </div>
    );
}
