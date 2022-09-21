using Moq;

namespace csm.Business.Logic.UnitTests {
    public class ContactSheetUnitTests {

        [Fact]
        public void ContactSheet_OutFilePath_AbsolutePath_ReplacesTitleKeywordWithHeaderTitle() {
            ContactSheet cs = new(new Mock<IFileSourceBuilder>().Object) {
                Source = "_"
            };
            cs.LoadParamsFromCommandLine(new string[] { 
                @"-outfile=C:\ContactSheets\{title}.png", 
                @"-htitle=Venemous" 
            });
            var outfile = cs.OutFilePath();
            
            Assert.Equal(@$"C:\ContactSheets\Venemous.png", outfile);
        }

        [Fact]
        public void ContactSheet_OutFilePath_RelativePath_UsesSourceParentDirectory() {
            Mock<IFileSourceBuilder> builder = new();
            Mock<IFileSource> source = new();
            builder.Setup(mock => mock.Build(It.IsAny<string>())).Returns(source.Object);
            source.Setup(mock => mock.ParentDirectoryPath).Returns(@"H:\FakeDirectory");
            ContactSheet cs = new(builder.Object) {
                Source = "_"
            };
            cs.LoadParamsFromCommandLine(new string[] { 
                @"-outfile={title}.png", 
                @"-htitle=Venemous" 
            });
            var outfile = cs.OutFilePath();

            Assert.Equal(@$"H:\FakeDirectory\Venemous.png", outfile);
        }
    }
}
