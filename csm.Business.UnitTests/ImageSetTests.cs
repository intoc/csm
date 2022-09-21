using csm.Business.Models;
using Moq;

namespace csm.Business.Logic.UnitTests;

public class ImageSetTests {

    [Fact]
    public async Task LoadImageListAsync_DiscludesExpectedFiles() {
        Mock<IFileSource> source = new();
        ImageSet set = new() {
            Source = source.Object
        };
        source.Setup(mock => mock.GetFilesAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<ImageFile> {
                new ImageFile("file.jpg"),
                new ImageFile("cover.jpg"),
                new ImageFile("sheet.jpg"),
                new ImageFile("sheet_1.jpg"),
                new ImageFile("hidden.jpg", true)
            });

        await set.LoadImageListAsync("_", 0, "sheet.jpg", "cover.jpg");

        Assert.Contains(set.Images, (image) => image.FileName == "file.jpg" && image.Include);
        Assert.Contains(set.Images, (image) => image.FileName == "cover.jpg" && !image.Include);
        Assert.Contains(set.Images, (image) => image.FileName == "sheet.jpg" && !image.Include);
        Assert.Contains(set.Images, (image) => image.FileName == "sheet_1.jpg" && !image.Include);
        Assert.DoesNotContain(set.Images, (image) => image.FileName == "hidden.jpg");
    }

    [Fact]
    public async Task LoadImageListAsync_GetsImageDimensionsFromSourceForAllNonHiddenImages() {
        Mock<IFileSource> source = new();
        ImageSet set = new() {
            Source = source.Object
        };
        source.Setup(mock => mock.GetFilesAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<ImageFile> {
                new ImageFile("file.jpg"),
                new ImageFile("cover.jpg"),
                new ImageFile("sheet.jpg"),
                new ImageFile("sheet_1.jpg"),
                new ImageFile("hidden.jpg", true)
            });

        await set.LoadImageListAsync("_", 0, "sheet.jpg", "cover.jpg");

        source.Verify(mock => mock.LoadImageDimensions(It.Is<ImageData>((image) => set.Images.Contains(image))), Times.Exactly(4));
    }

    [Fact]
    public void SourceChanged_OldSourceIsDisposed() {
        Mock<IFileSource> source = new();
        ImageSet set = new() {
            Source = source.Object
        };

        Mock<IFileSource> source2 = new();
        set.Source = source2.Object;

        source.Verify(mock => mock.Dispose(), Times.Once());
        source2.Verify(mock => mock.Dispose(), Times.Never());
    }

    [Fact]
    public async Task GuessFile_MatchesOnFirstMatchingFile() {
        Mock<IFileSource> source = new();
        ImageSet set = new() {
            Source = source.Object
        };
        source.Setup(mock => mock.GetFilesAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<ImageFile> {
                new ImageFile("cover-clean.jpg"),
                new ImageFile("cover.jpg"),
                new ImageFile("clean-cover.jpg")
            });

        FileParam fParam = new("-cfile", source.Object);

        await set.GuessFile(fParam, "_", @"cover\.jpg");

        Assert.Equal("cover.jpg", fParam.Path);
    }

    [Fact]
    public async Task GuessFile_SelectsFirstImageInSetIfNoMatch() {
        Mock<IFileSource> source = new();
        ImageSet set = new() {
            Source = source.Object
        };
        source.Setup(mock => mock.GetFilesAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<ImageFile> {
                new ImageFile("cover-clean.jpg"),
                new ImageFile("cover.jpg"),
                new ImageFile("clean-cover.jpg")
            });

        FileParam fParam = new("-cfile", source.Object);

        await set.GuessFile(fParam, "_", @"(cover\.png|cover\.gif)");

        Assert.Equal("cover-clean.jpg", fParam.Path);
    }

    [Fact]
    public async Task GuessFile_Forced_OverridesPreviousFile() {
        Mock<IFileSource> source = new();
        ImageSet set = new() {
            Source = source.Object
        };
        source.Setup(mock => mock.GetFilesAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<ImageFile> {
                new ImageFile("cover.jpg")
            });

        FileParam fParam = new("-cfile", source.Object) {
            File = new ImageFile("original.jpg")
        };

        bool force = true;

        await set.GuessFile(fParam, "_", @"(cover\.jpg|cover\.gif)", force);

        Assert.Equal("cover.jpg", fParam.Path);
    }

    [Fact]
    public async Task GuessFile_NotForced_KeepsPreviousFile() {
        Mock<IFileSource> source = new();
        ImageSet set = new() {
            Source = source.Object
        };
        source.Setup(mock => mock.GetFilesAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<ImageFile> {
                new ImageFile("cover.jpg")
            });

        FileParam fParam = new("-cfile", source.Object) {
            File = new ImageFile("original.jpg")
        };

        bool force = false;

        await set.GuessFile(fParam, "_", @"(cover\.jpg|cover\.gif)", force);

        Assert.Equal("original.jpg", fParam.Path);
    }

    [Fact]
    public async Task GuessFile_CallsFileSourceGetFiles_WithExtensionAndWildcard() {
        Mock<IFileSource> source = new();
        ImageSet set = new() {
            Source = source.Object
        };
        source.Setup(mock => mock.GetFilesAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<ImageFile> {
                new ImageFile("cover.jpg")
            });

        FileParam fParam = new("-cfile", source.Object);

        bool force = false;

        await set.GuessFile(fParam, "some_extension", @"(cover\.jpg|cover\.gif)", force);
        
        source.Verify(mock => mock.GetFilesAsync("*some_extension"), Times.Once());
    }
}
