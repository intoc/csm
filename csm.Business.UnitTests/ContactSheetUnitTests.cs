using Moq;

namespace csm.Business.Logic.UnitTests {
    public class ContactSheetUnitTests {

        [Theory]
        [InlineData(@"C:\ContactSheets\{title}.png", "Venomous", @"_", 0, @"C:\ContactSheets\Venomous.png")]
        [InlineData(@"C:\ContactSheets\{title}.png", "Venomous", @"_", 5, @"C:\ContactSheets\Venomous_5.png")]
        [InlineData(@"C:\ContactSheets\prefix.{title}.png", "Venomous", @"_", 0, @"C:\ContactSheets\prefix.Venomous.png")]
        [InlineData(@"C:\ContactSheets\prefix.{title}.png", "Venomous", @"_", 5, @"C:\ContactSheets\prefix.Venomous_5.png")]
        [InlineData(@"{title}.jpg", "Spiderman", @"H:\FakeDirectory", 0, @"H:\FakeDirectory\Spiderman.jpg")]
        [InlineData(@"{title}.jpg", "Spiderman", @"H:\FakeDirectory", 42, @"H:\FakeDirectory\Spiderman_42.jpg")]
        [InlineData(@"..\{title}\ContactSheet.jpg", "DocOc", @"H:\FakeDirectory", 0, @"H:\DocOc\ContactSheet.jpg")]
        [InlineData(@"..\{title}\ContactSheet.jpg", "DocOc", @"H:\FakeDirectory", 42, @"H:\DocOc\ContactSheet_42.jpg")]
        [InlineData(@"{title}\ContactSheet.jpg", "DocOc", @"H:\FakeDirectory", 0, @"H:\FakeDirectory\DocOc\ContactSheet.jpg")]
        [InlineData(@"{title}\ContactSheet.jpg", "DocOc", @"H:\FakeDirectory", 42, @"H:\FakeDirectory\DocOc\ContactSheet_42.jpg")]
        [InlineData(@"H:\{title}_Sheets\{title}_sheet1.jpg", "DocOc", @"_", 0, @"H:\DocOc_Sheets\DocOc_sheet1.jpg")]
        [InlineData(@"H:\{title}_Sheets\{title}_sheet1.jpg", "DocOc", @"_", 42, @"H:\DocOc_Sheets\DocOc_sheet1_42.jpg")]
        public void ContactSheet_OutFilePath_Theory(string outfilePathParamValue, string htitleParamValue, string sourceParent, int suffix, string expected) {
            Mock<IFileSourceBuilder> builder = new();
            Mock<IFileSource> source = new();
            builder.Setup(mock => mock.Build(It.IsAny<string>())).Returns(source.Object);
            source.Setup(mock => mock.ParentDirectoryPath).Returns(sourceParent);
            SheetLoader cs = new(builder.Object) {
                Source = "_"
            };
            cs.LoadParamsFromCommandLine(new string[] { 
                $"-outfile={outfilePathParamValue}", 
                $"-htitle={htitleParamValue}" 
            });
            var outfile = cs.OutFilePath(suffix);

            Assert.Equal(expected, outfile);
        }
    }
}
