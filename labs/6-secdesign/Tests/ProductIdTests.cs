﻿using System;
using System.Collections.Generic;
using SecureByDesign.Host;
using SecureByDesign.Host.Domain;
using SecureByDesign.Host.Domain.Model;
using Xunit;

namespace Tests
{
    [Trait("Category", "Unit")]
    public class ProductIdTests
    {
        [Theory]
        [MemberData(nameof(IdInjection))]
        [MemberData(nameof(InvalidIds))]
        public void ProductIdShouldReject(string id)
        {
            Assert.False(ProductId.IsValidId(id));
        }

        [Theory]
        [MemberData(nameof(IdInjection))]
        [MemberData(nameof(InvalidIds))]
        public void ProductIdConstructorShouldThrow(string id)
        {
            Assert.Throws<InvalidProductIdArgumentException>(() => new ProductId(id));
        }

        [Theory]
        [MemberData(nameof(ValidIds))]
        public void ProductIdConstructorShouldNotThrow(string id)
        {
            var product = new ProductId(id);
            Assert.True(product.Value == id);
        }

        public static IEnumerable<object[]> IdInjection => new[]
        {
            new object[] { "<script>" },
            new object[] { "'1==1" },
            new object[] { "--sql" }
        };

        public static IEnumerable<object[]> InvalidIds => new[]
        {
            new object[] { "" },
            new object[] { "no spaces" },
            new object[] { "thisisanidthatistoolong" },
            new object[] { "#" }
        };

        public static IEnumerable<object[]> ValidIds => new[]
        {
            new object[] { "abcdefghi" },
            new object[] { "123456789" },
            new object[] { "dhgf54" }
        };
    }
}

