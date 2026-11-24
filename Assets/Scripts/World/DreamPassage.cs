using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public sealed class DreamPassage : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private string destination;
    [SerializeField] private string prompt;
    [SerializeField, Min(0.1f)] private float reach = 1.7f;

    private bool loading;
    private bool readyForInput;
    private GUIStyle style;
    private bool InReach => player && ((Vector2)(player.position - transform.position)).sqrMagnitude <= reach * reach;

    private void Update()
    {
        if (loading) return;
        var key = Keyboard.current?.eKey;
        var button = Gamepad.current?.buttonSouth;
        bool pressed = key?.wasPressedThisFrame == true || button?.wasPressedThisFrame == true;
        // 抵达另一侧时，等上一张地图的入门按键完全释放，再接受下一次交互。
        if (!readyForInput)
        {
            readyForInput = !pressed && key?.isPressed != true && button?.isPressed != true;
            return;
        }
        if (!pressed || !InReach) return;

        if (!Application.CanStreamedLevelBeLoaded(destination))
        {
            Debug.LogError("场景未加入 Build Settings：" + destination, this);
            return;
        }
        loading = true;
        SceneManager.LoadSceneAsync(destination);
    }

    private void OnGUI()
    {
        if (loading || !InReach) return;
        if (style == null)
        {
            style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
            style.normal.textColor = new Color(1f, .98f, .88f);
        }
        style.fontSize = Mathf.Clamp(Screen.height / 32, 15, 24);
        float width = Mathf.Min(310, Screen.width - 16);
        var box = new Rect((Screen.width - width) * .5f, Screen.height - style.fontSize * 3.4f, width, style.fontSize * 2);
        GUI.Box(box, GUIContent.none);
        GUI.Label(box, prompt, style);
    }
}
