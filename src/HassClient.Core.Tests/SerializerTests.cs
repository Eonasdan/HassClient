﻿using System;
using System.Reflection;
using System.Runtime.Serialization;
using HassClient.Core.Models.Events;
using HassClient.Core.Models.KnownEnums;
using HassClient.Core.Serialization;
using NUnit.Framework;

namespace HassClient.Core.Tests
{
    [TestFixture(TestOf = typeof(HassSerializer))]
    public class SerializerTests
    {
        private const string ExpectedTestValueEnumResult = "test_value";
        private const string ExpectedEnumMemberTestValueEnumResult = "customized_name";
        private const string ExpectedTestPropertyResult = "test_property";
        private const string ExpectedTestFieldResult = "test_field";

        private enum TestEnum
        {
            DefaultValue = 0,

            TestValue,

            [EnumMember(Value = ExpectedEnumMemberTestValueEnumResult)]
            EnumMemberTestValue,
        }

        private class TestClass
        {
            public string TestProperty { get; set; }

            public string TestField;
        }

        [Test]
        public void EnumToSnakeCase()
        {
            var result = TestEnum.TestValue.ToSnakeCase();

            Assert.NotNull(result);
            Assert.AreEqual(ExpectedTestValueEnumResult, result);
        }

        [Test]
        public void EnumToSnakeCasePriorizesEnumMemberAttribute()
        {
            var value = TestEnum.EnumMemberTestValue;
            var memInfo = typeof(TestEnum).GetMember(value.ToString());
            var attribValue = memInfo[0].GetCustomAttribute<EnumMemberAttribute>().Value;
            var result = value.ToSnakeCase();

            Assert.NotNull(result);
            Assert.AreEqual(attribValue, result);
        }

        [Test]
        [TestCase(KnownDomains.Automation)]
        [TestCase(KnownEventTypes.AreaRegistryUpdated)]
        [TestCase(KnownServices.AddonRestart)]
        public void EnumToSnakeCaseWithKnownEnumThrows<T>(T value)
            where T : Enum
        {
            Assert.Throws<InvalidOperationException>(() => value.ToSnakeCase());
        }

        [Test]
        public void TryGetEnumFromSnakeCase()
        {
            var success = HassSerializer.TryGetEnumFromSnakeCase<TestEnum>(ExpectedTestValueEnumResult, out var result);

            Assert.IsTrue(success);
            Assert.AreEqual(TestEnum.TestValue, result);
        }

        [Test]
        public void TryGetEnumFromSnakeCaseWithInvalidValue()
        {
            var success = HassSerializer.TryGetEnumFromSnakeCase<TestEnum>("invalid_value", out var result);

            Assert.IsFalse(success);
            Assert.AreEqual(default(TestEnum), result);
        }

        [Test]
        public void EnumValuesAreConvertedToSnakeCase()
        {
            var value = TestEnum.TestValue;
            var result = HassSerializer.SerializeObject(value);

            Assert.NotNull(result);
            Assert.AreEqual($"\"{ExpectedTestValueEnumResult}\"", result);
        }

        [Test]
        public void EnumValuesAreConvertedToSnakeCasePriorizingEnumMemberAttribute()
        {

            var value = TestEnum.EnumMemberTestValue;
            var memInfo = typeof(TestEnum).GetMember(value.ToString());
            var attribValue = memInfo[0].GetCustomAttribute<EnumMemberAttribute>().Value;
            var result = HassSerializer.SerializeObject(value);

            Assert.NotNull(result);
            Assert.AreEqual($"\"{attribValue}\"", result);
        }

        [Test]
        public void EnumValuesAreConvertedFromSnakeCase()
        {
            var result = HassSerializer.DeserializeObject<TestEnum>($"\"{ExpectedTestValueEnumResult}\"");

            Assert.NotNull(result);
            Assert.AreEqual(TestEnum.TestValue, result);
        }

        [Test]
        public void EnumValuesAreConvertedFromSnakeCasePriorizingEnumMemberAttribute()
        {

            var value = TestEnum.EnumMemberTestValue;
            var memInfo = typeof(TestEnum).GetMember(value.ToString());
            var attribValue = memInfo[0].GetCustomAttribute<EnumMemberAttribute>().Value;
            var result = HassSerializer.DeserializeObject<TestEnum>($"\"{attribValue}\"");

            Assert.NotNull(result);
            Assert.AreEqual(value, result);
        }

        [Test]
        public void PropertiesAreConvertedToSnakeCase()
        {
            var value = new TestClass { TestProperty = nameof(TestClass.TestProperty) };
            var result = HassSerializer.SerializeObject(value);

            Assert.NotNull(result);
            Assert.IsTrue(result.Contains($"\"{ExpectedTestPropertyResult}\":\"{value.TestProperty}\""));
        }

        [Test]
        public void PropertiesAreConvertedFromSnakeCase()
        {
            var result = HassSerializer.DeserializeObject<TestClass>($"{{\"{ExpectedTestPropertyResult}\":\"{nameof(TestClass.TestProperty)}\"}}");

            Assert.NotNull(result);
            Assert.AreEqual(nameof(TestClass.TestProperty), result.TestProperty);
        }

        [Test]
        public void FieldsAreConvertedToSnakeCase()
        {
            var value = new TestClass { TestField = nameof(TestClass.TestField) };
            var result = HassSerializer.SerializeObject(value);

            Assert.NotNull(result);
            Assert.IsTrue(result.Contains($"\"{ExpectedTestFieldResult}\":\"{value.TestField}\""));
        }

        [Test]
        public void FieldsAreConvertedFromSnakeCase()
        {
            var result = HassSerializer.DeserializeObject<TestClass>($"{{\"{ExpectedTestFieldResult}\":\"{nameof(TestClass.TestField)}\"}}");

            Assert.NotNull(result);
            Assert.AreEqual(nameof(TestClass.TestField), result.TestField);
        }

        [Test]
        public void JObjectPropertiesAreConvertedToSnakeCase()
        {
            var value = new TestClass { TestProperty = nameof(TestClass.TestProperty) };
            var result = HassSerializer.CreateJObject(value);

            Assert.NotNull(result);
            Assert.AreEqual(value.TestProperty, result.GetValue(ExpectedTestPropertyResult).ToString());
        }

        [Test]
        public void JObjectFieldsAreConvertedToSnakeCase()
        {
            var value = new TestClass { TestField = nameof(TestClass.TestField) };
            var result = HassSerializer.CreateJObject(value);

            Assert.NotNull(result);
            Assert.AreEqual(value.TestField, result.GetValue(ExpectedTestFieldResult).ToString());
        }

        [Test]
        public void JObjectWithSelectedProperties()
        {
            var selectedProperties = new[] { nameof(TestClass.TestProperty) };
            var result = HassSerializer.CreateJObject(new TestClass(), selectedProperties);

            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(result.ContainsKey(ExpectedTestPropertyResult));
            Assert.IsFalse(result.ContainsKey(ExpectedTestFieldResult));
        }

        [Test]
        public void JObjectFromNullThrows()
        {
            Assert.Throws<ArgumentNullException>(() => HassSerializer.CreateJObject(null));
        }
    }
}
