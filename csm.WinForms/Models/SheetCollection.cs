using csm.Business.Logic;
using Serilog;


namespace csm.WinForms.Models {

    internal sealed class SheetCollection : IDisposable {

        #region Events

        public delegate void UpdatedHandler();
        public event UpdatedHandler Updated = delegate { };
        public event UpdatedHandler DrawCompleted = delegate { };

        #endregion

        public static bool CanRemove(SheetWrapper sheet) => sheet.State == SheetState.PreLoad || sheet.State == SheetState.Completed;

        public IList<SheetWrapper> Sheets => _sheets;

        public int MaxConcurrentLoad { get; set; }

        public int MaxConcurrentDraw { get; set; }

        public bool Paused { get; set; }

        public bool Any() => _sheets.Any();


        #region Stats

        public int Count => _sheets.Count;
        public int StateCount(SheetState state) => _sheets.Count(s => s.State == state);
        public int LoadingCount => StateCount(SheetState.Loading);
        public int QueuedCount => StateCount(SheetState.Queued);
        public int DrawingCount => StateCount(SheetState.Drawing);
        public int CompletedCount => StateCount(SheetState.Completed);
        public int FailedCount => StateCount(SheetState.Failed);
        public int NotCompletedCount => Count - CompletedCount;
        public int LoadingPercent => Any() ? (int)Math.Round((float)LoadingCount * 100 / MaxConcurrentLoad) : 0;
        public int QueuedPercent => Any() ? (int)Math.Round((float)QueuedCount * 100 / Math.Max(1, NotCompletedCount)) : 0;
        public int DrawingPercent => Any() ? (int)Math.Round((float)DrawingCount * 100 / MaxConcurrentDraw) : 0;
        public int CompletedPercent => Any() ? (int)Math.Round((float)CompletedCount * 100 / Count) : 0;
        private bool IsCompleted => CompletedCount >= (Count - FailedCount);

        #endregion

        private readonly SheetLoader _parentSheet;
        private readonly IList<SheetWrapper> _sheets;
        private bool _run = false;
        private IEnumerable<SheetWrapper> LoadQueue => _sheets.Where(s => s.State == SheetState.PreLoad);
        private IEnumerable<SheetWrapper> DrawQueue => _sheets.Where(s => s.Queued);
        private bool CanStartLoad => !Paused && _sheets.Count(s => s.State == SheetState.Loading) < MaxConcurrentLoad;
        private bool CanStartDraw => !Paused && _sheets.Count(s => s.State == SheetState.Drawing) < MaxConcurrentDraw;

        public SheetCollection(SheetLoader parentSheet) {
            _parentSheet = parentSheet;
            _sheets = new List<SheetWrapper>();
            Paused = true;
        }

        public void AddSheet(string path) {
            if (_sheets.Any(s => s.Source == path)) {
                return;
            }
            SheetLoader newSheet = new(new FileSourceBuilder(), false);
            var wrapper = new SheetWrapper(newSheet, path);
            newSheet.LoadParamsFromSheet(_parentSheet);
            _sheets.Add(wrapper);
        }

        public async Task LoadAndDraw() {
            if (Paused) {
                Paused = false;
            }
            _run = true;
            await Task.Run(async () => {
                while (_run && !IsCompleted) {
                    if (CanStartLoad && LoadQueue.Any()) {
                        var preloading = LoadQueue.First();
                        await preloading.Load();
                    }
                    // Check for any sheets loaded and ready to draw
                    // As long as there is a space available
                    if (CanStartDraw && DrawQueue.Any()) {
                        var sheet = DrawQueue.First();
                        try {
                            if (sheet.Source != null) {
                                await sheet.Draw();
                            }
                        } catch (Exception ex) {
                            sheet.Failed = true;
                            sheet.Errors.Add(ex.Message);
                            Log.Error(ex, "Failed to draw sheet for {0}", sheet.Source);
                        }
                    }
                    Updated?.Invoke();
                    // Wait to poll again
                    Thread.Sleep(250);
                }
                DrawCompleted?.Invoke();
                _run = false;
                Paused = true;
            });
        }

        public async Task<bool> Remove(SheetWrapper sheet) {
            if (CanRemove(sheet)) {
                _sheets.Remove(sheet);
                await DisposeSheets(sheet);
                return true;
            }
            return false;
        }

        private static async Task DisposeSheets(params SheetWrapper[] sheets) {
            foreach (SheetWrapper sheet in sheets) {
                await Task.Run(() => sheet.Dispose());
            }
        }

        public void Dispose() {
            _run = false;
            foreach (SheetWrapper sheet in _sheets) {
                Task.Run(() => sheet.Dispose());
            }
        }
    }
}
