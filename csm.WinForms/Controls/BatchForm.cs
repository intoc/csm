using csm.Business.Logic;
using csm.WinForms.Models;
using Serilog;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace csm.WinForms.Controls {

    public partial class BatchForm : Form {

        private readonly ContactSheet _parentSheet;
        private readonly IList<SheetWrapper> _sheets;
        private IEnumerable<SheetWrapper> LoadQueue => _sheets.Where(s => s.State == SheetState.PreLoad);
        private IEnumerable<SheetWrapper> DrawQueue => _sheets.Where(s => s.Queued);

        private DateTime _lastUpdate = DateTime.Now;
        private bool _run = false;

        private bool CanStartLoad => _sheets.Count(s => s.State == SheetState.Loading) < maxConcurrentLoadSpinner.Value;
        private bool CanStartDraw => _sheets.Count(s => s.State == SheetState.Drawing) < maxConcurrentDrawSpinner.Value;
        private bool IsCompleted => StateCount(SheetState.Completed) >= (_sheets.Count - StateCount(SheetState.Failed));
        private int StateCount(SheetState state) => _sheets.Count(s => s.State == state);
        private static bool CanRemove(SheetWrapper sheet) => sheet.State == SheetState.PreLoad || sheet.State == SheetState.Completed;

        public BatchForm(ContactSheet parentSheet) {
            _parentSheet = parentSheet;
            _sheets = new List<SheetWrapper>();
            
            InitializeComponent();
            
            sheetGrid.Columns[2].DefaultCellStyle.Format = "#0%";
            sheetGrid.Columns[3].DefaultCellStyle.Format = "#0%";
            sheetGrid.AutoGenerateColumns = false;
            sheetBinder.DataSource = new BindingList<SheetWrapper>(_sheets);
            sheetGrid.DataSource = sheetBinder;

            maxConcurrentLoadSpinner.Value = 10;
            maxConcurrentDrawSpinner.Value = 5;
            runButton.Enabled = false;
        }

        protected void ChooseDirectoryClicked(object sender, EventArgs e) {
            FolderBrowserDialog folder = new();
            if (folder.ShowDialog() == DialogResult.OK) {
                DirectoryInfo dir = new(folder.SelectedPath);
                var archives = dir.GetFiles()
                    .Where(f => Regex.IsMatch(f.Extension, @"\.(rar|zip|7z)$"))
                    .Select(f => f.FullName);
                var directories = dir.GetDirectories()
                    .Select(d => d.FullName);
                foreach (var path in directories.Concat(archives)) {
                    AddSheet(path);
                }
                sheetBinder.ResetBindings(false);
                runButton.Enabled = _sheets.Any();
                UpdateStats();
            }
        }

        protected void ChooseArchivesButtonClicked(object sender, EventArgs e) {
            OpenFileDialog ofd = new() {
                CheckPathExists = true,
                CheckFileExists = true,
                DefaultExt = "zip",
                Filter = "Archive files (*.zip, *.rar, *.7z)|*.zip;*.rar;*.7z",
                Multiselect = true
            };
            if (ofd.ShowDialog() == DialogResult.OK) {
                foreach (var path in ofd.FileNames) {
                    AddSheet(path);
                }
                sheetBinder.ResetBindings(false);
                runButton.Enabled = _sheets.Any();
                UpdateStats();
            }
        }

        private void AddSheet(string path) {
            if (_sheets.Any(s => s.Source == path)) {
                return;
            }
            ContactSheet newSheet = new(new FileSourceBuilder(), false);
            var wrapper = new SheetWrapper(newSheet, path);
            newSheet.ErrorOccurred += (msg, isFatal, ex) => {
                Log.Error(ex, "{0}: {1} {2}", path, msg, isFatal ? "[FATAL]" : string.Empty);
                if (isFatal) {
                    wrapper.Failed = true;
                }
                wrapper.ErrorText = msg;
            };
            newSheet.LoadParamsFromSheet(_parentSheet);
            _sheets.Add(wrapper);
        }

        private async void RunButtonClicked(object sender, EventArgs e) {
            if (_run) {
                _run = false;
                // It's a pause button right now. Keep updating the UI but don't process any new things
                await Task.Run(() => {
                    while (!_run && !IsDisposed) {
                        Thread.Sleep(250);
                        UpdateStatsAndList();
                    }
                });
            } else {
                await Run();
            }
        }

        private async Task Run() {
            runButton.Text = "Pause";
            _run = true;
            chooseArchivesButton.Enabled = false;
            chooseDirectoryButton.Enabled = false;
            await Task.Run(async () => {
                while (!IsDisposed && _run && !IsCompleted) {
                    if (CanStartLoad && LoadQueue.Any()) {
                        var preloading = LoadQueue.First();
                        preloading.Load();
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
                            sheet.ErrorText = ex.Message;
                            Log.Error(ex, "Failed to draw sheet for {0}", sheet.Source);
                        }
                    }
                    UpdateStatsAndList();
                    // Wait to poll again
                    Thread.Sleep(250);
                }
                UpdateStatsAndList();
                try {
                    Invoke(() => {
                        runButton.Text = "Run";
                        chooseArchivesButton.Enabled = true;
                        chooseDirectoryButton.Enabled = true;
                    });
                } catch (Exception) {
                    // The form is probably disposed, don't worry about it
                }
            });
        }

        private void UpdateStatsAndList() {
            RefreshList();
            UpdateStats();
        }
       
        private void UpdateStats() {
            if (IsDisposed) {
                return;
            }
            try {
                Invoke(() => {
                    // Counts
                    int loading = StateCount(SheetState.Loading);
                    int drawing = StateCount(SheetState.Drawing);
                    int completed = StateCount(SheetState.Completed);
                    loadingCountValueLabel.Text = loading.ToString();
                    drawQueueCountValueLabel.Text = DrawQueue.Count().ToString();
                    drawingCountValueLabel.Text = drawing.ToString();
                    completedCountValueLabel.Text = $"{completed}/{_sheets.Count}";

                    // Progress Bars
                    loadProgressBar.Value = _sheets.Any() ? 
                        (int)Math.Round(loading / (float)maxConcurrentLoadSpinner.Value * 100) : 0;
                    queueProgressBar.Value = _sheets.Any() ? 
                        (int)Math.Round(DrawQueue.Count() / Math.Max(1, (float)_sheets.Count(s => s.State != SheetState.Completed)) * 100) : 0;
                    drawingCountBar.Value = _sheets.Any() ? 
                        Math.Min(100, (int)Math.Round(drawing / (float)maxConcurrentDrawSpinner.Value * 100)) : 0;
                    completedProgressBar.Value = _sheets.Any() ?
                        (int)Math.Round(completed / (float)_sheets.Count * 100) : 0;
                });
            } catch (Exception) {
                // The form is disposed, nothing here needs to happen anymore
            }
        }

        private void RefreshList() {
            if (IsDisposed) {
                return;
            }
            try {
                Invoke(() => {
                    if ((DateTime.Now - _lastUpdate < TimeSpan.FromSeconds(2))) {
                        return;
                    }
                    _lastUpdate = DateTime.Now;
                    sheetGrid.Refresh();
                });
            } catch (Exception) {
                // The form is disposed, nothing here needs to happen anymore
            }
        }

        private async void BatchFormClosing(object sender, FormClosingEventArgs e) {
            await Task.Run(DisposeSheets);
        }

        private void DisposeSheets() {
            foreach (SheetWrapper sheet in _sheets) {
                sheet.Dispose();
            }
        }

        #region Grid Event Handlers

        private void RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e) {
            if (sheetGrid.Rows[e.RowIndex].DataBoundItem is SheetWrapper sheet && sheet.ErrorText != null) {
                sheetGrid.Rows[e.RowIndex].ErrorText = sheet.ErrorText;
            }
        }

        private void UserDeletedRow(object sender, DataGridViewRowEventArgs e) {
            UpdateStats();
            runButton.Enabled = _sheets.Any();
        }

        private void UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e) {
            // Only allow deleting sheets in the PreLoad or Completed state
            if (e.Row?.DataBoundItem is SheetWrapper sheet) {
                if (CanRemove(sheet)) {
                    sheet.Dispose();
                } else {
                    e.Cancel = true;
                }
            }
        }

        private void DeleteSelectedRows(object sender, EventArgs e) {
            var selectedSheets = sheetGrid.SelectedRows.Cast<DataGridViewRow>()
                .Where(row => row.DataBoundItem is SheetWrapper sheet && CanRemove(sheet))
                .Select(row => (SheetWrapper)row.DataBoundItem);
            foreach (var sheet in selectedSheets) {
                sheet.Dispose();
                _sheets.Remove(sheet);
            }

            sheetBinder.ResetBindings(false);
            UpdateStats();
            runButton.Enabled = _sheets.Any();
        }

        #endregion
    }
}
