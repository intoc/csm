namespace csm.Business.Models.UnitTests {
    public class ImageFileTests {

        [Theory]
        [InlineData(@"cover.jpg", @"")]
        [InlineData(@"C:\images\cover.jpg", @"C:\images")]
        public void ImageFile_Directory_Theory(string path, string expected) {
            ImageFile file = new(path);
            Assert.Equal(expected, file.Directory);
        }

        [Theory]
        [InlineData(@"cover.jpg", @"cover.jpg")]
        [InlineData(@"C:\images\cover.jpg", @"cover.jpg")]
        public void ImageFile_FileName_Theory(string path, string expected) {
            ImageFile file = new(path);
            Assert.Equal(expected, file.FileName);
        }

        [Theory]
        [InlineData(@"cover.jpg", @".jpg")]
        [InlineData(@"cover.infix.jpg", @".jpg")]
        [InlineData(@"cover", @"")]
        public void ImageFile_Extension_Theory(string path, string expected) {
            ImageFile file = new(path);
            Assert.Equal(expected, file.Extension);
        }
    }
}
