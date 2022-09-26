﻿using csm.Business.Logic;

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
                if (_sheet.LoadProgress < 1) {
                    return SheetState.Loading;
                }
                if (_sheet.DrawProgress < 1) {
                    return SheetState.Drawing;
                }
                return SheetState.Completed;
            }
        }

        public bool Queued { get; set; }
        public bool Failed { get; set; }

        public string? ErrorText { get; set; }

        public double DrawProgress => _sheet.DrawProgress;

        public double LoadProgress => _sheet.LoadProgress;

        private readonly ContactSheet _sheet;
        private readonly string _sourcePath;

        public SheetWrapper(ContactSheet sheet, string sourcePath) {
            _sheet = sheet;
            _sourcePath = sourcePath;
        }

        public void Load() {
            _sheet.Source = _sourcePath;
        }

        public async Task Draw() {
            await Task.Factory.StartNew(async () => await _sheet.DrawAndSave(true));
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
