using csm.Business.Logic;
using csm.WinForms.Models;
using Serilog;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;

namespace csm.WinForms.Controls {

    public partial class BatchForm : Form {

        private readonly ContactSheet _parentSheet;
        private readonly IList<SheetWrapper> _sheets;
        private readonly IList<SheetWrapper> _drawQueue = new List<SheetWrapper>();
        private DateTime _lastUpdate = DateTime.Now;
        private bool _run = false;

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

        protected void ChooseDirectory_Click(object sender, EventArgs e) {
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

        protected void ChooseArchivesButton_Click(object sender, EventArgs e) {
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
            newSheet.LoadProgressChanged += (cs, e) => {
                Invoke(() => {
                    lock (_drawQueue) {
                        if (e.Percentage >= 1 && !_drawQueue.Contains(wrapper)) {
                            _drawQueue.Add(wrapper);
                            wrapper.Queued = true;
                        }
                    }
                });
            };
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

        private async void RunButton_Click(object sender, EventArgs e) {
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
                while (!IsDisposed && _run && _sheets.Count(s => s.State == SheetState.Completed) < _sheets.Count(s => !s.Failed)) {
                    if (_sheets.Count(s => s.State == SheetState.Loading) < maxConcurrentLoadSpinner.Value
                        && _sheets.Any(s => s.State == SheetState.PreLoad)) {
                        var preloading = _sheets.FirstOrDefault(s => s.State == SheetState.PreLoad);
                        if (preloading != null) {
                            preloading.Load();
                        }
                    }
                    if (_drawQueue.Any()) {
                        var sheet = _drawQueue.First();
                        _drawQueue.Remove(sheet);
                        sheet.Queued = false;
                        try {
                            while (_sheets.Count(s => s.State == SheetState.Drawing) >= maxConcurrentDrawSpinner.Value) {
                                Thread.Sleep(500);
                            }
                            if (sheet.Source != null) {
                                await sheet.Draw();
                            } else {
                                _drawQueue.Add(sheet);
                                sheet.Queued = true;
                            }
                        } catch (Exception ex) {
                            sheet.Failed = true;
                            Log.Error(ex, "Failed to draw sheet for {0}", sheet.Source);
                        }
                    } else {
                        Thread.Sleep(500);
                    }
                    UpdateStatsAndList();
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
                    int loading = _sheets.Count(s => s.State == SheetState.Loading);
                    int drawing = _sheets.Count(s => s.State == SheetState.Drawing);
                    int completed = _sheets.Count(s => s.State == SheetState.Completed);
                    loadingCountValueLabel.Text = loading.ToString();
                    drawQueueCountValueLabel.Text = _sheets.Count(s => s.State == SheetState.Queued).ToString();
                    drawingCountValueLabel.Text = drawing.ToString();
                    completedCountValueLabel.Text = $"{completed}/{_sheets.Count}";

                    // Progress Bars
                    loadProgressBar.Value = _sheets.Any() ? (int)Math.Round(loading / (float)maxConcurrentLoadSpinner.Value * 100) : 0;
                    queueProgressBar.Value = _sheets.Any() ? (int)Math.Round(_drawQueue.Count / Math.Max(1, (float)_sheets.Count(s => s.State != SheetState.Completed)) * 100) : 0;
                    drawingCountBar.Value = _sheets.Any() ? Math.Min(100, (int)Math.Round(drawing / (float)maxConcurrentDrawSpinner.Value * 100)) : 0;
                    completedProgressBar.Value = _sheets.Any() ? (int)Math.Round(completed / (float)_sheets.Count * 100) : 0;
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
                    bool loadComplete = _sheets.All(s => s.State == SheetState.Completed);
                    int drawPercent = _sheets.Any() ? (int)Math.Round(_sheets.Sum(s => s.DrawProgress) / _sheets.Count * 100) : 100;
                    if ((DateTime.Now - _lastUpdate < TimeSpan.FromSeconds(2)) && !loadComplete && drawPercent < 100) {
                        return;
                    }
                    _lastUpdate = DateTime.Now;
                    sheetGrid.Refresh();
                });
            } catch (Exception) {
                // The form is disposed, nothing here needs to happen anymore
            }
        }

        private async void BatchForm_FormClosing(object sender, FormClosingEventArgs e) {
            await Task.Run(DisposeSheets);
        }

        private void DisposeSheets() {
            foreach (SheetWrapper sheet in _sheets) {
                sheet.Dispose();
            }
        }

        private void RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e) {
            if (sheetGrid.Rows[e.RowIndex].DataBoundItem is SheetWrapper sheet && sheet.ErrorText != null) {
                sheetGrid.Rows[e.RowIndex].ErrorText = sheet.ErrorText;
            }
        }

        private void SheetGrid_UserDeletedRow(object sender, DataGridViewRowEventArgs e) {
            UpdateStats();
            runButton.Enabled = _sheets.Any();
        }

        private void SheetGrid_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e) {
            // Only allow deleting sheets in the PreLoad or Completed state
            var sheet = e.Row?.DataBoundItem as SheetWrapper;
            if (sheet != null) {
                if (!(sheet.State == SheetState.PreLoad || sheet.State == SheetState.Completed)) {
                    e.Cancel = true;
                } else {
                    sheet.Dispose();
                }
            }
        }

        private void DeleteSelectedRows(object sender, EventArgs e) {
            var selectedSheets = sheetGrid.SelectedRows.Cast<DataGridViewRow>()
                .Where(row => row.DataBoundItem is SheetWrapper)
                .Select(row => (SheetWrapper)row.DataBoundItem);
            foreach (SheetWrapper sheet in selectedSheets) {
                if (sheet.State == SheetState.PreLoad || sheet.State == SheetState.Completed) {
                    sheet.Dispose();
                    _sheets.Remove(sheet);
                }
            }
            sheetBinder.ResetBindings(false);
            UpdateStats();
            runButton.Enabled = _sheets.Any();
        }
    }
}
