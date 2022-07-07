# TODO list

- [x] Read in list of MD files from  directory
- [x] Parse into TemplateSnippet
- [ ] Read in JSON
- [ ] Write out template w/CLASSNAME

# Syntx for the template file

A typical template will look like this:

```
# NAME OPTION=VALUE OPTION=VALUE

Some comments about what the template does
!!```
Template code
!!```

Maybe more comments

## SUBNAME
This template is only used by the upper template

```

You can include sample code blocks by saying that their language is SAMPLE or TEST. Otherwise, extra code blocks are an error.

```

!!```SAMPLE
this code block will be ignored
!!```
```

## Allowed options

### Code="Template Code"

Use this instead of the full template with backticks when you want to have a more condensed set of macros
for your templates

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

Will expand this template only when the If expression is true. The expression, unlike many, must be seperated by spaces. 

Value|Meaning
-----|-----
Source.Length|length of the 

#### Opcodes

Opcode|Meaning
----|----
\>|Greater-than


### Trim=true|false (says whether to trim the CR off of the template)

### Type=list Source=SOURCE

The code will be created from a list of input data. For the Bluetooth devices, this can be 
the Services or Services.Characteristics or the Links values.

Macros created when expanding the list element:

Macro|Value
-----|-----
TEXT|Text of the source
COUNT|Current list element index (starting at 0)
