import { dispatchTime, getCorrespondingColor, } from "./utils.js";

var locations = [null, null];
var currentSection = 0;
var currentRouteLayer = [];
var marker = null;

var markerDestination = L.icon({
    iconUrl: 'res/img/location-dot-solid.svg',
    iconSize: [25, 25],
    iconAnchor: [12, 25],
    popupAnchor: [0, -20]
});

var markerDeparture = L.icon({
    iconUrl: 'res/img/circle-dot-solid.svg',
    iconSize: [18, 18],
    iconAnchor: [9, 18],
    popupAnchor: [0, -15]
});


function clearLayers() {
    currentRouteLayer.forEach(layer => {
        map.removeLayer(layer);
    });
    currentRouteLayer = [];
}

function drawRoute(startLat, startLng, endLat, endLng) {
    clearLayers();
    const routeUrl = `http://localhost:8733/Design_Time_Addresses/RoutingServer/RoutingService/GetItinerary?startLat=${startLat}&startLon=${startLng}&endLat=${endLat}&endLon=${endLng}`;
    displayLoading(true);
    fetch(routeUrl, {
        method: 'GET',
        headers: {
            "Accept": "application/json",
        },

    })
    .then(response => {
        displayLoading(false);
        if (!response.ok) {
            throw { status: response.status, message: response.statusText };
        }
        return response.json(); 
    })
    .then(data => {
        const route = data.GetItineraryResult;
        displayLoading(false);
        let totalTime = 0;
        if (route && route.length > 0) {
            route.forEach(itinerary => {
            totalTime += itinerary.total_duration;
            const { geometry, instructions } = itinerary;
            const routeLayer = L.geoJSON(geometry,
                {
                    style: {
                        color: getCorrespondingColor(itinerary.profile),
                    }
                });
            currentRouteLayer.push(routeLayer);

            routeLayer.addTo(map);
            });
            dispatchTime(totalTime);
        } else {
            document.dispatchEvent(new CustomEvent('error', { detail: { message: 'Aucun itinéraire trouvé' } }));
        }})
        .catch(error => {
            displayLoading(false);
            if (error.status === 404) {
                document.dispatchEvent(new CustomEvent('error', { detail: { message: 'Aucun itinéraire trouvé' } }));
            }
            else {

                document.dispatchEvent(new CustomEvent('error', {
                    detail: {
                        message: 'Service non disponible, veuillez réessayer plus tard'
                    }
                }));
            }

        });
}

var map = L.map('map').setView([43.5804, 7.1251], 13);

L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">StrideMap</a> contributors'
}).addTo(map);

var zoomControl = map.zoomControl;
zoomControl.setPosition('bottomright');


document.addEventListener('currentSection', function (e) {
    currentSection = e.detail.section;
    marker = currentSection === 1 ? markerDeparture : markerDestination;
});

document.addEventListener('adressSelected', function (e) {

    if (e.detail.geometry === undefined) return;
    const coordinates = e.detail.geometry.coordinates;
    const lng = coordinates[0];
    const lat = coordinates[1];
    if (locations[currentSection - 1] !== null) {
        map.removeLayer(locations[currentSection - 1]);
    }

    locations[currentSection - 1] = L.marker([lat, lng], { icon: marker }).addTo(map)
        .bindPopup(e.detail.properties.label)
        .openPopup();

    if (locations[0] !== null && locations[1] !== null) {
        drawRoute(locations[0].getLatLng().lat, locations[0].getLatLng().lng, locations[1].getLatLng().lat, locations[1].getLatLng().lng);
    }
    map.setView([lat, lng], map.getZoom());
});


map.on('click', function (e) {
    if (currentSection === 0) return;

    if (locations[currentSection - 1] !== null) {
        map.removeLayer(locations[currentSection - 1]);
    }
    const { lat, lng } = e.latlng;

    locations[currentSection - 1] = L.marker([lat, lng], { icon: marker }).addTo(map)
        .bindPopup(`Latitude: ${lat.toFixed(5)}, Longitude: ${lng.toFixed(5)}`)
        .openPopup();
    addressSelected();

    if (locations[0] !== null && locations[1] !== null) {
        drawRoute(locations[0].getLatLng().lat, locations[0].getLatLng().lng, locations[1].getLatLng().lat, locations[1].getLatLng().lng);
    }
    map.setView([lat, lng], map.getZoom());
});

function addressSelected() {
    document.dispatchEvent(new CustomEvent('adressSelected', {
        detail: {
            properties: {
                label: `${locations[currentSection - 1].getLatLng().lat.toFixed(5)}, ${locations[currentSection - 1].getLatLng().lng.toFixed(5)}`
            }
        }
    }));
}

function displayLoading(display) {
    const loading = document.querySelector('.loader');
    const map = document.querySelector('#map');
    if (display) {
        loading.style.display = 'block';
        map.style.filter = 'blur(5px)';
    } else {
        loading.style.display = 'none';
        map.style.filter = 'none';

    }
}