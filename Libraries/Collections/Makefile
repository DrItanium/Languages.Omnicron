name := Libraries.Collections.dll 
thisdir := .
cmd_library := -t:library
cmd_out := -out:$(name)
cmd_compiler := dmcs
sources := *.cs 
lib_dir := -lib:../Extensions/
options := -r:Libraries.Extensions.dll
build: $(sources)
	dmcs -optimize $(options) $(lib_dir) $(cmd_library) $(cmd_out) $(sources)
debug: $(sources)
	dmcs -debug $(options) $(lib_dir) $(cmd_library) $(cmd_out) $(sources)
.PHONY: clean
clean: 
	-rm -f *.dll *.mdb
