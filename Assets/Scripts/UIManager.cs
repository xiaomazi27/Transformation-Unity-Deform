using UnityEngine;
using UnityEngine.UI;
using static GlobalDeform;

public class UIManager : MonoBehaviour
{
    [Header("GlobalDeform：锥化、扭曲、弯曲")]
    public GlobalDeform globalDeform;
    [Header("FFDDeform：FFD自由变形")]
    public FFDDeform ffdDeform;

    [Header("强度滑块（只给Global锥扭弯用）")]
    public Slider strengthSlider;

    void Start()
    {
        if (strengthSlider != null)
        {
            strengthSlider.onValueChanged.AddListener(OnSliderChange);
        }
    }

    // 锥化按钮
    public void OnTaper()
    {
        if (globalDeform == null) return;
        ffdDeform?.DeactivateFFD();
        globalDeform.SetMode(DeformMode.Taper);
        if (strengthSlider != null)
        {
            globalDeform.SetStrength(strengthSlider.value);
        }
        Debug.Log("点击Taper");
    }

    // 扭曲按钮
    public void OnTwist()
    {
        if (globalDeform == null) return;
        ffdDeform?.DeactivateFFD();
        globalDeform.SetMode(DeformMode.Twist);
        if (strengthSlider != null)
        {
            globalDeform.SetStrength(strengthSlider.value);
        }
        Debug.Log("点击Twist");
    }

    // 弯曲按钮
    public void OnBend()
    {
        if (globalDeform == null) return;
        ffdDeform?.DeactivateFFD();
        globalDeform.SetMode(DeformMode.Bend);
        if (strengthSlider != null)
        {
            globalDeform.SetStrength(strengthSlider.value);
        }
        Debug.Log("点击Bend");
    }

    // FFD按钮
    public void OnFFD()
    {
        if (ffdDeform == null) return;
        globalDeform?.SetMode(DeformMode.None);
        ffdDeform.ActivateFFD();
        Debug.Log("点击FFD");
    }

    // 重置按钮：全部关闭
    public void OnReset()
    {
        Debug.Log("===== 执行Reset =====");
        if (globalDeform != null)
        {
            globalDeform.SetMode(DeformMode.None);
            Debug.Log("关闭Global变形");
        }
        if (ffdDeform != null)
        {
            ffdDeform.DeactivateFFD();
            Debug.Log("关闭FFD，删除控制点");
        }
    }

    // 滑块拖动：只作用在Global变形上
    public void OnSliderChange(float value)
    {
        if (globalDeform != null && globalDeform.currentMode != DeformMode.None)
        {
            globalDeform.SetStrength(value);
        }
    }
}
