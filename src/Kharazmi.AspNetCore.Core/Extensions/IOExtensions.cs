using System.IO;

namespace Kharazmi.AspNetCore.Core.Extensions
{
    // ReSharper disable once InconsistentNaming
    public static partial class Core
    {
        public static bool IsFileLocked(this FileInfo file)
        {
            if (file == null)
                return false;

            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                stream?.Close();
            }
            
            return false;
        }
    }
}