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
                if (_sheet.LoadProgress < 1) {
                    return SheetState.Loading;
                }
                if (!_drawingStarted) {
                    return SheetState.Queued;
                }
                if (_sheet.DrawProgress < 1) {
                    return SheetState.Drawing;
                }
                return SheetState.Completed;
            }
        }

        public bool Queued => State == SheetState.Queued;
        public bool Failed { get; set; }

        public string? ErrorText => Errors.Any() ? string.Join(@" | ", Errors) : null;

        public IList<string> Errors { get; } = new List<string>();

        public double DrawProgress => _sheet.DrawProgress;

        public double LoadProgress => _sheet.LoadProgress;

        private readonly SheetLoader _sheet;
        private readonly string _sourcePath;
        private bool _drawingStarted = false;

        public SheetWrapper(SheetLoader sheet, string sourcePath) {
            _sheet = sheet;
            _sourcePath = sourcePath;
            _sheet.ErrorOccurred += HandleError;
        }

        private void HandleError(string msg, bool isFatal, Exception? ex) {
            var logger = Log.ForContext("Context", _sourcePath);
            logger.Error(ex, "{1} {2}", msg, isFatal ? "[FATAL]" : string.Empty);
            if (isFatal) {
                Failed = true;
            }
            Errors.Add(msg);
        }

        public async Task Load() {
            await _sheet.SetSourcePath(_sourcePath);
        }

        public async Task Draw() {
            _drawingStarted = true;
            await Task.Factory.StartNew(async () => {
                try {
                    await _sheet.DrawAndSave();
                } catch (Exception ex) {
                    HandleError("Fatal Error (See Console)", true, ex);
                }
            });
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
