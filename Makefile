.RECIPEPREFIX = >
root:=$(CURDIR)
name := Languages.Omnicron.dll 
thisdir := .
cmd_library := -t:library
cmd_out := -out:$(name)
cmd_compiler := dmcs
sources := *.cs 
lib_dir := $(root)/Libraries

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
> dmcs -optimize $(options) $(cmd_library) $(cmd_out) $(sources)
debug: copy 
> dmcs -debug $(options) $(cmd_library) $(cmd_out) $(sources)

clean: 
> -rm -f *.dll *.mdb *.exe
> cd $(lib_dir)/Collections && $(MAKE) clean
> cd $(lib_dir)/LexicalAnalysis && $(MAKE) clean
> cd $(lib_dir)/Extensions && $(MAKE) clean
> cd $(lib_dir)/Starlight && $(MAKE) clean
> cd $(lib_dir)/Tycho && $(MAKE) clean
> cd $(lib_dir)/Parser && $(MAKE) clean

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
> cd $(lib_dir)/LexicalAnalysis/ && cp *.dll $(root)/; \
> cd $(lib_dir)/Extensions/ && cp *.dll $(root)/; \
> cd $(lib_dir)/Collections/ && cp *.dll $(root)/; \
> cd $(lib_dir)/Starlight/ && cp *.dll $(root)/; \
> cd $(lib_dir)/Tycho/ && cp *.dll $(root)/; \
> cd $(lib_dir)/Parser/ && cp *.dll $(root)/; 
		
