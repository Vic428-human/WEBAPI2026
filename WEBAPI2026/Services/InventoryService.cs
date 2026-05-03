using System.Collections.Generic;
using WEBAPI2026.Models.Dtos;
using WEBAPI2026.Models.Requests;
using WEBAPI2026.Repositories;

namespace WEBAPI2026.Services
{
    // InventoryService 是「Inventory API 的業務邏輯層」
    //
    // 用 React / Go 角度理解：
    //
    // Controller / Handler：
    // - 接 request
    // - 驗證 header
    // - 驗證 body
    // - 回 response
    //
    // Service：
    // - 處理業務流程
    // - 決定要去哪裡拿資料
    //
    // Repository：
    // - 真正負責資料來源
    //
    // 目前流程：
    // InventoryController
    // → InventoryService
    // → IInventoryRepository
    // → InventoryRepository
    public class InventoryService
    {
        // 透過介面依賴 Repository
        //
        // 用 React / Go 角度理解：
        // Service 不直接 new Repository，
        // 而是透過 constructor injection 接收 Repository。
        //
        // 這樣未來 Repository 從 mock data 改成 Oracle 查詢時，
        // Service 不需要大改。
        private readonly IInventoryRepository _inventoryRepository;

        public InventoryService(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        // GetInventory 負責取得庫存資料
        //
        // 目前只是轉交給 Repository。
        //
        // 未來如果有業務邏輯，例如：
        // - 權限判斷
        // - 資料轉換
        // - 特殊篩選
        //
        // 可以放在 Service。
        public List<InventoryDto> GetInventory(DateRangeRequest request)
        {
            return _inventoryRepository.GetInventory(request);
        }
    }
}