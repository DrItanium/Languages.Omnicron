name := Libraries.Extensions.dll 
thisdir := .
cmd_library := -t:library
cmd_out := -out:$(name)
cmd_compiler := dmcs
sources := *.cs 
options := -define:STRING_EXTENSIONS,CURRYING,TESTING_EXTENSIONS,LINQ,MATH_FORMULA,POLYNOMIAL_APPROXIMATION 

build: $(sources)
	dmcs -optimize $(options) $(cmd_library) $(cmd_out) $(sources)
debug: $(sources)
	dmcs -debug $(options) $(cmd_library) $(cmd_out) $(sources)
.PHONY: clean
clean:
	-rm -f *.dll *.mdb
