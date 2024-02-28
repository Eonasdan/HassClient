using System;
using System.Collections.Generic;
using System.Linq;
using HassClient.Core.Helpers;
using HassClient.Core.Models.RegistryEntries;
using HassClient.Core.Serialization;
using Newtonsoft.Json.Linq;

namespace HassClient.WS.Tests.Mocks.HassServer
{
    public class MockHassDb
    {
        private readonly Dictionary<Type, HashSet<object>> _collectionsByType = new();

        private bool CreateObject(Type key, object value)
        {
            if (!_collectionsByType.TryGetValue(key, out var collection))
            {
                collection = new HashSet<object>();
                _collectionsByType.Add(key, collection);
            }

            return collection.Add(value);
        }

        private object UpdateObject(Type key, object target, JRaw value)
        {
            if (_collectionsByType.TryGetValue(key, out var collection) &&
                collection.TryGetValue(target, out var actual))
            {
                HassSerializer.PopulateObject(value, actual);
                return actual;
            }

            return default;
        }

        private bool DeleteObject(Type key, object value)
        {
            if (_collectionsByType.TryGetValue(key, out var collection))
            {
                return collection.Remove(value);
            }

            return false;
        }

        public bool CreateObject<T>(T value)
        {
            var key = typeof(T);
            return CreateObject(key, value);
        }
        public bool CreateObject(EntityRegistryEntryBase value)
        {
            var key = value.GetType();
            return CreateObject(key, value);
        }

        public T UpdateObject<T>(T target, JRaw value)
        {
            var key = typeof(T);
            return (T)UpdateObject(key, target, value);
        }

        public EntityRegistryEntryBase UpdateObject(EntityRegistryEntryBase target, JRaw value)
        {
            var key = value.GetType();
            return (EntityRegistryEntryBase)UpdateObject(key, target, value);
        }

        public IEnumerable<T> GetObjects<T>()
        {
            var key = typeof(T);
            if (_collectionsByType.TryGetValue(key, out var collection))
            {
                return collection.Cast<T>();
            }

            return Enumerable.Empty<T>();
        }

        public IEnumerable<object> GetObjects(Type type)
        {
            if (_collectionsByType.TryGetValue(type, out var collection))
            {
                return collection;
            }

            return Enumerable.Empty<object>();
        }

        public bool DeleteObject<T>(T value)
        {
            var key = typeof(T);
            return DeleteObject(key, value);
        }

        public bool DeleteObject(EntityRegistryEntryBase value)
        {
            var key = value.GetType();
            return DeleteObject(key, value);
        }

        public IEnumerable<EntityRegistryEntryBase> GetAllEntityEntries()
        {
            return _collectionsByType.Values.Where(x => x.FirstOrDefault() is EntityRegistryEntryBase)
                                                .SelectMany(x => x.Cast<EntityRegistryEntryBase>());
        }

        public IEnumerable<EntityRegistryEntryBase> GetAllEntityEntries(string domain)
        {
            var domainCollection = _collectionsByType.Values.FirstOrDefault(x => (x.FirstOrDefault() is EntityRegistryEntryBase entry) &&
                                                                                     entry.EntityId.GetDomain() == domain)?
                                         .Cast<EntityRegistryEntryBase>();
            return domainCollection ?? Enumerable.Empty<EntityRegistryEntryBase>();
        }

        public EntityRegistryEntryBase FindEntityEntry(string entityId)
        {
            var domainCollection = GetAllEntityEntries(entityId.GetDomain());
            return domainCollection?.FirstOrDefault(x => x.EntityId == entityId);
        }
    }
}
