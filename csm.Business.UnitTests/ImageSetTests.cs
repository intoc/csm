using csm.Business.Logic;
using csm.Business.Models;
using Moq;

namespace csm.Busines.Logic.UnitTests;

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
    public async Task GuessFile_MatchesOnFirstMatchingPatternAndFile() {
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

        await set.GuessFile(fParam, ".jpg", new string[] { @"cover\.jpg", @"clean\.jpg" });

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

        await set.GuessFile(fParam, ".jpg", new string[] { @"cover\.png", @"cover\.gif" });

        Assert.Equal("cover-clean.jpg", fParam.Path);
    }

    [Fact]
    public async Task GuessFile_ForcedOverridesPreviousFile() {
        Mock<IFileSource> source = new();
        ImageSet set = new() {
            Source = source.Object
        };
        source.Setup(mock => mock.GetFilesAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<ImageFile> {
                new ImageFile("cover.jpg")
            });

        FileParam fParam = new("-cfile", source.Object);
        fParam.File = new ImageFile("original.jpg");

        bool force = true;

        await set.GuessFile(fParam, ".jpg", new string[] { @"cover\.jpg", @"cover\.gif" }, force);

        Assert.Equal("cover.jpg", fParam.Path);
    }

    [Fact]
    public async Task GuessFile_NotForcedKeepsPreviousFile() {
        Mock<IFileSource> source = new();
        ImageSet set = new() {
            Source = source.Object
        };
        source.Setup(mock => mock.GetFilesAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<ImageFile> {
                new ImageFile("cover.jpg")
            });

        FileParam fParam = new("-cfile", source.Object);
        fParam.File = new ImageFile("original.jpg");

        bool force = false;

        await set.GuessFile(fParam, ".jpg", new string[] { @"cover\.jpg", @"cover\.gif" }, force);

        Assert.Equal("original.jpg", fParam.Path);
    }
}
