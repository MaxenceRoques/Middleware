import { instructionToIcon } from '../../scripts/utils.js';

class InstructionComponent extends HTMLElement {
    constructor() {
        super();
        const shadow = this.attachShadow({ mode: 'open' });

        // Create the container for the instruction
        let context = this;
        fetch('components/instruction/instruction.html').then(async function (response) {
            let container = await response.text();

            let template = new DOMParser().parseFromString(container, 'text/html').querySelector('template').content;
            shadow.appendChild(template.cloneNode(true));

            context.init();
        });
    }

    init() {
        let context = this;
        document.addEventListener('timeSelected', function (e) {
            context.shadowRoot.querySelector('.Time').innerText = e.detail.time;
        });
        document.addEventListener('firstStep', function (e) {
            context.shadowRoot.querySelector('.textualIndication').innerText = e.detail.firstStep;
            context.shadowRoot.querySelector('.instruction').style.display = 'flex';
            context.shadowRoot.querySelector('#arrow').classList.add(instructionToIcon(e.detail.firstStep));
        });
        
        
        
    }
}

customElements.define("instruction-component", InstructionComponent);