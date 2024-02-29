using System;
using System.Linq;
using System.Runtime.CompilerServices;
using HassClient.Core.Models.KnownEnums;
using HassClient.Core.Models.RegistryEntries.Modifiable;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace HassClient.Core.Tests.Models
{
    [TestFixture(TestOf = typeof(ModifiablePropertyCollection<string>))]
    public class ModifiablePropertyCollectionTests
    {
        [Test]
        public void DoesNotAcceptsDuplicatedValues()
        {
            var collectionProperty = CreateCollectionProperty();

            const string item = "Test";
            collectionProperty.Value.Add(item);
            collectionProperty.Value.Add(item);

            Assert.That(collectionProperty.Value, Has.Exactly(1).Matches<string>(p => p == item));
        }

        [Test]
        public void AddInvalidValueWhenUsingValidationFuncThrows()
        {
            var collectionProperty = CreateCollectionProperty(0, x => x == "Test");

            Assert.Throws<InvalidOperationException>(() => collectionProperty.Value.Add("Test2"));
        }

        [Test]
        public void AddValidValueWhenUsingValidationFuncDoesNotThrows()
        {
            const string item = "Test";
            var collectionProperty = CreateCollectionProperty(0, validationFunc: x => x == item);

            Assert.DoesNotThrow(() => collectionProperty.Value.Add(item));
        }

        [Test]
        public void SaveChangesMakesHasPendingChangesFalse()
        {
            var collectionProperty = CreateCollectionProperty(hasChanges: true);
            Assert.That(collectionProperty.HasPendingChanges, Is.True);

            collectionProperty.SaveChanges();
            Assert.That(collectionProperty.HasPendingChanges, Is.False);
        }

        [Test]
        public void DiscardPendingChangesMakesHasPendingChangesFalse()
        {
            var collectionProperty = CreateCollectionProperty(hasChanges: true);
            Assert.That(collectionProperty.HasPendingChanges, Is.True);

            collectionProperty.DiscardPendingChanges();
            Assert.That(collectionProperty.HasPendingChanges, Is.False);
        }

        [Test]
        public void DiscardPendingChangesRestoresPreviousValues()
        {
            var collectionProperty = CreateCollectionProperty();
            var initialValues = collectionProperty.Value.ToArray();

            collectionProperty.Value.Add("Test");
            collectionProperty.DiscardPendingChanges();
            Assert.That(initialValues, Is.EqualTo(collectionProperty.Value).AsCollection);
        }

        [Test]
        public void AddNewValueMakesHasPendingChangesTrue()
        {
            var collectionProperty = CreateCollectionProperty();
            Assert.That(collectionProperty.HasPendingChanges, Is.False);

            collectionProperty.Value.Add("Test");

            Assert.That(collectionProperty.HasPendingChanges, Is.True);
        }

        [Test]
        public void RemoveValueMakesHasPendingChangesTrue()
        {
            var collectionProperty = CreateCollectionProperty();
            Assert.That(collectionProperty.HasPendingChanges, Is.False);

            collectionProperty.Value.Remove(collectionProperty.Value.First());

            Assert.That(collectionProperty.HasPendingChanges, Is.True);
        }

        [Test]
        public void ClearValuesMakesHasPendingChangesTrue()
        {
            var collectionProperty = CreateCollectionProperty();
            Assert.That(collectionProperty.HasPendingChanges, Is.False);

            collectionProperty.Value.Clear();
            Assert.That(collectionProperty.HasPendingChanges, Is.True);
        }

        [Test]
        public void AddAndRemoveValueMakesHasPendingChangesFalse()
        {
            var collectionProperty = CreateCollectionProperty();
            Assert.That(collectionProperty.HasPendingChanges, Is.False);

            const string item = "Test";
            collectionProperty.Value.Add(item);
            Assert.That(collectionProperty.HasPendingChanges, Is.True);

            collectionProperty.Value.Remove(item);
            Assert.That(collectionProperty.HasPendingChanges, Is.False);
        }

        [Test]
        public void RemoveAndAddValueMakesHasPendingChangesFalse()
        {
            var collectionProperty = CreateCollectionProperty();
            var existingValue = collectionProperty.Value.First();
            Assert.That(collectionProperty.HasPendingChanges, Is.False);

            collectionProperty.Value.Remove(existingValue);
            Assert.That(collectionProperty.HasPendingChanges, Is.True);

            collectionProperty.Value.Add(existingValue);
            Assert.That(collectionProperty.HasPendingChanges, Is.False);
        }

        [Test]
        public void ClearAndAddSameValuesMakesHasPendingChangesFalse()
        {
            var collectionProperty = CreateCollectionProperty();
            var initialValues = collectionProperty.Value.ToArray();
            Assert.That(collectionProperty.HasPendingChanges, Is.False);

            collectionProperty.Value.Clear();
            Assert.That(collectionProperty.HasPendingChanges, Is.True);

            foreach (var item in initialValues)
            {
                collectionProperty.Value.Add(item);
            }

            Assert.That(collectionProperty.HasPendingChanges, Is.False);
        }

        private ModifiablePropertyCollection<string> CreateCollectionProperty(int elementCount = 2,
            Func<string, bool> validationFunc = null, bool hasChanges = false,
            [CallerMemberName] string collectionName = null)
        {
            var collection = new ModifiablePropertyCollection<string>(collectionName, validationFunc);
            for (var i = 0; i < elementCount; i++)
            {
                collection.Value.Add(MockHelpers.GetRandomEntityId(KnownDomains.Climate));
            }

            if (!hasChanges)
            {
                collection.SaveChanges();
            }

            return collection;
        }
    }
}