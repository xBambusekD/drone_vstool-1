using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class VideoPlayerControls : MonoBehaviour {

    [SerializeField]
    protected Button playButton;
    [SerializeField]
    protected Button pauseButton;
    [SerializeField]
    protected Slider progressBar;

    public bool IsPlaying {
        get; protected set;
    } = false;

    private void Start() {
        playButton.gameObject.SetActive(true);
        pauseButton.gameObject.SetActive(false);
        progressBar.value = 0;
    }

    public virtual void OnPlayButton() {
        int maxValue = FlightLogPlayerManager.Instance.LoadedLogLines;

        if (maxValue > 0) {
            progressBar.maxValue = FlightLogPlayerManager.Instance.LoadedLogLines - 1;
            playButton.gameObject.SetActive(false);
            pauseButton.gameObject.SetActive(true);
            IsPlaying = true;
        }
    }

    public virtual void OnPauseButton() {
        playButton.gameObject.SetActive(true);
        pauseButton.gameObject.SetActive(false);
        IsPlaying = false;
    }

    public abstract void OnProgressBarValueChange(float value);

    public abstract void UpdateStatus(bool play);

    public abstract void UpdateProgressBar(int value);

}
