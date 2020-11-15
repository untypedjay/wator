using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using VPS.Wator.Dialogs;

namespace VPS.Wator {
  public partial class MainForm : Form {
    private Settings settings;

    public MainForm() {
      InitializeComponent();

      settings = new Settings();

      worker.WorkerSupportsCancellation = true;
      worker.WorkerReportsProgress = true;
      worker.DoWork += new DoWorkEventHandler(worker_DoWork);
      worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
      worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
    }

    #region BackgroundWorker Events
    private void worker_DoWork(object sender, DoWorkEventArgs e) {
      Type worldType = (Type)e.Argument;
      IWatorWorld world;
      Stopwatch watch = new Stopwatch();
      List<TimeSpan> times = new List<TimeSpan>();

      int run = 0;
      while ((run < settings.Runs) && (!worker.CancellationPending)) {
        watch.Reset();
        watch.Start();

        world = (IWatorWorld)Activator.CreateInstance(worldType, settings);
        if (settings.DisplayWorld)
          worker.ReportProgress(0, world.GenerateImage());

        int iteration = 0;
        while ((iteration < settings.Iterations) && (!worker.CancellationPending)) {
          world.ExecuteStep();
          if (settings.DisplayWorld && ((iteration % settings.DisplayInterval) == 0))
            worker.ReportProgress(0, world.GenerateImage());
          iteration++;
        }

        watch.Stop();
        times.Add(watch.Elapsed);
        run++;
      }
      e.Result = times;
      if (worker.CancellationPending)
        e.Cancel = true;
    }

    private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
      pictureBox.Image = (Bitmap)e.UserState;
    }

    void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
      if (e.Error != null)
        MessageBox.Show(this, e.Error.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
      else if (!e.Cancelled) {
        using (AnalysisDialog dialog = new AnalysisDialog((List<TimeSpan>)e.Result, settings)) {
          dialog.ShowDialog(this);
        }
      }

      runButton.Enabled = true;
      stopButton.Enabled = false;
      pictureBox.Cursor = Cursors.Default;
    }
    #endregion

    #region Button Events
    private void runButton_Click(object sender, EventArgs e) {
      stopButton.Enabled = true;
      runButton.Enabled = false;
      pictureBox.Cursor = Cursors.AppStarting;

      Type type = Assembly.GetExecutingAssembly().GetTypes().
                  Where(x => x.GetInterfaces().Contains(typeof(IWatorWorld))).
                  Where(x => x.Name == settings.Version.ToString()).
                  First();
      worker.RunWorkerAsync(type);
    }

    private void stopButton_Click(object sender, EventArgs e) {
      stopButton.Enabled = false;
      worker.CancelAsync();
    }
    #endregion

    #region Menu Events
    private void settingsToolStripMenuItem_Click(object sender, EventArgs e) {
      using (SettingsDialog dialog = new SettingsDialog(settings)) {
        dialog.ShowDialog(this);
      }
    }

    private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
      Application.Exit();
    }
    #endregion

    protected override void OnClosing(CancelEventArgs e) {
      worker.CancelAsync();
      base.OnClosing(e);
    }
  }
}