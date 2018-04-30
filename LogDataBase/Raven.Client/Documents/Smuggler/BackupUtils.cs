using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Raven.Client.Documents.Smuggler
{
    public static class BackupUtils
    {
        public static IOrderedEnumerable<string> OrderBackups(this IEnumerable<string> data)
        {
            return data
                .OrderBy(Path.GetFileNameWithoutExtension)
                .ThenBy(Path.GetExtension, PeriodicBackupFileExtensionComparer.Instance)
                .ThenBy(File.GetLastWriteTimeUtc);
        }
    }
}
