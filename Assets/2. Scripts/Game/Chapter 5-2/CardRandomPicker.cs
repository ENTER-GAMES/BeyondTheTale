using System.Collections.Generic;
using UnityEngine;

public class CardRandomPicker : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> cardList;
    private List<GameObject> cardListClone;

    [SerializeField]
    private DialCodeText[] alphabets;
    [SerializeField]
    private DialCodeText[] patterns;
    [SerializeField]
    private DialCodeText[] numbers;

    [SerializeField]
    private Pin pin;

    private void Awake()
    {
        cardListClone = CopyList(cardList);
        DeactivateAll();
    }

    private List<GameObject> CopyList(List<GameObject> list)
    {
        List<GameObject> copy = new List<GameObject>();
        foreach (GameObject item in list)
            copy.Add(item);
        return copy;
    }

    public void Init()
    {
        int[] targetIndexs = new int[3];
        targetIndexs[0] = PickAlphabetCard() + 1;
        targetIndexs[1] = PickPatternCard() + 1;
        targetIndexs[2] = PickNumberCard() + 1;
        pin.Init(targetIndexs);
    }

    private GameObject PickRandomCard()
    {
        int randomIndex = Random.Range(0, cardListClone.Count);
        GameObject card = cardListClone[randomIndex];
        cardListClone.RemoveAt(randomIndex);
        card.gameObject.SetActive(true);
        return card;
    }

    private int PickAlphabetCard()
    {
        GameObject card = PickRandomCard();
        int randomIndex = Random.Range(0, alphabets.Length);
        alphabets[randomIndex].Init(card.transform);
        return randomIndex;
    }

    private int PickPatternCard()
    {
        GameObject card = PickRandomCard();
        int randomIndex = Random.Range(0, patterns.Length);
        patterns[randomIndex].Init(card.transform);
        return randomIndex;
    }

    private int PickNumberCard()
    {
        GameObject card = PickRandomCard();
        int randomIndex = Random.Range(0, numbers.Length);
        numbers[randomIndex].Init(card.transform);
        return randomIndex;
    }

    private void DeactivateCardAll()
    {
        for (int i = 0; i < cardList.Count; i++)
            cardList[i].gameObject.SetActive(false);
    }

    private void DeactivateDialCodeAll()
    {
        for (int i = 0; i < alphabets.Length; i++)
            alphabets[i].gameObject.SetActive(false);

        for (int i = 0; i < patterns.Length; i++)
            patterns[i].gameObject.SetActive(false);

        for (int i = 0; i < numbers.Length; i++)
            numbers[i].gameObject.SetActive(false);
    }

    public void DeactivateAll()
    {
        DeactivateCardAll();
        DeactivateDialCodeAll();
    }
}
