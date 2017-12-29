using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yle
{
    /// <summary>
    /// Yle api default configuration
    /// </summary>
    static class YleConfiguration
    {
        // Define URL
        public const string URL_YLE_PROGRAMS = "https://external.api.yle.fi/v1/programs/items.json";
        public const string URL_YLE_PLAYOUTS = "https://external.api.yle.fi/v1/media/playouts.json";
        public const string URL_CDN_IMAGE = "http://images.cdn.yle.fi/image/upload/";

        // Default image parameter
        public const int DEFAULT_THUMBNAIL_WIDTH = 250;
        public const int DEFAULT_THUMBNAIL_HEIGHT = 140;
        public const string DEFAULT_THUMBNAIL_CROP = "c_fit";   
        public const string DEFAULT_THUMBNAIL_FORMAT = "jpg";

        // Default Stream parameter
        public const string DEFAULT_STREAM_PROTOCOL = "HLS";

        // Default Search Parameter
        public const YleAvailability DEFAULT_SEARCH_AVAILABILITY = YleAvailability.OnDemand;
    }


}