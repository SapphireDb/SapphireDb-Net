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
        public static void CallUpdateCollection(
            Type modelType,
            ISubject<object> collectionDataSubject,
            IObservable<InfoResponse> collectionInformation,
            ChangeResponse changeResponse)
        {
            typeof(CollectionHelper).GetMethod(nameof(UpdateCollection))?.MakeGenericMethod(modelType).Invoke(null,
                new object[] {collectionDataSubject, collectionInformation, changeResponse});
        }
        
        public static void UpdateCollection<T>(
            ISubject<object> collectionDataSubject,
            IObservable<InfoResponse> collectionInformation,
            ChangeResponse changeResponse)
        {
            collectionInformation
                .Select((information) =>
                {
                    return collectionDataSubject.Select((collectionData) =>
                        new Tuple<InfoResponse, List<T>>(information, (List<T>)collectionData));
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
                        values.Add(changeModel);
                        collectionDataSubject.OnNext(values);
                    }
                    else
                    {
                        Type modelType = typeof(T);
                        List<PropertyInfo> primaryKeyProperties = modelType.GetProperties()
                            .Where(pi => info.PrimaryKeys.Any(primaryKey =>
                                primaryKey.Equals(pi.Name, StringComparison.InvariantCultureIgnoreCase)))
                            .ToList();
                        
                        int index = values.FindIndex((value) =>
                        {
                            return primaryKeyProperties
                                .All(property => property.GetValue(changeModel).Equals(property.GetValue(value)));
                        });

                        if (index == -1)
                        {
                            return;
                        }
                        
                        if (changeResponse.State == ChangeResponse.ChangeState.Modified)
                        {
                            values.RemoveAt(index);
                            values.Insert(index, changeModel);
                        }
                        else if (changeResponse.State == ChangeResponse.ChangeState.Deleted)
                        {
                            values.RemoveAt(index);
                        }
                        
                        collectionDataSubject.OnNext(values);
                    }
                });
        }
    }
}