using System;

 namespace Kharazmi.AspNetCore.Localization
{
    public class LocalizationEntity
    {
        public LocalizationEntity()
        {
            Id = Guid.NewGuid().ToString("N");
        }
        
        public string Id { get; set; }

        /// <summary>
        /// Localization key, Unique Key of the string to be localized, like Administration.User.Fields.UserName
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Localized value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Key of the culture, like "fa" or "fa-IR"
        /// </summary>
        public string CultureName { get; set; }

        /// <summary>
        /// Localization resource name, like SharedResource,LabelResource,MessageResource,...
        /// </summary>
        public string Resource { get; set; }

        public override string ToString()
        {
            return $"Key: {Key}, Value: {Value}, CultureName: {CultureName}";
        }
    }
}