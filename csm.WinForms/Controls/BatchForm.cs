using csm.Business.Logic;
using csm.WinForms.Models;
using csm.WinForms.Models.Settings;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace csm.WinForms.Controls {

    public partial class BatchForm : Form {

        private readonly SheetCollection _batch;

        private readonly AppSettings _appSettings;

        internal BatchForm(SheetLoader parentSheet, AppSettings settings) {

            _appSettings = settings;
            _batch = new SheetCollection(parentSheet);
            _batch.Updated += UpdateStatsAndList;
            _batch.DrawCompleted += SheetsDrawCompleted;

            InitializeComponent();

            loadProgressColumn.DefaultCellStyle.Format = "#0%";
            drawProgressColumn.DefaultCellStyle.Format = "#0%";
            sheetGrid.AutoGenerateColumns = false;
            sheetBinder.DataSource = new BindingList<SheetWrapper>(_batch.Sheets);
            sheetGrid.DataSource = sheetBinder;

            maxConcurrentLoadSpinner.Value = _appSettings.BatchProcessing.DefaultMaxLoad;
            maxConcurrentDrawSpinner.Value = _appSettings.BatchProcessing.DefaultMaxDraw;
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
                    _batch.AddSheet(path);
                }
                sheetBinder.ResetBindings(false);
                runButton.Enabled = _batch.Sheets.Any();
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
                    _batch.AddSheet(path);
                }
                sheetBinder.ResetBindings(false);
                runButton.Enabled = _batch.Sheets.Any();
                UpdateStats();
            }
        }

        private async void RunButtonClicked(object sender, EventArgs e) {
            if (!_batch.Paused) {
                _batch.Paused = true;
                runButton.Text = "Run";
                chooseArchivesButton.Enabled = true;
                chooseDirectoryButton.Enabled = true;
            } else {
                runButton.Text = "Pause";
                chooseArchivesButton.Enabled = false;
                chooseDirectoryButton.Enabled = false;
                await _batch.LoadAndDraw();
            }
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
                    loadingCountValueLabel.Text = _batch.LoadingCount.ToString();
                    drawQueueCountValueLabel.Text = _batch.QueuedCount.ToString();
                    drawingCountValueLabel.Text = _batch.DrawingCount.ToString();
                    completedCountValueLabel.Text = $"{_batch.CompletedCount}/{_batch.Sheets.Count}";

                    // Progress Bars
                    loadProgressBar.Value = _batch.LoadingPercent;
                    queueProgressBar.Value = _batch.QueuedPercent;
                    drawingCountBar.Value = _batch.DrawingPercent;
                    completedProgressBar.Value = _batch.CompletedPercent;
                });
            } catch (Exception) {
                // The form is disposed, nothing here needs to happen anymore
            }
        }

        private void SheetsDrawCompleted() {
            try {
                // Wait a half second just to make sure the stats are right
                UpdateStatsAndList();
                Invoke(() => {
                    runButton.Text = "Run";
                    chooseArchivesButton.Enabled = true;
                    chooseDirectoryButton.Enabled = true;
                });
            } catch (Exception) {
                // The form is probably disposed, don't worry about it
            }
        }

        private void RefreshList() {
            if (IsDisposed) {
                return;
            }
            try {
                Invoke(() => {
                    sheetGrid.Refresh();
                });
            } catch (Exception) {
                // The form is disposed, nothing here needs to happen anymore
            }
        }

        private void BatchFormClosing(object sender, FormClosingEventArgs e) {
            _batch.Dispose();
        }

        #region Grid Event Handlers

        private void RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e) {
            if (sheetGrid.Rows[e.RowIndex].DataBoundItem is SheetWrapper sheet && sheet.ErrorText != null) {
                sheetGrid.Rows[e.RowIndex].ErrorText = sheet.ErrorText;
            }
        }

        private void UserDeletedRow(object sender, DataGridViewRowEventArgs e) {
            UpdateStats();
            runButton.Enabled = _batch.Any();
        }

        private async void UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e) {
            if (e.Row?.DataBoundItem is SheetWrapper sheet && SheetCollection.CanRemove(sheet)) {
                await Task.Run(() => sheet.Dispose());
            } else {
                e.Cancel = true;
            }
        }

        private async void DeleteSelectedRows(object sender, EventArgs e) {
            var selectedSheets = sheetGrid.SelectedRows.Cast<DataGridViewRow>()
                .Where(row => row.DataBoundItem is SheetWrapper sheet)
                .Select(row => (SheetWrapper)row.DataBoundItem);

            bool removed = false;
            foreach (var sheet in selectedSheets) {
                removed = removed || await _batch.Remove(sheet);
            }

            if (removed) {
                sheetBinder.ResetBindings(false);
                UpdateStats();
                runButton.Enabled = _batch.Any();
            }
        }

        #endregion

        private void MaxConcurrentLoadSpinnerValueChanged(object sender, EventArgs e) {
            _batch.MaxConcurrentLoad = (int)maxConcurrentLoadSpinner.Value;
        }

        private void MaxConcurrentDrawSpinnerValueChanged(object sender, EventArgs e) {
            _batch.MaxConcurrentDraw = (int)maxConcurrentDrawSpinner.Value;
        }
    }
}
