using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IInterfaceA { }

public enum EnumA { Enum1, Enum2 }

public class CodingStandard : MonoBehaviour
{
    private static readonly string STATIC_A = "Static A";

    public static UnityEvent OnChangedPosition = new UnityEvent();

    // Variable
    [Header("Header")]
    [SerializeField]
    private int variablePrivate;            // 변수 의미 주석으로 달아주기
    private bool isBoolean;                 // 변수 의미 주석으로 달아주기
    public int VariablePublic;              // 변수 의미 주석으로 달아주기

    // Property: 단순 값 전달에만 사용
    public int PropertyVariableA => variablePrivate;
    public int PropertyVariableB { get; set; }

    #region Component
    private Component componentA;
    private Component componentB;
    private Component componentC;
    #endregion

    private Animator animator;
    private Material[] materials;

    private Coroutine routine;

    private void Start()
    {
        // return은 옆에
        if (isBoolean) return;

        // 한 줄 if/else는 중괄호 생략 가능
        if (isBoolean)
            Start();
        else
            Start();

        // bool 비교는 == 없이
        if (!isBoolean)
            Start();

        // null 체크는 명시
        if (componentA == null)
        {

        }

        // for에서는 i++로
        for (int i = 0; i < 10; i++)
        {

        }

        // Nullable한 Component, Event에 ? 사용 가능
        animator?.Play("Animation");
        animator?.SetBool("IsBoolean", false);
    }

    private void SetCurrentMaterials(Material[] materials)
    {
        // 이름 같은 변수면 this 작성
        this.materials = materials;

        // var 쓰지 않기
        foreach (Material material in materials)
        {
            material.SetColor("color", Color.black);
        }

        // 알잘딱으로 개행
        Physics.Raycast(
            Vector3.zero, Vector3.up, 30, -1,
            QueryTriggerInteraction.UseGlobal
        );
    }

    // 함수명은 최대한 줄이지 않고 작성
    private Material[] GetCurrentMaterials()
    {
        // 지역변수는 의미를 회손하지 않는 선에서 단축
        Material[] curMats = new Material[0];

        return curMats;
    }

    public void StartRoutine()
    {
        routine = StartCoroutine(Routine(1, 2, 3));
    }

    public void StopRoutine()
    {
        StopCoroutine(routine);
    }

    private IEnumerator Routine(int a, int b, int c)
    {
        yield return new WaitForSeconds(0);
    }

    private class CodingStandardSubClass
    {
        // 생성자 접근자 명시
        public CodingStandardSubClass()
        {

        }
    }
}

