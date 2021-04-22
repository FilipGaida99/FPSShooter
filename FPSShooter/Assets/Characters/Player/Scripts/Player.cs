using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, DestroyAble
{
    public float Life { get; private set;}
    public bool IsAlive { get => Life > 0; }

    [SerializeField]
    private float grabDistance = 1;
    [SerializeField]
    private int maxWeaponCount = 4;

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


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, grabDistance);
    }

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

    virtual public IEnumerator Start()
    {
        for(int i = 0; i< weaponsObjects.Count; i++)
        {
            PrepareWeapon(i);
        }
        chosedWeapon = primaryWeapon;
        weaponsObjects[chosedWeapon].SetActive(true);
        InGameUIController.Instance.weaponChoose.ChooseWeapon(chosedWeapon, false);
        RefreshBulletsUI();
        InGameUIController.Instance.healthBar.SetHealth(Life, maxLife);

        //Wait for everything set up and start game.
        yield return null;
        GameManager.Instance.StartGame();
    }

    virtual public void Shoot()
    {
        if(weapons[chosedWeapon] != null && !isChanging)
        {
            weapons[chosedWeapon].Shoot(transform.position, transform.forward);
            RefreshBulletsUI();
        }
    }

    virtual public void FreeTrigger()
    {
        if (weapons[chosedWeapon] != null && !isChanging)
        {
            weapons[chosedWeapon].FreeTrigger();
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

    virtual public void TryGrab()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, grabDistance))
        {
            var crate = hit.collider.gameObject.GetComponent<SupplyCrate>();
            if(crate != null)
            {
                crate.Resupply(this);
            }
        }
    }

    virtual public bool AddWeapon(GameObject prefab)
    {
        bool wasResupplied = false;
        if(ResuplyAmunition(prefab, out wasResupplied)){
            return wasResupplied;
        }

        if ((weaponsPrefabs.Count + 1) > maxWeaponCount)
        {
            ReplaceWeapon(prefab);

        }
        else
        {
            GrabNewWeapon(prefab);
        }
        return true;
    }

    virtual public bool Heal(float healValue)
    {
        if(maxLife - Life < float.Epsilon)
        {
            return false;
        }

        Life += healValue;
        Life = Mathf.Clamp(Life, 0, maxLife);
        InGameUIController.Instance.healthBar.SetHealth(Life, maxLife);
        return true;
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

    private bool ResuplyAmunition(GameObject prefab, out bool resupplyResult)
    {
        for (int i = 0; i < weaponsPrefabs.Count; i++)
        {
            if (prefab == weaponsPrefabs[i])
            {
                resupplyResult = weapons[i].ResupplyBullets();
                RefreshBulletsUI();
                return true;
            }
        }
        resupplyResult = false;
        return false;
    }

    private void GrabNewWeapon(GameObject prefab)
    {
        weaponsPrefabs.Add(prefab);
        InstantiateWeapon(prefab);
        PrepareWeapon(weapons.Count - 1);
        ChangeWeapon(weapons.Count - 1);
    }
    private void ReplaceWeapon(GameObject prefab)
    {
        var indexToErase = chosedWeapon;
        if (indexToErase == meleeWeaponIndex)
        {
            indexToErase++;
            if (indexToErase >= weaponsPrefabs.Count)
            {
                indexToErase = 0;
            }
        }
        weaponsPrefabs[indexToErase] = prefab;
        Destroy(weaponsObjects[indexToErase]);
        weaponsObjects[indexToErase] = Instantiate(prefab, transform);
        weapons[indexToErase] = weaponsObjects[indexToErase].GetComponent<Weapon>();
        
        PrepareWeapon(indexToErase);
        ChangeWeapon(indexToErase - 1);
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

    private void PrepareWeapon(int index)
    {
        InGameUIController.Instance.weaponChoose.SetWeaponSlot(index, weapons[index].Icon, (index + 1).ToString());
        weaponsObjects[index].SetActive(false);
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
        GameManager.Instance.EndGame();

    }
    #endregion
}
