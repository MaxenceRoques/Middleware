class SearchHelpComponent extends HTMLElement {
    constructor() {
        super();
        const shadow = this.attachShadow({ mode: 'open' });

        // Create the container for the search bar
        let context = this;
        fetch('components/search-help/search-help.html').then(async function (response) {
            let container = await response.text();

            let template = new DOMParser().parseFromString(container, 'text/html').querySelector('template').content;
            shadow.appendChild(template.cloneNode(true));

            context.init();
        });
    }
   
    init() {
        let context = this;

        let searchHelp = this.shadowRoot.querySelector('.search-help');

        document.addEventListener('click', function (event) {
            let SearchHelpComponent = document.querySelector('search-help');
            let searchContent = context.shadowRoot.querySelector('.help-content');
            if (event.target !== SearchHelpComponent && event.target !== searchContent) {
                searchContent.style.animation = 'fadeOut 0.5s';
                setTimeout(() => {
                    searchContent.style.visibility = 'hidden';
                }, 500);
            }
        });

        searchHelp.addEventListener('click', function () {
            let searchContent = context.shadowRoot.querySelector('.help-content');

            if (searchContent.style.visibility === 'hidden' || searchContent.style.visibility === '') {
                searchContent.style.animation = 'fadeIn 0.5s';
                searchContent.style.visibility = 'visible';
            }
            else {
                searchContent.style.animation = 'fadeOut 0.5s';
                setTimeout(() => {
                    searchContent.style.visibility = 'hidden';
                }, 500);    
            }
        });
    }

}

customElements.define('search-help', SearchHelpComponent);
