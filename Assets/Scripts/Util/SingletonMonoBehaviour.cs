using UnityEngine;

/// <summary>
/// MonoBehaviourのシングルトン基底クラス。
/// このクラスを継承することで、自動的にシングルトンパターンが適用されます。
/// </summary>
/// <typeparam name="T">シングルトンとして扱う派生クラスの型</typeparam>
public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static readonly object _lock = new object();
    private static bool _isApplicationQuitting = false;

    /// <summary>
    /// シングルトンインスタンスを取得します。
    /// インスタンスが存在しない場合は、自動的に新しいGameObjectを作成してコンポーネントをアタッチします。
    /// </summary>
    public static T Instance
    {
        get
        {
            if (_isApplicationQuitting)
            {
                Debug.LogWarning($"[Singleton] Instance of {typeof(T)} is requested after application quit. Returning null.");
                return null;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    // シーン内に既存のインスタンスを検索
                    _instance = FindAnyObjectByType<T>();

                    if (_instance == null)
                    {
                        // 存在しない場合は新しいGameObjectを作成
                        var singletonObject = new GameObject($"[Singleton] {typeof(T).Name}");
                        _instance = singletonObject.AddComponent<T>();
                        DontDestroyOnLoad(singletonObject);
                        Debug.Log($"[Singleton] Created new instance of {typeof(T).Name}");
                    }
                }

                return _instance;
            }
        }
    }

    /// <summary>
    /// Awakeでシングルトンの初期化を行います。
    /// 派生クラスでAwakeをオーバーライドする場合は、必ずbase.Awake()を呼び出してください。
    /// </summary>
    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
            OnInitialize();
        }
        else if (_instance != this)
        {
            Debug.LogWarning($"[Singleton] Duplicate instance of {typeof(T).Name} detected. Destroying duplicate.");
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// シングルトンの初期化時に呼び出されます。
    /// 派生クラスで初期化処理が必要な場合はこのメソッドをオーバーライドしてください。
    /// </summary>
    protected virtual void OnInitialize()
    {
        // 派生クラスで実装
    }

    /// <summary>
    /// アプリケーション終了時の処理
    /// </summary>
    protected virtual void OnApplicationQuit()
    {
        _isApplicationQuitting = true;
    }

    /// <summary>
    /// シングルトンインスタンスを破棄します。
    /// </summary>
    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}
