# TODO list


- [x] V1 is done and functional! 


# What this is

The Bluetooth Code Generator converts JSON descriptions of Bluetooth devices into working code. The code is divided into low-level protocol code to directly connect to a device, and XAML (and the XAML.CS) files to form crude interfaces to manipulate the devices.

The Bluetooth Code Generator is used in conjunction with the Bluetooth Device Controller program.

Problems solved include:

1. Bluetooth requires a lot of common "boilerplate" code; the Bluetooth Code Generator creates those for you
2. Some Bluetooth LE devices have complex characteristics; the JSON descriptions include a mini-language for describing those characteristics. (Note that some devices are more complex than the mini-language can describe)
3. A preliminary version of the JSON can be generated automatically by the Bluetooth Device Controller program.
4. The UX code can generate useful data tables and graphs for numeric data. 
5. The UX code can generate some level of "fancy" code for controlling devices

Problems not solved:
1. Some Bluetooth devices are COM-port based. These are handled by a seperate system of **SerialData** files
2. Some Bluetooth devices are BLE version of COM-port based devices; these are handled with a series of "macro" values are difficult to properly describe and require an expert to create
3. Some Bluetooth devices use advertisements for their data. This requires C# code directly in the Blueooth Device Controller; there's no JSON description of the advertisement format.
4. You can modify the resulting files. However, once modified, the code cannot be usefully regenerated.

# Regenerating code

Use the ```run.bat``` file

# Syntax for the template file

A typical template will always have a format  like this:

```
# NAME OPTION=VALUE OPTION=VALUE

Some comments about what the template does.
There must **always** be a blank line before the template code! Otherwise it doesn't get picked up correctly.

!!```
Template code
!!```

Maybe more comments. The Name and the Options are important in the processing; the NAME will be the template name and the vawrious options say how to apply the template. For example, a file template needs to be at the top level and have a FileName=_value_ where the _value_ can include any of the macro values.

## SUBNAME
This template is only used by the upper template

```

You can include sample code blocks by saying that their language is SAMPLE or TEST. Otherwise, extra code blocks are an error.

```

!!```SAMPLE
this code block will be ignored
!!```
```

## Expansion types

There are multiple ways expansion can work

Type|Where placed|Details
-----|-----|-----|-----
Global|top-level|This is the default and doesn't need any options
Global-List|top-level|Done when Type=list and a Source is provided. Each child in the Source list will be visited and expanded, and the results all concatenated together. This is used by, e.g., the LINKS to make a single top-level (global) list of links.
Parent-List|above the child|Done when Type=list and a Source is provided and ListOutput=parent
Child|at the child level|Done when Type=list and a Source is provided and the ListOutput=child. 

## Allowed options

### Code="Template Code"

Use this instead of the full template with backticks when you want to have a more condensed set of macros
for your templates

### CodeListSeparator=", "

Used to separate list elements

### CodeListSubZero="Template code"

This is used when Type=list when the list has no items. If the expansion source is a dotted one (e.g., does a Services.Characteristics), this works on the bottommost expansion. 

See also CodeListZero="Template code"

### CodeListZero="Template code"

This is used when Type=list when the list has no items. If the expansion source is a dotted one (e.g., does a Services.Characteristics), this works on the overall expansion. 

See also CodeListSubZero="Template code"


### CodeWrap="Template code [[TEXT]] with embed"

The CodeWrap is generally used with lists. It lets you surround the resulting list with code. The list code will always be called TEXT.

### FileName=VALUE (always on the main template)

### If="Source.Length > 0"

Quick and simple If statement for expansion. Will expand this template only when the If expression is true. The expression, unlike many, must be seperated by spaces. Strings are just strings, which makes dealing with spaces impossible (for example, you can't have an If that compares to the string "example string with spaces".

Example: If="[[Verb]] contains :RdInNo:"

#### Special values

Value|Meaning
-----|-----
Source.Length|length of the replaced text


#### Numeric Opcodes

Opcode|Meaning
----|----
\>|Greater-than

#### String Opcodes

Opcode|Meaning
----|----
==|Equals
!=|Not-equals
contains|The left string contains the right string. Compare is case-insensitive
!contains|The left string does not contain the right string. Compare is case-insensitive
length\>|Length of left-hand macro is greater-than right hand value

### ListOutput=parent or child or global (default)

Says where to place the result of expanding a Type=list expansion


### Trim=true|false (d=false) says whether to trim the CR off of the template

Sometimes the template expansion should include the CR at the and of the code block, and sometimes not. Use the Trim option to set the way you need it. The default is **false**, so it's often only applied when you need a Trim=true. 

### Type=list Source=SOURCE

The code will be created from a list of input data. For the Bluetooth devices, this can be 
the Services or Services/Characteristics or the Links values or several other speciality values.

Macros created when expanding the list element:

Macro|Value
-----|-----
TEXT|Text of the source
COUNT|Current list element index (starting at 0)
Child.Count|Current list element for just the bottom level of expansion

If there's an If expression and it's false, the counts will not be incremented. 
