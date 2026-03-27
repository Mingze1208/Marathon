using UnityEngine;

public class MouseWoodController : MonoBehaviour
{
    [Header("References")]
    public GameObject woodPrefab; // 要放置到场景中的木头预制体
    public Wood woodData; // ScriptableObject 类型的木头数据（用于背包中的引用）

    private Wood heldWood = null; // 鼠标当前持有的木头（若为 null 则表示未持有）
    private Camera mainCam;
    private GameObject cursorWoodInstance; // 鼠标持有木头时展示的图标

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        // 检测是否生成木头图标
        if (heldWood != null && cursorWoodInstance != null && mainCam != null)
        {
            Vector3 worldPos = mainCam.ScreenToWorldPoint(Input.mousePosition); //看不懂
            worldPos.z = 0f;
            cursorWoodInstance.transform.position = worldPos;
        }

        if (Input.GetMouseButtonDown(0))
        {
            HandleLeftClick();
        }
    }

    private void HandleLeftClick()
    {
        Vector3 worldPos3 = mainCam.ScreenToWorldPoint(Input.mousePosition); //看不懂
        worldPos3.z = 0f;
        Vector2 worldPos = worldPos3;

        // 先检测是否点击到带 Collider2D 的物体（例如 player）
        Collider2D hit = Physics2D.OverlapPoint(worldPos);
        if (hit != null)
        {
            // 如果点击到 player
            player p = hit.GetComponentInParent<player>();
            if (p != null)
            {
                if (heldWood == null)
                {
                    // 尝试从 player 身上拿一根木头
                    // PickWood() 已在 player 中实现为移除一根并返回是否成功
                    bool ok = p.PickWood();
                    if (ok)
                    {
                        // 我们用共享的 ScriptableObject 表示被拿起的木头类型
                        heldWood = woodData;
                        CreateCursorVisual();
                    }
                }
                else
                {
                    // 将手上的木头放回 player 背包
                    bool ok = p.AddWood(heldWood);
                    if (ok)
                    {
                        heldWood = null;
                        DestroyCursorVisual();
                    }
                    // 如果放回失败（背包满），不做处理
                }

                return;
            }
        }

        // 若没点击到 player（空白处或其他物体），且当前持有木头，则在点击位置放置木头
        if (heldWood != null)
        {
            if (woodPrefab != null)
            {
                Instantiate(woodPrefab, worldPos3, Quaternion.identity);
                heldWood = null;
                DestroyCursorVisual();
            }
            else
            {
                Debug.LogWarning("MouseWoodController: woodPrefab 未绑定，无法放置木头。");
            }
        }
    }

    private void CreateCursorVisual()
    {
        DestroyCursorVisual();

        if (woodPrefab != null)
        {
            // 用一个运行时实例作为临时视觉效果，并去除其物理组件
            cursorWoodInstance = Instantiate(woodPrefab);
            // 移除 Rigidbody2D 和 Collider2D 组件
            var rb = cursorWoodInstance.GetComponent<Rigidbody2D>();
            if (rb != null) Destroy(rb);
            var col = cursorWoodInstance.GetComponent<Collider2D>();
            if (col != null) Destroy(col);
        }
        else if (woodData != null && woodData.icon != null)
        {
            cursorWoodInstance = new GameObject("CursorWood");
            var sr = cursorWoodInstance.AddComponent<SpriteRenderer>();
            sr.sprite = woodData.icon;
            sr.sortingOrder = 1000;
        }
        else
        {
            // 创建一个空的 new GameObject("CursorWood") 作为保底占位符
            cursorWoodInstance = new GameObject("CursorWood");
        }

        if (cursorWoodInstance != null && mainCam != null)
        {
            Vector3 worldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0f;
            cursorWoodInstance.transform.position = worldPos;
        }
    }

    private void DestroyCursorVisual()
    {
        if (cursorWoodInstance != null)
        {
            Destroy(cursorWoodInstance);
            cursorWoodInstance = null;
        }
    }

    // 供 UI 显示当前是否持有木头
    public bool IsHoldingWood()
    {
        return heldWood != null;
    }
}