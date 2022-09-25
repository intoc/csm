:: Param 1: Directory path (no trailing slash)
:: Param 2: Archive extension (with dot). Omit to process all files in the directory.
:: Change csm parameters to whatever you want
for %%v in ("%~1\*%~2") do csm "%%v" -nogui=true -outfile=ContactSheets\{title}.jpg -border=2 -cover=true -cfill=true -cmaxw=45 -width=1920 -cols=8 -qual=90 -header=true -hsize=24 -hstats=true 