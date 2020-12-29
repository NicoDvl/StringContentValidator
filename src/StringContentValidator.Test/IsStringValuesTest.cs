using StringContentValidator.Test.Utilities;
using System;
using Xunit;

namespace StringContentValidator.Test
{
    public class IsStringValuesTest
    {
        private static string[] TypesValues = new string[] { "P", "F", "R", "A" };

        [Fact]
        public void IsStringValues_should_success()
        {
            
            PropertyValidator<Row> validator = PropertyValidator<Row>.For(x => x.Key);
            validator
                .IsNotNullOrEmpty()
                .IsStringValues(TypesValues)
                .Validate(new Row() { Key = "P" });
            Assert.True(validator.IsValid);
        }

        [Fact]
        public void IsStringValues_should_fail()
        {
            PropertyValidator<Row> validator = PropertyValidator<Row>.For(x => x.Key);
            validator
                .IsNotNullOrEmpty()
                .IsStringValues(TypesValues)
                .Validate(new Row() { Key = "B" });
            Assert.False(validator.IsValid);
        }

        [Fact]
        public void IsStringValues_without_and_with_ignore_case()
        {
            var row = new Row() { Key = "p" };

            PropertyValidator<Row> validatorMustFail = PropertyValidator<Row>.For(x => x.Key);
            validatorMustFail
                .IsNotNullOrEmpty()
                .IsStringValues(TypesValues)
                .Validate(row);
            Assert.False(validatorMustFail.IsValid);

            PropertyValidator<Row> validatorMustSuccess = PropertyValidator<Row>.For(x => x.Key);
            validatorMustSuccess
                .IsNotNullOrEmpty()
                .IsStringValues(TypesValues, StringComparer.CurrentCultureIgnoreCase)
                .Validate(row);
            Assert.True(validatorMustSuccess.IsValid);
        }
    }
}
