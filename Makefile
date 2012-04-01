.RECIPEPREFIX = >
name := Languages.Omnicron.dll 
thisdir := .
cmd_library := -t:library
cmd_out := -out:$(name)
cmd_compiler := dmcs
sources := *.cs 
lib_dir := -lib:../LexicalAnalysis/ \
           -lib:../Collections/ \
           -lib:../Extensions/ \
			  -lib:../../Frameworks/Plugin/ \
			  -lib:../Starlight/ \
			  -lib:../Tycho/ \
			  -lib:../LexicalAnalysis/ \
			  -lib:../Parsing/ \
			  -lib:../Messaging/
options := -r:Libraries.Collections.dll \
           -r:Libraries.LexicalAnalysis.dll \
			  -r:Libraries.Extensions.dll \
			  -r:Libraries.Collections.dll \
			  -r:Frameworks.Plugin.dll \
			  -r:Libraries.Starlight.dll \
			  -r:Libraries.Tycho.dll \
			  -r:Libraries.Parsing.dll \
			  -r:Libraries.Messaging.dll \
			  -r:System.Windows.Forms.dll \
			  -r:System.Drawing.dll \
			  -r:System.Data.dll \
			  -win32res:.resx
result := $(name)

build: $(sources)
> dmcs -optimize $(options) $(lib_dir) $(cmd_library) $(cmd_out) $(sources)
debug: $(sources)
> dmcs -debug $(options) $(lib_dir) $(cmd_library) $(cmd_out) $(sources)
.PHONY : clean
clean: 
> -rm -f *.dll *.mdb
