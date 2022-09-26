using csm.Business.Logic;
using csm.Business.Models;
using Serilog;
using System;
using System.ComponentModel;

namespace csm.WinForms.Controls {
    public partial class BatchForm : Form {

        private readonly ContactSheet _parentSheet;
        private readonly IList<ContactSheet> _sheets;
        private readonly IList<ContactSheet> _drawQueue = new List<ContactSheet>();
        private readonly HashSet<string> _drawingSources = new();
        private readonly HashSet<string> _completedSources = new();
        private readonly IDictionary<ContactSheet, string> _erroredSources = new Dictionary<ContactSheet, string>();
        private bool _drawStarted = false;

        DateTime _lastUpdate = DateTime.Now;

        public BatchForm(ContactSheet parentSheet) {
            _parentSheet = parentSheet;
            _sheets = new List<ContactSheet>();
            InitializeComponent();
            sheetGrid.Columns[2].DefaultCellStyle.Format = "#0%";
            sheetGrid.Columns[3].DefaultCellStyle.Format = "#0%";
            sheetGrid.AutoGenerateColumns = false;
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
                sheetBinder.DataSource = new BindingList<ContactSheet>(_sheets);
                sheetGrid.DataSource = sheetBinder;
                maxConcurrentSpinner.Maximum = _sheets.Count;
                maxConcurrentSpinner.Value = 5;
                sheetsCountValueLabel.Text = _sheets.Count.ToString();
                loadingCountValueLabel.Text = _sheets.Count.ToString();
            }
        }

        private void AddSheet(string path) {
            ContactSheet newSheet = new(new FileSourceBuilder(), false);
            newSheet.SourceChanged += (_) => Invoke(RefreshList);
            newSheet.LoadProgressChanged += (cs, e) => {
                Invoke(() => {
                    RefreshList();
                    lock (_drawQueue) {
                        if (e.Percentage >= 1 && !_drawQueue.Contains(cs)) {
                            _drawQueue.Add(cs);
                        }
                    }
                    UpdateStats();
                });
            };
            newSheet.DrawProgressChanged += (cs, e) => {
                Invoke(() => {
                    RefreshList();
                    if (e.Percentage >= 1 && cs.Source != null) {
                        _completedSources.Add(cs.Source);
                        _drawingSources.Remove(cs.Source);
                        UpdateStats();
                    }
                });
            };
            newSheet.ErrorOccurred += (msg, ex) => {
                Log.Error(ex ,"{0}: {1}", path, msg);
                lock (_drawingSources) {
                    if (newSheet.Source != null &&_drawingSources.Contains(newSheet.Source)) {
                        _drawingSources.Remove(newSheet.Source);
                    }
                    _erroredSources[newSheet] = msg;
                }
            };
            newSheet.LoadParamsFromSheet(_parentSheet);
            newSheet.Source = path;
            _sheets.Add(newSheet);
        }

        private async void RunButton_Click(object sender, EventArgs e) {
            await DrawSheets();
        }

        private async Task DrawSheets() {
            _drawStarted = true;
            runButton.Enabled = false;
            chooseArchivesButton.Enabled = false;
            await Task.Run(async () => {
                while (_completedSources.Count < _sheets.Count) {
                    if (_drawQueue.Any()) {
                        var sheet = _drawQueue.First();
                        _drawQueue.Remove(sheet);
                        try {
                            while (_drawingSources.Count >= maxConcurrentSpinner.Value) {
                                Thread.Sleep(500);
                            }
                            if (sheet.Source != null) {
                                _drawingSources.Add(sheet.Source);
                                await Task.Factory.StartNew(async () => await sheet.DrawAndSave(false));
                            } else {
                                _drawQueue.Add(sheet);
                            }
                        } catch (Exception ex) {
                            Log.Error(ex, "Failed to draw sheet for {0}", sheet.Source);
                        }
                    } else {
                        Thread.Sleep(500);
                    }
                    UpdateStats();
                }
            });
        }

        private void UpdateStats() {
            Invoke(async () => {
                loadingCountValueLabel.Text = _sheets.Count(s => s.LoadProgress < 1).ToString();
                drawQueueCountValueLabel.Text = _drawQueue.Count.ToString();
                drawingCountValueLabel.Text = _drawingSources.Count.ToString();
                completedCountValueLabel.Text = _completedSources.Count.ToString();
                loadProgressBar.Value = (int)Math.Round(_sheets.Sum(s => s.LoadProgress) / _sheets.Count * 100);
                queueProgressBar.Value = (int)Math.Round(_drawQueue.Count / (float)_sheets.Count * 100);
                drawingCountBar.Value = Math.Min(100, (int)Math.Round(_drawingSources.Count / (float)maxConcurrentSpinner.Value * 100));

                if (loadProgressBar.Value == 100 && runAfterLoadCheckBox.Checked && !_drawStarted) {
                    await DrawSheets();
                }
            });
        }

        private int _lastScrollIndex = 0;
        private void RefreshList() {
            bool loadComplete = _sheets.All(s => s.FirstLoadComplete);
            int drawPercent = (int)Math.Round(_sheets.Sum(s => s.DrawProgress) / _sheets.Count * 100);
            if ((DateTime.Now - _lastUpdate < TimeSpan.FromSeconds(1)) && !loadComplete && drawPercent < 100) {
                return;
            }
            _lastUpdate = DateTime.Now;
            completedProgressBar.Value = drawPercent;
            if (sheetGrid.CurrentRow.Index > 0) {
                _lastScrollIndex = sheetGrid.CurrentRow.Index;
            }
            sheetBinder.ResetBindings(false);
        }

        private void RefreshListButton_Click(object sender, EventArgs e) {
            sheetBinder.ResetBindings(false);
        }

        
        private void BatchForm_FormClosing(object sender, FormClosingEventArgs e) {
            DisposeSheets();
        }

        private void DisposeSheets() {
            foreach (ContactSheet sheet in _sheets) {
                sheet.Dispose();
            }
        }

        private void RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e) {
            if (sheetGrid.Rows[e.RowIndex].DataBoundItem is ContactSheet sheet && _erroredSources.ContainsKey(sheet)) {
                sheetGrid.Rows[e.RowIndex].ErrorText = _erroredSources[sheet];
            }
        }

        private void sheetGrid_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e) {
            sheetGrid.FirstDisplayedScrollingRowIndex = _lastScrollIndex;
        }

        private void RunAfterLoadCheckBox_CheckedChanged(object sender, EventArgs e) {
            runButton.Enabled = !runAfterLoadCheckBox.Checked;
        }
    }
}
