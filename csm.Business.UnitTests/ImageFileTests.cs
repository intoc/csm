namespace csm.Business.Models.UnitTests {
    public class ImageFileTests {

        [Fact]
        public void ImageFile_RelativePath_DirectoryIsRelative() {
            ImageFile file = new("cover.jpg");
            Assert.Equal(@".\", file.Directory);
        }

        [Fact]
        public void ImageFile_AbsolutePath_DirectoryIsAbsolute() {
            ImageFile file = new(@"C:\images\cover.jpg");
            Assert.Equal(@"C:\images\", file.Directory);
        }

        [Fact]
        public void ImageFile_RelativePath_FileNameIsPath() {
            ImageFile file = new("cover.jpg");
            Assert.Equal("cover.jpg", file.FileName);
        }

        [Fact]
        public void ImageFile_AbsolutePath_FileNameIsRelative() {
            ImageFile file = new(@"C:\images\cover.jpg");
            Assert.Equal("cover.jpg", file.FileName);
        }

        [Fact]
        public void ImageFile_PathHasExtension_ExtensionIsCorrect() {
            ImageFile file = new("cover.jpg");
            Assert.Equal(".jpg", file.Extension);
        }

        [Fact]
        public void ImageFile_PathHasMultipleDots_ExtensionIsCorrect() {
            ImageFile file = new("cover.infix.jpg");
            Assert.Equal(".jpg", file.Extension);
        }

        [Fact]
        public void ImageFile_PathHasNoExtension_ExtensionIsNull() {
            ImageFile file = new("cover");
            Assert.Null(file.Extension);
        }
    }
}
