# Intelligent Contact Sheet Maker - csm
This program is used to make good-looking "Contact Sheets," or image files containing
thumbnail representations of a list of images. It has a GUI and command-line interface. You can start the GUI
by simply running csm.exe. Supported inputs are directories, .zip, .7z, and .rar files.

## Main GUI
If you want to save your settings, you can click `Save Settings`, `File -> Save Settings`, 
or `File -> Save Settings As…` The `Save Settings` options will save to the currently open settings file
(shown next to the button) or *default.xml* if no file is open.
You can later load your settings files with `File -> Load Settings`.
iCSM will always open *default.xml* on load if it exists.
You can view helpful information about most of the options by mousing over their titles. 
You can find the same information in this document under [Parameters](#Parameters).

## File View
Use `Tools -> View Files`, the corresponding button, or ctrl-F to make any changes to the file list. You can
select multiple rows and exclude them with the Delete key or the `Remove` button. Click `Reset` to rest to the default
inclusion for all files. The selected cover file and any files that are similarly named to the output file are excluded automatically.

## Making the Contact Sheet from the GUI
When you’re happy with your settings and the files, click `Run`, select `Tools -> Draw
Sheet`, or hit F5. The `Size` column will be updated with the new thumbnail sizes, and the
drawing progress will be shown in the status bar. The program will remain open until you close it (unless otherwise specified), 
so you can run again with different settings, and also change the directory using `Choose Folder` or `Choose Archive` in the Tools menu, 
or the corresponding button.

## Batch Processing
Once you like the way your output files look and you want to make a lot of them with the
same settings at once, open up the Batch Processing GUI from the `Tools` menu or hit `ctrl-B`.
Here you can add all of the items (archives and subdirectories) in a directory, or select
single or multiple archive files and add them to the list for processing. 

Set the `Max Concurrent Load` and `Max Cuncurrent Draw` spinners to your liking; 
lower numbers will put less strain on your computer
and larger numbers will make the process go faster (to a point.) 

You can also remove sources
from the list with the `Remove Selected` button or the `Del` key, as long as they are in
either the `PreLoad` or `Completed` state.

The `Pause` button will moving states from `PreLoad` to `Loading` or `Queued` to `Drawing`, 
but sources already in the `Loading` state will continue to load in the background, 
and sources already in the `Drawing` state will continue to draw in the background.


## Command Line
csm is also a fully functional command-line program. In order to use it without the GUI, you
must start it with the `–nogui` option.

*Usage*: `csm [path to image directory or archive] [param1=val1] [param2=val2] [...]`

You can actually put the path anywhere in the command, as long as it's in quotes. If you omit the path,
then the current executable directory will be used. Parameters may be in any order, and they don't all have to be used.

For Windows users who would like to do some batch processing without the GUI, there is an example batch file `batch.bat`. 
Open it in a text editor before trying to use it.

### Parameters
| Parameter | Name | Type | Default | Description |
| --------- | ---- | ---- | ------- | ----------- |
| `-fregx` | File Name Pattern | Regex | \\.(jpg\|jpeg\|png)$ | A regular expression used to match on file names to be included from the source.  |
| `-mindiminput` | Minimum Input Dimension (Height and Width) | px | 0 | The minimum width and height for input images to be included in the output sheet. (Changing this option reloads the file list).  |
| `-width` | Width of Contact Sheet | px | 900 | The width of the contact sheet in pixels.  |
| `-cols` | Maximum Images per Row | Number | 6 | The maximum number of images per row. Rows with mixed aspect ratios will probably have fewer images.  |
| `-mindim` | Minimum Thumbnail Dimension (Height and Width) | px | 0 | All thumbnails' widths and heights will be at least this value.  |
| `-border` | Image Border Width | px | 0 | The border width around an image. The border will be black.  |
| `-qual` | Image Quality | % | 90 | The Jpeg compression quality of the output image.  |
| `-exit` | Exit on Complete | true/false | false | Close the GUI when the sheet is finished. Doesn't apply to the command-line.  |
| `-openoutdir` | Open Output Directory on Complete | true/false | true | Open the output directory in Windows Explorer when completed (GUI mode only).  |
| `-preview` | Preview Layout Only | true/false | false | Only draw the layout, not the images. Near instantaneous.  |
| `-outfile` | Output File | File Path | ContactSheet.jpg | File name or path of the output contact sheet file. Path can be absolute or relative to the source's parent directory. Use {title} to include the header title in the file name. (Changing this option reloads the file list).  |
| `-header` | Show Header | true/false | false | Shows the title of the sheet and other info at the top of the sheet.  |
| `-hsize` | Header Font Size | pt | 12 | The font size of the header text.  |
| `-hbold` | Bold Header | true/false | false | Makes the header text bold.  |
| `-htitle` | Header Title | Words | Title | The name of the image set. (Not loaded from settings) |
| `-hstats` | Show Statistics | true/false | false | Show statistics about the images, e.g. "(x100) max 1200x600px".  |
| `-labels` | Show Image Labels | true/false | false | Shows the filename for each image.  |
| `-lsize` | Image Label Font Size | pt | 8 | The font size of the image label text.  |
| `-cover` | Show Cover | true/false | false | Show a cover at the top of the sheet. (Changing this option reloads the file list).  |
| `-cregx` | Cover Pattern | Regex | cover\\. | The primary regular expression used to match cover files. (Changing this option reloads the file list).  |
| `-cfile` | Cover File | file | [none] | The path to the file to use as the cover. (Changing this option reloads the file list). (Not loaded from settings) |
| `-cfill` | Attempt to Fill Cover Gaps | true/false | false | Fill any gaps on the sides of the cover with thumbnails; moves the cover to the left side.  |
| `-cmaxw` | Maximum cover width | % | 75 | The maximum percentage of the sheet width that the cover's width can be (approximate)  |
| `-sfile` | Settings file path | File Path | default.aspx | The path to a settings file. Can be absolute or relative. |
| `-help` | View help message (no value required) | None | N/A | Show a help message on the command line with parameter documentation. |

## Tips
- csm will not recognize image files with the `Hidden` attribute. It will recognize them for covers.
- To create a shortcut that you can drag folders onto with your preferred parameters, first create a
shortcut to csm.exe, then right-click the shortcut and select Properties. Once there, add your
parameters to the end of the `Target` field, e.g. `C:\csm\csm.exe -nogui=true -sfile=mysettings.xml`.
The source path will be passed in automatically if you drag the folder or archive onto the shortcut.
- The csm GUI will always try to load *default.xml* at start if `-sfile` isn't specified in the command line arguments, but you can load other files afterwards.
- Use `{title}` in `-outfile` to include the archive/directory/header title in the output file name or path. For example,
`-outfile="../{title} CS.jpg"` will put the output file in the parent directory of the directory containing the source file or directory, with a name like "My Favorite Photos CS.jpg".
- All file sources are checked for images recursively. If your sources contain small files that
you don't want to include in the output, use the `-mindiminput` parameter to filter out small images.
- The `-cregx` parameter uses Regular Expressions. If you want to match on multiple cover name possibilities, 
you can use something like `(cover|big|max|square|folder)\.(jpg|png)$`. 
- The `-fregx` parameter also uses Regular Expressions so you can match on multiple file types.
- Try [regexr.com](https://regexr.com) to experiment with Regular Expressions.
- The appsettings.json file is used for configuration that doesn't make sense to include in the sheet settings. Right now
it only has a Serilog node, which can be modified to change the logging template or logging level. csm logs at **Debug**, **Information**, and **Error** levels.
Setting `MinimumLevel` to **Error** may increase performance in GUI mode, but there won't be much output on the console.

## Linux
The linux command-line only version of this app requires the dotnet 6 runtime. 
[This](http://anonym.es/?https://learn.microsoft.com/en-us/dotnet/core/install/linux-scripted-manual#manual-install) appears to be the most reliable way to install it.

You may also need to install these packages with apt-get:
- libc6-dev
- libgdiplus
- libx11-dev
