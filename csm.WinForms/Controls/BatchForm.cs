using csm.Business.Logic;
using csm.WinForms.Models;
using Serilog;
using System.ComponentModel;
using System.Reflection;

namespace csm.WinForms.Controls {

    public partial class BatchForm : Form {

        private readonly ContactSheet _parentSheet;
        private readonly IList<SheetWrapper> _sheets;
        private readonly IList<SheetWrapper> _drawQueue = new List<SheetWrapper>();
        DateTime _lastUpdate = DateTime.Now;

        public BatchForm(ContactSheet parentSheet) {
            _parentSheet = parentSheet;
            _sheets = new List<SheetWrapper>();
            InitializeComponent();
            sheetGrid.Columns[2].DefaultCellStyle.Format = "#0%";
            sheetGrid.Columns[3].DefaultCellStyle.Format = "#0%";
            sheetGrid.AutoGenerateColumns = false;

            maxConcurrentLoadSpinner.Value = 10;
            maxConcurrentDrawSpinner.Value = 5;
        }

        protected void ChooseArchivesButton_Click(object sender, EventArgs e) => ChooseArchives();

        public void ChooseArchives() {
            OpenFileDialog ofd = new() {
                CheckPathExists = true,
                CheckFileExists = true,
                DefaultExt = "zip",
                Filter = "Archive files (*.zip, *.rar, *.7z)|*.zip;*.rar;*.7z",
                Multiselect = true
            };
            if (ofd.ShowDialog() == DialogResult.OK) {
                DisposeSheets();
                _sheets.Clear();
                foreach (var path in ofd.FileNames) {
                    AddSheet(path);
                }
                sheetBinder.DataSource = new BindingList<SheetWrapper>(_sheets);
                sheetGrid.DataSource = sheetBinder;

                maxConcurrentLoadSpinner.Value = Math.Min(maxConcurrentLoadSpinner.Value, _sheets.Count);
                maxConcurrentLoadSpinner.Maximum = _sheets.Count;
                maxConcurrentDrawSpinner.Value = Math.Min(maxConcurrentDrawSpinner.Value, _sheets.Count);
                maxConcurrentDrawSpinner.Maximum = _sheets.Count;
                completedCountValueLabel.Text = $"0/{_sheets.Count}";
            }
        }
        private void AddSheet(string path) {
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
            await Run();
        }

        private async Task Run() {
            runButton.Enabled = false;
            chooseArchivesButton.Enabled = false;
            await Task.Run(async () => {
                while (_sheets.Count(s => s.State == SheetState.Completed) < _sheets.Count(s => !s.Failed)) {
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
            });
        }
        private void UpdateStatsAndList() {
            RefreshList();
            UpdateStats();
        }

        private void UpdateStats() {
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
                    loadProgressBar.Value = (int)Math.Round(loading / (float)maxConcurrentLoadSpinner.Value * 100);
                    queueProgressBar.Value = (int)Math.Round(_drawQueue.Count / Math.Max(1, (float)_sheets.Count(s => s.State != SheetState.Completed)) * 100);
                    drawingCountBar.Value = Math.Min(100, (int)Math.Round(drawing / (float)maxConcurrentDrawSpinner.Value * 100));
                    completedProgressBar.Value = (int)Math.Round(completed / (float)_sheets.Count * 100);
                });
            } catch (TargetInvocationException) {
                // The form is disposed, nothing here needs to happen anymore
            }
        }

        private void RefreshList() {
            try {
                Invoke(() => {
                    bool loadComplete = _sheets.All(s => s.State == SheetState.Completed);
                    int drawPercent = (int)Math.Round(_sheets.Sum(s => s.DrawProgress) / _sheets.Count * 100);
                    if ((DateTime.Now - _lastUpdate < TimeSpan.FromSeconds(2)) && !loadComplete && drawPercent < 100) {
                        return;
                    }
                    _lastUpdate = DateTime.Now;
                    sheetGrid.Refresh();
                });
            } catch (TargetInvocationException) {
                // The form is disposed, nothing here needs to happen anymore
            }
        }

        private void RefreshListButton_Click(object sender, EventArgs e) {
            sheetBinder.ResetBindings(false);
        }

        private void BatchForm_FormClosing(object sender, FormClosingEventArgs e) {
            DisposeSheets();
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

    }
}
