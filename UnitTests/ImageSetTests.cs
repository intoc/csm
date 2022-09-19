using csm.Models;
using Moq;

namespace csm.Logic.UnitTests;

public class ImageSetTests {

    [Fact]
    public async Task LoadImageListAsync_DiscludesExpectedFiles() {

        ImageSet set = new();

        Mock<IFileSource> source = new();
        set.Source = source.Object;
        source.Setup(mock => mock.GetFilesAsync(It.IsAny<string>())).ReturnsAsync(
            new List<ImageFile> {
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
}
