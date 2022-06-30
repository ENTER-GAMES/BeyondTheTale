using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : ControllerBasedShadowDetectorTool
{
    private enum Mode { Select, Move, Rotate, Scale }
    [Header("Mode")]
    [SerializeField]
    private Mode currentMode = Mode.Select;

    // 선택된 그림자 오브젝트 리스트
    private List<ControllerBasedShadowObject> selectedShadowObjects
        = new List<ControllerBasedShadowObject>();

    private Vector2 lastMouseWorldPosition;

    [Header("Key Code")]
    [SerializeField]
    private KeyCode selectModeKey = KeyCode.Q;
    [SerializeField]
    private KeyCode moveModeKey = KeyCode.W;
    [SerializeField]
    private KeyCode rotateModeKey = KeyCode.E;
    [SerializeField]
    private KeyCode scaleModeKey = KeyCode.R;
    [SerializeField]
    private KeyCode multiSelectKey = KeyCode.LeftShift;
    private bool isMultiSelectable = false;                 // 다중 선택 모드 여부

    public override void Deselect()
    {
        base.Deselect();

        Clear();
    }

    protected override void Update()
    {
        base.Update();

        // 다중 선택 모드 활성화
        if (Input.GetKeyDown(multiSelectKey))
            isMultiSelectable = true;
        else if (Input.GetKeyUp(multiSelectKey))
            isMultiSelectable = false;

        // 모드 변경
        if (Input.GetKeyDown(selectModeKey)) ChangeMode(Mode.Select);
        else if (Input.GetKeyDown(moveModeKey)) ChangeMode(Mode.Move);
        else if (Input.GetKeyDown(rotateModeKey)) ChangeMode(Mode.Rotate);
        else if (Input.GetKeyDown(scaleModeKey)) ChangeMode(Mode.Scale);
    }

    private void ChangeMode(Mode mode)
    {
        if (currentMode == mode) return;

        currentMode = mode;
    }

    protected override void OnMouseDown(Vector3 mousePosition, int mouseIndex = 0)
    {
        base.OnMouseDown(mousePosition, mouseIndex);

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePosition);

        lastMouseWorldPosition = mouseWorldPos; ;
    }

    protected override void OnMouse(Vector3 mousePosition, int mouseIndex = 0)
    {
        base.OnMouse(mousePosition, mouseIndex);

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePosition);

        if (currentMode == Mode.Move) Move(mouseWorldPos);
        else if (currentMode == Mode.Rotate) Rotate(mouseWorldPos);
        else if (currentMode == Mode.Scale) Scale(mouseWorldPos);

        lastMouseWorldPosition = mouseWorldPos; ;
    }

    protected override void OnMouseUp(Vector3 mousePosition, int mouseIndex = 0)
    {
        base.OnMouseUp(mousePosition, mouseIndex);

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePosition);

        if (currentMode == Mode.Select)
        {
            ControllerBasedShadowObject shadowObject = Raycast(mouseWorldPos);
            Select(shadowObject);
        }
    }

    private ControllerBasedShadowObject Raycast(Vector2 startPosition)
    {
        RaycastHit2D hit = Physics2D.Raycast(startPosition, Vector2.zero, int.MaxValue, targetLayerMask);

        if (hit.collider == null) return null;

        if (hit.collider.TryGetComponent<ControllerBasedShadowObject>(out ControllerBasedShadowObject shadowObject))
            return shadowObject;
        else
            return null;
    }

    private void Select(ControllerBasedShadowObject item)
    {
        // 빈 공간 클릭하면
        if (item == null)
        {
            // 전부 삭제
            Clear();
            return;
        }

        // 다중 선택 모드일 경우
        if (isMultiSelectable)
        {
            // 이미 선택되어 있던 그림자일 경우
            if (selectedShadowObjects.Contains(item))
            {
                // 해당 그림자만 제외
                selectedShadowObjects.Remove(item);
                item.Deselect();
                item.transform.parent = transform.root;
            }
            else
            {
                // 해당 그림자 추가
                selectedShadowObjects.Add(item);
                item.Select();
            }
        }
        // 단일 선택 모드일 경우
        else
        {
            // 해당 그림자만 선택
            Clear();
            selectedShadowObjects.Add(item);
            item.Select();
        }

        // 중앙 설정
        foreach (ControllerBasedShadowObject obj in selectedShadowObjects)
            obj.transform.parent = transform.root;

        transform.position = FindCenterOfSelcetedItems();
        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.localScale = Vector3.one;

        foreach (ControllerBasedShadowObject obj in selectedShadowObjects)
            obj.transform.parent = this.transform;
    }

    private void Clear()
    {
        // 선택 취소
        foreach (ControllerBasedShadowObject obj in selectedShadowObjects)
        {
            obj.Deselect();
            obj.transform.parent = transform.root;
        }

        // 리스트 초기화
        selectedShadowObjects.Clear();

        return;
    }

    private Vector3 FindCenterOfSelcetedItems()
    {
        Vector3 min = new Vector3(int.MaxValue, int.MaxValue, 0);
        Vector3 max = new Vector3(int.MinValue, int.MinValue, 0);

        foreach (ControllerBasedShadowObject obj in selectedShadowObjects)
        {
            Vector2 curMin = obj.Bounds.min;
            Vector2 curMax = obj.Bounds.max;

            if (curMin.x < min.x) min.x = curMin.x;
            if (curMin.y < min.y) min.y = curMin.y;
            if (curMax.x > max.x) max.x = curMax.x;
            if (curMax.y > max.y) max.y = curMax.y;
        }

        return new Vector3((min.x + max.x) / 2f, (min.y + max.y) / 2f, 0);
    }

    private void Move(Vector2 currentMouseWorldPosition)
    {
        Vector3 amount = (Vector2)currentMouseWorldPosition - (Vector2)lastMouseWorldPosition;

        transform.position += amount;
    }

    private void Rotate(Vector2 currentMouseWorldPosition)
    {
        float amount = currentMouseWorldPosition.y - lastMouseWorldPosition.y;
        amount *= 30;

        transform.Rotate(new Vector3(0, 0, amount));
    }

    private void Scale(Vector2 currentMouseWorldPosition)
    {
        float amount = currentMouseWorldPosition.y - lastMouseWorldPosition.y;
        amount *= 0.5f;

        transform.localScale += Vector3.one * amount;
    }
}
