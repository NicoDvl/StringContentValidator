using StringContentValidator.Test.Utilities;
using System;
using System.Globalization;
using Xunit;

namespace StringContentValidator.Test
{
    public class TryParseDecimal
    {
        [Fact]
        public void TryParseDecimal_should_fail()
        {
            PropertyValidator<Row> validator = PropertyValidator<Row>.For(x => x.DecimalValue);
            validator.TryParseDecimal()
                .Validate(new Row() { DecimalValue = "a1234" });
            Assert.False(validator.IsValid);
        }

        [Fact]
        public void TryParseDecimal_should_pass()
        {
            PropertyValidator<Row> validator = PropertyValidator<Row>.For(x => x.DecimalValue);
            validator.TryParseDecimal(CultureInfo.InvariantCulture)
                .Validate(new Row() { DecimalValue = "1234.55" });
            Assert.True(validator.IsValid);
        }

        [Fact]
        public void TryParseDecimal_negative_value_should_pass()
        {
            PropertyValidator<Row> validator = PropertyValidator<Row>.For(x => x.DecimalValue);
            validator.TryParseDecimal(CultureInfo.InvariantCulture)
                .Validate(new Row() { DecimalValue = "-1234.55" });
            Assert.True(validator.IsValid);
        }

        [Fact]
        public void TryParseDecimal_culture_specific_should_pass()
        {
            PropertyValidator<Row> validator = PropertyValidator<Row>.For(x => x.DecimalValue);
            validator.TryParseDecimal(new CultureInfo("fr-FR"))
                .Validate(new Row() { DecimalValue = "-1234,55" });
            Assert.True(validator.IsValid);
        }
    }
}
