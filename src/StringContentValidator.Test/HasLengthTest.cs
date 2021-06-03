using StringContentValidator.Test.Utilities;
using System;
using Xunit;

namespace StringContentValidator.Test
{
    public class HasLengthTest
    {
        [Fact]
        public void IsNotNull_should_fail()
        {
            PropertyValidator<Row> validator1 = PropertyValidator<Row>.For(x => x.Key);
            validator1
                .IsNotNull()
                .HasLength(1, 5)
                .Validate(new Row() { Key = "" });
            Assert.False(validator1.IsValid);

            PropertyValidator<Row> validator2 = PropertyValidator<Row>.For(x => x.Key);
            validator2
                .IsNotNull()
                .HasLength(0, 5)
                .Validate(new Row() { Key = "123456" });
            Assert.False(validator2.IsValid);
        }

        [Fact]
        public void IsNotNull_should_pass()
        {
            PropertyValidator<Row> validator1 = PropertyValidator<Row>.For(x => x.Key);
            validator1
                .IsNotNull()
                .HasLength(0, 5)
                .Validate(new Row() { Key = "" });
            Assert.True(validator1.IsValid);

            PropertyValidator<Row> validator2 = PropertyValidator<Row>.For(x => x.Key);
            validator2
                .IsNotNull()
                .HasLength(0, 5)
                .Validate(new Row() { Key = "12345" });
            Assert.True(validator2.IsValid);
        }
    }
}
