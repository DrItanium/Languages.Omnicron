.RECIPEPREFIX = >
root:=$(CURDIR)
name := Languages.Omnicron.dll 
thisdir := .
cmd_library := -t:library
cmd_out := -out:$(name)
cmd_compiler := dmcs
sources := *.cs 
lib_dir := -lib:../LexicalAnalysis/ \
           -lib:../Collections/ \
           -lib:../Extensions/ \
			  -lib:../Starlight/ \
			  -lib:../Tycho/ \
			  -lib:../Parser/ 
options := -r:Libraries.Collections.dll \
           -r:Libraries.LexicalAnalysis.dll \
			  -r:Libraries.Extensions.dll \
			  -r:Libraries.Starlight.dll \
			  -r:Libraries.Tycho.dll \
			  -r:Libraries.Parser.dll \
			  -r:System.Windows.Forms.dll \
			  -r:System.Drawing.dll \
			  -r:System.Data.dll \
			  -win32res:OmnicronDialogForm.resx
result := $(name)

build: copy
> dmcs -optimize $(options) $(lib_dir) $(cmd_library) $(cmd_out) $(sources)
debug: copy 
> dmcs -debug $(options) $(lib_dir) $(cmd_library) $(cmd_out) $(sources)
.PHONY : clean
clean: 
> -rm -f *.dll *.mdb *.exe

collections: extensions 
> cd $(root)/Libraries/Collections && $(MAKE) 

starlight: 
> cd $(root)/Libraries/Starlight && $(MAKE) 

tycho: lexical 
> cd $(root)/Libraries/Tycho && $(MAKE) 

parser: tycho starlight  
> cd $(root)/Libraries/Parser && $(MAKE)

extensions: 
> cd $(root)/Libraries/Extensions && $(MAKE)

lexical: collections 
> cd $(root)/Libraries/LexicalAnalysis && $(MAKE)

copy: parser 
> cd $(root)/Libraries/LexicalAnalysis/ && cp *.dll $(root)/; \
> cd $(root)/Libraries/Extensions/ && cp *.dll $(root)/; \
> cd $(root)/Libraries/Collections/ && cp *.dll $(root)/; \
> cd $(root)/Libraries/Starlight/ && cp *.dll $(root)/; \
> cd $(root)/Libraries/Tycho/ && cp *.dll $(root)/; \
> cd $(root)/Libraries/Parser/ && cp *.dll $(root)/; 
		
