namespace csm.Business.Logic.UnitTests {
    public class ContactSheetUnitTests {

        [Fact(Skip = "Not ready. ContactSheet has too many dependencies on the file system.")]
        public void ContactSheetUnitTest() {
            ContactSheet cs = new() {
                Source = @".\"
            };
            cs.LoadParamsFromCommandLine(new string[] { @"-outfile=C:\{title}.jpg" });
            var outfile = cs.OutFilePath();
            
            Assert.Equal(@$"C:\Users\Public.jpg", outfile);
        }
    }
}
