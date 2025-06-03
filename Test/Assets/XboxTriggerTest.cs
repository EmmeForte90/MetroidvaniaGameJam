using UnityEngine;

public class XboxTriggerTest : MonoBehaviour
{
    public string rtAxis       = "RT";        // configurato su 6° asse
    public string ltAxis       = "LT";        // configurato su 3° asse
    public string comboAxis    = "Triggers";  // configurato su 9° asse
    [Header("Threshold to consider 'pressed'")]
    [Header("Press Threshold")]
    [Range(0f, 1f)] public float threshold = 0.1f;

    void Update()
    {
        float rt = Input.GetAxisRaw(rtAxis);
        float lt = Input.GetAxisRaw(ltAxis);
        float combo = Input.GetAxisRaw(comboAxis);
        if (rt < threshold){rt = Mathf.Max(0f, combo); }
        if (lt < threshold){lt = Mathf.Max(0f, -combo); }   
        if (rt > threshold){OnRightTrigger(rt);}
        if (lt > threshold){OnLeftTrigger(lt);}
    }

    protected virtual void OnRightTrigger(float value)
    {
        Debug.Log($"RT: {value:F2}");
        // la tua logica per RT
    }
    

    protected virtual void OnLeftTrigger(float value)
    {
        Debug.Log($"LT: {value:F2}");
        // la tua logica per LT
    }
}
