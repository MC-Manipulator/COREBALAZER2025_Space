using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// InputBuffer.cs（不修改，无单例）
public class InputBuffer : MonoBehaviour
{
    [Header("输入缓冲时间")]
    public float bufferDuration = 0.2f;

    private List<float> _inputTimestamps = new List<float>();

    private void Update()
    {
        // 处理键盘输入（例如空格键）
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddInput();
        }
    }

    public void AddInput()
    {
        _inputTimestamps.Add(Time.time);
    }

    public bool HasValidInput()
    {
        ClearExpiredInputs();
        return _inputTimestamps.Count > 0;
    }
    // 添加公共方法获取输入数量
    public int GetInputCount()
    {
        ClearExpiredInputs();
        return _inputTimestamps.Count;
    }

    // 清理过期输入（可选）
    public void ClearExpiredInputs()
    {
        _inputTimestamps.RemoveAll(t => Time.time - t > bufferDuration); 
    }

    // 清理所有输入记录
    public void ClearInputs()
    {
        _inputTimestamps.Clear();
    }
}
