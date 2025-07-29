using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GyroMoveController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI accelerationText;
    [SerializeField] private Button resetButton;
    
    [Range(0.01f, 1f)]
    public float smoothFactor = 0.1f; // 小さいほど滑らか（でも遅い）
    Vector3 smoothedAcceleration;

    float angle = 90;

    private Vector3 baseAccel;
    private bool isCalibrated = false;


    Vector3 smoothedAccel;

    void Start()
    {
        Calibrate(); // ゲーム開始時に一回だけ補正

        resetButton.onClick.AddListener(Calibrate);
    }
    
    // 📍この関数を呼ぶと現在のスマホの傾きを基準にする
    public void Calibrate()
    {
        baseAccel = Input.acceleration;
    }

    void Update()
    {
        Vector3 raw = Input.acceleration;

        // 初回だけ現在の姿勢を基準として記録
        if (!isCalibrated)
        {
            baseAccel = raw;
            isCalibrated = true;
        }

        // ローパスフィルターで滑らかに
        smoothedAccel = Vector3.Lerp(smoothedAccel, raw, smoothFactor);

        // 基準姿勢との差分を取る（前構えた状態を中心に）
        Vector3 delta = smoothedAccel - baseAccel;

        // 差分を回転に変換（X:上下, Z:左右の傾き）
        float tiltX = Mathf.Clamp(delta.y * angle, -angle, angle);
        float tiltZ = Mathf.Clamp(-delta.x * angle, -angle, angle);

        transform.localRotation = Quaternion.Euler(tiltX, 0f, tiltZ);

        accelerationText.text = $"{SystemInfo.supportsGyroscope}\n" + 
                                $"x: {tiltX.ToString("0.0")}\n" +
                                $"y: {tiltX.ToString("0.0")}\n" +
                                $"z: {tiltZ.ToString("0.0")}";
    }

    // リセット機能（必要に応じて呼び出せる）
    public void Recalibrate()
    {
        baseAccel = Input.acceleration;
    }

    // public RawImage background;
    // private WebCamTexture webcamTexture;

    // void Start()
    // {
    //     // 使用可能なカメラデバイス一覧を取得
    //     WebCamDevice[] devices = WebCamTexture.devices;
    //     if (devices.Length > 0)
    //     {
    //         // 通常はフロントカメラの後ろ（Back Facing）を使う
    //         string camName = devices[0].name;
    //         for (int i = 0; i < devices.Length; i++)
    //         {
    //             if (!devices[i].isFrontFacing)
    //             {
    //                 camName = devices[i].name;
    //                 break;
    //             }
    //         }

    //         webcamTexture = new WebCamTexture(camName, Screen.width, Screen.height);
    //         background.texture = webcamTexture;
    //         background.material.mainTexture = webcamTexture;
    //         webcamTexture.Play();
    //     }
    //     else
    //     {
    //         Debug.LogWarning("カメラが見つかりません！");
    //     }
    // }

    // void OnDestroy()
    // {
    //     if (webcamTexture != null && webcamTexture.isPlaying)
    //         webcamTexture.Stop();
    // }
}
