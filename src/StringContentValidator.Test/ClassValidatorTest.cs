using StringContentValidator.Test.Utilities;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
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
                .For(x => x.Key, p => p.IsNotNull())
                .For(x => x.DateTimeValue, p => p.IsNotNull()) // error
                .Validate(new Row() { Key = "mykey", DateTimeValue = null });

            Assert.Single(validator.ValidationErrors);
        }

        [Fact]
        public void ClassValidator_customgetter_ok()
        {
            ClassValidator<Row> validator = ClassValidator<Row>
                .Init()
                .For(x => x.Key, x => x.Key.ToUpper(), p => p.IsNotNullOrEmpty().IsStringValues(new string[] { "MYKEY" }))
                .Validate(new Row() { Key = "mykey", DateTimeValue = null });

            Assert.Empty(validator.ValidationErrors);
        }

        [Fact]
        public void ClassValidator_class_sample()
        {
            ClassValidator<Row> validator = ClassValidator<Row>.Init()
                .For(x => x.Key, p => p.IsNotNull().HasLength(5, 10))
                .For(x => x.DateTimeValue, p => p.IsNotNull().TryParseDateTime("yyyyMMdd"))
                .For(x => x.DecimalValue, p => p.IsNotNull().TryParseDecimal(CultureInfo.InvariantCulture))
                .Validate(new Row()
                {
                    Key = "thiskey",
                    DateTimeValue = "20181201",
                    DecimalValue = "123.45"
                });

            Assert.Empty(validator.ValidationErrors);
        }

        [Fact]
        public void ClassValidator_dynamic_oneerror()
        {
            dynamic row = new ExpandoObject();
            row.Key = "mykey";
            row.DateTimeValue = null;

            ClassDynamicValidator validator = ClassDynamicValidator
                .Init()
                .For(x => x.Key, "Key", p => p.IsNotNull())
                .For(x => x.DateTimeValue, "DateTimeValue", p => p.IsNotNull()) // error
                .Validate(row);

            Assert.Single(validator.ValidationErrors);
        }

        [Fact]
        public void ClassValidator_dynamic_sample()
        {
            dynamic row = new ExpandoObject();
            row.Key = "mykey";
            row.DateTimeValue = "20181201";
            row.DecimalValue = "123.45";

            ClassDynamicValidator validator = ClassDynamicValidator
                .Init()
                .For(x => x.Key, "Key", p => p.IsNotNull().HasLength(5, 10))
                .For(x => x.DateTimeValue, "DateTimeValue", p => p.TryParseDateTime("yyyyMMdd"))
                .For(x => x.DecimalValue, "DecimalValue", p => p.IsNotNull().TryParseDecimal(CultureInfo.InvariantCulture))
                .Validate(row);

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
            var defaultList = new List<Row>(){
                new Row() { Key = "mykey", DateTimeValue = "20181201" },
                new Row() { Key = "mykey", DateTimeValue = "20181301" }, // error
                new Row() { Key = "mykey", DateTimeValue = null } // error
            };

            ClassValidator<Row> CreateValidator(ClassValidatorOption option, List<Row> list)
            {
                return ClassValidator<Row>
                .Init(option) // show index in error list. default index
                .For(x => x.Key, p => p.IsNotNull())
                .For(x => x.DateTimeValue, p => p.IsNotNull().TryParseDateTime("yyyyMMdd"))
                .ValidateList(list);
            }

            // show index in error list. default index
            ClassValidator<Row> validator = CreateValidator(
                new ClassValidatorOption() { ShowRowIndex = true }, 
                defaultList
            );

            Assert.Equal(3, validator.ValidationErrors.Count);
            Assert.Contains("Row 2 ", validator.ValidationErrors[0].ErrorMessage);
            Assert.Contains("Row 3 ", validator.ValidationErrors[1].ErrorMessage);

            // show index in error list. force index
            ClassValidator<Row> validatorForceIndex = CreateValidator(
                new ClassValidatorOption() { ShowRowIndex = true, RowIndexStartsAt = 5 },
                defaultList
            );

            Assert.Equal(3, validatorForceIndex.ValidationErrors.Count);
            Assert.Contains("Row 6 ", validatorForceIndex.ValidationErrors[0].ErrorMessage);
            Assert.Contains("Row 7 ", validatorForceIndex.ValidationErrors[1].ErrorMessage);
        }
    }
}
