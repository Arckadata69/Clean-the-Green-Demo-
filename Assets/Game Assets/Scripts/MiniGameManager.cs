using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MiniGameManager : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] DraggableTrash trashPrefab;
    [SerializeField] Transform spawnPoint;        // the circle at the bottom
    [SerializeField] float trashSize = 1f;
    [SerializeField] int sortingOrder = 20;
    [SerializeField] ShopItem[] gloves;           // Plastic Gloves, Rubber Gloves, Grabber
    [SerializeField] string returnScene = "Map";

    [Header("Rules")]
    [SerializeField] int correctPerLevel = 5;
    [SerializeField] int maxMultiplier = 32;
    [SerializeField] int minCaps = 1;
    [SerializeField] int maxCaps = 5;
    [SerializeField] bool multiplyCaps = true;    // caps earned x the multiplier
    [SerializeField] bool retryOnWrong = true;    // wrong trash goes back to the circle

    [Header("Optional texts")]
    [SerializeField] TMP_Text capsText;
    [SerializeField] TMP_Text multiplierText;
    [SerializeField] TMP_Text streakText;
    [SerializeField] TMP_Text remainingText;

    readonly List<TrashEntry> pending = new List<TrashEntry>();
    int baseMultiplier, multiplier, streak;

    void Start()
    {
        baseMultiplier = GetBaseMultiplier();
        multiplier = baseMultiplier;

        foreach (var e in SackDisplay.Items)
            if (e.category != TrashCategory.Collectible && e.sprite != null) pending.Add(e);

        for (int i = pending.Count - 1; i > 0; i--)          // shuffle
        {
            int j = Random.Range(0, i + 1);
            (pending[i], pending[j]) = (pending[j], pending[i]);
        }

        UpdateUI();
        SpawnNext();
    }

    int GetBaseMultiplier()
    {
        if (gloves != null)
            foreach (var g in gloves)
                if (PlayerData.IsEquipped(g)) return Mathf.Max(1, Mathf.RoundToInt(g.scoreMultiplier));
        return 1;
    }

    void SpawnNext()
    {
        if (pending.Count == 0) { StartCoroutine(Finish()); return; }

        var t = Instantiate(trashPrefab);
        t.Setup(pending[0], spawnPoint.position, trashSize, sortingOrder);
        t.onDropped = OnDropped;
    }

    void OnDropped(DraggableTrash t, Bin bin)
    {
        if (bin == null) { t.ReturnHome(); return; }       // dropped on nothing: no penalty

        TrashEntry entry = t.Entry;

        if (bin.accepts == entry.category)
        {
            int reward = Random.Range(minCaps, maxCaps + 1);
            if (multiplyCaps) reward *= multiplier;
            PlayerData.Caps += reward;

            streak++;
            if (streak >= correctPerLevel)
            {
                streak = 0;
                multiplier = Mathf.Min(multiplier * 2, maxMultiplier);
            }

            SackDisplay.Remove(entry);
            pending.RemoveAt(0);
            bin.PopCorrect();
            t.Consume(bin.transform.position);
            StartCoroutine(SpawnAfter(0.3f));
        }
        else
        {
            streak = 0;
            multiplier = baseMultiplier;
            bin.ShakeWrong();

            if (retryOnWrong)
            {
                t.ReturnHome();
            }
            else
            {
                SackDisplay.Remove(entry);
                pending.RemoveAt(0);
                t.Consume(bin.transform.position);
                StartCoroutine(SpawnAfter(0.3f));
            }
        }
        UpdateUI();
    }

    IEnumerator SpawnAfter(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        SpawnNext();
    }

    IEnumerator Finish()
    {
        if (remainingText) remainingText.text = "Sack empty!";
        yield return new WaitForSecondsRealtime(1.5f);
        SceneManager.LoadScene(returnScene);
    }

    void UpdateUI()
    {
        if (capsText) capsText.text = "(" + PlayerData.Caps.ToString("N0") + ")";
        if (multiplierText) multiplierText.text = multiplier + "x";
        if (streakText) streakText.text = streak + "/" + correctPerLevel;
        if (remainingText) remainingText.text = pending.Count + " left";
    }
}