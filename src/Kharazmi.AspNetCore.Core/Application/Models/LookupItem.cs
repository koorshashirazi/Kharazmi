namespace Kharazmi.AspNetCore.Core.Application.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class LookupItem : LookupItem<string>
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class LookupItem<TValue>
    {
        /// <summary>
        /// 
        /// </summary>
        public TValue Value { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool Selected { get; set; }
    }
}