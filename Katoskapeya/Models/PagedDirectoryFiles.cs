using System.Collections.Generic;
using System.IO;

namespace Kataskopeya.Models
{
    public class PagedDirectoryFiles
    {
        public IEnumerable<FileInfo> Files { get; set; }

        public bool IsNextPage { get; set; }
    }
}
