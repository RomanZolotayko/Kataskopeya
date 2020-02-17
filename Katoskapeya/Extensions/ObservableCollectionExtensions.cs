using Kataskopeya.Common.Enums;
using Kataskopeya.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Kataskopeya.Extensions
{
    public static class ObservableCollectionExtensions
    {
        public static ObservableCollection<T> SortObservableCollection<T, X>(this ObservableCollection<T> collection, OrderingTypes type, Func<T, X> predicate)
        {
            var orderedCollection = new List<T>();

            switch (type)
            {
                case OrderingTypes.Ascending:
                    orderedCollection = collection.OrderBy(predicate).ToList();
                    break;

                case OrderingTypes.Descending:
                    orderedCollection = collection.OrderByDescending(predicate).ToList();
                    break;
            }

            var result = new ObservableCollection<T>();

            foreach (var item in orderedCollection)
            {
                result.Add(item);
            }

            return result;
        }

        public static ObservableCollection<T> FilterObservableCollection<T>(this ObservableCollection<T> collection, string searchQuery) where T : ArchiveVideo
        {
            var orderedCollection = collection.Where(x => x.Name.Contains(searchQuery));

            var result = new ObservableCollection<T>();

            foreach (var item in orderedCollection)
            {
                result.Add(item);
            }

            return result;
        }
    }
}
