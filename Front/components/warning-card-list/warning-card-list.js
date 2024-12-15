//This component was not finished. He was designed to display warning related to the itinerary
class WarningCardListComponent extends HTMLElement {
    constructor() {
        super();
        const shadow = this.attachShadow({ mode: 'open' });

        let context = this;
        fetch('components/warning-card-list/warning-card-list.html').then(async function (response) {
            let container = await response.text();

            let template = new DOMParser().parseFromString(container, 'text/html').querySelector('template').content;
            shadow.appendChild(template.cloneNode(true));
        });

    }
}

customElements.define('warning-card-list', WarningCardListComponent);
