# data-processor

This repository contains libraries that support parsing and validating data from a text file. 

It started as a POC for some ideas about importing data using a declarative way of defining the file specs. 
It is not ready yet, but if you often implement new file specifications in your projects and had asked whether there is a more productive way of doing it, you are not alone and I believe this is a good approach to consider, stay tuned. 

Even more, if you have ideas that you feel could be part of this project and you want to share and/or contribute you are welcome to do so.

## Basic Idea
The basic idea is that a file specification should be expressed in a declarative file.

Example:\
This is a file than contains data about a consumer and balance.
It contains one `HEADER` line, several `BALANCE` lines, and one `TRAILER` line.

```
HEADER,09212013,ABCDCompLndn,0001
BALANCE,1001,111-22-1001,fname-01,lname-01,10212000,1000.00
BALANCE,1002,111-22-1002,fname-02,lname-02,10222000,2000.00
BALANCE,1003,111-22-1003,fname-03,lname-03,10232000,3000.00
TRAILER,6000.00,3
```
