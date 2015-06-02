/**
 * Created by comdiv on 15.11.2014.
 */
define([
    "leaflet-amd",
    "wellknown",
    "leaflet-awesome-amd",
    "leaflet-draw-amd",
    "leaflet-markercluster-amd",
    "totalleaflet-angular"

],function($l,$wk){
    //preserving license info
    $l.__licenseInfo = "Leaflet, a JavaScript library for mobile-friendly interactive maps. http://leafletjs.com\
    (c) 2010-2013, Vladimir Agafonkin\
    (c) 2010-2011, CloudMade\
    \
    Leaflet.AwesomeMarkers, a plugin that adds colorful iconic markers for Leaflet, based on the Font Awesome icons\
    (c) 2012-2013, Lennard Voogdt\
    http://leafletjs.com\
    https://github.com/lvoogdt\
    \
    Leaflet.draw, a plugin that adds drawing and editing tools to Leaflet powered maps.\
    (c) 2012-2013, Jacob Toye, Smartrak\
    https://github.com/Leaflet/Leaflet.draw\
    http://leafletjs.com\
    https://github.com/jacobtoye\
    \
    Leaflet.markercluster, Provides Beautiful Animated Marker Clustering functionality for Leaflet, a JS library for interactive maps.\
    https://github.com/Leaflet/Leaflet.markercluster\
    (c) 2012-20\
    13, Dave Leaver, smartrak";
    L.AwesomeMarkers.Icon.prototype.options.prefix = 'fa';
    L.$wk = $wk;
    return window.L;
});