using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace Portkit.Extensions
{
    public static class StorageItemEx
    {
        public static async Task<bool> CheckIsEmpty(this IStorageItem item)
        {
            var props = await item.GetBasicPropertiesAsync();
            return props.Size <= 0;
        }

        public static async Task<bool> ContainsFileAsync(this IStorageFolder folder, string fileName)
        {
            try
            {
                IStorageFile file = await folder.GetFileAsync(fileName);
                return file != null;
            }
            catch (IOException)
            {
                return false;
            }
        }
    }
}
