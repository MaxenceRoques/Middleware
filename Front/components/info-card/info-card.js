class InfoCardComponent extends HTMLElement {
    constructor() {
        super();
        const shadow = this.attachShadow({ mode: 'open' });
  

        this.icon = this.getAttribute('icon') || 'fa-info';
        this.description = this.getAttribute('description') || 'Default description';
        this.backgroundColor = this.getAttribute('background-color') || '#ffffff';
        this.colorIcon = this.getAttribute('color-icon') || '#000000'; 
        this.boxShadow = this.getAttribute('box-shadow') || '0 10px 20px rgba(0, 0, 0, 0.35)';
        this.borderRadius = this.getAttribute('border-radius') || '10px';

        let context = this;
        fetch('components/info-card/info-card.html').then(async function (response) {
            let container = await response.text();

            let template = new DOMParser().parseFromString(container, 'text/html').querySelector('template').content;
            shadow.appendChild(template.cloneNode(true));
            context.init();
        });
    }

    init() {
        this.updateElement();
    }

    static get observedAttributes() {
        return ['icon', 'description', 'background-color', 'color-icon', 'box-shadow', 'border-radius'];
    }

    attributeChangedCallback(name, oldValue, newValue) {
        if (oldValue !== newValue) {
            this.updateElement();
        }
    }

    updateElement() {
        const icon = this.getAttribute('icon')  || this.icon;
        const description = this.getAttribute('description') || this.description;
        const backgroundColor = this.getAttribute('background-color') || this.backgroundColor;
        const colorIcon = this.getAttribute('color-icon')  || this.colorIcon;
        const boxShadow = this.getAttribute('box-shadow') || this.boxShadow;
        const borderRadius = this.getAttribute('border-radius') || this.borderRadius;


        const iconElement = this.shadowRoot.querySelector('i');
        const descriptionElement = this.shadowRoot.querySelector('p');
        const container = this.shadowRoot.querySelector('div');

        if (iconElement) {
            iconElement.className = ''; 
            iconElement.classList.add('fa-solid');
            iconElement.classList.add(icon); 
            iconElement.style.color = colorIcon || '';
        }

        if (descriptionElement) {
            descriptionElement.textContent = description || '';
        }

        if (container) {
            container.style.backgroundColor = backgroundColor || '';
            container.style.boxShadow = boxShadow || '';
            container.style.borderRadius = borderRadius || '';
        }
    }
}

customElements.define('info-card', InfoCardComponent);