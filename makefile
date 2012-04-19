LOG=build.log

ifeq (${LANG}, ja_JP.UTF-8)
	CAT=iconv -f sjis -t utf-8 ${LOG}
else
	CAT=type ${LOG}
endif

PREFIX=..

TARGET=MdNote
SLN=${TARGET}.sln
BIN=${TARGET}.exe
BINDIR=${TARGET}/bin/Debug

DOC=readme.html
IMAGESDIR=images
IMAGES=${IMAGESDIR}/*

VER=`grep AssemblyVersion ${TARGET}/Properties/AssemblyInfo.cs | sed 's/.*"\(.*\)".*/\1/'`

all:
	@/c/WINDOWS/Microsoft.NET/Framework/v3.5/MSBuild.exe ${SLN} > ${LOG}; $(CAT)

run:
	@${BINDIR}/${BIN} &

${DOC}: readme.md
	@./bin/markdown2html.sh ${DOC}

doc: ${DOC}

clean:
	rm -f ${BIN} mdnote-*.zip ${DOC} ${LOG}
