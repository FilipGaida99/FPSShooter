using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
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



    // Start is called before the first frame update
    void Awake()
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
    }

    private void Start()
    {
        foreach(var weaponObject in weaponsObjects)
        {
            weaponObject.SetActive(false);
        }
        chosedWeapon = primaryWeapon;
        weaponsObjects[chosedWeapon].SetActive(true);
    }

    public void Shoot()
    {
        if(weapons[chosedWeapon] != null)
        {
            weapons[chosedWeapon].Shoot(transform.position, transform.forward);
        }
    }

    public void Reload()
    {
        if (weapons[chosedWeapon] != null)
        {
            weapons[chosedWeapon].Reload();
        }
    }

    public bool DealDamage(float damage)
    {
        Life -= damage;
        characterAudioController.state = CharacterState.Suffering;
        tintCamera.tintValue = 0.7f;
        if (!IsAlive)
        {
            characterAudioController.state = CharacterState.Dying;
            StartCoroutine(Dying());
        }
        characterAudioController.UpdateSound();
        return !IsAlive;
    }

    private bool InstantiateWeapon(GameObject prefab)
    {
        if(prefab == null)
        {
            return false;
        }
        var weapon = prefab.GetComponent<Weapon>();
        if(weapon == null)
        {
            return false;
        }
        var newWeapon = Instantiate(prefab, transform);
        weaponsObjects.Add(newWeapon);
        weapons.Add(newWeapon.GetComponent<Weapon>());
        return true;
    }

    public void ChangeWeapon(int newChosed)
    {
        //Show/hide
        weapons[chosedWeapon].OnHide();
        weaponsObjects[chosedWeapon].SetActive(false);
        chosedWeapon = newChosed;
        weaponsObjects[chosedWeapon].SetActive(true);

        //TODO:usunąć bo to głupie
        weaponsObjects[chosedWeapon].transform.localPosition = weaponsPrefabs[chosedWeapon].transform.localPosition;
        weaponsObjects[chosedWeapon].transform.localRotation = weaponsPrefabs[chosedWeapon].transform.localRotation;
        weapons[chosedWeapon].OnChoose();
    }

    public void ChangeWeaponToNext(int direction)
    {
        var newChose = chosedWeapon + direction;
        if(newChose >= weapons.Count)
        {
            newChose = 0;
        }
        else if (newChose < 0)
        {
            newChose = weapons.Count - 1;
        }
        ChangeWeapon(newChose);
    }

    private bool HideShowWeapon(GameObject newShowed, GameObject newHidden)
    {
        //TODO zrobić pojawianie sie broni
        return false;
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
    
}
