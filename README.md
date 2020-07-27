# data-processor

This repository contains libraries that support parsing and validating data from a text file. 

It started as a POC for some ideas about importing data using a declarative way of defining the file specs. 
It is not ready yet, but if you often implement new file specifications in your projects and had asked whether there is a more productive way of doing it, you are not alone and I believe this is a good approach to consider, stay tuned. 

Even more, if you have ideas that you feel could be part of this project and you want to share and/or contribute you are welcome to do so.

## Basic Idea
The basic idea is that a file specification can be expressed in a declarative file.

Example:\
This is a text file that contains balances.
It contains one `HEADER` line, several `BALANCE` lines, and one `TRAILER` line.

```
HEADER,09212013,ABCDCompLndn,0001
BALANCE,1001,111-22-1001,fname-01,lname-01,10212000,1000.00
BALANCE,1002,111-22-1002,fname-02,lname-02,10222000,2000.00
BALANCE,1003,111-22-1003,fname-03,lname-03,10232000,3000.00
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
          <rule name="SequenceNumber-MinNumberFieldRule" rule="MinNumberFieldRule" description="Minimum sequence number should be 10" args="{'ruleValue':'10'}" isFixable="true"/>
          <rule name="SequenceNumber-MaxNumberFieldRule" rule="MaxNumberFieldRule" description="Maximum sequence number should be 100" args="{'ruleValue':'100'}" />
        </rules>
      </field>
      <field name="Optional" description="Optional Field" />
    </fields>
  </header>
  <data>
    <fields>
      <field name="RecordType" description="Record Type (Data Row)" decoder="TextDecoder" pattern="BALANCE" />
      <field name="ConsumerID" description="Consumer ID" decoder="IntegerDecoder" pattern="[0-9]{1,10}" />
      <field name="SSN" description="SSN" decoder="TextDecoder" pattern="\d{3}-\d{2}-\d{4}" />
      <field name="FirstName" description="First Name" decoder="TextDecoder" pattern="[a-zA-Z0-9\s-']{2,35}" />
      <field name="LastName" description="LastName" decoder="TextDecoder" pattern="[a-zA-Z0-9\s-']{2,35}" />
      <field name="DOB" description="DOB" decoder="DateDecoder" pattern="MMddyyyy" />
      <field name="Balance" description="Amount" decoder="DecimalDecoder" pattern="-{0,1}[0-9]{1,10}\.[0-9]{2}">
        <aggregators>
          <aggregator name="BalanceAggregator" description="Balance aggregator" aggregator="SumAggregator" />
          <aggregator name="DataRowCountAggregator" description="Data row counter" aggregator="RowCountAggregator" />
        </aggregators>
      </field>
    </fields>
  </data>
  <trailer>
    <fields>
      <field name="RecordType" description="Record Type (Trailer Line)" decoder="TextDecoder" pattern="TRAILER" />
      <field name="BalanceTotal" description="Sum of all balances" decoder="DecimalDecoder" pattern="-{0,1}[0-9]{1,10}\.[0-9]{2}" />
      <field name="RecordCount" description="Record Count" decoder="IntegerDecoder" pattern="\d{1,5}" />
    </fields>
  </trailer>
</inputDataDefinition>
```

The element `<inputDataDefinition>` defines properties about the file. e.g.: Name, Version, Delimiter, etc.

The elements `<header>`, `<data>`, and `<trailer>` define the fields in each line type.

The elements `<field>` dfines a field in a the line.\
e.g.:
```xml
<field name="RecordType" description="Record Type (Header Row)" decoder="TextDecoder" pattern="HEADER" />
```
Defines the `Record Type` field instructing the file processor to use the type TextDecoder when parsing the value in the file. 
The pattern attribute is used by TextDecoder when parsing and validating the value. In this case the expected value is `HEADER`

```xml
<field name="SequenceNumber" description="Sequence Number" decoder="IntegerDecoder" pattern="(?!0{4})[0-9]{4}">
```
Defines the ` Sequence Number` field and the pattern is set with the regular expression ` (?!0{4})[0-9]{4}`. The regular expression is used to define the possible values from `0001` to `9999`.

