using StringContentValidator.Test.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace StringContentValidator.Test
{
    public class IfTest
    {
        [Fact]
        public void IfTest_trials()
        {
            PropertyValidator<Row> validator = PropertyValidator<Row>.For(x => x.DecimalValue);
            validator.If(x => x.Key == "P", p => p.IsNotNull().TryParseDecimal());

            validator.Validate(new Row() { Key = "Z" });
            Assert.True(validator.IsValid);

            validator.Validate( new Row() { Key = "P" });
            Assert.False(validator.IsValid);

            validator.Validate(new Row() { Key = "P", DecimalValue = "123" });
            Assert.True(validator.IsValid);

            validator.Validate(new Row() { Key = "P", DecimalValue = "123,12" }); // test decimal separator
            Assert.True(validator.IsValid);

            validator.Validate(new Row() { Key = "Z", DecimalValue = "123,12" });
            Assert.True(validator.IsValid);
        }
    }
}

