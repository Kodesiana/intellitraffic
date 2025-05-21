var map;
var markers = {};

const MarkerIcon = L.Icon.extend({
    options: {
        shadowUrl: '/icons/marker-shadow.png',
        iconSize: [25, 41],
        iconAnchor: [12, 41],
        popupAnchor: [1, -34],
        shadowSize: [41, 41]
    }
});

const densityColor = {
    Unknown: new MarkerIcon({iconUrl: '/icons/marker-icon-black.png'}),
    Light: new MarkerIcon({iconUrl: '/icons/marker-icon-green.png'}),
    Moderate: new MarkerIcon({iconUrl: '/icons/marker-icon-orange.png'}),
    Heavy: new MarkerIcon({iconUrl: '/icons/marker-icon-red.png'}),
}

export function initMap() {
    map = L.map('map', {
        minZoom: 10,
        maxZoom: 18
    }).setView([-6.5962986, 106.7972421], 15);

    L.control.scale().addTo(map);

    L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
    }).addTo(map);
}

export function setMarkers(markers) {
    for (const marker of markers) {
        const popupContents = `<b>${marker.givenName}</b><br>${marker.overlayName}<br>
<a class="button is-inverted mt-4" href="/stream/${marker.cameraId}">
    <span class="icon is-small has-text-danger">
        <i class="fa-solid fa-video"></i>
    </span>
    <span>Live Stream</span>
</a>
<a class="button is-link is-inverted mt-4" href="/history/${marker.cameraId}">
    <span class="icon is-small">
      <i class="fa-solid fa-clock-rotate-left"></i>
    </span>
    <span>View History</span>
</a>`;
        markers[marker.id] = L.marker([marker.latitude, marker.longitude], {icon: densityColor[marker.density]}).bindPopup(popupContents).addTo(map);

        // markers[marker.id] = L.circle([marker.latitude, marker.longitude], {
        //     color: densityColor[marker.density],
        //     fillColor: '#f03',
        //     fillOpacity: 0.3,
        //     radius: 50
        // }).bindPopup(popupContents).addTo(map);
    }
}

export function toggleTheme() {
    const currentTheme = document.documentElement.getAttribute("data-theme");
    const newTheme = currentTheme === "dark" ? "light" : "dark";
    document.documentElement.setAttribute("data-theme", newTheme);
}
