namespace Kharazmi.BackgroundJob
{
    /// <summary>
    /// JobPriority
    /// </summary>
    public enum JobPriority : byte
    {
        /// <summary> </summary>
        Low = 5,
        /// <summary> </summary>
        BelowNormal = 10,
        /// <summary> </summary>
        Normal = 15,
        /// <summary> </summary>
        AboveNormal = 20,
        /// <summary> </summary>
        High = 25
    }
}