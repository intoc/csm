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

    [Theory]
    [InlineData(new string[] { "cover.bmp", "cover-clean.jpg", "cover.jpg", "clean-cover.jpg" }, @"cover\.jpg", false, false, true, "cover.jpg")]
    [InlineData(new string[] { "cover.png", "cover-clean.png", "cover.jpg", "clean-cover.jpg" }, @"notpresent\.(jpg|gif)", false, false, true, "cover.png")]
    [InlineData(new string[] { "cover.jpg.false.jpg", "true.jpg.cover.jpg", "cover.jpg", "clean-cover.jpg" }, @"cover\.(jpg|gif)$", false, false, true, "true.jpg.cover.jpg")]
    [InlineData(new string[] { "cover.jpg", "cover.png" }, @"cover\.(jpg|gif)", true, true, true, "cover.jpg")]
    [InlineData(new string[] { "cover.tiff", "cover.jpg" }, @"(cover\.jpg|cover\.gif)", true, false, false, "original.jpg")]
    public async Task GuessFile_Theory(string[] sourceFiles, string regex, bool hasFile, bool force, bool expectToGetFiles, string expected) {
        
        // Setup
        Mock<IFileSource> source = new();
        ImageSet set = new() {
            Source = source.Object
        };
        source.Setup(mock => mock.GetFilesAsync(It.IsAny<string>()))
            .ReturnsAsync(sourceFiles.Select(file =>new ImageFile(file)));
        FileParam fParam = new("-cfile", source.Object);
        if (hasFile) {
            fParam.File = new ImageFile("original.jpg");
        }

        // Act
        await set.GuessFile(fParam, "some_extension", regex, force);

        // Verify
        source.Verify(mock => mock.GetFilesAsync("*some_extension"), expectToGetFiles ? Times.Once() : Times.Never());
        Assert.Equal(expected, fParam.Path);
    }
}
