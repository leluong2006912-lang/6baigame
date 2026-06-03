using UnityEngine;

// Lớp Spawner dùng để tự động tạo ra các đối tượng (như chướng ngại vật, vật phẩm) ngẫu nhiên theo tỷ lệ xuất hiện
public class Spawner : MonoBehaviour
{
    // Định nghĩa một cấu trúc dữ liệu (struct) để lưu thông tin của từng đối tượng có thể spawn
    [System.Serializable]
    public struct SpawnableObject
    {
        public GameObject prefab; // Prefab của đối tượng sẽ được tạo ra trong Game

        [Range(0f, 1f)]
        public float spawnChance; // Tỷ lệ xuất hiện của đối tượng này (giới hạn thanh trượt từ 0 đến 1 trong Inspector)
    }

    public SpawnableObject[] objects; // Mảng chứa danh sách các đối tượng có thể được tạo ra
    public float minSpawnRate = 1f;   // Thời gian tối thiểu giữa các lần tạo đối tượng (giây)
    public float maxSpawnRate = 2f;   // Thời gian tối đa giữa các lần tạo đối tượng (giây)

    // Hàm này tự động chạy khi GameObject chứa Script này được kích hoạt (Enable)
    private void OnEnable()
    {
        // Gọi hàm Spawn sau một khoảng thời gian ngẫu nhiên giữa minSpawnRate và maxSpawnRate
        Invoke(nameof(Spawn), Random.Range(minSpawnRate, maxSpawnRate));
    }

    // Hàm này tự động chạy khi GameObject chứa Script này bị vô hiệu hóa (Disable) hoặc bị hủy
    private void OnDisable()
    {
        // Hủy bỏ tất cả các lệnh Invoke đang chờ để tránh việc hàm Spawn tiếp tục chạy ngầm gây lỗi
        CancelInvoke();
    }

    // Hàm xử lý logic chính để chọn và tạo đối tượng
    private void Spawn()
    {
        // Lấy một giá trị số thực ngẫu nhiên từ 0.0 đến 1.0
        float spawnChance = Random.value;

        // Vòng lặp duyệt qua từng đối tượng trong danh sách cấu hình (Thuật toán chọn ngẫu nhiên theo trọng số / Vòng quay may mắn)
        foreach (SpawnableObject obj in objects)
        {
            // Nếu giá trị ngẫu nhiên nhỏ hơn tỷ lệ của đối tượng hiện tại, đối tượng này được chọn để tạo ra
            if (spawnChance < obj.spawnChance)
            {
                // Khởi tạo đối tượng từ prefab
                GameObject obstacle = Instantiate(obj.prefab);
                // Dịch chuyển vị trí đối tượng mới tạo dựa theo vị trí hiện tại của Spawner
                obstacle.transform.position += transform.position;
                // Thoát khỏi vòng lặp foreach ngay lập tức sau khi đã tạo được 1 đối tượng
                break;
            }

            // Nếu không trúng đối tượng này, trừ đi tỷ lệ của nó và tiếp tục kiểm tra đối tượng tiếp theo trong danh sách
            spawnChance -= obj.spawnChance;
        }

        // Tiếp tục lên lịch gọi lại chính hàm Spawn này sau một khoảng thời gian ngẫu nhiên mới để tạo thành vòng lặp vô tận
        Invoke(nameof(Spawn), Random.Range(minSpawnRate, maxSpawnRate));
    }

}