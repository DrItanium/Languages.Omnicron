.RECIPEPREFIX = >
root:=$(CURDIR)
name := Omnicron.exe
cmd_out := -out:$(name)
cmd_compiler := dmcs
sources := *.cs 
lib_dir := -lib:../../Libraries/LexicalAnalysis/ \
           -lib:../../Libraries/Collections/ \
           -lib:../../Libraries/Extensions/ \
			  -lib:../../Libraries/Starlight/ \
			  -lib:../../Libraries/Tycho/ \
			  -lib:../../Libraries/Parser/ \
			  -lib:../../
options := -r:Libraries.Collections.dll \
           -r:Libraries.LexicalAnalysis.dll \
			  -r:Libraries.Extensions.dll \
			  -r:Libraries.Starlight.dll \
			  -r:Libraries.Tycho.dll \
			  -r:Libraries.Parser.dll \
			  -r:System.Windows.Forms.dll \
			  -r:System.Drawing.dll \
			  -r:System.Data.dll \
			  -r:Languages.Omnicron.dll 

build: copy
> dmcs -optimize $(options) $(lib_dir) $(cmd_out) $(sources)
debug: copy-debug
> dmcs -debug $(options) $(lib_dir) $(cmd_out) $(sources)

.PHONY : clean
clean: 
> -rm -f *.dll *.mdb *.exe

omnicron: 
> cd $(root)/../../ && $(MAKE)

omnicron-debug: 
> cd $(root)/../../ && $(MAKE) debug

copy: omnicron 
> cd $(root)/../../ && cp *.dll $(root)/;

copy-debug: omnicron-debug
> cd $(root)/../../ && cp *.dll *.mdb $(root)/;
		
