using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using TMPro;


/// <summary>
/// ��ź�� �ʵ忡 �����ǰ��� �����ð��� ������ �����Ѵ�.
/// </summary>
/// 

public class ExplosionBomb : MonoBehaviour
{
    [SerializeField] GameObject particleFromWithinBin;
    [SerializeField] GameObject particleSelf;
    [SerializeField] TextMeshPro textMesh;

    bool isSelfBomb = false;
    bool countDownStart = false;
    int materialIndex = 0;
    Renderer bombRenderer;
    Color redColor;
    [SerializeField] Material[] bombMaterial;
    [Header("���� ���ϸ�")]
    [SerializeField] string bombBoomSound;

    private void Awake()
    {
        bombRenderer = transform.GetChild(0).GetComponent<Renderer>();
        redColor = new Color
        {
            r = 50
        };
        bombRenderer.material.EnableKeyword("_EMISSION");
    }
    private void OnEnable()
    {
        StartCoroutine(StartCount());

    }
    void ChangeEmission()
    {
        bombRenderer.material.SetColor("_EmissionColor", redColor * 0.1f);
        redColor.r += 50;
    }
    void ChangeEmission(int index = 0)
    {
        if (index < 0 || 4 < index) return;

        bombRenderer.material = bombMaterial[index];
    }

    IEnumerator StartCount()
    {
        float endCount = GameManager.Instance.BombTimer + 1;
        int preCount = (int)endCount - 2;
            materialIndex = 0;

        while (0 <= endCount)
        {

            endCount -= Time.deltaTime;

            if ((int)endCount < preCount)
            {
                preCount = (int)endCount;

                ChangeEmission(materialIndex);
                materialIndex++;

            }
            yield return null;
        }
        Bomb();
    }

    void Bomb()
    {
        isSelfBomb = true;

        GameManager.Instance.basketHP -= GameManager.Instance.bombDamage * 2;

        Instantiate(particleSelf, transform.position, Quaternion.identity);

        Achievement.Instance.AchieveCheck(eAchieveState.ExplosionBomb);

        SoundMG.Instance.PlaySFX(bombBoomSound);

        Destroy(this.transform.root.gameObject);
    }

    /// Bin���� ó���ؾ��ҵ��ϴ�.

    void InstantiateParticle(bool _isSelfBomb)
    {
        if (_isSelfBomb)
            Instantiate(particleSelf, transform.position, Quaternion.identity);
        else
            Instantiate(particleFromWithinBin, transform.position, Quaternion.identity);
    }
}
