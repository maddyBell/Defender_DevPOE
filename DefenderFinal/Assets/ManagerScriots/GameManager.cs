using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    [Header("Bones Economy")]
    public int bones = 10;   // starting bones
    public int bonesPerKill = 5; // reward per enemy kill
    public TextMeshProUGUI bonesText;

    [Header("UI Panels")]
    public GameObject optionsMenuPanel;
    public GameObject gameOverCanvas;
    public GameObject flamePrefab;
    public DefenderPlacement defenderPlacement;
    public GameObject eventSystem;
    //Buttons

    public GameObject wizard;
    public GameObject archer;
    public GameObject cannon;

    public GameObject tFlameUpgrade;
    public GameObject tPoisonUpgrade;
    public GameObject dFlameUpgrade;
    public GameObject dPoisonUpgrade;

    public bool addDefenderOpen = false;
    public bool upgradeTowerOpen = false;
    public bool upgradeDefenderOpen = false;
    public int numDefenders = 10;

    public GameObject[] Defenders;


    public int currentDefenderIndex = 0;


    private bool isGamePaused = false;

    void Awake()
    {
        eventSystem.SetActive(true);
    }
    void Start()
    {
        DisplayBones();
        if (optionsMenuPanel) optionsMenuPanel.SetActive(false);
        if (gameOverCanvas) gameOverCanvas.SetActive(false);
        Defenders = new GameObject[numDefenders];

        HideButtonsOnStart();

    }

    // --- Bones Economy ---
    public bool SpendBones(int cost)
    {
        if (bones >= cost)
        {
            bones -= cost;
            DisplayBones();
            return true;
        }
        return false;
    }

    public void CollectBones()
    {
        bones += bonesPerKill;
        DisplayBones();
    }

    public int GetBones() => bones;

    private void DisplayBones()
    {
        if (bonesText) bonesText.text = bones.ToString();
    }

    // --- Options Menu ---
    public void OpenOptionsMenu()
    {
        if (!optionsMenuPanel) return;

        optionsMenuPanel.SetActive(true);
        PauseGame();
    }

    public void ResumeGame()
    {
        if (!optionsMenuPanel) return;

        optionsMenuPanel.SetActive(false);
        UnpauseGame();
    }

    public void RestartGame()
    {
        UnpauseGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitToOpeningScene()
    {
        UnpauseGame();
        SceneManager.LoadScene("OpenScene");
    }

    // --- Game Over ---
    public void GameOver()
    {
        if (!gameOverCanvas) return;

        gameOverCanvas.SetActive(true);
        PauseGame();
    }

    // --- Pause / Unpause ---
    private void PauseGame()
    {
        isGamePaused = true;
        Time.timeScale = 0f;
    }

    private void UnpauseGame()
    {
        isGamePaused = false;
        Time.timeScale = 1f;
    }

    // --- Tower Death Check ---
    public void TowerDead(bool isDead)
    {
        if (isDead)
        {
            GameOver();
            Debug.Log("Game Over! The tower has been destroyed.");
        }
    }

    public void UpgradeTower(string upgradeType)
    {
        Tower tower = FindObjectOfType<Tower>();
        if (tower != null)
        {
            tower.UpgradeTower(upgradeType);
            Debug.Log("Upgraded Tower with " + upgradeType);
        }
    }

    public void TestClick()
    {
        Debug.Log("BUTTON CLICKED");
    }
    public void OpenAddDefenderMenu()
    {
        Debug.Log("Clicked");
        if (addDefenderOpen)
        {
            addDefenderOpen = false;
            wizard.SetActive(false);
            archer.SetActive(false);
            cannon.SetActive(false);
            return;
        }
        else
        {
            addDefenderOpen = true;
            wizard.SetActive(true);
            archer.SetActive(true);
            cannon.SetActive(true);

            Debug.Log("Open");
        }

    }
    public void OpenDefenderUpgradeMenu()
    {
        if (upgradeDefenderOpen)
        {
            upgradeDefenderOpen = false;
            dFlameUpgrade.SetActive(false);
            dPoisonUpgrade.SetActive(false);

            return;
        }
        else
        {
            upgradeDefenderOpen = true;
            dFlameUpgrade.SetActive(true);
            dPoisonUpgrade.SetActive(true);

        }

    }
    public void OpenTowerUpgradeMenu()
    {
        if (upgradeTowerOpen)
        {
            upgradeTowerOpen = false;
            tFlameUpgrade.SetActive(false);
            tPoisonUpgrade.SetActive(false);

            return;
        }
        else
        {
            upgradeTowerOpen = true;
            tFlameUpgrade.SetActive(true);
            tPoisonUpgrade.SetActive(true);

        }

    }

    public void UpgradeDefender(string upgradeType)
    {
        Debug.Log("Upgrading Defenders with " + upgradeType);

        if (upgradeType == "Poison" && bones >= 15 || upgradeType == "Flame" && bones >= 20)
        {
            foreach (var defender in GetAllDefenders())
            {
                if (defender.GetComponent<Defender>() != null)
                {
                    defender.GetComponent<Defender>().UpgradingDefender(upgradeType);
                }
            }

            if (upgradeType == "Poison")
            {
                SpendBones(15);
            }
            else
            {
                SpendBones(20);
            }
        }

    }

    public void HideButtonsOnStart()
    {
        wizard.SetActive(false);
        archer.SetActive(false);
        cannon.SetActive(false);

        tFlameUpgrade.SetActive(false);
        tPoisonUpgrade.SetActive(false);

        dFlameUpgrade.SetActive(false);
        dPoisonUpgrade.SetActive(false);
    }

    public void AddDefender(GameObject wizardDefender)
    {
        Defenders[currentDefenderIndex] = wizardDefender;
        currentDefenderIndex++;
    }

    public GameObject[] GetAllDefenders()
    {

        Defender[] defenders = FindObjectsOfType<Defender>();
        GameObject[] defenderObjects = new GameObject[defenders.Length];


        for (int i = 0; i < defenders.Length; i++)
        {
            defenderObjects[i] = defenders[i].gameObject;
        }

        return defenderObjects;
    }

}