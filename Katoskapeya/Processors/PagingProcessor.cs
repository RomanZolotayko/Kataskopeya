using Kataskopeya.Common.Enums;
using Kataskopeya.Models;
using System.IO;
using System.Linq;

namespace Kataskopeya.Processors
{
    public class PagingProcessor
    {
        public PagedDirectoryFiles GetPagedDirectoryFiles(DirectoryInfo filepath, int pageNumber, int pagePayload, PagingOperations operation, string fileExtension)
        {
            var isNextPageExist = false;

            switch (operation)
            {
                case PagingOperations.Next:
                    pageNumber++;
                    break;

                case PagingOperations.Previous:
                    pageNumber--;
                    break;
            }

            var files = filepath.GetFiles(fileExtension)
           .Where(x => x.Length != 0)
           .Skip(pageNumber == 0 ? 0 : pageNumber * pagePayload)
           .Take(pagePayload);

            isNextPageExist = files.Count() >= 18 ? true : false;

            return new PagedDirectoryFiles
            {
                Files = files,
                IsNextPage = isNextPageExist
            };
        }
    }
}
