using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIController : MonoBehaviour
{
    private static InGameUIController _instance;
    public static InGameUIController Instance { get { return _instance; } }

    public AimingImage aimUI;
    public CountText magazineText;
    public CountText bulletsText;
    public CountText enemyCountText;
    public WaveCountText waveText;
    public HealthBar healthBar;
    public WeaponChoose weaponChoose;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        if(!isInitialized)
        {
            Debug.LogWarning("No properly setted ui components");
        }
    }

    public void SetMagazineAndBullets(int magazine, int bullets)
    {
        magazineText.SetTextValue(magazine);
        bulletsText.SetTextValue(bullets);
    }

    public void SetWave(int wave)
    {
        waveText.animated = true;
        waveText.SetTextValue(wave);
    }

    public bool isInitialized => aimUI != null && magazineText != null && bulletsText != null && healthBar != null && weaponChoose != null;
}
