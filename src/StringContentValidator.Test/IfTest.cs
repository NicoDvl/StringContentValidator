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

        [Fact]
        public void IfTest_class()
        {
            List<Row> list = new List<Row>(){
                new Row() { Key = "A", DecimalValue = "zz" },
                new Row() { Key = "A", DecimalValue = "1" }, // error
                new Row() { Key = "", DecimalValue = "1", DateTimeValue = null }, // error
                new Row() { Key = "", DecimalValue = "", DateTimeValue = "20012301" } // error
            };

            ClassValidator<Row> validator = ClassValidator<Row>
                .Init(new ClassValidatorOption() { ShowRowIndex = true })
                .ForIf(x => x.DecimalValue, e => e.Key != "A", p => p.TryParseDecimal())
                .ForIf(x => x.DateTimeValue, e => string.IsNullOrEmpty(e.Key), p => p.IsNotNullOrEmpty())
                .ValidateList(list);

            Assert.Equal(2, validator.ValidationErrors.Count);
        }
    }
}

