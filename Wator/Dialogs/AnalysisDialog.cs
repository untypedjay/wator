using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace VPS.Wator.Dialogs {
  // This dialog is displayed when the simulation is finished
  // and shows statistics about the run time of the simulation.
  public partial class AnalysisDialog : Form {
    public AnalysisDialog() {
      InitializeComponent();
    }
    public AnalysisDialog(List<TimeSpan> times, Settings settings)
      : this() {
      int count = times.Count;
      double total = 0;
      double average;
      double variance = 0;

      // calculate average and variance of runtime
      for (int i = 0; i < count; i++) {
        total += times[i].TotalMilliseconds;
        variance += times[i].TotalMilliseconds * times[i].TotalMilliseconds;
      }
      average = total / count;
      variance = (variance / count) - (average * average);

      List<string> message = new List<string>();
      message.Add("Runs:                    " + settings.Runs.ToString());
      message.Add("Iterations:              " + settings.Iterations.ToString());
      message.Add("Runtime in Milliseconds: " + total.ToString());
      message.Add("Avg. Milliseconds / Run: " + average.ToString());
      message.Add("Std. Deviation:          " + Math.Sqrt(variance).ToString());
      message.Add("------------------------");
      message.Add("Runtimes in Milliseconds:");
      for (int i = 1; i <= count; i++) {
        message.Add("Run " + i.ToString("00") + ":                  " + times[i - 1].TotalMilliseconds.ToString());
      }
      textBox.Lines = message.ToArray();
    }
  }
}