using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace WinUI3Controls
{
    public class UserMapPrivacyPreferences
    {
        /// <summary>
        /// Is only used to determine if we should tell the user they have to set a privacy setting for the first time.
        /// Is never used to determine any actual privacy setting.
        /// </summary>
        public bool UserHasPickedPrivacySettings { get; set; } = false;

        /// <summary>
        /// When true, all third party services are blocked. Defaults to true so that users are protected by default.
        /// </summary>
        public bool DisableAll3rdPartyServices { get; set; } = true;

        public bool AllowOpenStreetMapUnderlyingValue { get; set; } = true;

        [JsonIgnore] // value is calculated from DisableAll3rdPartyServices and AllowOpenStreetMapUnderlyingValue
        public bool AllowOpenStreetMap
        {
            get
            {
                if (DisableAll3rdPartyServices) return false;
                return AllowOpenStreetMapUnderlyingValue;
            }
        }

        /// <summary>
        /// Returns true IFF it would be true but all third party services are blocked
        /// </summary>
        [JsonIgnore] // Value is calculated from DisableAll3rdPartyServices and AllowOpenStreetMapUnderlyingValue
        public bool AllowOpenStreetMapIsBlocked
        {
            get
            {
                var retval = AllowOpenStreetMapUnderlyingValue && DisableAll3rdPartyServices;
                return retval;
            }
        }
    }
}
