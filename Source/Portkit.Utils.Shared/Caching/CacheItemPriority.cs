namespace Portkit.Utils.Caching
{
    /// <summary>
    /// Represents item storage priority.
    /// </summary>
    public enum CacheItemPriority
    {
        /// <summary>
        /// Normal storage priority.
        /// </summary>
        Normal,

        /// <summary>
        /// High storage priority. Items must be cleared exclusively, otherwise they will not expire.
        /// </summary>
        High
    }
}