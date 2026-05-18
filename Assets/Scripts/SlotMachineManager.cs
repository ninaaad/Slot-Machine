// SlotMachineManager.cs
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SlotMachineManager : MonoBehaviour
{
    [Header("Reels")]
    public ReelController[] reels;

    [Header("UI References")]
    public TextMeshProUGUI balanceText;
    public TextMeshProUGUI betText;
    public TextMeshProUGUI winText;
    public Button spinButton;
    public GameObject popupPanel;
    public TextMeshProUGUI popupMessage;

    [Header("Lever")]
    public GameObject leverNormal;
    public GameObject leverPulled;

    [Header("Game Settings")]
    public int startingBalance = 1000;
    public int betAmount = 10;

    private int _balance;
    private int _reelsStopped;

    // Weighted chances per symbol index
    // Cherry=40%, Bell=30%, Diamond=20%, Seven=10%
    private int[] _weights = { 40, 30, 20, 10 };
    private const int TOTAL_WEIGHT = 100;

    void Start()
    {
        _balance = startingBalance;
        UpdateUI();
        winText.text = "";
        popupPanel.SetActive(false);

        // Make sure lever starts in normal state
        if (leverNormal != null) leverNormal.SetActive(true);
        if (leverPulled != null) leverPulled.SetActive(false);
    }

    /// <summary>
    /// Called when the Spin/Lever button is pressed.
    /// </summary>
    public void OnSpinButtonPressed()
    {
        if (IsAnyReelSpinning() || _balance < betAmount)
        {
            Debug.Log("Cannot spin: spinning or low balance.");
            return;
        }

        _balance -= betAmount;
        _reelsStopped = 0;
        spinButton.interactable = false;
        winText.text = "";
        UpdateUI();

        // Pull lever visually
        SetLever(pulled: true);

        // Generate weighted random results
        int[] results = GenerateResults();

        // Spin reels with staggered stops
        for (int i = 0; i < reels.Length; i++)
        {
            int idx = i;
            float stopDelay = 1.5f + (i * 0.6f);
            reels[i].Spin(results[i], stopDelay,
                () => OnReelStopped(results));
        }
    }

    /// <summary>
    /// Generates weighted results with a near-miss bias
    /// for excitement — 35% chance first two reels match.
    /// </summary>
    int[] GenerateResults()
    {
        int[] results = new int[reels.Length];
        bool nearMiss = Random.Range(0, 100) < 35;
        int sharedSymbol = GetWeightedRandom();

        for (int i = 0; i < reels.Length; i++)
        {
            if (nearMiss && i < reels.Length - 1)
                results[i] = sharedSymbol;
            else
                results[i] = GetWeightedRandom();
        }

        return results;
    }

    int GetWeightedRandom()
    {
        int roll = Random.Range(0, TOTAL_WEIGHT);
        int cumulative = 0;
        for (int i = 0; i < _weights.Length; i++)
        {
            cumulative += _weights[i];
            if (roll < cumulative) return i;
        }
        return _weights.Length - 1;
    }

    /// <summary>
    /// Called by each reel when it stops.
    /// Waits for all reels before evaluating.
    /// </summary>
    void OnReelStopped(int[] results)
    {
        _reelsStopped++;
        Debug.Log($"Reel stopped: {_reelsStopped}/{reels.Length}");
        if (_reelsStopped < reels.Length) return;

        EvaluateResult();
    }

    /// <summary>
    /// Checks if all center symbols match and awards payout.
    /// </summary>
    void EvaluateResult()
    {
        // Revert lever to normal
        SetLever(pulled: false);

        int firstIndex = reels[0].GetResultIndex();
        bool allMatch = true;

        for (int i = 1; i < reels.Length; i++)
        {
            Debug.Log($"Reel {i}: index={reels[i].GetResultIndex()}" +
                $" symbol={reels[i].GetResult().symbolName}");
            if (reels[i].GetResultIndex() != firstIndex)
            {
                allMatch = false;
                break;
            }
        }

        if (allMatch)
        {
            SymbolData winner = reels[0].GetResult();
            int payout = betAmount * winner.payoutMultiplier;
            _balance += payout;
            winText.text = $"YOU WIN ${payout}!";
            ShowPopup($"JACKPOT!\n+${payout}");
            Debug.Log($"WIN! {winner.symbolName} x{winner.payoutMultiplier}" +
                $" = ${payout}");
        }
        else
        {
            winText.text = "Try Again!";
            Debug.Log("No match.");
        }

        spinButton.interactable = true;
        UpdateUI();
    }

    /// <summary>
    /// Switches between normal and pulled lever states.
    /// </summary>
    void SetLever(bool pulled)
    {
        if (leverNormal != null) leverNormal.SetActive(!pulled);
        if (leverPulled != null) leverPulled.SetActive(pulled);
    }

    void UpdateUI()
    {
        balanceText.text = $"Balance: ${_balance}";
        betText.text = $"Bet: ${betAmount}";
    }

    void ShowPopup(string message)
    {
        popupMessage.text = message;
        popupPanel.SetActive(true);
    }

    public void OnPopupClose() => popupPanel.SetActive(false);

    public void OnBetIncrease()
    {
        betAmount = Mathf.Min(betAmount + 10, 100);
        UpdateUI();
    }

    public void OnBetDecrease()
    {
        betAmount = Mathf.Max(betAmount - 10, 10);
        UpdateUI();
    }

    bool IsAnyReelSpinning()
    {
        foreach (var reel in reels)
            if (reel.IsSpinning) return true;
        return false;
    }
}