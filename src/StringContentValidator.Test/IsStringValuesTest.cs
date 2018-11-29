using StringContentValidator.Test.Utilities;
using System;
using Xunit;

namespace StringContentValidator.Test
{
    public class IsStringValuesTest
    {
        private static string[] TypesValues = new string[] { "P", "F", "R", "A" };

        [Fact]
        public void IsStringValues_should_successl()
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
            string[] TypesValues = new string[] { "P", "F", "R", "A" };
            PropertyValidator<Row> validator = PropertyValidator<Row>.For(x => x.Key);
            validator
                .IsNotNullOrEmpty()
                .IsStringValues(TypesValues)
                .Validate(new Row() { Key = "B" });
            Assert.False(validator.IsValid);
        }
    }
}
