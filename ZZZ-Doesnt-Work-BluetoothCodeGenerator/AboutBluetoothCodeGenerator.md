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

### FileName=VALUE

### Trim=true|false