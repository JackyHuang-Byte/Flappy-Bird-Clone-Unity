using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class testscript : MonoBehaviour
{
    public Button ClickButton;
    public float totalDuration;
    public float maxMagnitude;
    public float easePower = 3f;
    public float decayPoint = 3f; //one third of time
    public float maxDelay = 0.08f;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClickButton()
    {
        StartCoroutine(ScreenShake(totalDuration, maxMagnitude, easePower, decayPoint, maxDelay));
    }


    public IEnumerator ScreenShake(float totalDuration, float maxMagnitude, float easePower, float decayPoint, float maxDelay)
    {
        float elapsedTime = 0f; // 当前经过的时间
        float halfDuration = totalDuration / decayPoint; // one-third of total duration
        float magnitude;
        float shakeTimer = 0;
        float frequency;

        Vector3 originalPosition = ClickButton.transform.position; // 记录原始位置

        while (elapsedTime < totalDuration)
        {
            elapsedTime += Time.deltaTime;
            shakeTimer += Time.deltaTime;

            // 计算当前震动强度

            if (elapsedTime < halfDuration)
            {
                // 阶段1：从接近0成长到最大值
                magnitude = Mathf.Lerp(0, maxMagnitude, EaseOut(elapsedTime / halfDuration, easePower));
                frequency = Mathf.Lerp(maxDelay, 0, EaseOut(elapsedTime / halfDuration, easePower));

                Debug.Log("Phase #1: " + elapsedTime + "; Magnitude: " + magnitude + "; Frequency: " + frequency); 
            }
            else
            {
                // 阶段2：从最大值逐渐衰减到0
                float t = (elapsedTime - halfDuration) / (totalDuration - halfDuration); // 归一化到0~1
                magnitude = Mathf.Lerp(maxMagnitude, 0, EaseIn(t, easePower));
                frequency = Mathf.Lerp(0, maxDelay, EaseIn(t, easePower));

                Debug.Log("Phase #2: " + elapsedTime + "; Magnitude: " + magnitude +  "; Frequency: " + frequency);
            }

            if (shakeTimer < frequency)
            {
                yield return null;
                continue;
            }


            // 随机偏移
            shakeTimer = 0;
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;

            //float offsetX = magnitude;

            // 应用偏移到相机位置
            ClickButton.transform.position = new Vector3(originalPosition.x + offsetX, originalPosition.y + offsetY, originalPosition.z);

            //offsetX = magnitude;
            //ClickButton.transform.position = new Vector3(originalPosition.x + offsetX, originalPosition.y, originalPosition.z);


            yield return null;
            //yield return new WaitForSeconds(0.25f);

        }

        // 恢复到原始位置
        ClickButton.transform.position = originalPosition;

        Debug.Log("End of OnClick Event");

    }

    // Ease-Out 缓动函数（快速增长）
    private float EaseOut(float t, float p)
    {
        return 1 - Mathf.Pow(1 - t, p); // Ease-Out

    }

    // Ease-In 缓动函数（缓慢衰减）
    private float EaseIn(float t, float p)
    {
        return Mathf.Pow(t, p); // Ease-In
    }



}