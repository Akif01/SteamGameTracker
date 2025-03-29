namespace SteamGameTracker.DataTransferObjects
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    namespace SteamApi.Models
    {
        public class AppDetailsDTO : Dictionary<int, SuccessDTO>
        {
        }
    }
}
