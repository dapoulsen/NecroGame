using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RadialFillManager : MonoBehaviour
{
    [SerializeField] Image radialFillImage;
    [SerializeField] float fillDuration = 1f;
    
    private Coroutine _fillCoroutine = null;

    public void UpdateRadialProgressCircle(int valueTofill, int maxFill)
    {
        float targetFillAmount = (float)(valueTofill + 1) / maxFill;

        if (_fillCoroutine == null)
        {
            StopCoroutine(_fillCoroutine);
        }
        
        _fillCoroutine = StartCoroutine(AnimateFill(targetFillAmount));
    }

    private IEnumerator AnimateFill(float targetFillAmount)
    {
        float initialFillAmount = radialFillImage.fillAmount;
        float elapsedTime = 0f;

        while (elapsedTime < fillDuration)
        {
            elapsedTime += Time.deltaTime;
            radialFillImage.fillAmount = Mathf.Lerp(initialFillAmount, targetFillAmount, elapsedTime / fillDuration);
            yield return null;
        }
        
        radialFillImage.fillAmount = targetFillAmount;
        
        _fillCoroutine = null;
    }
}


