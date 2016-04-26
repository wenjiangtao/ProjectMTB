using UnityEngine;
using System.Collections;

namespace MTB
{
    public class SearchComponentFactory
    {
        public static ISearchComponent createSearchComponent(SearchComponentType type, GameObject host)
        {
            if (type == SearchComponentType.Player_Search)
                return new SearchPlayerComponent(host) as ISearchComponent;
            if (type == SearchComponentType.Monster_Search)
                return new SearchMonsterComponent(host) as ISearchComponent;
            if (type == SearchComponentType.List_Search)
                return new SearchListComponent(host) as ISearchComponent;
            return null;
        }
    }
}
