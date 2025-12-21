using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Speech.Synthesis;
using System.Windows.Forms;
using GenerativeAI;
	
namespace TalkingClipboard
{
  /// <summary>
  /// Source code for the TalkingClipboard article.
  /// Author: Ravi Bhavnani, ravib@ravib.com
  /// License: CodeProject Open License
  /// </summary>
  public partial class MainFrm : Form
  {
    #region Win32 integration

      const int WM_DRAWCLIPBOARD = 0x0308;

      const int WM_CHANGECBCHAIN = 0x030D;

      [DllImport("user32.dll")]
      static extern IntPtr SetClipboardViewer (IntPtr hWndNewViewer);

      [DllImport("user32.dll")]
      static extern bool ChangeClipboardChain (IntPtr hWndRemove, IntPtr hWndNewNext);

      [DllImport("user32.dll")]
		  public static extern int SendMessage (IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

    #endregion

    #region Constructor

      /// <summary>
      /// Default constructor.
      /// </summary>
      public MainFrm()
      {
        InitializeComponent();

        // Load list of available voices
        ReadOnlyCollection<InstalledVoice> voices = _synth.GetInstalledVoices();
        foreach (InstalledVoice voice in voices)
          _comboVoices.Items.Add (voice.VoiceInfo.Name);
        if (_comboVoices.Items.Count > 0)
            _comboVoices.SelectedIndex = 0;

        // Keep window topmost
        this.TopMost = true;
      }

    #endregion

    #region Overrides

      /// <summary>
      /// Main window procedure.
      /// </summary>
      protected override void WndProc
        (ref Message m)
      {
        base.WndProc (ref m);

        switch (m.Msg) {
          case WM_DRAWCLIPBOARD:
            // The contents of the clipboard have changed, so load the
            // text box and speak its contents.
            Trace.WriteLine ("WM_DRAWCLIPBOARD handled");
            if (_loaded && !_checkDontWatchClipboard.Checked) {
                IDataObject dataObj = Clipboard.GetDataObject();
                if (dataObj.GetDataPresent (DataFormats.Text)) {
                    string clipboardText = dataObj.GetData (DataFormats.Text) as string;
                    _editText.Text = clipboardText;
                    if (_synth.State == SynthesizerState.Speaking)
                        _synth.SpeakAsyncCancelAll();
                    _btnSpeak.PerformClick();
                }
            }
            break;

          case WM_CHANGECBCHAIN:
            // The chain of clipboard listeners has changed.
            Trace.WriteLine ("WM_CHANGECBCHAIN handled");
            if (m.WParam == _chainedWnd)
                _chainedWnd = m.LParam;
            else
                SendMessage (_chainedWnd, m.Msg, m.WParam, m.LParam);
            break;
        }
      }

    #endregion

    #region Form events

      /// <summary>
      /// The form has loaded.
      /// </summary>
      private void MainFrm_Load
        (object sender,
         EventArgs e)
      {
        // Start listening for clipboard changes
        _chainedWnd = SetClipboardViewer (this.Handle);
        _loaded = true;
      }

      /// <summary>
      /// The form is closing.
      /// </summary>
      private void MainFrm_FormClosing
        (object sender,
         FormClosingEventArgs e)
      {
        // Stop listening to clipboard changes
        ChangeClipboardChain (this.Handle, _chainedWnd);
        _chainedWnd = IntPtr.Zero;
      }

    #endregion

    #region Control events

      /// <summary>
      /// A new voice has been selected.
      /// </summary>
      private void _comboVoices_SelectionChangeCommitted
        (object sender,
         EventArgs e)
      {
        string voiceName = _comboVoices.SelectedItem as string;
        _synth.SelectVoice (voiceName);
      }

      /// <summary>
      /// A new voice speed has been selected.
      /// </summary>
      private void _knobRate_ValueChanged
        (object sender,
         EventArgs e)
      {
        int rate = 2 * (_knobRate.Value - 5);
        _synth.Rate = rate;
        _lblSpeed.Text = string.Format ("Speed ({0}):", rate.ToString());
      }

      /// <summary>
      /// Start speaking.
      /// </summary>
      private void _btnSpeak_Click
        (object sender,
         EventArgs e)
      {
        _btnStop.PerformClick();
        _synth.SpeakAsync (_editText.Text);
      }

      /// <summary>
      /// Start speaking.
      /// </summary>
      private void _btnStop_Click
        (object sender,
         EventArgs e)
      {
        if (_synth.State == SynthesizerState.Paused)
            _synth.Resume();
        _synth.SpeakAsyncCancelAll();
        _btnPauseRestart.Text = "Pause";
      }

      /// <summary>
      /// Pause/Restart speech.
      /// </summary>
      private void _btnPauseRestart_Click
        (object sender,
         EventArgs e)
      {
        if (_synth.State == SynthesizerState.Speaking) {
            _synth.Pause();
            _btnPauseRestart.Text = "Resume";
        } else {
            if (_synth.State == SynthesizerState.Paused) {
                _synth.Resume();
                _btnPauseRestart.Text = "Pause";
            }
        }
      }

      /// <summary>
      /// Save speech to .WAV file.
      /// </summary>
      private void _btnSaveAsWavFile_Click
        (object sender,
         EventArgs e)
      {
        // Get name of .WAV file - return if user cancels
        SaveFileDialog sfd = new SaveFileDialog();
        sfd.Filter = "WAV files (*.wav)|*.wav|All files (*.*)|*.*";
        if (sfd.ShowDialog() == DialogResult.Cancel)
            return;
        string wavFilename = sfd.FileName;

        // Save speech as .WAV file
        _synth.SetOutputToWaveFile (wavFilename);
        _synth.Speak (_editText.Text);
        _synth.SetOutputToDefaultAudioDevice();
      }

    #endregion

    #region Fields

      /// <summary>
      /// Flag: the form has loaded.
      /// </summary>
      bool _loaded;

      /// <summary>
      /// Next window in clipboard viewer chain to receive clipboard notifications.
      /// </summary>
      IntPtr  _chainedWnd;

      /// <summary>
      /// The TTS engine.
      /// </summary>
      SpeechSynthesizer _synth = new SpeechSynthesizer();

    #endregion
  }
}
