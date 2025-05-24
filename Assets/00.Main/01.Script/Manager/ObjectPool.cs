using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Pool
{
    public int poolCount; // ������Ʈ�� ���� �� ����
    public string poolName; // Ǯ���� ������Ʈ�� �̸�

    public int poolLength => pool.Count;

    public GameObject poolObject; // Ǯ���� ������Ʈ�� ������
    public Transform parentObject;

    private Queue<GameObject> pool = new Queue<GameObject>(); // Ǯ���� ������Ʈ�� ���� ť

    public void Enqueue(GameObject _object) => pool.Enqueue(_object);
    public GameObject Dequeue() => pool.Dequeue();
}


public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance = null;

    public Dictionary<string, Pool> poolDictionary = new Dictionary<string, Pool>();

    public List<Pool> poolList = new List<Pool>();

    #region Unity_Function
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    private void Start() => _Init();
    #endregion

    #region Private_Fucntion
    private void _Init()
    {
        foreach (Pool pool in poolList) // pool List�� ������ pool Dictionary�� �߰��ϴ� ����
            poolDictionary.Add(pool.poolName, pool);

        foreach (Pool pool in poolDictionary.Values) // pool Dictionary�� ������ŭ ������Ʈ�� �����ϰ� ��Ȱȭ�ϴ� ����
        {
            GameObject parent = new GameObject(); // �� ������Ʈ�� �θ�ν� ����ϱ� ���� ����

            pool.parentObject = parent.transform; // Ǯ�� �θ� ������Ʈ�� ��ġ�� ����

            parent.transform.SetParent(transform); // ������ �� ������Ʈ�� ��ġ�� �ڱ� �ڽ�����
            parent.name = pool.poolName; // ������ ������Ʈ�� �̸��� Ǯ ������Ʈ �̸�����

            for (int i = 0; i < pool.poolCount; i++) // ���� Ǯ�� ���� ���� ��ŭ �ݺ�
            {
                GameObject currentObject = Instantiate(pool.poolObject, parent.transform); // ���� Ǯ�� �������� ������
                currentObject.SetActive(false); // ��Ȱ��ȭ

                pool.Enqueue(currentObject); // ���õ� Ǯ�� �߰�
            }
        }
    }

    private GameObject _SpawnFromPool(string name, Vector3 position)
    {
        Pool currentPool = poolDictionary[name]; // ��ųʸ����� �Է¹��� �̸��� ã�Ƽ� �ʱ�ȭ

        if (currentPool.poolLength <= 0)
        {
            GameObject obj = Instantiate(currentPool.poolObject, currentPool.parentObject);
            obj.SetActive(false);
            currentPool.Enqueue(obj);
        }

        GameObject currentObject = currentPool.Dequeue(); // ���õ� Ǯ���� Dequeue �Լ��� ������Ʈ ��������
        currentObject.transform.position = position; // �Է¹��� ��ġ�� ����

        currentObject.SetActive(true); // Ȱ��ȭ

        return currentObject;
    }

    private GameObject _SpawnFromPool(string name, Vector3 position, Quaternion rotate)
    {
        Pool currentPool = poolDictionary[name]; // ��ųʸ����� �Է¹��� �̸��� ã�Ƽ� �ʱ�ȭ

        if (currentPool.poolLength <= 0)
        {
            GameObject obj = Instantiate(currentPool.poolObject, currentPool.parentObject);
            obj.SetActive(false);
            currentPool.Enqueue(obj);
        }

        GameObject currentObject = currentPool.Dequeue(); // ���õ� Ǯ���� Dequeue �Լ��� ������Ʈ ��������
        currentObject.transform.position = position; // �Է¹��� ��ġ�� ����
        currentObject.transform.rotation = rotate; // �Է¹��� ������ ����

        currentObject.SetActive(true); // Ȱ��ȭ

        return currentObject;
    }

    private void _ReturnToPool(string name, GameObject currentObject)
    {
        Pool pool = poolDictionary[name]; // �̸��� �´� Ǯ�� ã�Ƽ� ����

        currentObject.SetActive(false); // ��Ȱ��ȭ
        currentObject.transform.SetParent(pool.parentObject); // ����� ���� ������Ʈ�� �θ� �ٽ� ����

        pool.Enqueue(currentObject);
    }
    #endregion  

    #region Public_Function
    /// <summary>
    /// Ǯ���� ������Ʈ ������ ����
    /// </summary>
    /// <param name="name">Ǯ���� ������Ʈ�� �̸�</param> 
    /// <param name="position">Ǯ���� ��ġ</param>
    /// <returns></returns>
    public static GameObject SpawnFromPool(string name, Vector3 position) => instance._SpawnFromPool(name, position);
    /// <summary>
    ///  Ǯ���� ������Ʈ ������ ����
    /// </summary>
    /// <param name="name">Ǯ���� ������Ʈ �̸�</param>
    /// <param name="position">Ǯ���� ��ġ</param>
    /// <param name="rotate">Ǯ���� ����</param>
    /// <returns></returns>
    public static GameObject SpawnFromPool(string name, Vector3 position, Quaternion rotate) => instance._SpawnFromPool(name, position, rotate);

    /// <summary>
    /// Ǯ�� �ǵ���
    /// </summary>
    /// <param name="name">������ ������Ʈ �̸�</param>
    /// <param name="currentObejct">�ǵ��� ������Ʈ</param>
    public static void ReturnToPool(string name, GameObject currentObejct) => instance._ReturnToPool(name, currentObejct);
    #endregion
}