using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, DestroyAble
{
    public float Life { get; private set;}
    public bool IsAlive { get => Life > 0; }

    [SerializeField]
    private float maxLife = 100;

    [Header("Starting Weapon Configuration")]
    [SerializeField]
    private List<GameObject> weaponsPrefabs;
    private List<Weapon> weapons;
    private List<GameObject> weaponsObjects;
    [SerializeField]
    private int meleeWeaponIndex = 0;
    [SerializeField]
    private int primaryWeapon = 1;

    private int chosedWeapon = 0;

    [Header("Dying Parameters")]
    [SerializeField]
    private float dyingTime = 1;

    private TintCameraPP tintCamera;

    private CharacterAudioController characterAudioController;

    [Header("Weapon Change Parameters")]
    private Transform weaponSack;
    private bool isChanging;
    private int newWeapon;
    [SerializeField]
    private float hideShowTime = 0.1f;

    // Start is called before the first frame update
    virtual public void Awake()
    {
        weapons = new List<Weapon>();
        weaponsObjects = new List<GameObject>();
        Life = maxLife;
        tintCamera = GetComponentInChildren<TintCameraPP>();
        foreach(var prefab in weaponsPrefabs)
        {
            InstantiateWeapon(prefab);
        }
        characterAudioController = GetComponent<CharacterAudioController>();
        weaponSack = GetComponentInChildren<WeaponSack>().transform;
    }

    virtual public void Start()
    {
        for(int i = 0; i< weaponsObjects.Count; i++)
        {
            InGameUIController.Instance.weaponChoose.SetWeaponSlot(i, weapons[i].Icon, (i + 1).ToString());
            weaponsObjects[i].SetActive(false);
        }
        chosedWeapon = primaryWeapon;
        weaponsObjects[chosedWeapon].SetActive(true);
        InGameUIController.Instance.weaponChoose.ChooseWeapon(chosedWeapon, false);
        RefreshBulletsUI();
        InGameUIController.Instance.healthBar.SetHealth(Life, maxLife);
    }

    virtual public void Shoot()
    {
        if(weapons[chosedWeapon] != null && !isChanging)
        {
            weapons[chosedWeapon].Shoot(transform.position, transform.forward);
            RefreshBulletsUI();
        }
    }

    virtual public void Reload()
    {
        if (weapons[chosedWeapon] != null && !isChanging)
        {
            weapons[chosedWeapon].Reload();
        }
    }

    virtual public void QuickAttack()
    {
        if (!isChanging)
        {
            StartCoroutine(QuickAttackRoutine());
        }
    }

    virtual public void TakeDamage(float damage, Vector3 hitPoint)
    {
        Life -= damage;
        characterAudioController.state = CharacterState.Suffering;
        tintCamera.tintValue = 0.7f;
        InGameUIController.Instance.healthBar.SetHealth(Life, maxLife);
        if (!IsAlive)
        {
            characterAudioController.state = CharacterState.Dying;
            StartCoroutine(Dying());
        }
        characterAudioController.UpdateSound();
    }

    virtual public void ChangeWeapon(int newChosed)
    {
        if (newChosed >= weapons.Count)
        {
            newChosed = 0;
        }
        else if (newChosed < 0)
        {
            newChosed = weapons.Count - 1;
        }

        if (!isChanging && chosedWeapon != newChosed)
        {
            newWeapon = newChosed;
            StartCoroutine(HideShowWeapon(weaponsObjects[newChosed], weaponsObjects[chosedWeapon], weaponsPrefabs[newChosed].transform));
            InGameUIController.Instance.weaponChoose.ChooseWeapon(newChosed);
        }
    }

    virtual public void ChangeWeaponToNext(int direction)
    {
        var newChose = chosedWeapon + direction;
        ChangeWeapon(newChose);
    }

    virtual public void SetRecoilFromMove(float value)
    {
        var weapon = weapons[chosedWeapon] as RecoilingWeapon;
        if(weapon != null)
        {
            weapon.RecoilMagnitude = Mathf.Max(weapon.RecoilMagnitude, value);
        }
    }

    private bool InstantiateWeapon(GameObject prefab)
    {
        if (prefab == null)
        {
            return false;
        }
        var weapon = prefab.GetComponent<Weapon>();
        if (weapon == null)
        {
            return false;
        }
        var newWeapon = Instantiate(prefab, transform);
        weaponsObjects.Add(newWeapon);
        weapons.Add(newWeapon.GetComponent<Weapon>());

        return true;
    }

    private void RefreshBulletsUI()
    {
        InGameUIController.Instance.SetMagazineAndBullets(weapons[chosedWeapon].Magazine, weapons[chosedWeapon].BulletsLeft);
    }

    private void ChangeCallback(GameObject newShowed, GameObject newHidden)
    {
        weapons[chosedWeapon].OnHide();
        newHidden.SetActive(false);
        chosedWeapon = newWeapon;
        newShowed.SetActive(true);
        weapons[chosedWeapon].OnShow();
        RefreshBulletsUI();
    }

    private IEnumerator QuickAttackRoutine()
    {
        var previousChoosed = chosedWeapon;
        ChangeWeapon(meleeWeaponIndex);

        while (isChanging)
        {
            yield return null;
        }

        Shoot();

        while (!weapons[chosedWeapon].IsReady)
        {
            yield return null;
        }

        ChangeWeapon(previousChoosed);
    }

    #region Animations
    private IEnumerator HideShowWeapon(GameObject newShowed, GameObject newHidden, Transform showedPosition)
    {

        isChanging = true;
        yield return AnimateHideShow(newHidden, newHidden.transform, weaponSack);

        ChangeCallback(newShowed, newHidden);

        yield return AnimateHideShow(newShowed, weaponSack, showedPosition);
        isChanging = false;
    }

    private IEnumerator AnimateHideShow(GameObject weapon, Transform start, Transform end)
    {
        float elapsedTime = 0;
        var startPosition = start.localPosition;
        var startRotation = start.localRotation;

        var endPosition = end.localPosition;
        var endRotation = end.localRotation;

        while (elapsedTime <= hideShowTime)
        {
            weapon.transform.localPosition = Vector3.Lerp(startPosition, endPosition, elapsedTime / hideShowTime);
            weapon.transform.localRotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / hideShowTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        weapon.transform.localPosition = endPosition;
        weapon.transform.localRotation = endRotation;
    }

    private IEnumerator Dying()
    {
        GetComponent<CharacterController>().enabled = false;
        GetComponent<InputController>().enabled = false;
        var timeRemain = dyingTime;
        var startRotation = transform.rotation;
        var startEuler = startRotation.eulerAngles;
        startEuler.x = -90;
        var endRotation = Quaternion.Euler(startEuler);
        while ((timeRemain -= Time.deltaTime) > 0)
        {
            transform.rotation = Quaternion.Slerp(endRotation, startRotation, timeRemain / dyingTime);
            yield return null;
        }
        Destroy(this);
        //TODO: zmienić na wysłanie końca gry do gamemanagera

    }
    #endregion
}
