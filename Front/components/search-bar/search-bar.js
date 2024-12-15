class SearchBarComponent extends HTMLElement {
    constructor() {
        super();
        const shadow = this.attachShadow({ mode: 'open' });

        // Create the container for the search bar
        let context = this;
        fetch('components/search-bar/search-bar.html').then(async function (response) {
            let container = await response.text();

            let template = new DOMParser().parseFromString(container, 'text/html').querySelector('template').content;
            shadow.appendChild(template.cloneNode(true));

            context.init();
        });
    }

    init() {
        let context = this;
        let currentSection = null;
        let searchBarDeparture = this.shadowRoot.querySelector('.departure');
        let searchBarDestination = this.shadowRoot.querySelector('.destination');
        let searchBar = context.shadowRoot.querySelector('.search-bar');
        let removeDestination = this.shadowRoot.querySelector('.destination > .fa-xmark'); 
        let removeDeparture = this.shadowRoot.querySelector('.departure > .fa-xmark');
        let searchBarDestinationInput = context.shadowRoot.querySelector('.destination > input');
        let searchBarDepartureInput = context.shadowRoot.querySelector('.departure > input');

        searchBarDeparture.addEventListener('input', () => {
            if (searchBarDepartureInput.value === '') {
                context.resetDisplay(searchBarDestination, searchBar);
                searchBarDeparture.style.borderBottom = '1px solid #00000050';
                context.handleInputChange(searchBarDepartureInput, 'departure');
                return;
            }
            searchBarDestination.style.display = 'none';
            searchBarDeparture.style.borderBottom = '0';
            searchBar.style.height = '3rem';
            context.handleInputChange(searchBarDepartureInput, 'departure');
        });

        searchBarDepartureInput.addEventListener('keydown', (e) => {
            if (e.key === 'Enter') {
                context.resetDisplay(searchBarDestination, searchBar);
                searchBarDeparture.style.borderBottom = '1px solid #00000050';
                context.sendUndefinedInput();
            }
        });

        searchBarDepartureInput.addEventListener('focus', () => {
            currentSection = searchBarDeparture;
            context.shareCurrentSection(currentSection);
        });
        

        searchBarDestination.addEventListener('input', () => {
            if (searchBarDestinationInput.value === '') {
                context.resetDisplay(searchBarDeparture, searchBar);
                context.handleInputChange(searchBarDepartureInput, 'departure');
                return;
            }   
            searchBarDeparture.style.display = 'none';
            searchBarDestination.style.borderBottom = '0';
            searchBar.style.height = '3rem';
            context.handleInputChange(searchBarDestinationInput, 'destination');
        });

        searchBarDestinationInput.addEventListener('keydown', (e) => {
            if (e.key === 'Enter') {
                context.resetDisplay(searchBarDeparture, searchBar);
                context.sendUndefinedInput();
            }
        }); 

        searchBarDestinationInput.addEventListener('focus', () => {
            currentSection = searchBarDestination;
            context.shareCurrentSection(currentSection);
        });
        
        removeDestination.addEventListener('click', () => {
            searchBarDestinationInput.value = '';
            context.resetDisplay(searchBarDeparture, searchBar);
            this.sendEmptyInput();
        });

        removeDeparture.addEventListener('click', () => {
            searchBarDepartureInput.value = '';
            context.resetDisplay(searchBarDestination, searchBar);
            searchBarDeparture.style.borderBottom = '1px solid #00000050';
        });

        document.addEventListener('adressSelected', function (e) {
            let adress = e.detail;
            switch (currentSection) {
                case searchBarDeparture:
                    searchBarDepartureInput.value = adress.properties.label;
                    searchBarDeparture.style.borderBottom = '1px solid #00000050';
                    context.resetDisplay(searchBarDestination, searchBar);
                    break;
                case searchBarDestination:
                    searchBarDestinationInput.value = adress.properties.label;
                    context.resetDisplay(searchBarDeparture, searchBar);
                    break;
            }
        }); 



    }
    
    resetDisplay(searchBarItem, searchBar) {
        searchBar.style.height = '5rem';
        searchBarItem.style.display = 'grid';
        searchBarItem.style.gridTemplateColumns = 'minmax(30px, 1fr) 20fr minmax(30px, 1fr)';
        searchBarItem.style.alignItems = 'center';
        searchBarItem.style.width = '100%';
        searchBarItem.style.height = '50%';
    }

    

    handleInputChange(input, field) {
        document.dispatchEvent(new CustomEvent('inputChange', {
            detail: {
                address: input.value,
                field: field
            }
        }));
    }

    sendEmptyInput() {
        document.dispatchEvent(new CustomEvent('inputChange', {
            detail: {
                address: ''
            }
        }));
    }

    sendUndefinedInput() {
        document.dispatchEvent(new CustomEvent('inputChange', {
            detail: {
                address: undefined
            }
        }));
    }

    shareCurrentSection(section) {
        switch(section) {
            case null:
                section = 0;
                break;
            case this.shadowRoot.querySelector('.departure'):
                section = 1;
                break;
            case this.shadowRoot.querySelector('.destination'):
                section = 2;
                break;
        }
        document.dispatchEvent(new CustomEvent('currentSection', {
            detail: {
                section: section,
            }
        }));
    }
}

customElements.define('search-bar', SearchBarComponent);
