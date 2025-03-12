using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;

namespace Kharazmi.AspNetCore.Web.Localizations
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISharedLocalizationService
    {
        /// <summary>
        /// 
        /// </summary>
        IHtmlLocalizer HtmlLocalizer { get; }
        /// <summary>
        /// 
        /// </summary>
        IStringLocalizer Localizer { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        string this[string key] { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        LocalizedString GetValue(string key);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        LocalizedString GetValueTitle(string key);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        string GetValueFormatter(string key, params object[] parameter);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        LocalizedHtmlString GetValue(string key, params object[] parameter);
    }
}