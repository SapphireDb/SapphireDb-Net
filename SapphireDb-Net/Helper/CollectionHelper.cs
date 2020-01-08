using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using SapphireDb_Net.Command.Info;
using SapphireDb_Net.Command.Subscribe;

namespace SapphireDb_Net.Helper
{
    public static class CollectionHelper
    {
        public static void UpdateCollection<T>(
            ISubject<List<T>> collectionDataSubject,
            IObservable<InfoResponse> collectionInformation,
            ChangeResponse changeResponse)
        {
            collectionInformation
                .Select((information) =>
                {
                    return collectionDataSubject.Select((collectionData) =>
                        new Tuple<InfoResponse, List<T>>(information, collectionData));
                })
                .Switch()
                .Take(1)
                .Subscribe((data) =>
                {
                    List<T> values = data.Item2;
                    InfoResponse info = data.Item1;

                    T changeModel = changeResponse.Value.ToObject<T>();

                    if (changeResponse.State == ChangeResponse.ChangeState.Added)
                    {
                        values.Add(changeResponse.Value.ToObject<T>());
                        collectionDataSubject.OnNext(values);
                    }
                    else
                    {
                        Type modelType = typeof(T);
                        List<PropertyInfo> primaryKeyProperties = modelType.GetProperties()
                            .Where(pi => info.PrimaryKeys.Any(primaryKey =>
                                primaryKey.Equals(pi.Name, StringComparison.InvariantCultureIgnoreCase)))
                            .ToList();
                        
                        if (changeResponse.State == ChangeResponse.ChangeState.Modified)
                        {
                            // TODO: Implement modified
                        }
                        else if (changeResponse.State == ChangeResponse.ChangeState.Deleted)
                        {
                            int index = values.FindIndex((value) =>
                            {
                                return primaryKeyProperties
                                    .All(property => property.GetValue(changeModel).Equals(property.GetValue(value)));
                            });

                            if (index != -1)
                            {
                                values.RemoveAt(index);
                                collectionDataSubject.OnNext(values);
                            }
                        }
                    }
                    
                    
                });
        }
    }
}