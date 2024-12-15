class ErrorMessageComponent extends HTMLElement {
    constructor() {
        super();
        const shadow = this.attachShadow({ mode: 'open' });

        // Create the container for the search bar
        let context = this;
        fetch('components/error-message/error-message.html').then(async function (response) {
            let container = await response.text();

            let template = new DOMParser().parseFromString(container, 'text/html').querySelector('template').content;
            shadow.appendChild(template.cloneNode(true));

            context.init();
        });
    }

    init() {
        let context = this;

        //reset the error message when the input change
        document.addEventListener('inputChange', function () {
            context.shadowRoot.querySelector('div').style.display = 'none';
        });

        document.addEventListener('adressSelected', function () {
            context.shadowRoot.querySelector('div').style.display = 'none';
        });

        //display the error message when an error is catched
        document.addEventListener('error', function (e) {
            context.shadowRoot.querySelector('p').textContent = e.detail.message;
            context.shadowRoot.querySelector('div').style.display = 'block';
            context.setUpStyle();
        });
    }

    setUpStyle() {
        let div = this.shadowRoot.querySelector('div');
        div.style.padding = '0 5%';
        div.style.display = 'flex';
        div.style.justifyContent = 'start';
        div.style.gap = '3%';
        div.style.alignItems = 'center';
        div.style.width = '90%';
        div.style.height = '2.5rem';
        div.style.fontSize = '1rem';
        div.style.borderRadius = '10px';
        div.style.overflow = 'hidden';
        div.style.backgroundColor = '#FF7272';
        div.style.border = '1px solid #ff16165f';
        div.style.boxShadow = '0 10px 20px rgba(0, 0, 0, 0.35)';
    }

}

customElements.define('error-message', ErrorMessageComponent);
