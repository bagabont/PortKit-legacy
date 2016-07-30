using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace Portkit.Extensions
{
    public static class StorageItemEx
    {
        /// <summary>
        /// Check if the <see cref="IStorageItem"/> item's size is equal to (or less than, if that is even possible) 0.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<bool> CheckIsEmptyAsync(this IStorageItem item)
        {
            var props = await item.GetBasicPropertiesAsync();
            if (props == null)
            {
                throw new IOException("Failed to retrieve basic item properties.");
            }
            return props.Size <= 0;
        }

        /// <summary>
        /// Wrap the <see cref="IOException"/> thrown if the item doesn't exist and return a <see cref="bool"/> result.
        /// </summary>
        /// <param name="folder">Source folder</param>
        /// <param name="fileName">Filename to check for.</param>
        /// <returns></returns>
        public static async Task<bool> ContainsFileAsync(this IStorageFolder folder, string fileName)
        {
            if (folder == null)
            {
                return false;
            }

            try
            {
                return await folder.GetFileAsync(fileName) != null;
            }
            catch (IOException)
            {
                return false;
            }
        }
    }
}
