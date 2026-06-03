using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Đảm bảo Script này luôn được khởi chạy trước các Script thông thường khác (-1) để thiết lập hệ thống game
[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    // Thiết kế theo Pattern Singleton để các Script khác (Player, Spawner...) có thể dễ dàng gọi GameManager.Instance
    public static GameManager Instance { get; private set; }

    public float initialGameSpeed = 5f;   // Tốc độ di chuyển ban đầu của game
    public float gameSpeedIncrease = 0.1f; // Lượng tốc độ tăng tiến theo thời gian
    public float gameSpeed { get; private set; } // Tốc độ hiện tại của game (chỉ đọc từ bên ngoài)

    // Khai báo các thành phần UI (Giao diện người dùng) hiển thị trong Game
    [SerializeField] private TextMeshProUGUI scoreText;    // Chữ hiển thị điểm hiện tại
    [SerializeField] private TextMeshProUGUI hiscoreText;  // Chữ hiển thị điểm cao nhất (Kỷ lục)
    [SerializeField] private TextMeshProUGUI gameOverText; // Chữ báo "Game Over"
    [SerializeField] private Button retryButton;           // Nút bấm để chơi lại

    private Player player;   // Tham chiếu tới Script điều khiển Người chơi
    private Spawner spawner; // Tham chiếu tới Script tạo chướng ngại vật

    private float score;     // Biến lưu điểm số hiện tại (dạng số thực để cộng dồn mượt mà)
    public float Score => score; // Property công khai để các Script khác có thể lấy giá trị score

    // Hàm khởi tạo, chạy ngay khi Object được nạp vào bộ nhớ
    private void Awake()
    {
        // Kiểm tra và đảm bảo chỉ có duy nhất 1 Instance của GameManager tồn tại trong Scene
        if (Instance != null)
        {
            DestroyImmediate(gameObject); // Nếu đã có GameManager rồi, xóa ngay bản sao này
        }
        else
        {
            Instance = this; // Nếu chưa có, gán bản này làm Instance chính thức
        }
    }

    // Chạy khi Object này bị hủy bỏ
    private void OnDestroy()
    {
        // Giải phóng bộ nhớ cho Instance nếu GameManager này bị xóa
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // Chạy một lần duy nhất trước Frame đầu tiên của Game
    private void Start()
    {
        // Tìm kiếm các đối tượng Player và Spawner đang có trong màn chơi
        player = FindObjectOfType<Player>();
        spawner = FindObjectOfType<Spawner>();

        // Bắt đầu một game mới
        NewGame();
    }

    // Hàm thiết lập để bắt đầu một lượt chơi mới
    public void NewGame()
    {
        // Tìm tất cả các chướng ngại vật cũ còn sót lại trên màn hình
        Obstacle[] obstacles = FindObjectsOfType<Obstacle>();

        // Vòng lặp xóa sạch các chướng ngại vật cũ để dọn dẹp màn chơi
        foreach (var obstacle in obstacles)
        {
            Destroy(obstacle.gameObject);
        }

        score = 0f;                    // Reset điểm số về 0
        gameSpeed = initialGameSpeed;  // Reset tốc độ game về mức ban đầu
        enabled = true;                // Kích hoạt lại hàm Update() để tiếp tục tính điểm/tốc độ

        player.gameObject.SetActive(true);   // Hiển thị và cho phép người chơi hoạt động
        spawner.gameObject.SetActive(true);  // Kích hoạt lại bộ tạo chướng ngại vật
        gameOverText.gameObject.SetActive(false); // Ẩn chữ Game Over
        retryButton.gameObject.SetActive(false);   // Ẩn nút Retry

        UpdateHiscore(); // Cập nhật lại UI điểm cao nhất từ bộ nhớ
    }

    // Hàm xử lý khi người chơi thua cuộc
    public void GameOver()
    {
        gameSpeed = 0f;   // Dừng toàn bộ chuyển động của game (đóng băng màn hình)
        enabled = false;  // Tắt hàm Update() để ngừng tăng điểm và ngừng tăng tốc độ

        player.gameObject.SetActive(false);  // Ẩn nhân vật người chơi
        spawner.gameObject.SetActive(false); // Ngừng tạo thêm chướng ngại vật mới
        gameOverText.gameObject.SetActive(true); // Hiển thị chữ Game Over lên màn hình
        retryButton.gameObject.SetActive(true);   // Hiển thị nút Retry để người chơi bấm vào

        UpdateHiscore(); // Kiểm tra và lưu lại kỷ lục mới nếu có
    }

    // Hàm chạy liên tục theo từng khung hình (Frame)
    private void Update()
    {
        // Tăng dần tốc độ game dựa theo thời gian thực (Time.deltaTime giúp mượt mà trên mọi mức FPS)
        gameSpeed += gameSpeedIncrease * Time.deltaTime;
        // Điểm tăng lên dựa theo tốc độ game (càng chạy nhanh điểm tăng càng lẹ)
        score += gameSpeed * Time.deltaTime;
        // Hiển thị điểm số lên UI dưới dạng số nguyên có 5 chữ số cố định (Ví dụ: 00015, 00123)
        scoreText.text = Mathf.FloorToInt(score).ToString("D5");
    }

    // Hàm xử lý lưu trữ và hiển thị điểm cao nhất (Kỷ lục)
    private void UpdateHiscore()
    {
        // Tải điểm cao nhất từ bộ nhớ máy (PlayerPrefs), mặc định là 0 nếu chơi lần đầu
        float hiscore = PlayerPrefs.GetFloat("hiscore", 0);

        // Nếu điểm lượt này cao hơn kỷ lục cũ
        if (score > hiscore)
        {
            hiscore = score; // Gán kỷ lục mới bằng điểm hiện tại
            PlayerPrefs.SetFloat("hiscore", hiscore); // Lưu kỷ lục mới này vào bộ nhớ máy
        }

        // Hiển thị điểm kỷ lục lên UI với định dạng số nguyên 5 chữ số (D5)
        hiscoreText.text = Mathf.FloorToInt(hiscore).ToString("D5");
    }

}