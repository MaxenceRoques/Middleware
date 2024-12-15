class AutoCompleterComponent extends HTMLElement {
    constructor() {
        super();
        const shadow = this.attachShadow({ mode: 'open' });

        // Create the container for the search bar
        let context = this;
        fetch('components/autocompleter/autocompleter.html').then(async function (response) {
            let container = await response.text();

            let template = new DOMParser().parseFromString(container, 'text/html').querySelector('template').content;
            shadow.appendChild(template.cloneNode(true));

            context.init();
        });
    }

    init() {
        let context = this;
        this.callTimeout = 0;
        document.addEventListener('inputChange', function (e) {
            let inputContent = e.detail.address;
            if (inputContent != undefined) {
                inputContent.replaceAll(' ', '+');
            }
            else {
                context.shadowRoot.querySelector('.autocompleter').style.display = 'none';
            }
            if (context.callTimeout) clearTimeout(context.callTimeout);

            context.callTimeout = setTimeout(function () {
                if (inputContent === '') return;
                let url = `https://api-adresse.data.gouv.fr/search/?q=${inputContent}&limit=5`;
                if (e.detail.address === undefined) {
                    document.dispatchEvent(new CustomEvent('error', { detail: { message: 'Adresse introuvable' } }));
                    return;
                }
                else if (e.detail.address.length === '') {
                    context.shadowRoot.querySelector('.autocompleter').style.display = 'none';
                    return;
                }
                fetch(url).then(response => response.json().then(data => {
                    context.updateAutoCompleteList(data.features, e.detail.field === 'departure');
                }));
            }, 1000);
        });

    }

    updateAutoCompleteList(adresses, isDeparture) {
        let autocompleter = this.shadowRoot.querySelector('.autocompleter');
        let list = this.shadowRoot.querySelector('.list');
        list.innerHTML = '';
        if (adresses === undefined || adresses.length === 0) {
            document.dispatchEvent(new CustomEvent('error', { detail: { message: 'Adresse introuvable' } }));
            autocompleter.style.display = 'none';
            return;
        }
        if (isDeparture) {
            this.appendMyLocation(list);
        }
        autocompleter.style.display = 'block';
        adresses.forEach(adress => {
            let li = document.createElement('li');
            //add icon to li
            let icon = document.createElement('i');
            icon.classList.add('fa-solid');
            icon.classList.add('fa-magnifying-glass');
            li.appendChild(icon);
            //add adress to li

            let span = document.createElement('span');
            span.textContent = adress.properties.label;
            li.appendChild(span);
            li.addEventListener('click', function () {
                document.dispatchEvent(new CustomEvent('adressSelected', { detail: adress }));
                autocompleter.style.display = 'none';

            });
            list.appendChild(li);
        });
    }

    appendMyLocation(list) {
        let li = document.createElement('li');

        // Add icon to li
        let icon = document.createElement('i');
        icon.classList.add('fa-solid');
        icon.classList.add('fa-location');
        li.appendChild(icon);

        // Add address to li
        let span = document.createElement('span');
        span.textContent = 'Ma position';
        li.appendChild(span);

        li.addEventListener('click', async () => {
            try {

                const location = await this.askForLocation();
                document.dispatchEvent(new CustomEvent('adressSelected', {
                    detail: {
                        properties: {
                            label: `${location.latitude.toFixed(5)}, ${location.longitude.toFixed(5)}`
                        },
                        geometry: {
                            coordinates: [location.longitude, location.latitude]
                        }
                    }
                }));

                this.shadowRoot.querySelector('.autocompleter').style.display = 'none';


            } catch (error) {
                console.error('Erreur lors de la récupération de la position :', error);
            }
        });

        list.appendChild(li);
    }

    askForLocation() {
        return new Promise((resolve, reject) => {
            if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(
                    function (position) {
                        const { latitude, longitude } = position.coords;
                        resolve({ latitude, longitude });
                    },
                    function (error) {
                        switch (error.code) {
                            case error.PERMISSION_DENIED:
                                reject('L\'utilisateur a refusé la demande de localisation.');
                                break;
                            case error.POSITION_UNAVAILABLE:
                                reject('La position n\'est pas disponible.');
                                break;
                            case error.TIMEOUT:
                                reject('La demande a expiré.');
                                break;
                            default:
                                reject('Une erreur inconnue est survenue.');
                                break;
                        }
                    },
                    {
                        enableHighAccuracy: true,
                        timeout: 5000,
                        maximumAge: 0
                    }
                );
            } else {
                reject('La géolocalisation n\'est pas supportée par ce navigateur.');
            }
        });
    }
}

customElements.define('autocompleter-input', AutoCompleterComponent);
