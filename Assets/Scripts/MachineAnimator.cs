using UnityEngine;
using UnityEngine.UI;

public class MachineAnimator : MonoBehaviour
{
    public Sprite[] frames;         // slot-machine1 to 5
    public Image targetImage;
    public float fps = 8f;

    private float _timer;
    private int _currentFrame;

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= 1f / fps)
        {
            _timer = 0f;
            _currentFrame = (_currentFrame + 1) % frames.Length;
            targetImage.sprite = frames[_currentFrame];
        }
    }
}