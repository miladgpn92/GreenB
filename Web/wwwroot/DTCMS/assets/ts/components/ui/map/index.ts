import * as L from "leaflet";

interface latlng {
    lat: number;
    lng: number;
};

(() => {
    const mapContainer: NodeListOf<HTMLElement> = document.querySelectorAll(`.map`);

    let myIcon = L.icon({
        iconUrl: "../../DTCMS/assets/images/icon/marker.svg",
        tooltipAnchor: [200, -5]

    });
    if (mapContainer) {
        Array.from(mapContainer).forEach((item: HTMLElement) => {
            const layer = L.tileLayer(
                "https://{s}.tile.openstreetmap.fr/hot/{z}/{x}/{y}.png",
                {
                    maxZoom: 20,
                }
            );
            let multiPin = item.getAttribute("data-multi-pin")
            if (multiPin && multiPin === "true") {
                let hiddenInputClass = item.getAttribute("data-lat-long")
                let hiddenInput: HTMLInputElement = document.querySelector(`.${hiddenInputClass}`);
                let markerList: latlng[] = hiddenInput.value ? JSON.parse(hiddenInput.value) : []
                const map = L.map(item, {
                    center: [markerList ? markerList[0].lat : 36.470352, markerList ? markerList[0].lng : 52.346796],
                    zoom: 20,
                });
                layer.addTo(map);
                markerList.forEach((location: latlng) => {
                    const marker = L.marker([location.lat, location.lng], {
                        icon: myIcon,
                        draggable: true,
                    });
                    marker.addTo(map);
                    let tooltip = L.tooltip({
                        direction: "top"
                    })
                        .setLatLng(marker.getLatLng())
                        .setContent('برای حذف پین دو بار روی پین کلیک کنید')
                    marker.bindTooltip(tooltip)

                    marker.on("dblclick", (ev) => {
                        markerList = markerList.filter(item => item.lat !== marker.getLatLng().lat || item.lng !== marker.getLatLng().lng)
                        map.removeLayer(marker);
                        hiddenInput.value = JSON.stringify(markerList)
                        map.removeLayer(marker)

                    })

                    marker.on("dragend", (e) => {
                        markerList = []
                        map.eachLayer(function (layer) {
                            if (layer instanceof L.Marker) {
                                markerList = [...markerList, { lat: layer.getLatLng().lat, lng: layer.getLatLng().lng }]
                            }
                        });
                        hiddenInput.value = JSON.stringify(markerList)
                    });
                });
                map.on("click", (e) => {
                    markerList = [...markerList, { lat: e.latlng.lat, lng: e.latlng.lng }]
                    hiddenInput.value = JSON.stringify(markerList)
                    const Newmarker = L.marker(e.latlng, {
                        icon: myIcon,
                        draggable: true,

                    });
                    Newmarker.addTo(map);


                    let tooltip = L.tooltip({
                        direction: "top"
                    })
                        .setLatLng(e.latlng)
                        .setContent('برای حذف پین دو بار روی پین کلیک کنید')
                    Newmarker.bindTooltip(tooltip)

                    Newmarker.on("dblclick", (ev) => {
                        markerList = markerList.filter(item => item.lat !== Newmarker.getLatLng().lat || item.lng !== Newmarker.getLatLng().lng)
                        map.removeLayer(Newmarker);
                        hiddenInput.value = JSON.stringify(markerList)
                        map.removeLayer(Newmarker)

                    })

                    Newmarker.on("dragend", (e) => {
                        markerList = []
                        map.eachLayer(function (layer) {
                            if (layer instanceof L.Marker) {
                                markerList = [...markerList, { lat: layer.getLatLng().lat, lng: layer.getLatLng().lng }]
                            }
                        });
                        hiddenInput.value = JSON.stringify(markerList)
                    });
                });

            } else {
                let hiddenInputLatClass = item.getAttribute("data-lat")
                let hiddenInputLngClass = item.getAttribute("data-long")
                let hiddenInputLat: HTMLInputElement = document.querySelector(`.${hiddenInputLatClass}`);
                let hiddenInputLng: HTMLInputElement = document.querySelector(`.${hiddenInputLngClass}`);

                const map = L.map(item, {
                    center: [hiddenInputLat.value ? parseFloat(hiddenInputLat.value) : 36.470352, hiddenInputLng.value ? parseFloat(hiddenInputLng.value) : 52.346796],
                    zoom: 20,

                });

                if (hiddenInputLat.value && hiddenInputLng.value) {
                    const marker = L.marker([parseFloat(hiddenInputLat.value), parseFloat(hiddenInputLng.value)], {
                        icon: myIcon,
                        draggable: true,
                    });
                    marker.addTo(map);
                    marker.on("dragend", (e) => {
                        hiddenInputLat.value = e.target.getLatLng().lat
                        hiddenInputLng.value = e.target.getLatLng().lng
                    });
                }
                layer.addTo(map);
                map.on("click", (e) => {
                    hiddenInputLat.value = e.latlng.lat
                    hiddenInputLng.value = e.latlng.lng
                    map.eachLayer(function (layer) {
                        if (layer instanceof L.Marker) {
                            map.removeLayer(layer);
                        }
                    });
                    const Newmarker = L.marker(e.latlng, {
                        icon: myIcon,
                        draggable: true,
                    });
                    Newmarker.addTo(map);
                    Newmarker.on("dragend", (eve) => {
                        hiddenInputLat.value = eve.target.getLatLng().lat
                        hiddenInputLng.value = eve.target.getLatLng().lng
                    });
                });
            }
        })
    }


}
)()