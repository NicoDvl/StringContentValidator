# StringContentValidation

A library for fluent validation of content from text file. Ensures that the text content read from csv files, excel files is valid. Provides maximum feedbacks for correcting input files and display errors to the end users.

## Get Started

StringContentValidation can be installed using the Nuget package manager or the dotnet CLI.

Install-Package StringContentValidator

## Validation use case

You want to validate the text content from a csv or excel or... simple eg :

| Key     | DecimalValue | DateTimeValue |
| ------- | ------------ | ------------- |
| thiskey | 123,45       | 20181201      |

Define a class with string properties that represent the columns of the files and validate with :

```cs
public class Row
{
    public string Key { get; set; }
    public string DecimalValue { get; set; }
    public string DateTimeValue { get; set; }
}

// ...

ClassValidator<Row> validator = ClassValidator<Row>
    .Init()
    .AddProperty(PropertyValidator<Row>.For(x => x.Key).IsNotNull().HasLength(5,10))
    .AddProperty(PropertyValidator<Row>.For(x => x.DateTimeValue).IsNotNull().IsDateTime("yyyyMMdd"))
    .AddProperty(PropertyValidator<Row>.For(x => x.DecimalValue).IsNotNull().IsDecimal())
    .Validate(new Row()
    {
        Key = "thiskey",
        DateTimeValue = "20181201",
        DecimalValue = "123,45"
    });
```

If you do not want to define a class, you can use a dynamic object :

```cs

dynamic dynamicRow = new ExpandoObject();
dynamicRow.Key = "mykey";
dynamicRow.DateTimeValue = "20181201";
dynamicRow.DecimalValue = "123,45";

ClassValidator<dynamic> validator = ClassValidator<dynamic>
    .Init()
    .AddProperty(PropertyValidator<dynamic>.ForDynamic(x => x.Key, "Key").IsNotNull().HasLength(5, 10))
    .AddProperty(PropertyValidator<dynamic>.ForDynamic(x => x.DateTimeValue, "DateTimeValue").IsDateTime("yyyyMMdd"))
    .AddProperty(PropertyValidator<dynamic>.ForDynamic(x => x.DecimalValue, "DecimalValue").IsNotNull().IsDecimal())
    .Validate(dynamicRow);

```

Obviously, in a real scenario the rows are loaded from a stream or an additional library of your choice.

## Validation for a collection

Validation can be done for a collection of rows

```cs

ClassValidator<Row> validator = ClassValidator<Row>
    .Init(new ClassValidatorOption() { ShowRowIndex = true }) // show index in error list
    .AddProperty(PropertyValidator<Row>.For(x => x.Key).IsNotNull())
    .AddProperty(PropertyValidator<Row>.For(x => x.DateTimeValue).IsNotNull().IsDateTime("yyyyMMdd"))
    .ValidateList(
        new List<Row>(){
            new Row() { Key = "mykey", DateTimeValue = "20181201" },
            new Row() { Key = "mykey", DateTimeValue = "20181301" }, // error
            new Row() { Key = "mykey", DateTimeValue = null } // error
        }
    );

```
