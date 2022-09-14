# Intelligent Contact Sheet Maker - iCSM

On normal execution, you will first be presented with a directory selection dialog. 
Select the directory containing the images you want to use in your contact sheet. Click OK, 
and you’ll get the main userinterface. Here you can set all the parameters you want.
You can skip the directory selection step if you drag an image folder onto the program icon or a shortcut to it.

## Main GUI
If you want to save your settings, you can click `Save Settings`, `File -> Save Settings`, 
or `File -> Save Settings As…` The `Save Settings` options will save to the currently open settings file
(shown next to the button) or *default.xml* if no file is open.
You can later load your settings files with `File -> Load Settings`.
iCSM will always open *default.xml* on load if it exists.
You can view helpful information about most of the options by mouseing over their titles. 
You can find the same information in this document under [Parameters](#Parameters).

## A note on multi-threading
Multi-threading can speed up draw times on sets of large images, especially with multiple processors/cores. 
Users with lower-end computers will probably want to stick with single-threading. 

## File View
Use `Tools -> View Files`, the corresponding button, or ctrl-F to make any changes to the file list. You can
select multiple rows and remove them with the Delete key or the Remove button. If you made a
mistake, click Reload to grab all of the files again. You can double-click on a row to view the image with
your default image viewer.

## Making the Contact Sheet from the GUI
When you’re happy with your settings and the files, click Run in the main UI, select `Tools -> Draw
Sheet`, or hit F5. The `Width` and `Height` columns will be updated with the new thumbnail sizes, and the
drawing progress will be shown in the status bar. Once completed, the directory containing your contact
sheet will be opened in Explorer. The program will remain open until you close it (unless otherwise specified), so you can run again
with different settings, and also change the directory using `Change Directory` in the Tools menu, or the
corresponding button.

## Command Line
iCSM is also a (nearly) fully functional command-line program. In order to use it without the GUI, you
must start it with the `–nogui` option.

*Usage*: `csm [path to image directory] [param1=val1] [param2=val2] [...]`

You can actually put the path anywhere in the command, as long as it's in quotes. If you omit the path,
then the current directory will be used. 
Parameters may be in any order, and they don't all have to be used.

### Parameters
`-nogui` **Don't Launch GUI** *[true/false, Default=False]*. Prevents the GUI (Graphical User Interface) from being launched.\

`-filetype` **File Type** *[Extension, Default=.jpg]*. The extension of the image file type you want to use.\
`-cols` **Maximum Images per Row** *[Number, Default=6]*. The maximum number of images per row. Rows with mixed aspect ratios will probably have fewer images.\
`-mindiminput` **Minimum Input Dimension (Height and Width)** *[px, Default=0]*. The minimum width and height for input images to be included in the output sheet. (Changing this option reloads the file list)\
`-width` **Width of Contact Sheet** *[px, Default=900]*. The width of the contact sheet in pixels.\
`-cols` **Maximum Images per Row** *[Number, Default=6]*. The maximum number of images per row. Rows with mixed aspect ratios will probably have fewer images.\
`-mindim` **Minimum Thumbnail Dimension (Height and Width)** *[px, Default=0]*. All thumbnails' widths and heights will be at least this value.\
`-border` **Image Border Width** *[px, Default=0]*. The border width around an image. The border will be black.\
`-qual` **Image Quality** *[%, Default=90]*. The Jpeg compression quality of the output image.\
`-interp` **Use High Quality Interpolation** *[true/false, Default=True]*. Nicer looking thumbnails, but more taxing on processing. May be slow.\
`-thread` **Use Multi-Threading** *[true/false, Default=False]*. Decreases draw time, may cause unresponsiveness.\
`-tmax` **Maximum Threads** *[Number, Default=4]*. The maximum number of concurrent thumbnail-drawing threads. Decrease if multi-threading is locking up your system, increase if it isn't and you want to go faster.\
`-exit` **Exit on Complete** *[true/false, Default=False]*. Close the GUI when the sheet is finished. Doesn't apply to the command-line.\
`-openoutdir` **Open Output Directory on Complete** *[true/false, Default=True]*. Open the output directory in Windows Explorer when completed (GUI mode only).\
`-preview` **Preview Layout Only** *[true/false, Default=False]*. Only draw the layout, not the images. Near instantaneous.\
`-outfile` **Output File** *[File Path, Default=ContactSheet.jpg]*. File name or path of the output contact sheet file. Path can be absolute or relative to the source directory. (Changing this option reloads the file list)

`-header` **Show Header** *[true/false, Default=False]*. Shows the title of the sheet and other info at the top of the sheet.\
`-hsize` **Header Font Size** *[pt, Default=12]*. The font size of the header text\
`-hbold` **Bold Header** *[true/false, Default=False]*. Makes the header text bold\
`-htitle` **Header Title** *[Words, Default=Title]*. The name of the image set. This is not loaded from settings files.\
`-hstats` **Show Statistics** *[true/false, Default=False]*. Show statistics about the images, e.g. "(x100) max 1200x600px"

`-labels` **Show Image Labels** *[true/false, Default=False]*. Shows the filename for each image.\
`-lsize` **Image Label Font Size** *[pt, Default=8]*. The font size of the image label text

`-cover` **Show Cover** *[true/false, Default=False]*. Show a cover at the top of the sheet. (Changing this option reloads the file list)\
`-cregx` **Cover Pattern** *[Regex, Default=]*. The primary regular expression used to match cover files. (Changing this option reloads the file list).\
`-cfile` **Cover File** *[file, Default=[empty]]*. The path to the file to use as the cover. This is not loaded from settings files. (Changing this option reloads the file list)\
`-cfill` **Attempt to Fill Cover Gaps** *[true/false, Default=False]*. Fill any gaps on the sides of the cover with thumbnails; moves the cover to the left side.

`-sfile` Specify a settings file path.\
`--help` View this help message.

## Tips
iCSM will not recognize image files with the `Hidden` attribute. It will recognize them for covers.
To create a shortcut that you can drag folders onto with your preferred parameters, first create a
shortcut to csm.exe, then right-click the shortcut and select Properties. Once there, add your
parameters to the end of the `Target` field, or just use the Settings file feature to create different
settings files. iCSM will always load *default.xml* at start, but you can load other files afterwards.