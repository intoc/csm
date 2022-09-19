using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csm.Models {
    public class ImageFile {

        public bool Hidden { get; private set; }

        public string Path { get; private set; }

        public ImageFile(string path, bool hidden = false) {
            Path = path;
            Hidden = hidden;
        }
    }
}
