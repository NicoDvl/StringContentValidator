using StringContentValidator.Test.Utilities;
using System;
using Xunit;

namespace StringContentValidator.Test
{
    public class TryRegex
    {
        [Fact]
        public void TryRegex_should_fail()
        {
            PropertyValidator<Row> validator = PropertyValidator<Row>.For(x => x.Key);
            validator
                .TryRegex("^[a-z0-9\\-]+$")
                .Validate(new Row() { Key = "key-0123" });
            Assert.True(validator.IsValid);
        }

        [Fact]
        public void TryRegex_should_pass()
        {
            PropertyValidator<Row> validator = PropertyValidator<Row>.For(x => x.Key);
            validator
                .TryRegex("^[a-z0-9\\-]+$")
                .Validate(new Row() { Key = "key-0123KEY" });
            Assert.False(validator.IsValid);
        }
    }
}
