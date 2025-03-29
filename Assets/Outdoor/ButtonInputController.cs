using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonInputController : MonoBehaviour
{
    public Button myButton;
    private RectTransform _buttonRect;
    [Header("输入缓冲引用")]
    public InputBuffer inputBuffer; // 手动拖拽赋值

    void Start()
    {
        _buttonRect = myButton.GetComponent<RectTransform>();
        // 绑定点击事件到 Unity 的 Button 组件
        myButton.onClick.AddListener(OnButtonClick);
    }

    void Update()
    {
        // 鼠标左键触发点击事件（仅在按钮区域内）
        if (Input.GetMouseButtonDown(0) && IsMouseOverButton())
        {
            myButton.onClick.Invoke(); // 触发事件（颜色 + Log）
        }
    }

    bool IsMouseOverButton()
    {
        Vector2 mousePosition = Input.mousePosition;
        return RectTransformUtility.RectangleContainsScreenPoint(_buttonRect, mousePosition);
    }

    void OnButtonClick()
    {
        Debug.Log("按钮被触发！");
        inputBuffer.AddInput();
    }
}