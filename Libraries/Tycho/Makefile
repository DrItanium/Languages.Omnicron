.RECIPEPREFIX = >
name := Libraries.Tycho.dll 
thisdir := .
cmd_library := -t:library
cmd_out := -out:$(name)
cmd_compiler := dmcs
sources := *.cs Metadata/*.cs
lib_dir := -lib:../LexicalAnalysis/ -lib:../Collections/ -lib:../Extensions/
options := -r:Libraries.Collections.dll -r:Libraries.LexicalAnalysis.dll -r:Libraries.Extensions.dll
result := $(name)

build: $(sources)
> dmcs -optimize $(options) $(lib_dir) $(cmd_library) $(cmd_out) $(sources)
debug: $(sources)
> dmcs -debug $(options) $(lib_dir) $(cmd_library) $(cmd_out) $(sources)
.PHONY : clean
clean: 
> -rm -f *.dll *.mdb
