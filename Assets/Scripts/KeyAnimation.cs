using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class KeyAnimation : MonoBehaviour
{
    public Sprite keyUp;
    public Sprite keyDown;

    public float switchInterval = 0.5f;

    private SpriteRenderer _sr;

    private float _timer;

    private bool _isDown;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        _timer = 0f;
        _isDown = false;
        _sr.sprite = keyUp;
    }

    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= switchInterval)
        {
            _timer = 0f;
            _isDown = !_isDown;
            _sr.sprite = _isDown ? keyDown : keyUp;
        }
    }
}
