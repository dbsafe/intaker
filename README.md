# data-processor

This repository contains libraries that support parsing and validating data from a text file. 

It started as a POC for some ideas about importing data using a declarative way of defining the file specs. 
It is not ready yet, but if you often implement new file specifications in your projects and had asked whether there is a more productive way of doing it, you are not alone and I believe this is a good approach to consider, stay tuned. 

Even more, if you have ideas that you feel could be part of this project and you want to share and/or contribute you are welcome to do so.

## Basic Idea
The basic idea is that a file specification can be declared in a file.

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
          <rule name="SequenceNumber-MinNumberFieldRule" rule="MinNumberFieldRule" description="Sequence number should equal or greater than 1" args="{'ruleValue':'1'}" isFixable="true"/>
          <rule name="SequenceNumber-MaxNumberFieldRule" rule="MaxNumberFieldRule" description="Sequence number should be equal or less than 100" args="{'ruleValue':'100'}" />
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
      <field name="DOB" description="DOB" decoder="DateDecoder" pattern="MMddyyyy" isFixable="true"/>
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
          <rule name="BalanceTotal-MatchesAggregateRule" rule="MatchesAggregateRule" description="Balance Total is incorrect" args="{'ruleValue':'BalanceAggregator'}" isFixable="true"/>
        </rules>
      </field>
      <field name="RecordCount" description="Record Count" decoder="IntegerDecoder" pattern="\d{1,5}">
        <rules>
          <rule name="RecordCount-MatchesAggregateRule" rule="MatchesAggregateRule" description="Record Count should match the number data row" args="{'ruleValue':'DataRowCountAggregator'}" />
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
<field name="RecordType" description="Record Type (Header Row)" decoder="TextDecoder" pattern="HEADER" />
```

#### Attributes
**Attribute** | **Description**
--- | ---
name | Required attribute. Specifies the name of the field.
description | Required attribute. Specifies the description of the field. Used as part of the message when field validation fails.
decoder | Name of the `FieldDecoder` class used when parsing the field. When this value is not specified the field is read without performing any validation.
pattern | Required attribute when `decoder` has a value. It specifies the regular expression used to validate the field.

### Field Decoders

Field decoders are used to parse and to perform format and type validation of a field. 
You can specify the field decoder in the `decoder` attribute of the `<field>` element.

e.g.:
```xml
<field name="SequenceNumber" description="Sequence Number" decoder="IntegerDecoder" pattern="(?!0{4})[0-9]{4}">
```
defines the field `SequenceNumber` and assigns the field decoder `IntegerDecoder`. The `pattern` attrubute defines a regular expression used by the decoder, `(?!0{4})[0-9]{4}` 
defines that expected values are from `0001` to `9999`.

The library implements standard decoders, e.g.: `TextDecoder`, `IntegerDecoder`, `DecimalDecoder`, and `DateDecoder`. You can define custom decoders and use them in the file definition.

#### Child elements
**Element** | **Description**
--- | ---
`<rules>` | Contains validation rules that are applied to a field.
`<aggregators>` | Contains aggregators used for aggregating data or for counting all the lines or lines with a specific condition.

## `<aggregator>` element

