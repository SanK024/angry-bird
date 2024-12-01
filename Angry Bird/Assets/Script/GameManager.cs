using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int MaxNumberofShots = 3;
    [SerializeField] private float _secondsToWaitBeforeDeathCheck = 3f;
    [SerializeField] private GameObject _restartGameObject;
    [SerializeField] private SlingShotHandler _slinkShotHandler;
    [SerializeField] private Image _nextLevelImage;

    private int _usedNumberOfShots;

    private IconHandler _iconHandler;

    private List<Baddie> _baddie = new List<Baddie>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        _iconHandler = FindObjectOfType<IconHandler>();

        Baddie[] baddies = FindObjectsOfType<Baddie>();
        for(int i=0; i<baddies.Length; i++)
        {
            _baddie.Add(baddies[i]);
        }
        _nextLevelImage.enabled = false;
    }

    public void UseShot()
    {
        _usedNumberOfShots++;
        _iconHandler.UsedShot(_usedNumberOfShots);

        CheckForLastShot();
    }

    public bool HasEnoughShots()
    {
        if (_usedNumberOfShots < MaxNumberofShots)
        {
            return true;
        }
        return false;
    }

    public void CheckForLastShot()
    {
        if(_usedNumberOfShots == MaxNumberofShots)
        {
            StartCoroutine(CheckAfterWaitTime());
        }
    }

    private IEnumerator CheckAfterWaitTime()
    {
        yield return new WaitForSeconds(_secondsToWaitBeforeDeathCheck);

        if (_baddie.Count == 0)
        {
            WinGame();
        }
        else
        {
            RestartGame();
        }
    }

    public void RemoveBaddie(Baddie baddie)
    {
        _baddie.Remove(baddie);
        CheckForAllDeadBaddies();
    }

    private void CheckForAllDeadBaddies()
    {
        if (_baddie.Count == 0)
        {
            WinGame();
        }
    }



    #region Win/Lose

    private void WinGame()
    {
        _restartGameObject.SetActive(true);
        _slinkShotHandler.enabled = false;

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int maxLevels = SceneManager.sceneCountInBuildSettings;

        if(currentSceneIndex+1 < maxLevels)
        {
            _nextLevelImage.enabled = true;
        }
    }

    public void RestartGame()
    {
        DOTween.Clear(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    #endregion

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
