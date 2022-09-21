using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csm.Business.Logic {
    public interface IFileSourceBuilder {

        IFileSource Build(string? path);
    }
}
