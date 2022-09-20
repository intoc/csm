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
select multiple rows and remove them with the Delete key or the Remove button. If you made a
mistake, click Reload to grab all of the files again. You can double-click on a row to view the image with
your default image viewer.

## Making the Contact Sheet from the GUI
When you’re happy with your settings and the files, click `Run`, select `Tools -> Draw
Sheet`, or hit F5. The `Width` and `Height` columns will be updated with the new thumbnail sizes, and the
drawing progress will be shown in the status bar. Once completed, the directory containing your contact
sheet will be opened in Explorer. The program will remain open until you close it (unless otherwise specified), so you can run again
with different settings, and also change the directory using `Choose Folder` or `Choose Archive` in the Tools menu, or the
corresponding button.

## Command Line
csm is also a fully functional command-line program. In order to use it without the GUI, you
must start it with the `–nogui` option.

*Usage*: `csm [path to image directory or archive] [param1=val1] [param2=val2] [...]`

You can actually put the path anywhere in the command, as long as it's in quotes. If you omit the path,
then the current executable directory will be used. Parameters may be in any order, and they don't all have to be used.

### Parameters
| Parameter | Name | Type | Default | Description |
| --------- | ---- | ---- | ------- | ----------- |
| `-nogui` | Don't Launch GUI | true/false | false | Prevents the GUI (Graphical User Interface) from being launched.  |
| `-filetype` | File Type | Extension | .jpg | The extension of the image file type you want to use.  |
| `-mindiminput` | Minimum Input Dimension (Height and Width) | px | 0 | The minimum width and height for input images to be included in the output sheet. (Changing this option reloads the file list).  |
| `-width` | Width of Contact Sheet | px | 900 | The width of the contact sheet in pixels.  |
| `-cols` | Maximum Images per Row | Number | 6 | The maximum number of images per row. Rows with mixed aspect ratios will probably have fewer images.  |
| `-mindim` | Minimum Thumbnail Dimension (Height and Width) | px | 0 | All thumbnails' widths and heights will be at least this value.  |
| `-border` | Image Border Width | px | 0 | The border width around an image. The border will be black.  |
| `-qual` | Image Quality | % | 90 | The Jpeg compression quality of the output image.  |
| `-interp` | Use High Quality Interpolation | true/false | true | Nicer looking thumbnails, but more taxing on processing. May be slow.  |
| `-exit` | Exit on Complete | true/false | false | Close the GUI when the sheet is finished. Doesn't apply to the command-line.  |
| `-openoutdir` | Open Output Directory on Complete | true/false | true | Open the output directory in Windows Explorer when completed (GUI mode only).  |
| `-preview` | Preview Layout Only | true/false | false | Only draw the layout, not the images. Near instantaneous.  |
| `-outfile` | Output File | File Path | ContactSheet.jpg | File name or path of the output contact sheet file. Path can be absolute or relative to the source's parent directory. Use {title} to include the header title in the file name. (Changing this option reloads the file list). |
| `-header` | Show Header | true/false | false | Shows the title of the sheet and other info at the top of the sheet.  |
| `-hsize` | Header Font Size | pt | 12 | The font size of the header text.  |
| `-hbold` | Bold Header | true/false | false | Makes the header text bold.  |
| `-htitle` | Header Title | Words | Title | The name of the image set. (Not loaded from settings) |
| `-hstats` | Show Statistics | true/false | false | Show statistics about the images, e.g. "(x100) max 1200x600px".  |
| `-labels` | Show Image Labels | true/false | false | Shows the filename for each image.  |
| `-lsize` | Image Label Font Size | pt | 8 | The font size of the image label text.  |
| `-cover` | Show Cover | true/false | false | Show a cover at the top of the sheet. (Changing this option reloads the file list).  |
| `-cregx` | Cover Pattern | Regex | cover\. | The primary regular expression used to match cover files. (Changing this option reloads the file list).  |
| `-cfile` | Cover File | file | [none] | The path to the file to use as the cover. (Changing this option reloads the file list). (Not loaded from settings) |
| `-cfill` | Attempt to Fill Cover Gaps | true/false | false | Fill any gaps on the sides of the cover with thumbnails; moves the cover to the left side.  |
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
- The `-cregx` parameter uses Regular Expresses. If you want to match on multiple cover name possibilities, you can use something like `(big|cover|max|folder)\.jpg`. 
Try [regexr.com](https://regexr.com) to experiment with Regular Expressions.