using System;
 using System.Collections.Generic;

 namespace Kharazmi.AspNetCore.Localization
{
    public interface ILocalizerService
    {
        /// <summary>
        /// Insert the specified resourceKey, value, cultureName and resourceName.
        /// </summary>
        /// <param name="resourceKey">Specified Key in  resource type</param>
        /// <param name="value">Value</param>
        /// <param name="cultureName">Current cultureName</param>
        /// <param name="resourceSource">Type of resource</param>
        void Insert(string resourceKey, string value, string cultureName, Type resourceSource);

        /// <summary>
        /// Update the value by specified resourceKey, cultureName and resourceName.
        /// </summary>
        /// <param name="resourceKey">Specified Key in  resource type</param>
        /// <param name="value">Value</param>
        /// <param name="cultureName">Current cultureName</param>
        /// <param name="resourceSource">Type of resource</param>
        void Update(string resourceKey, string value, string cultureName, Type resourceSource);

        /// <summary>
        /// Delete by specified resourceKey, cultureName and resourceName.
        /// </summary>
        /// <param name="resourceKey">Specified Key in  resource type</param>
        /// <param name="cultureName">Current cultureName</param>
        /// <param name="resourceSource">Type of resource</param>
        void Delete(string resourceKey, string cultureName, Type resourceSource);

        /// <summary>
        /// Insert renge of keyValue, cultureName and resourceName.
        /// </summary>
        /// <param name="keyValue">Specified Keys in  resource type</param>
        /// <param name="cultureName">Current cultureName</param>
        /// <param name="resourceSource">Type of resource</param>
        void Insert(IEnumerable<KeyValuePair<string, string>> keyValue, string cultureName, Type resourceSource);

        /// <summary>
        /// Update renge of specified keyValue, cultureName and resourceName.
        /// </summary>
        /// <param name="keyValue">Specified Keys in  resource type</param>
        /// <param name="cultureName">Current cultureName</param>
        /// <param name="resourceSource">Type of resource</param>
        void Update(IEnumerable<KeyValuePair<string, string>> keyValue, string cultureName, Type resourceSource);

        /// <summary>
        /// Delete renge of specified names, cultureName and resourceName.
        /// </summary>
        /// <param name="keyValue">Specified Keys in  resource type</param>
        /// <param name="cultureName">Current cultureName</param>
        /// <param name="resourceSource">Type of resource</param>
        void Delete(IEnumerable<string> keyValue, string cultureName, Type resourceSource);

        /// <summary>
        /// Exports the xml.
        /// </summary>
        /// <returns>The xml.</returns>
        /// <param name="cultureName">Current cultureName</param>
        /// <param name="resourceSource">Type of resource</param>
        string ExportXml(string cultureName, Type resourceSource);

        /// <summary>
        /// Exports the json.
        /// </summary>
        /// <returns>The json.</returns>
        /// <param name="cultureName">Current cultureName</param>
        /// <param name="resourceSource">Type of resource</param>
        string ExportJson(string cultureName, Type resourceSource);
    }
}
