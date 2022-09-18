using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csm.Logic {
    internal interface IFileSource {

        bool IsReady { get; }

        IEnumerable<FileInfo> GetFiles();

    }
}
