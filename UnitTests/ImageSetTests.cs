using csm.Models;
using Moq;

namespace csm.Logic.UnitTests;

public class ImageSetTests {

    [Fact]
    public async Task RefreshTest() {

        ImageSet set = new();

        Mock<IFileSource> source = new();
        set.Source = source.Object;
        source.Setup(mock => mock.GetFilesAsync(It.IsAny<string>())).ReturnsAsync(
            new List<ImageFile> {
                new ImageFile("file.jpg", false),
                new ImageFile("cover.jpg", false),
                new ImageFile("sheet.jpg", false)
            });

        await set.LoadImageListAsync(".jpg", 0, "sheet.jpg", "cover.jpg");

        Assert.Contains(set.Images, (image) => image.FileName == "file.jpg" && image.Include);
        Assert.Contains(set.Images, (image) => image.FileName == "cover.jpg" && !image.Include);
        Assert.Contains(set.Images, (image) => image.FileName == "sheet.jpg" && !image.Include);
    }
}
