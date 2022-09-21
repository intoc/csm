namespace csm.Business.Models.UnitTests {
    public class ImageFileTests {

        [Theory]
        [InlineData(@"cover.jpg", @".\")]
        [InlineData(@"C:\images\cover.jpg", @"C:\images\")]
        public void ImageFile_DirectoryTheory(string path, string expected) {
            ImageFile file = new(path);
            Assert.Equal(expected, file.Directory);
        }

        [Theory]
        [InlineData(@"cover.jpg", @"cover.jpg")]
        [InlineData(@"C:\images\cover.jpg", @"cover.jpg")]
        public void ImageFile_FileNameTheory(string path, string expected) {
            ImageFile file = new(path);
            Assert.Equal(expected, file.FileName);
        }

        [Theory]
        [InlineData(@"cover.jpg", @".jpg")]
        [InlineData(@"cover.infix.jpg", @".jpg")]
        [InlineData(@"cover", null)]
        public void ImageFile_ExtensionTheory(string path, string expected) {
            ImageFile file = new(path);
            Assert.Equal(expected, file.Extension);
        }
    }
}
