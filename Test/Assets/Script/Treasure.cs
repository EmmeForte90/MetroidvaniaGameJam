using System.Collections;
using UnityEngine;
using Spine.Unity;
using Spine;
using UnityEngine.UI;

public class Treasure : MonoBehaviour
{
    [Header("ID Forziere")]
    public GameObject ThisTreasure;
    public int IDTreasure;

    [Header("Apertura")]
    public bool Lock = false;
    private bool touch = false;
    private bool opened = false;

    [Header("VFX")]
    public GameObject StartVFX;
    public GameObject Button;
    public Image img;

    [Header("Reward")]
    public bool isMoney = true;
    public GameObject[] Reward;
    private Transform rewardSpawnPoint; 
    private bool CanReawrd = true;
    private bool SingleItem = true;

    [Header("Animazioni")]
    public string currentState = "Idle";
    private string currentAnimationName;
    [SpineAnimation] public string idle;
    [SpineAnimation] public string open;

    [Header("Spine Setup")]
    public SkeletonAnimation skeletonAnimation;
    public Spine.AnimationState spineAnimationState;
    public Spine.Skeleton skeleton;

    private void OnEnable(){StartCoroutine(DataChest());}

    private IEnumerator DataChest()
    {
        yield return new WaitForSeconds(0.2f);
        if (GameManager.instance.ChestId[IDTreasure])
        {
            ThisTreasure.SetActive(false);
            opened = true;
        }
        else{ThisTreasure.SetActive(true);}
    }

    void Awake()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        if (skeletonAnimation == null)
            Debug.LogError("Spine SkeletonAnimation mancante!");

        StartVFX.SetActive(false);
        Button.SetActive(false);

        spineAnimationState = skeletonAnimation.AnimationState;
        skeleton = skeletonAnimation.skeleton;

        PlayAnimationLoop(idle);
    }

    void Update()
    {
        if (!touch || opened) return;

        Button.SetActive(true);

        if (Input.GetButtonDown("Fire1"))
        {
            if (!Lock || GameManager.instance.HaveKey[IDTreasure]){StartCoroutine(OpenChest());Button.SetActive(false);}
            else{Debug.LogError("can't open");StartCoroutine(ShakeAndFlashRedUI());}
        }
    }

    public void OnTriggerStay(Collider collision){if (collision.CompareTag("Player")){touch = true;}}

    public void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            touch = false;
            Button.SetActive(false);
        }
    }

    private IEnumerator OpenChest()
    {
        if (opened) yield break;

        opened = true;
        GameManager.instance.ChestId[IDTreasure] = true;
        GameManager.instance.KeyUI.sprite = GameManager.instance.Key[0];

        StartVFX.SetActive(true);
        currentState = "Open";
        PlayAnimationLoop(open);
        RelaseReward();

        yield return new WaitForSeconds(0.2f);

        Debug.Log("Forziere aperto");
        // Qui puoi aggiungere drop/reward o disattivazione dell'oggetto dopo un attimo
        // Destroy(ThisTreasure); o yield return new WaitForSeconds(...) ecc.
    }

    public void RelaseReward()
    {
        if(isMoney)
        {
        if (CanReawrd)
        {
            int count = Random.Range(1, 6); // genera da 1 a 5 inclusi

            for (int i = 0; i < count; i++)
            {
                if (Reward.Length == 0) return;

                GameObject rewardPrefab = Reward[Random.Range(0, Reward.Length)];

                Vector3 offset = new Vector3(Random.Range(-0.2f, 0.2f), 0.2f, Random.Range(-0.2f, 0.2f));
                Vector3 spawnPos = rewardSpawnPoint != null 
                    ? rewardSpawnPoint.position + offset 
                    : transform.position + offset;

                Instantiate(rewardPrefab, spawnPos, Quaternion.identity);
            }
            CanReawrd = false;
        }}
        else if(!isMoney)
        {
            if(SingleItem){
                SingleItem = false;
            Vector3 offset = new Vector3(Random.Range(-0.2f, 0.2f), 0.2f, Random.Range(-0.2f, 0.2f));
                Vector3 spawnPos = rewardSpawnPoint != null 
                    ? rewardSpawnPoint.position + offset 
                    : transform.position + offset;
            Instantiate(Reward[0], spawnPos, Quaternion.identity);
            }
        }
    }

    #region Animazioni
    public void PlayAnimationLoop(string animName)
    {
        if (currentAnimationName == animName) return;

        spineAnimationState.SetAnimation(0, animName, false);
        currentAnimationName = animName;
    }

    public IEnumerator ShakeAndFlashRedUI()
    {
    if (img == null) yield break;

    Color originalColor = img.color;
    Vector3 originalPos = Button.transform.localPosition;

    float duration = 0.5f;
    float elapsed = 0f;
    float strength = 0.2f; // shake più visibile per UI in pixel

    img.color = Color.red;

    while (elapsed < duration)
    {
        float offsetX = Random.Range(-strength, strength);
        float offsetY = Random.Range(-strength, strength);
        Button.transform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0);

        elapsed += Time.deltaTime;
        yield return null;
    }

    Button.transform.localPosition = originalPos;
    img.color = originalColor;
    }
    #endregion
}
