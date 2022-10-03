using Serilog;

namespace csm.Business.Logic {
    public interface IFileSourceBuilder {

        IFileSource Build(string? path, ILogger logger);
    }
}
