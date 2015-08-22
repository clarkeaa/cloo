FSLEX := $(wildcard FsLexYacc.*/build/fslex.exe)
FSYACC := $(wildcard FsLexYacc.*/build/fsyacc.exe)
FSLEXYACCRUNTIMEPATH := $(wildcard FsLexYacc.Runtime.*/lib/net40)
FSLEXYACCRUNTIME := $(FSLEXYACCRUNTIMEPATH)/FsLexYacc.Runtime.dll

FSC := fsharpc
FSI := fsharpi
FSLEX := mono $(FSLEX)
FSYACC := mono $(FSYACC)
RM := rm -rf

OUTPUTS = Lexer.fs Parser.fs Program.exe

build: $(OUTPUTS)

Lexer.fs: Lexer.fsl
	$(FSLEX) --unicode $<

Parser.fs: Parser.fsy
	$(FSYACC) --module $(basename $<) $<

Program.exe: LispOp.fs LispVal.fs Parser.fs Lexer.fs Program.fs
	$(FSC) --standalone -r $(FSLEXYACCRUNTIME) $^ -o:$@

run: Program.exe 
	mono $<

get-fslexyacc:
	nuget install FsLexYacc 

.PHONY:
clean:
	$(RM) $(OUTPUTS) *~
