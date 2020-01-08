using System;

namespace SapphireDb_Net.Helper
{
    public static class CollectionNameHelper
    {
        public static Tuple<string, string> ParseCollectionName(this string collectionNameRaw)
        {
            string[] collectionNameParts = collectionNameRaw.Split('.');

            string collectionName = collectionNameParts.Length == 1 ? collectionNameParts[0] : collectionNameParts[1];
            string contextName = collectionNameParts.Length == 2 ? collectionNameParts[0] : "default";
            
            return new Tuple<string, string>(collectionName, contextName);
        }
    }
}