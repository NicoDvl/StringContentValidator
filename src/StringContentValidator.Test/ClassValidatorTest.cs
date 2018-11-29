using StringContentValidator.Test.Utilities;
using System;
using System.Collections.Generic;
using System.Dynamic;
using Xunit;

namespace StringContentValidator.Test
{
    public class ClassValidatorTest
    {
        [Fact]
        public void ClassValidator_oneerror()
        {
            ClassValidator<Row> validator = ClassValidator<Row>
                .Init()
                .AddProperty(PropertyValidator<Row>.For(x => x.Key).IsNotNull())
                .AddProperty(PropertyValidator<Row>.For(x => x.DateTimeValue).IsNotNull()) // error
                .Validate(new Row() { Key = "mykey", DateTimeValue = null });

            Assert.Single(validator.ValidationErrors);
        }

        [Fact]
        public void ClassValidator_class_sample()
        {
            ClassValidator<Row> validator = ClassValidator<Row>
                .Init()
                .AddProperty(PropertyValidator<Row>.For(x => x.Key).IsNotNull().HasLength(5,10))
                .AddProperty(PropertyValidator<Row>.For(x => x.DateTimeValue).IsNotNull().TryParseDateTime("yyyyMMdd"))
                .AddProperty(PropertyValidator<Row>.For(x => x.DecimalValue).IsNotNull().TryParseDecimal())
                .Validate(new Row()
                {
                    Key = "thiskey",
                    DateTimeValue = "20181201",
                    DecimalValue = "123,45"
                });

            Assert.Empty(validator.ValidationErrors);
        }

        [Fact]
        public void ClassValidator_dynamic_oneerror()
        {
            dynamic dynamicRow = new ExpandoObject();
            dynamicRow.Key = "mykey";
            dynamicRow.DateTimeValue = null;

            ClassValidator<dynamic> validator = ClassValidator<dynamic>
                .Init()
                .AddProperty(PropertyValidator<dynamic>.ForDynamic(x => x.Key, "Key").IsNotNull())
                .AddProperty(PropertyValidator<dynamic>.ForDynamic(x => x.DateTimeValue, "DateTimeValue").IsNotNull()) // error
                .Validate(dynamicRow);

            Assert.Single(validator.ValidationErrors);
        }

        [Fact]
        public void ClassValidator_dynamic_sample()
        {
            dynamic dynamicRow = new ExpandoObject();
            dynamicRow.Key = "mykey";
            dynamicRow.DateTimeValue = "20181201";
            dynamicRow.DecimalValue = "123,45";

            ClassValidator<dynamic> validator = ClassValidator<dynamic>
                .Init()
                .AddProperty(PropertyValidator<dynamic>.ForDynamic(x => x.Key, "Key").IsNotNull().HasLength(5, 10))
                .AddProperty(PropertyValidator<dynamic>.ForDynamic(x => x.DateTimeValue, "DateTimeValue").TryParseDateTime("yyyyMMdd"))
                .AddProperty(PropertyValidator<dynamic>.ForDynamic(x => x.DecimalValue, "DecimalValue").IsNotNull().TryParseDecimal())
                .Validate(dynamicRow);

            Assert.Empty(validator.ValidationErrors);
        }


        [Fact]
        public void ClassValidator_list_oneerror()
        {
            ClassValidator<Row> validator = ClassValidator<Row>
                .Init()
                .AddProperty(PropertyValidator<Row>.For(x => x.Key).IsNotNull())
                .AddProperty(PropertyValidator<Row>.For(x => x.DateTimeValue).IsNotNull()) // error
                .ValidateList(
                    new List<Row>(){
                        new Row() { Key = "notnull", DateTimeValue = null },
                        new Row() { Key = "notnull", DateTimeValue = null }
                    }
                );

            Assert.Equal(2, validator.ValidationErrors.Count);
        }

        [Fact]
        [UseCulture("en")]
        public void ClassValidator_list_with_index()
        {
            ClassValidator<Row> validator = ClassValidator<Row>
                .Init(new ClassValidatorOption() { ShowRowIndex = true }) // show index in error list
                .AddProperty(PropertyValidator<Row>.For(x => x.Key).IsNotNull())
                .AddProperty(PropertyValidator<Row>.For(x => x.DateTimeValue).IsNotNull().TryParseDateTime("yyyyMMdd"))
                .ValidateList(
                    new List<Row>(){
                        new Row() { Key = "mykey", DateTimeValue = "20181201" },
                        new Row() { Key = "mykey", DateTimeValue = "20181301" }, // error
                        new Row() { Key = "mykey", DateTimeValue = null } // error
                    }
                );

            Assert.Equal(2, validator.ValidationErrors.Count);
            Assert.Contains("Row 1 ", validator.ValidationErrors[0].ErrorMessage);
            Assert.Contains("Row 2 ", validator.ValidationErrors[1].ErrorMessage);
        }
    }
}
