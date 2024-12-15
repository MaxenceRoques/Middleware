import { instructionToIcon, getCorrespondingColor } from "../../scripts/utils.js";
import { acknowledgeAllMessages } from "../../scripts/activeMQHandler.js";
class ListInstruction extends HTMLElement {
    instructionDisplayed = false;
    axis = 'X';
    constructor() {
        super();
        const shadow = this.attachShadow({ mode: 'open' });

        fetch('components/list-instruction/list-instruction.html')
            .then(async (response) => {
                let container = await response.text();
                let template = new DOMParser()
                    .parseFromString(container, 'text/html')
                    .querySelector('template').content;

                shadow.appendChild(template.cloneNode(true));
                this.init();

            });
    }

    init() {
        document.addEventListener('step', (event) => {
            this.renderInstructions(event.detail.instruction, event.detail.profile, event.detail.isNewItinerary);
            this.listenForClick();
        });

        const instructionList = this.shadowRoot.querySelector('.instruction-list');
        instructionList.addEventListener('scroll', () => {
            if (instructionList.scrollTop + instructionList.clientHeight + 100 >= instructionList.scrollHeight) {
                acknowledgeAllMessages();
            }
        });
        
    }

    renderInstructions(instruction, profile, isNewItinerary) {
        const instructionList = this.shadowRoot.querySelector('.instruction-list');
        if (isNewItinerary) {
            instructionList.innerHTML = '';
        }
        const color = getCorrespondingColor(profile);
        const instructionCard = document.createElement('info-card');
        instructionCard.setAttribute('icon', instructionToIcon(instruction));
        instructionCard.setAttribute('description', instruction);
        instructionCard.setAttribute('background-color', color);
        instructionCard.setAttribute('color-icon', '#000000');
        instructionCard.setAttribute('box-shadow', 'none');
        instructionList.appendChild(instructionCard);  
    }

    listenForClick() {
        const button = this.shadowRoot.querySelector('.instruction-list');
        button.addEventListener('click', () => {
            this.instructionDisplayed = !this.instructionDisplayed;
            this.toggleInstructions();
        });
    }

    toggleInstructions() {
        const instructionList = this.shadowRoot.querySelector('.instruction-list');
        const documentList = document.querySelector('list-instruction');
        let instructionListHeight = instructionList.getBoundingClientRect().height-80;
        if (this.instructionDisplayed) {
            if (this.axis === 'Y') {
                instructionList.style.transform = 'translate' + this.axis + '(' + instructionListHeight + 'px)';
            }
            else {
                instructionList.style.transform = 'translate' + this.axis + '(100%)';
            }
            documentList.style.pointerEvents = 'none';
            instructionList.style.pointerEvents = 'auto';
            
        } else {
            if (this.axis === 'Y') {
                instructionList.style.height = 'auto';
            }
            instructionList.style.transform = 'translate' + this.axis + '(0%)';
            documentList.style.pointerEvents = 'auto';
            
        }
    }

    connectedCallback() {
        this.handleResize();
        window.addEventListener('resize', this.handleResize);
    }

    disconnectedCallback() {
        window.removeEventListener('resize', this.handleResize);
    }

    handleResize = () => {
        let changed = false;
        if (window.innerWidth < 600 && this.axis === 'X') {
            this.axis = 'Y';
            changed = true;
        } else if (window.innerWidth >= 600 && this.axis === 'Y') {
            this.axis = 'X';
            changed = true;
        }

        const instructionList = this.shadowRoot.querySelector('.instruction-list');
        const documentList = document.querySelector('list-instruction');
        if (instructionList && documentList && changed) {
            instructionList.style.transform = 'translate' + this.axis + '(0%)';
            instructionList.style.height = 'auto';
            documentList.style.pointerEvents = 'auto';
            this.instructionDisplayed = false;
        }


    }
}


customElements.define('list-instruction', ListInstruction);