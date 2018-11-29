using StringContentValidator.Test.Utilities;
using System;
using Xunit;

namespace StringContentValidator.Test
{
    public class IsNotNullTest
    {
        [Fact]
        public void IsNotNull_should_fail()
        {
            PropertyValidator<Row> validator = PropertyValidator<Row>.For(x => x.Key);
            validator
                .IsNotNull()
                .Validate(new Row() { Key = null });
            Assert.False(validator.IsValid);
        }

        [Fact]
        public void IsNotNull_should_pass()
        {
            PropertyValidator<Row> validator = PropertyValidator<Row>.For(x => x.Key);
            validator
                .IsNotNull()
                .Validate(new Row() { Key = "hasValue" });
            Assert.True(validator.IsValid);
        }

        [Fact]
        [UseCulture("en")]
        public void IsNotNull_default_message()
        {
            PropertyValidator<Row> validator = PropertyValidator<Row>.For(x => x.Key);
            validator
                .IsNotNull()
                .Validate(new Row() { Key = null });
            Assert.Equal("Field Key : Value is mandatory", validator.ValidationErrors[0].ErrorMessage);
        }

        [Fact]
        [UseCulture("en")]
        public void IsNotNull_default_message_row_index()
        {
            PropertyValidator<Row> validator = PropertyValidator<Row>.For(x => x.Key);
            validator
                .SetRowIndex(1)
                .IsNotNull()
                .Validate(new Row() { Key = null });
            Assert.Equal("Row 1 Field Key : Value is mandatory", validator.ValidationErrors[0].ErrorMessage);
        }

        [Fact]
        [UseCulture("en")]
        public void IsNotNull_override_message()
        {
            PropertyValidator<Row> validatorA = PropertyValidator<Row>.For(x => x.Key);
            validatorA
                .IsNotNull().OverrideErrorMessage(m => $"override default message")
                .Validate(new Row() { Key = null});
            Assert.Equal("override default message", validatorA.ValidationErrors[0].ErrorMessage);

            PropertyValidator<Row> validatorB = PropertyValidator<Row>.For(x => x.Key);
            validatorB
                .IsNotNull().OverrideErrorMessage(m => $"override default message", true)
                .Validate(new Row() { Key = null });
            Assert.Equal("Field Key : override default message", validatorB.ValidationErrors[0].ErrorMessage);
        }
    }
}
