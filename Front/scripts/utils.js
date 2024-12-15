
const up = 'fa-up-long';
const left = 'fa-left-long';
const right = 'fa-right-long';
const info = 'fa-info';

export function instructionToIcon(instruction) {
    if (instruction.includes('tout droit')) {
        return up;
    } else if (instruction.includes('droite')) {
        return right;
    } else if (instruction.includes('gauche')) {
        return left;
    } else {
        return info;
    }   
}

export function dispatchTime(timeInSeconds) {
    let time = '';
    if (timeInSeconds < 60) {
        time = `${timeInSeconds}s`;
    }
    else if (timeInSeconds < 3600) {
        time = `${(timeInSeconds / 60).toFixed(0)}min`;
    }
    else if (timeInSeconds < 86400) {
        time = `${(timeInSeconds / 3600).toFixed(0)}h${((timeInSeconds % 3600) / 60).toFixed(0)}`;
    }
    else {
        time = `${(timeInSeconds / 86400).toFixed(0)}d`;
    }

    document.dispatchEvent(new CustomEvent('timeSelected', {
        detail: {
            time: time
        }
    }));
}

export function getCorrespondingColor(type) {
    switch (type) {
        case 'foot-walking':
            return '#0099ff';
        case 'cycling-road':
            return '#00cc00';
    }
}
