using StringContentValidator.Test.Utilities;
using System;
using Xunit;

namespace StringContentValidator.Test
{
    public class TryDateTimeTest
    {
        [Fact]
        public void IsDateTimeTest_should_fail()
        {
            PropertyValidator<Row> validator = PropertyValidator<Row>.For(x => x.DateTimeValue);
            validator
                .TryParseDateTime("yyyyMMdd")
                .Validate(new Row() { DateTimeValue = "20201320" });
            Assert.False(validator.IsValid);
        }

        [Fact]
        public void IsDateTimeTest_should_pass()
        {
            PropertyValidator<Row> validator = PropertyValidator<Row>.For(x => x.DateTimeValue);
            validator
                .TryParseDateTime("yyyyMMdd")
                .Validate(new Row() { DateTimeValue = "20201220" });
            Assert.True(validator.IsValid);
        }
    }
}
