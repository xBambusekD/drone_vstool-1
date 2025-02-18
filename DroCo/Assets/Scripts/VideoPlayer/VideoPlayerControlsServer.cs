using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoPlayerControlsServer : VideoPlayerControls {


    public override void OnPlayButton() {
        base.OnPlayButton();

        StartCoroutine(PlayFlightLog());

        ExperimentManager.Instance.OnPlayButtonPressed();
    }

    private IEnumerator PlayFlightLog() {
        while (IsPlaying) {
            progressBar.value += 1;
            if (progressBar.value == progressBar.maxValue) {
                progressBar.value = 0;
                OnPauseButton();
            }

            yield return new WaitForSeconds(0.05f);
        }
    }

    public override void OnPauseButton() {
        base.OnPauseButton();

        ExperimentManager.Instance.OnPauseButtonPressed();
    }

    public override void OnProgressBarValueChange(float value) {
        if (progressBar.value == progressBar.maxValue) {
            OnPauseButton();
        } else {
            ExperimentManager.Instance.SyncVideoPlayerControls((int) value);
            FlightLogPlayerManager.Instance.PlayLogMessage((int) value);
        }
    }

    public override void UpdateStatus(bool play) {
        if (play) {
            OnPlayButton();
        } else {
            OnPauseButton();
        }
    }

    public override void UpdateProgressBar(int value) {
        progressBar.value = value;
    }
}
