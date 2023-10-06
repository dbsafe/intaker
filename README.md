[![Build Status](https://dev.azure.com/dbsafe/dbsafe/_apis/build/status/intaker/intaker?branchName=master)](https://dev.azure.com/dbsafe/dbsafe/_build/latest?definitionId=11&branchName=master)

# Intaker

File Parsing & Data Validation with .Net. It uses a declarative way of defining the file specs. 

Intaker UI is a demo website deployed to 
> AWS: http://intaker-ui.s3-website-us-east-1.amazonaws.com/

> Azure: http://intaker-demo-webapp.azurewebsites.net/

Other demo projects
https://github.com/dbsafe/intaker-demo

## Dependencies
#### Intaker library uses
- .Net Core
#### Demo UI uses
- .Net Core
- Blazor
- MatBlazor - https://www.matblazor.com/
- Tabulator - http://tabulator.info/
- Ace Editor - https://ace.c9.io/

## Basic Idea
The basic idea is that a file specification can be declared using a XML file.

Example:\
This is a text file that contains balances.
It contains one `HEADER` line, several `BALANCE` lines, and one `TRAILER` line.

```
HEADER,09212013,ABCDCompLndn,0001
BALANCE,1001,111-22-1001,fname-01,lname-01,10212000,1000.00,AA
BALANCE,1002,111-22-1002,fname-02,lname-02,10222000,2000.00,
BALANCE,1003,111-22-1003,fname-03,lname-03,10232000,3000.00,
TRAILER,6000.00,3
```

The file specification can be defined in a XML file

```xml
<?xml version="1.0" encoding="utf-8"?>
<inputDataDefinition name="Balance" version="1.0" description="Demo file" delimiter="," hasFieldsEnclosedInQuotes="false" frameworkVersion="1.0" createRowJsonEnabled="true">
  <header>
    <fields>
      <field name="RecordType" description="Record Type (Header Row)" decoder="TextDecoder" pattern="HEADER" />
      <field name="CreationDate" description="Creation Date" decoder="DateDecoder" pattern="MMddyyyy" />
      <field name="LocationID" description="Location ID" decoder="TextDecoder" pattern="[a-zA-Z]{12}" />
      <field name="SequenceNumber" description="Sequence Number" decoder="IntegerDecoder" pattern="(?!0{4})[0-9]{4}">
        <rules>
          <rule name="SequenceNumber-MinNumberFieldRule" rule="MinNumberFieldRule" description="Sequence number should equal or greater than 1" arg="1" failValidationResult="Warning" />
          <rule name="SequenceNumber-MaxNumberFieldRule" rule="MaxNumberFieldRule" description="Sequence number should be equal or less than 100" arg="100" />
        </rules>
      </field>
    </fields>
  </header>
  <data>
    <fields>
      <field name="RecordType" description="Record Type (Data Row)" decoder="TextDecoder" pattern="BALANCE" />
      <field name="ConsumerID" description="Consumer ID" decoder="IntegerDecoder" pattern="[0-9]{1,10}" />
      <field name="SSN" description="SSN" decoder="TextDecoder" pattern="\d{3}-\d{2}-\d{4}" />
      <field name="FirstName" description="First Name" decoder="TextDecoder" pattern="[a-zA-Z0-9\s-']{2,35}" />
      <field name="LastName" description="LastName" decoder="TextDecoder" pattern="[a-zA-Z0-9\s-']{2,35}" />
      <field name="DOB" description="DOB" decoder="DateDecoder" pattern="MMddyyyy" failValidationResult="Warning" />
      <field name="Balance" description="Amount" decoder="DecimalDecoder" pattern="-{0,1}[0-9]{1,10}\.[0-9]{2}">
        <aggregators>
          <aggregator name="BalanceAggregator" description="Balance aggregator" aggregator="SumAggregator" />
          <aggregator name="DataRowCountAggregator" description="Data row counter" aggregator="RowCountAggregator" />
        </aggregators>
      </field>
      <field name="CustomField" description="Custom Field without validation" />
    </fields>
  </data>
  <trailer>
    <fields>
      <field name="RecordType" description="Record Type (Trailer Line)" decoder="TextDecoder" pattern="TRAILER" />
      <field name="BalanceTotal" description="Balance Total" decoder="DecimalDecoder" pattern="-{0,1}[0-9]{1,10}\.[0-9]{2}">
        <rules>
          <rule name="BalanceTotal-MatchesAggregateRule" rule="MatchesAggregateRule" description="Balance Total is incorrect" arg="BalanceAggregator" failValidationResult="Warning" />
        </rules>
      </field>
      <field name="RecordCount" description="Record Count" decoder="IntegerDecoder" pattern="\d{1,5}">
        <rules>
          <rule name="RecordCount-MatchesAggregateRule" rule="MatchesAggregateRule" description="Record Count should match the number data row" arg="DataRowCountAggregator" />
        </rules>
      </field>
    </fields>
  </trailer>
</inputDataDefinition>
```

The element `<inputDataDefinition>` defines properties about the file. e.g.: Name, Version, Delimiter, etc.

The elements `<header>`, `<data>`, and `<trailer>` define the fields in each line type.


## `<field>` element
Defines a field in a line in the file. Can be used to define fields in header, data, and trailer lines.
  
#### Syntax
```xml
<field name="RecordType" description="Record Type (Header Row)" decoder="TextDecoder" pattern="HEADER" failValidationResult="Error" />
```

#### Attributes
**Attribute** | **Description**
--- | ---
name | Required attribute. Specifies the name of the field.
description | Required attribute. Specifies the description of the field. Used as part of the message when field validation fails.
decoder | Name of the `FieldDecoder` class used when parsing the field. When this value is not specified the field is read without performing any validation.
pattern | Required attribute when `decoder` has a value. It specifies a regular expression used to validate the field.
failValidationResult | Optional. Defines the validation result used when the validation fails. Default "Error".  [See ValidationResultType](#validationresulttype)

### Field Decoder Class

Field decoders are used to parse and to perform format and type validation of a field. 
You can specify the field decoder in the `decoder` attribute of the `<field>` element.

e.g.:
```xml
<field name="SequenceNumber" description="Sequence Number" decoder="IntegerDecoder" pattern="(?!0{4})[0-9]{4}">
```
The previous example defines the field `SequenceNumber` and assigns the field decoder `IntegerDecoder`. The `pattern` attribute defines a regular expression used by the decoder, `(?!0{4})[0-9]{4}` 
defines that expected values are from `0001` to `9999`.

The library implements the standard decoders `TextDecoder`, `IntegerDecoder`, `DecimalDecoder`, and `DateDecoder`. You can define custom decoders and use them in the file definition.

#### Child elements
**Element** | **Description**
--- | ---
`<aggregators>` | Contains aggregators used for aggregating data or for counting all the lines or lines with a specific condition.
`<rules>` | Contains validation rules that are applied to a field.

## `<aggregator>` element
Defines an aggregator used for aggregating data, for counting all the data lines in the file, or for counting the data lines with certain condition. The aggregator is applied to each line as the parsing process traverses the lines in the file.

Aggregators support the validation of the integrity of the data. 

e.g: An aggregator for a field that represents an amount can be used to validate the total amount in the trailer line.

#### Syntax
```xml
<aggregator name="BalanceAggregator" description="Balance aggregator" aggregator="SumAggregator" />
```

#### Attributes
**Attribute** | **Description**
--- | ---
name | Required attribute. Specifies the name of the aggregator.
description | Specifies the description of the aggregator.
aggregator | Name of the `FieldAggregator` class used when aggregating the data.

### Field Aggregator Class
Field aggregators are used to aggregate data or to count lines. 

The library implements the standard aggregators `RowCountAggregator` and `SumAggregator`. You can define custom aggregators and use them in the file definition.

## `<rule>` element
Defines a rule used to validate a field. A field can be validated using multiple rules.

#### Syntax
```xml
<rule name="BalanceTotal-MatchesAggregateRule" rule="MatchesAggregateRule" description="Balance Total is incorrect" arg="BalanceAggregator" failValidationResult="Warning" />
```

#### Attributes
**Attribute** | **Description**
--- | ---
name | Required attribute. Specifies the name of the rule.
description | Specifies the description of the rule.
rule | Name of the `FieldRule` class used when validating the field.
arg  | Argument pased to the rule
failValidationResult | Optional. Defines the validation result used when the validation fails. Default "Error". [See ValidationResultType](#validationresulttype)

### Field Rule Class
The library implements the standard rules `MinNumberFieldRule`, `MaxNumberFieldRule`, `MinDateFieldRule`, `MaxDateFieldRule`, and `MatchesAggregateRule`.

`MatchesAggregateRule` can be used to validate whether a value in the header/trailer matches the value of an aggregator. The name of the aggregator must be passed in the `arg` attribute.
e.g.: `arg="BalanceAggregator"`

You can define custom rules and use them in the file definition.

### ValidationResultType

```cs
public enum ValidationResultType
{
    /// <summary>
    /// Validation succeed.
    /// </summary>
    Valid = 1,

    /// <summary>
    /// Validation failed. Adds flexibility by treating some validation fails as warnings.
    /// </summary>
    Warning = 2,

    /// <summary>
    /// Validation failed.
    /// </summary>
    Error = 3,

    /// <summary>
    /// Validation failed. Causes the decoding process to abort.
    /// </summary>
    Critical = 4
}
```

When creating the definition file the ValidationResultType is used for setting the severity of a failed validation.

