using csm.Business.Logic;
using Serilog;

namespace csm.WinForms.Models {

    internal enum SheetState {
        PreLoad,
        Loading,
        Queued,
        Drawing,
        Completed,
        Failed
    }

    internal sealed class SheetWrapper : IDisposable {
        public string? Source => _sourcePath;

        public string Outfile => _sheet.OutFilePath();
        public SheetState State {

            get {
                if (_sheet.Source == null) {
                    return SheetState.PreLoad;
                }
                if (Failed) {
                    return SheetState.Failed;
                }
                if (Queued) {
                    return SheetState.Queued;
                }
                if (!_drawingStarted && _sheet.LoadProgress < 1) {
                    return SheetState.Loading;
                }
                if (_drawingStarted && _sheet.DrawProgress < 1) {
                    return SheetState.Drawing;
                }
                return SheetState.Completed;
            }
        }

        public bool Queued => !_drawingStarted && LoadProgress == 1;
        public bool Failed { get; set; }

        public string? ErrorText { get; set; }

        public double DrawProgress => _sheet.DrawProgress;

        public double LoadProgress => _sheet.LoadProgress;

        private readonly SheetLoader _sheet;
        private readonly string _sourcePath;
        private bool _drawingStarted = false;

        public SheetWrapper(SheetLoader sheet, string sourcePath) {
            _sheet = sheet;
            _sourcePath = sourcePath;
            _sheet.ErrorOccurred += (msg, isFatal, ex) => {
                Log.Error(ex, "{0}: {1} {2}", sourcePath, msg, isFatal ? "[FATAL]" : string.Empty);
                if (isFatal) {
                    Failed = true;
                }
                ErrorText = msg;
            };
        }

        public async Task Load() {
            await _sheet.SetSourcePath(_sourcePath);
        }

        public async Task Draw() {
            _drawingStarted = true;
            await Task.Factory.StartNew(async () => await _sheet.DrawAndSave());
        }

        public void Dispose() {
            try {
                _sheet.Dispose();
            } catch {
                // We tried
            }
        }
    }
}
