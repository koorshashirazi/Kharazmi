﻿using Kharazmi.AspNetCore.Core.Enumerations;
using Newtonsoft.Json;

namespace Kharazmi.AspNetCore.Core.XUnitTest.EnumConverter
{
    public class ExpirationTypeJsonConverter
    {
        [JsonConverter(typeof(EnumerationNameConverter<ExpirationType, int>))]
        public ExpirationType ExpirationType { get; set; }
    }
}