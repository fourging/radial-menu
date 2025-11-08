using System;
using System.Collections.Generic;
using SodaCraft.Localizations;
using UnityEngine;
using RadialMenu.Logic;

namespace RadialMenu.Patches;

/// <summary>
/// Mod 本地化辅助类
/// 管理 Mod 的多语言文本
/// </summary>
public static class LocalizationHelper
{
    // 本地化键名前缀，避免与游戏原有键冲突
    private const string KeyPrefix = "RadialMenu_";

    // 本地化文本数据
    private static readonly Dictionary<SystemLanguage, Dictionary<string, string>> LocalizationData = [];

    /// <summary>
    /// 语言变更事件 - UI 组件可订阅此事件以在语言改变时刷新显示
    /// </summary>
    public static event Action<SystemLanguage>? OnLanguageChanged;

    /// <summary>
    /// 初始化本地化系统
    /// </summary>
    public static void Initialize()
    {
        try
        {
            Log.Info("Initializing localization system...");

            // 注册语言切换事件
            LocalizationManager.OnSetLanguage += LanguageChangedHandler;

            // 加载所有语言的翻译
            LoadTranslations();

            // 应用当前语言的翻译
            ApplyTranslations(LocalizationManager.CurrentLanguage);

            Log.Info($"Localization initialized for language: {LocalizationManager.CurrentLanguage}");
        }
        catch (System.Exception ex)
        {
            Log.Info($"Failed to initialize localization: {ex}");
        }
    }

    /// <summary>
    /// 清理本地化系统
    /// </summary>
    public static void Cleanup()
    {
        try
        {
            LocalizationManager.OnSetLanguage -= LanguageChangedHandler;

            // 移除所有覆盖的文本
            foreach (var langData in LocalizationData.Values)
            {
                foreach (var key in langData.Keys)
                {
                    LocalizationManager.RemoveOverrideText(GetFullKey(key));
                }
            }

            Log.Error( "Localization system cleaned up");
        }
        catch (System.Exception ex)
        {
            Log.Error($"Failed to cleanup localization: {ex}");
        }
    }

    /// <summary>
    /// 语言切换事件处理 - 同时触发公共事件供 UI 组件订阅
    /// </summary>
    private static void LanguageChangedHandler(SystemLanguage newLanguage)
    {
        try
        {
            Log.Info($"Language changed to: {newLanguage}");
            ApplyTranslations(newLanguage);

            // 触发公共事件，通知所有订阅者
            OnLanguageChanged?.Invoke(newLanguage);
        }
        catch (System.Exception ex)
        {
            Log.Error($"Failed to handle language change: {ex}");
        }
    }

    /// <summary>
    /// 加载所有语言的翻译数据
    /// </summary>
    private static void LoadTranslations()
    {
        // 简体中文
        LocalizationData[SystemLanguage.ChineseSimplified] = new Dictionary<string, string>
        {
            // 设置界面
            { "Settings_Title", "环形菜单设置" },
            
            // 配置项描述
            { "Config_FoodBindSectors", "自动绑定食物的扇区编号" },
            { "Config_ignoreDurabilityValue", "低于多少耐久的急救箱自动略过圆盘使用" },
            { "Config_showLowValueFood", "提示低价值物品是否包含食物" },
            { "Config_quickUseLastItemQ", "是否开启短按Q触发快速使用上次物品功能" },
            { "Config_iconSize", "图标尺寸" },
            { "Config_iconDistanceFactor", "图标距离圆心的比例" },
            { "Config_uiScalePercent", "背景UI缩放百分比" },
            { "Config_innerDeadZoneCoefficient", "内圈死区系数" },
            { "Config_outerDeadZoneCoefficient", "外圈死区系数" },
            { "Config_longPressQWaitDuration", "呼出圆盘长按时间" },
            { "Config_UI8style", "8扇区UI方案" },
            { "Config_UI6style", "6扇区UI方案" },
            { "Config_sectorCount", "扇区数量" },
            { "Config_isBulletTimeEnabled", "是否开启子弹时间" },
            { "Config_bulletTimeMultiplier", "子弹时间游戏速度倍率" },
            { "Config_radialMenuActivationKey", "呼出圆盘使用的按键" },
            { "Config_enableFirstPersonAdaptation", "是否启动第一人称适配" },
            { "Config_firstPersonSensitivity", "第一人称下呼出轮盘的灵敏度" },
            { "Config_enableThirdPersonAdaptation", "是否启动第三人称适配" },
            { "Config_thirdPersonSensitivityMultiplier", "第三人称下呼出轮盘的灵敏度" },
            { "Config_lockRadialMenuToCenter", "是否锁定轮盘呼出位置为屏幕中心" },
            { "Config_hintOptionsOnly", "下面的选项无实际功能，仅做提示" },
            { "Config_radialMenuStuckHint", "轮盘卡在屏幕上关不掉？试试按下'win'键吧" },
            { "Config_haveFunHint", "祝你玩的开心" },
            { "Config_rightClickCloseRadialMenuHint", "在呼出轮盘之后点击鼠标右键可以快速关闭轮盘" },
            { "Config_disableSpeechBubbles", "是否关闭鸭子头顶的语音气泡" },
            { "Config_playerHatedTypeIDs", "黑名单食物ID（会排到推荐列表最末尾）" },
            
            // UI提示信息
            { "UI_ItemCount", "剩余数量: {0}" },
            { "UI_BindingNotAllowed", "这个不让绑定" },
            { "UI_InstallModConfig", "请查阅创意工坊首页并安装新的依赖模组" },
            { "UI_DefaultStyle", "默认" },
            { "UI_StyleOption", "{0}方案" },
            { "UI_NoStyleDetected", "未检测到{0}扇区的背景套装，使用默认选项" },
            
            // 日志信息
            { "Log_RadialMenuInit", "环形菜单初始化开始..." },
            { "Log_RadialMenuComplete", "环形菜单初始化完成" },
            { "Log_BindingComplete", "已将绑定持久化：扇区={0}，TypeID={1}，DisplayName={2}，autoBound={3}" },
            { "Log_IconDistanceUpdated", "图标距离因子更新为 {0}，已重新计算图标位置" },
            { "Log_StyleUnavailable", "当前样式 {0} 对于 {1} 扇区不可用，使用默认样式 {2}" },
            { "Log_LoadingStyle", "加载 {0} 扇区背景，使用样式: {1}" },
            { "Log_SectorAngle", "计算了 {0} 个扇区的图标位置，每个扇区角度: {1}°" },
            
            // 物品使用相关提示
            { "Use_ExplosionArt", "爆炸就是艺术！" },
            { "Use_EatItem", "吃 {0}！" },
            { "Use_EquipItem", "装备 {0}！" },
            { "Use_UseItem", "使用 {0}！" },
            { "Use_HealthRecovered", "补充生命值！" },
            { "Use_Ouch", "好痛！得赶紧治疗一下" },
            { "Use_ReplaceAfterUse", "用完这次就换新的了" },
            { "Use_HealthRemaining", "还能回{0}血量" },
            { "Use_DrinkItem", "喝 {0}" },
            { "Use_ColaOverflow", "呲！糟糕，可乐溢出来了！" },
            { "Use_ColaDrinking", "吨吨吨吨吨……" },
            { "Use_TasteItem", "偷偷尝一口 {0}～" },
            { "Use_ColaByeBye", "可乐一开，烦恼拜拜。" },
            { "Use_Cheers", "干杯！" },
            { "Use_DrinkFirst", "先干为敬！" },
            { "Use_DrinkForgetWorries", "喝了这杯忘掉烦恼～" },
            { "Use_FoodTasty", "{0}真好次！" },
            { "Use_SoFragrant", "艾玛真香！" },
            { "Use_HealthFull", "血量已经满了" },
            { "Use_StrongDrink", "劲真足！" },
            { "Use_DrinkTasty", "真好喝！" },
            { "Use_DrinkSecretly", "偷偷喝一口{0}～" },
            { "Use_FoodCannotUse", "食物 {0} 无法使用" },
            { "Log_FoodEaten", "食用了食物：{0}" },

            // 吃屎相关提示
            { "Use_DuckRefusesPoop", "鸭鸭拒绝进食！这不是食物！" },
            { "Use_DontEatDuckPoop", "别让鸭鸭吃粑粑啊！" },
            { "Use_PoopDetected", "检测到可疑物体：高风险生物废料！" },
            { "Use_PoopGourmet", "鸭鸭不当美食家，请收起你的奇怪口味！" },
            { "Use_DuckCry", "鸭鸭流下了悲伤的泪水：‘我做错了什么？’" },
            { "Use_DuckQuestionLife", "鸭鸭开始思考生命的意义……以及你的问题。" },
            { "Use_DuckReputation", "鸭鸭的社会声誉下降了99点！" },
            { "Use_DuckGag", "呕——鸭鸭快吐了！" },
            { "Use_DuckCivilRights", "鸭鸭要求尊重基本饮食权！" },
            { "Use_SurvivalMode", "求生本能启动……但代价是灵魂受创。" },
            { "Use_PoopCuisine", "新菜系解锁：法式粑粑配鸭。" },
            { "Use_DuckBetrayed", "鸭鸭感到被深深地背叛了……" },

            //绑定屎相关提示
            { "FoodBind_AutoBindBurger", "已自动绑定老八秘制小汉堡！" },
            { "FoodBind_FoundPoop", "发现一坨{0}在背包里！臭死了" },
            { "FoodBind_PoopDetected", "检测到可疑物体：高风险生物废料！" },

            
            // 绑定相关提示
            { "Binding_Success", "已绑定：{0}" },
            { "Binding_Success_Short", "已绑定{0}" },
            { "Binding_Failed", "绑定失败：{0}" },
            { "Item_RemainingCount", "背包内剩余数量 {0}" },
            { "Item_NoMoreItems_1", "背包里没有这个物品了哦！" },
            { "Item_NoMoreItems_2", "这个物品已经用完啦！" },
            { "Item_NoMoreItems_3", "你已经没有这个物品了！" },
            { "Item_NoMoreItems_4", "用光光啦！" },
            { "Item_NoMoreItems_5", "没有辣！" },
            { "Item_NoMoreItems_6", "花光啦！" },
            
            // 输入处理相关提示
            { "Input_SelectItemFirst", "请先选择一个物品再开始绑定" },
            { "Input_NoRadialHere", "这里不准呼出圆盘菜单哦！" },
            { "Input_NoLowValueItem", "没有找到不划算的物品" },
            { "Input_LowestValueItem", "价重比最低的是{0}！" },
            { "Input_SuggestDrop", "建议先丢掉{0}！" },
            { "Input_CannotCarry", "背不动了，先把{0}扔了吧～" },
            { "Input_NotWorthMoney", "{0}？这东西不怎么值钱的样子？" },
            { "Input_HeavyDrop", "重死了！快把{0}扔了！" },
            { "Input_CannotCarryAlt", "背不动了，先把{0}扔了吧～" },
            { "Input_LeastWorth", "{0}最不值钱～" },
            { "Input_AuthorRequest", "求求给环形菜单点个赞吧，谢谢你啦！" },
            { "Menu_FocusLostClosed", "焦点丢失，圆盘菜单已自动关闭" },
            { "Menu_PressToReopen", "按住 {0} 键重新打开圆盘菜单" },
            
            // 食物绑定相关提示
            { "Food_AutoBindSector", "已为扇区 {0} 自动绑定食物：{1}" },
            { "Food_PreviousFoodEaten", "之前的食物吃光了，现在吃这个 {0} ！" },
            { "Food_TiredOfOld", "早就吃腻了，终于换成我喜欢的{0}了！" },
            { "Food_FoundBetter", "发现更好吃的{0}，已经换好了！" },
            { "Food_CheapDelicious", "{0}便宜又美味！" },
        };

        // 繁体中文
        LocalizationData[SystemLanguage.ChineseTraditional] = new Dictionary<string, string>
        {
            // 設置介面
            { "Settings_Title", "環形選單設置" },
            
            // 配置項描述
            { "Config_FoodBindSectors", "自動綁定食物的扇區編號" },
            { "Config_ignoreDurabilityValue", "低於多少耐久的急救箱自動略過圓盤使用" },
            { "Config_showLowValueFood", "提示低價值物品是否包含食物" },
            { "Config_quickUseLastItemQ", "是否啟用短按Q快速使用上次物品功能" },
            { "Config_iconSize", "圖示尺寸" },
            { "Config_iconDistanceFactor", "圖示距離圓心的比例" },
            { "Config_uiScalePercent", "背景UI縮放百分比" },
            { "Config_innerDeadZoneCoefficient", "內圈死區係數" },
            { "Config_outerDeadZoneCoefficient", "外圈死區係數" },
            { "Config_longPressQWaitDuration", "呼出圓盤長按時間" },
            { "Config_UI8style", "8扇區UI方案" },
            { "Config_UI6style", "6扇區UI方案" },
            { "Config_sectorCount", "扇區數量" },
            { "Config_isBulletTimeEnabled", "是否啟用子彈時間" },
            { "Config_bulletTimeMultiplier", "子彈時間遊戲速度倍率" },
            { "Config_radialMenuActivationKey", "呼出圓盤使用的按鍵" },
            { "Config_enableFirstPersonAdaptation", "是否啟用第一人稱適配" },
            { "Config_firstPersonSensitivity", "第一人稱下呼出轉盤的靈敏度" },
            { "Config_enableThirdPersonAdaptation", "是否啟用第三人稱適配" },
            { "Config_thirdPersonSensitivityMultiplier", "第三人稱下呼出轉盤的靈敏度" },
            { "Config_lockRadialMenuToCenter", "是否鎖定轉盤呼出位置為螢幕中心" },
            { "Config_hintOptionsOnly", "下列選項僅為提示，無實際功能" },
            { "Config_radialMenuStuckHint", "轉盤卡在螢幕上關不掉？試試按下'win'鍵吧" },
            { "Config_haveFunHint", "祝你遊玩愉快" },
            { "Config_rightClickCloseRadialMenuHint", "呼出轉盤後點擊滑鼠右鍵可快速關閉" },
            { "Config_disableSpeechBubbles", "是否關閉鴨子頭頂的對話氣泡" },
            { "Config_playerHatedTypeIDs", "黑名單食物ID（會排到推薦列表最末尾）" }, 
                      
            // UI提示信息
            { "UI_ItemCount", "數量: {0}" },
            { "UI_BindingNotAllowed", "此物品無法綁定" },
            { "UI_InstallModConfig", "請查閱創意工坊頁面並安裝所需的依賴模組" },
            { "UI_DefaultStyle", "預設" },
            { "UI_StyleOption", "{0} 方案" },
            { "UI_NoStyleDetected", "未檢測到 {0} 扇區的背景套裝，使用預設" },
            
            // 日誌信息
            { "Log_RadialMenuInit", "環形選單初始化開始..." },
            { "Log_RadialMenuComplete", "環形選單初始化完成" },
            { "Log_BindingComplete", "已將綁定持久化：扇區={0}，TypeID={1}，DisplayName={2}，autoBound={3}" },
            { "Log_IconDistanceUpdated", "圖示距離因子更新為 {0}，已重新計算圖示位置" },
            { "Log_StyleUnavailable", "樣式 {0} 對於 {1} 扇區不可用，使用預設樣式 {2}" },
            { "Log_LoadingStyle", "載入 {0} 扇區背景，使用樣式: {1}" },
            { "Log_SectorAngle", "計算了 {0} 個扇區的圖示位置，每個扇區角度: {1}°" },
            
            // 物品使用相關提示
            { "Use_ExplosionArt", "爆炸就是藝術！" },
            { "Use_EatItem", "吃 {0}！" },
            { "Use_EquipItem", "裝備 {0}！" },
            { "Use_UseItem", "使用 {0}！" },
            { "Use_HealthRecovered", "恢復了生命值！" },
            { "Use_Ouch", "好痛！得趕緊治療一下" },
            { "Use_ReplaceAfterUse", "用完就換新的" },
            { "Use_HealthRemaining", "還能回{0}血量" },
            { "Use_DrinkItem", "喝 {0}" },
            { "Use_ColaOverflow", "噗！可樂溢出來了！" },
            { "Use_ColaDrinking", "咕嚕咕嚕咕嚕……" },
            { "Use_TasteItem", "偷偷嚐一口 {0}～" },
            { "Use_ColaByeBye", "可樂一開，煩惱拜拜。" },
            { "Use_Cheers", "乾杯！" },
            { "Use_DrinkFirst", "先乾為敬！" },
            { "Use_DrinkForgetWorries", "喝了這杯忘掉煩惱～" },
            { "Use_FoodTasty", "{0} 真好吃！" },
            { "Use_SoFragrant", "哇，好香！" },
            { "Use_HealthFull", "血量已滿" },
            { "Use_StrongDrink", "酒力十足！" },
            { "Use_DrinkTasty", "真好喝！" },
            { "Use_DrinkSecretly", "偷偷喝一口 {0}～" },
            { "Use_FoodCannotUse", "食物 {0} 無法使用" },
            { "Log_FoodEaten", "食用了食物：{0}" },

            // 吃屎相關提示
            { "Use_DuckRefusesPoop", "鴨鴨拒絕進食！那不是食物！" },
            { "Use_DontEatDuckPoop", "別讓鴨鴨吃糞便啊！" },
            { "Use_PoopDetected", "檢測到可疑物體：高風險生物廢棄物！" },
            { "Use_PoopGourmet", "鴨鴨不是美食家，請收起你的奇怪口味！" },
            { "Use_DuckCry", "鴨鴨流下了悲傷的淚水：『我做錯了什麼？』" },
            { "Use_DuckQuestionLife", "鴨鴨開始思考生命的意義……還有你的選擇。" },
            { "Use_DuckReputation", "鴨鴨的社會聲譽下降了99點！" },
            { "Use_DuckGag", "噁——鴨鴨快吐了！" },
            { "Use_DuckCivilRights", "鴨鴨要求尊重基本飲食權！" },
            { "Use_SurvivalMode", "求生本能啟動……但代價是靈魂受創。" },
            { "Use_PoopCuisine", "新菜系解鎖：法式糞便配鴨。" },
            { "Use_DuckBetrayed", "鴨鴨感到被深深背叛了……" },

            // 綁定屎相關提示
            { "FoodBind_AutoBindBurger", "已自動綁定老八秘製小漢堡！" },
            { "FoodBind_FoundPoop", "發現一坨{0}在背包裡！好臭" },

            
            // 綁定相關提示
            { "Binding_Success", "已綁定：{0}" },
            { "Binding_Success_Short", "已綁定{0}" },
            { "Binding_Failed", "綁定失敗：{0}" },
            { "Item_RemainingCount", "背包內剩餘數量 {0}" },
            { "Item_NoMoreItems_1", "背包裡沒有這個物品了哦！" },
            { "Item_NoMoreItems_2", "這個物品已經用完啦！" },
            { "Item_NoMoreItems_3", "你已經沒有這個物品了！" },
            { "Item_NoMoreItems_4", "用光光啦！" },
            { "Item_NoMoreItems_5", "沒有了！" },
            { "Item_NoMoreItems_6", "花光啦！" },
            
            // 輸入處理相關提示
            { "Input_SelectItemFirst", "請先選擇一個物品再開始綁定" },
            { "Input_NoRadialHere", "此處無法呼出圓盤選單！" },
            { "Input_NoLowValueItem", "未找到低價值物品" },
            { "Input_LowestValueItem", "價重比最低的是{0}！" },
            { "Input_SuggestDrop", "建議先丟掉{0}！" },
            { "Input_CannotCarry", "背不動了，先把{0}扔了吧～" },
            { "Input_NotWorthMoney", "{0}？這東西看起來不太值錢。" },
            { "Input_HeavyDrop", "太重了！快把{0}扔了！" },
            { "Input_CannotCarryAlt", "背不動了，先把{0}扔了吧～" },
            { "Input_LeastWorth", "{0} 最不值錢～" },
            { "Input_AuthorRequest", "拜託給環形選單點個讚，感謝！" },
            { "Menu_FocusLostClosed", "焦點丟失，圓盤選單已自動關閉" },
            { "Menu_PressToReopen", "按住 {0} 鍵重新打開圓盤選單" },
            
            // 食物綁定相關提示
            { "Food_AutoBindSector", "已為扇區 {0} 自動綁定食物：{1}" },
            { "Food_PreviousFoodEaten", "之前的食物吃光了，現在吃這個 {0} ！" },
            { "Food_TiredOfOld", "早就吃膩了，終於換成我喜歡的 {0} 了！" },
            { "Food_FoundBetter", "發現更好吃的 {0}，已經替換！" },
            { "Food_CheapDelicious", "{0} 又便宜又美味！" },
        };

        // 英语
        LocalizationData[SystemLanguage.English] = new Dictionary<string, string>
        {
            // Settings
            { "Settings_Title", "Radial Menu Settings" },
            
            // Configuration descriptions
            { "Config_FoodBindSectors", "Auto-bind food sector number" },
            { "Config_ignoreDurabilityValue", "Skip using medkits below this durability value" },
            { "Config_showLowValueFood", "Include food when checking low-value items" },
            { "Config_quickUseLastItemQ", "Enable quick-use last item with short Q press" },
            { "Config_iconSize", "Icon size" },
            { "Config_iconDistanceFactor", "Icon distance factor from center" },
            { "Config_uiScalePercent", "Background UI scale percent" },
            { "Config_innerDeadZoneCoefficient", "Inner dead zone coefficient" },
            { "Config_outerDeadZoneCoefficient", "Outer dead zone coefficient" },
            { "Config_longPressQWaitDuration", "Long-press Q duration to open radial menu" },
            { "Config_UI8style", "8-sector UI style" },
            { "Config_UI6style", "6-sector UI style" },
            { "Config_sectorCount", "Number of sectors" },
            { "Config_isBulletTimeEnabled", "Enable bullet time" },
            { "Config_bulletTimeMultiplier", "Bullet time speed multiplier" },
            { "Config_radialMenuActivationKey", "Key to open radial menu" },
            { "Config_enableFirstPersonAdaptation", "Enable first-person adaptation" },
            { "Config_firstPersonSensitivity", "First-person radial sensitivity" },
            { "Config_enableThirdPersonAdaptation", "Enable third-person adaptation" },
            { "Config_thirdPersonSensitivityMultiplier", "Third-person sensitivity multiplier" },
            { "Config_lockRadialMenuToCenter", "Lock radial menu to screen center" },
            { "Config_hintOptionsOnly", "The following options are hints only and have no effect" },
            { "Config_radialMenuStuckHint", "Radial menu stuck on screen? Try pressing the 'Win' key" },
            { "Config_haveFunHint", "Have fun" },
            { "Config_rightClickCloseRadialMenuHint", "Right-click to quickly close the radial menu after opening" },
            { "Config_disableSpeechBubbles", "Disable ducks' speech bubbles" },
            { "Config_playerHatedTypeIDs", "Blacklisted foods ID(will be placed at the end of the recommendation list)" }, // English              
            // UI messages
            { "UI_ItemCount", "Count: {0}" },
            { "UI_BindingNotAllowed", "This item cannot be bound" },
            { "UI_InstallModConfig", "Check the Workshop page and install required dependency mods" },
            { "UI_DefaultStyle", "Default" },
            { "UI_StyleOption", "{0} style" },
            { "UI_NoStyleDetected", "No background set detected for {0} sectors; using default" },
            
            // Logs
            { "Log_RadialMenuInit", "Initializing radial menu..." },
            { "Log_RadialMenuComplete", "Radial menu initialization complete" },
            { "Log_BindingComplete", "Persisted binding: Sector={0}, TypeID={1}, DisplayName={2}, autoBound={3}" },
            { "Log_IconDistanceUpdated", "Icon distance factor updated to {0}; recalculated icon positions" },
            { "Log_StyleUnavailable", "Style {0} is unavailable for sector {1}; using default {2}" },
            { "Log_LoadingStyle", "Loading background for sector {0}, using style: {1}" },
            { "Log_SectorAngle", "Calculated icon positions for {0} sectors; sector angle: {1}°" },
            
            // Item use messages
            { "Use_ExplosionArt", "Explosion is art!" },
            { "Use_EatItem", "Eat {0}!" },
            { "Use_EquipItem", "Equip {0}!" },
            { "Use_UseItem", "Use {0}!" },
            { "Use_HealthRecovered", "Health restored!" },
            { "Use_Ouch", "Ouch! Need to heal" },
            { "Use_ReplaceAfterUse", "Replace after use" },
            { "Use_HealthRemaining", "Will recover {0} HP" },
            { "Use_DrinkItem", "Drink {0}" },
            { "Use_ColaOverflow", "Fizz! The cola overflowed!" },
            { "Use_ColaDrinking", "Glug glug glug..." },
            { "Use_TasteItem", "Take a sneaky taste of {0}~" },
            { "Use_ColaByeBye", "Pop the cola — worries be gone." },
            { "Use_Cheers", "Cheers!" },
            { "Use_DrinkFirst", "Drink first!" },
            { "Use_DrinkForgetWorries", "Drink this and forget your worries~" },
            { "Use_FoodTasty", "{0} is delicious!" },
            { "Use_SoFragrant", "Smells amazing!" },
            { "Use_HealthFull", "Health is full" },
            { "Use_StrongDrink", "That's a strong one!" },
            { "Use_DrinkTasty", "Tastes great!" },
            { "Use_DrinkSecretly", "Sneak a sip of {0}~" },
            { "Use_FoodCannotUse", "Food {0} cannot be used" },
            { "Log_FoodEaten", "Ate food: {0}" },

            // Poop-related messages
            { "Use_DuckRefusesPoop", "The duck refuses to eat that — it's not food!" },
            { "Use_DontEatDuckPoop", "Don't let the duck eat poop!" },
            { "Use_PoopDetected", "Suspicious item detected: high-risk biological waste!" },
            { "Use_PoopGourmet", "The duck is not a gourmet — put away your weird tastes!" },
            { "Use_DuckCry", "The duck shed a sad tear: 'What did I do wrong?'" },
            { "Use_DuckQuestionLife", "The duck ponders the meaning of life... and your choices." },
            { "Use_DuckReputation", "The duck's reputation fell by 99 points!" },
            { "Use_DuckGag", "Ugh — the duck is about to gag!" },
            { "Use_DuckCivilRights", "The duck demands respect for its basic dietary rights!" },
            { "Use_SurvivalMode", "Survival mode activated... at the cost of the soul." },
            { "Use_PoopCuisine", "New cuisine unlocked: French Poop à la Duck." },
            { "Use_DuckBetrayed", "The duck feels deeply betrayed..." },

            // Food-bind poop jokes
            { "FoodBind_AutoBindBurger", "Auto-bound Lao Ba's secret burger!" },
            { "FoodBind_FoundPoop", "Found a pile of {0} in inventory! Ew." },

            // Binding messages
            { "Binding_Success", "Bound: {0}" },
            { "Binding_Success_Short", "Bound {0}" },
            { "Binding_Failed", "Binding failed: {0}" },
            { "Item_RemainingCount", "Remaining in inventory {0}" },
            { "Item_NoMoreItems_1", "You don't have this item in your inventory!" },
            { "Item_NoMoreItems_2", "This item is all used up!" },
            { "Item_NoMoreItems_3", "You no longer have this item!" },
            { "Item_NoMoreItems_4", "All gone!" },
            { "Item_NoMoreItems_5", "None left!" },
            { "Item_NoMoreItems_6", "Spent it all!" },

            // Input handling messages
            { "Input_SelectItemFirst", "Please select an item before binding" },
            { "Input_NoRadialHere", "You can't open the radial menu here!" },
            { "Input_NoLowValueItem", "No low-value item found" },
            { "Input_LowestValueItem", "Lowest value-to-weight ratio is {0}!" },
            { "Input_SuggestDrop", "Suggest dropping {0}!" },
            { "Input_CannotCarry", "Can't carry more — drop {0} first~" },
            { "Input_NotWorthMoney", "{0}? This doesn't seem worth much." },
            { "Input_HeavyDrop", "Too heavy! Drop {0}!" },
            { "Input_CannotCarryAlt", "Can't carry more — drop {0} first~" },
            { "Input_LeastWorth", "{0} is the least valuable~" },
            { "Input_AuthorRequest", "Please give the radial menu a like — thanks!" },
            { "Menu_FocusLostClosed", "Focus lost, radial menu automatically closed" },
            { "Menu_PressToReopen", "Hold {0} to reopen the radial menu" },

            // Food binding messages
            { "Food_AutoBindSector", "Auto-bound food {1} to sector {0}" },
            { "Food_PreviousFoodEaten", "Previous food was eaten, now eating {0}!" },
            { "Food_TiredOfOld", "Tired of the old one — finally switched to {0}!" },
            { "Food_FoundBetter", "Found a better {0}, replaced it!" },
            { "Food_CheapDelicious", "{0} is cheap and tasty!" },
        };

        // 日语
        LocalizationData[SystemLanguage.Japanese] = new Dictionary<string, string>
        {
            // 設定画面
            { "Settings_Title", "ラジアルメニュー設定" },

            // 設定項目説明
            { "Config_FoodBindSectors", "食べ物を自動で割り当てるセクター番号" },
            { "Config_ignoreDurabilityValue", "耐久値がこれ以下の救急箱は使用をスキップ" },
            { "Config_showLowValueFood", "低価値アイテムの判定に食べ物を含めるか" },
            { "Config_quickUseLastItemQ", "短押しQで前回のアイテムを素早く使用する" },
            { "Config_iconSize", "アイコンサイズ" },
            { "Config_iconDistanceFactor", "アイコンの中心からの距離係数" },
            { "Config_uiScalePercent", "背景UIの拡大率（％）" },
            { "Config_innerDeadZoneCoefficient", "内側デッドゾーン係数" },
            { "Config_outerDeadZoneCoefficient", "外側デッドゾーン係数" },
            { "Config_longPressQWaitDuration", "ラジアルメニューを出すための長押し時間" },
            { "Config_UI8style", "8セクターUIスタイル" },
            { "Config_UI6style", "6セクターUIスタイル" },
            { "Config_sectorCount", "セクター数" },
            { "Config_isBulletTimeEnabled", "バレットタイムを有効にする" },
            { "Config_bulletTimeMultiplier", "バレットタイム時のゲーム速度倍率" },
            { "Config_radialMenuActivationKey", "ラジアルメニューを開くキー" },
            { "Config_enableFirstPersonAdaptation", "一人称視点適応を有効にする" },
            { "Config_firstPersonSensitivity", "一人称での呼び出し感度" },
            { "Config_enableThirdPersonAdaptation", "三人称視点適応を有効にする" },
            { "Config_thirdPersonSensitivityMultiplier", "三人称での感度倍率" },
            { "Config_lockRadialMenuToCenter", "ラジアルメニューを画面中央に固定する" },
            { "Config_hintOptionsOnly", "以下のオプションはヒントのみで機能しません" },
            { "Config_radialMenuStuckHint", "メニューが画面に張り付いたら 'Win' キーを押してみてください" },
            { "Config_haveFunHint", "どうぞ楽しんでください" },
            { "Config_rightClickCloseRadialMenuHint", "メニューを開いた後、右クリックで素早く閉じられます" },
            { "Config_disableSpeechBubbles", "アヒルの吹き出しを無効にする" },
            { "Config_playerHatedTypeIDs", "ブラックリスト食品ID（おすすめリストの最後に配置されます）" }, // 日本語  

            // UI表示
            { "UI_ItemCount", "個数: {0}" },
            { "UI_BindingNotAllowed", "このアイテムはバインドできません" },
            { "UI_InstallModConfig", "ワークショップページを確認し、依存MODをインストールしてください" },
            { "UI_DefaultStyle", "デフォルト" },
            { "UI_StyleOption", "{0} スタイル" },
            { "UI_NoStyleDetected", "{0} セクター用の背景が見つかりません。デフォルトを使用します" },

            // ログ
            { "Log_RadialMenuInit", "ラジアルメニューの初期化を開始します..." },
            { "Log_RadialMenuComplete", "ラジアルメニューの初期化が完了しました" },
            { "Log_BindingComplete", "バインドを永続化しました: Sector={0}, TypeID={1}, DisplayName={2}, autoBound={3}" },
            { "Log_IconDistanceUpdated", "アイコン距離係数を {0} に更新しました。位置を再計算しています" },
            { "Log_StyleUnavailable", "スタイル {0} はセクター {1} には利用できません。デフォルト {2} を使用します" },
            { "Log_LoadingStyle", "{0} セクターの背景を読み込み中、使用スタイル: {1}" },
            { "Log_SectorAngle", "{0} 個のセクターのアイコン位置を計算しました。各セクター角度: {1}°" },

            // アイテム使用関連
            { "Use_ExplosionArt", "爆発こそ芸術！" },
            { "Use_EatItem", "{0} を食べる！" },
            { "Use_EquipItem", "{0} を装備する！" },
            { "Use_UseItem", "{0} を使用する！" },
            { "Use_HealthRecovered", "体力が回復した！" },
            { "Use_Ouch", "痛い！治療しないと" },
            { "Use_ReplaceAfterUse", "使用後に交換します" },
            { "Use_HealthRemaining", "{0} 回復します" },
            { "Use_DrinkItem", "{0} を飲む" },
            { "Use_ColaOverflow", "プシュッ！コーラがあふれた！" },
            { "Use_ColaDrinking", "ゴクゴクゴク..." },
            { "Use_TasteItem", "こっそり {0} を一口味見～" },
            { "Use_ColaByeBye", "コーラが開けば、悩みはさようなら。" },
            { "Use_Cheers", "乾杯！" },
            { "Use_DrinkFirst", "まずは一杯！" },
            { "Use_DrinkForgetWorries", "これを飲んで悩みを忘れてね～" },
            { "Use_FoodTasty", "{0} はおいしい！" },
            { "Use_SoFragrant", "いい香り！" },
            { "Use_HealthFull", "体力が満タンです" },
            { "Use_StrongDrink", "効きが強い！" },
            { "Use_DrinkTasty", "とても美味しい！" },
            { "Use_DrinkSecretly", "{0} をこっそり一口～" },
            { "Use_FoodCannotUse", "{0} は使用できません" },
            { "Log_FoodEaten", "食べたもの: {0}" },

            // うんち関連メッセージ
            { "Use_DuckRefusesPoop", "アヒルはそれを食べません！食べ物じゃない！" },
            { "Use_DontEatDuckPoop", "アヒルにうんちを食べさせないで！" },
            { "Use_PoopDetected", "不審な物体を検出：高リスク生物廃棄物！" },
            { "Use_PoopGourmet", "アヒルはグルメではありません。変な趣味はしまわって！" },
            { "Use_DuckCry", "アヒルが悲しい涙を流した：「私、何をしたの？」" },
            { "Use_DuckQuestionLife", "アヒルが人生の意味を考え始めた…そしてあなたの行動を。" },
            { "Use_DuckReputation", "アヒルの評判が99ポイント下がった！" },
            { "Use_DuckGag", "うっ…アヒルが吐きそう！" },
            { "Use_DuckCivilRights", "アヒルは基本的な食の権利を求める！" },
            { "Use_SurvivalMode", "サバイバル本能が発動…代償は魂。" },
            { "Use_PoopCuisine", "新しい料理アンロック：フレンチ・ポープ（鴨添え）" },
            { "Use_DuckBetrayed", "アヒルは深く裏切られた気持ちだ…" },

            // うんちのバインド
            { "FoodBind_AutoBindBurger", "老八の秘製バーガーを自動バインドしました！" },
            { "FoodBind_FoundPoop", "所持品に{0}の塊を発見！くさい！" },

            // バインド関連
            { "Binding_Success", "バインドしました：{0}" },
            { "Binding_Success_Short", "バインド{0}" },
            { "Binding_Failed", "バインド失敗：{0}" },
            { "Item_RemainingCount", "所持数 {0}" },
            { "Item_NoMoreItems_1", "所持していません！" },
            { "Item_NoMoreItems_2", "このアイテムは使い切りました！" },
            { "Item_NoMoreItems_3", "もうこのアイテムはありません！" },
            { "Item_NoMoreItems_4", "全部なくなった！" },
            { "Item_NoMoreItems_5", "残ってない！" },
            { "Item_NoMoreItems_6", "使い切った！" },

            // 入力処理
            { "Input_SelectItemFirst", "まずアイテムを選択してください" },
            { "Input_NoRadialHere", "ここではラジアルメニューを開けません！" },
            { "Input_NoLowValueItem", "低価値アイテムが見つかりません" },
            { "Input_LowestValueItem", "最も価値重量比が低いのは {0} です！" },
            { "Input_SuggestDrop", "{0} を捨てることをおすすめします！" },
            { "Input_CannotCarry", "これ以上持てません。先に {0} を捨ててください～" },
            { "Input_NotWorthMoney", "{0}？あまり価値がないようです。" },
            { "Input_HeavyDrop", "重すぎる！{0} を捨てて！" },
            { "Input_CannotCarryAlt", "これ以上持てません。先に {0} を捨ててください～" },
            { "Input_LeastWorth", "{0} が一番価値が低いです～" },
            { "Input_AuthorRequest", "ラジアルメニューにいいねをお願いします、ありがとう！" },
            { "Menu_FocusLostClosed", "フォーカスを失いました。ラジアルメニューを自動で閉じました" },
            { "Menu_PressToReopen", "{0} を押し続けてラジアルメニューを再度開く" },

            // 食べ物の自動バインド
            { "Food_AutoBindSector", "セクター {0} に食べ物 {1} を自動バインドしました" },
            { "Food_PreviousFoodEaten", "前の食べ物は食べられました。今は {0} を食べます！" },
            { "Food_TiredOfOld", "もう古いのには飽きた。やっと好きな {0} に変えた！" },
            { "Food_FoundBetter", "より良い {0} を見つけました。交換しました！" },
            { "Food_CheapDelicious", "{0} は安くておいしい！" },
        };

        // 韩语
        LocalizationData[SystemLanguage.Korean] = new Dictionary<string, string>
        {
            // 설정 화면
            { "Settings_Title", "원형 메뉴 설정" },
            
            // 설정 항목 설명
            { "Config_FoodBindSectors", "음식 자동 바인딩 구역 번호" },
            { "Config_ignoreDurabilityValue", "내구도 이하인 구급상자 자동 제외 기준" },
            { "Config_showLowValueFood", "가방에서 저가 아이템을 찾을 때 음식도 포함할지"},
            { "Config_quickUseLastItemQ", "Q 단축키로 최근 사용 아이템 빠른 사용 활성화" },
            { "Config_iconSize", "아이콘 크기" },
            { "Config_iconDistanceFactor", "아이콘의 중심 거리 비율" },
            { "Config_uiScalePercent", "배경 UI 축소·확대 비율(%)" },
            { "Config_innerDeadZoneCoefficient", "내부 데드존 계수" },
            { "Config_outerDeadZoneCoefficient", "외부 데드존 계수" },
            { "Config_longPressQWaitDuration", "Q를 길게 눌러 메뉴를 여는 시간(지연)" },
            { "Config_UI8style", "8분할 UI 스타일" },
            { "Config_UI6style", "6분할 UI 스타일" },
            { "Config_sectorCount", "구역(섹터) 수" },
            { "Config_isBulletTimeEnabled", "블릿타임(슬로우모션) 사용" },
            { "Config_bulletTimeMultiplier", "블릿타임 속도 배율" },
            { "Config_radialMenuActivationKey", "원형 메뉴 호출 단축키" },
            { "Config_enableFirstPersonAdaptation", "1인칭 모드 적응 활성화" },
            { "Config_firstPersonSensitivity", "1인칭에서 메뉴 감도" },
            { "Config_enableThirdPersonAdaptation", "3인칭 모드 적응 활성화" },
            { "Config_thirdPersonSensitivityMultiplier", "3인칭에서 메뉴 감도 배율" },
            { "Config_lockRadialMenuToCenter", "메뉴를 화면 중앙에서만 열기" },
            { "Config_hintOptionsOnly", "아래 옵션은 설명용이며 실제 기능을 수행하지 않습니다" },
            { "Config_radialMenuStuckHint", "메뉴가 화면에 멈춰 닫히지 않나요? 'Win' 키를 눌러보세요!" },
            { "Config_haveFunHint", "즐겜하세요!" },
            { "Config_rightClickCloseRadialMenuHint", "메뉴 호출 후 마우스 우클릭으로 빠르게 닫을 수 있습니다" },
            { "Config_disableSpeechBubbles", "오리 머리 위 말풍선 비활성화" },
            { "Config_playerHatedTypeIDs", "블랙리스트 음식ID(추천 목록의 맨 끝에 배치됩니다)" }, // 한국어  


            // UI 표시
            { "UI_ItemCount", "남은 수량: {0}" },
            { "UI_BindingNotAllowed", "이 항목은 바인딩할 수 없습니다" },
            { "UI_InstallModConfig", "창작마당(워크숍)에서 필요한 의존 모드를 설치하세요" },
            { "UI_DefaultStyle", "기본" },
            { "UI_StyleOption", "{0} 스타일" },
            { "UI_NoStyleDetected", "{0} 구역용 배경 스타일을 찾지 못해 기본값을 사용합니다" },
            
            // 로그
            { "Log_RadialMenuInit", "원형 메뉴 초기화 시작..." },
            { "Log_RadialMenuComplete", "원형 메뉴 초기화 완료" },
            { "Log_BindingComplete", "바인딩이 영구 저장되었습니다: 구역={0}, TypeID={1}, DisplayName={2}, autoBound={3}" },
            { "Log_IconDistanceUpdated", "아이콘 거리 계수 {0}로 업데이트됨 — 아이콘 위치 재계산 완료" },
            { "Log_StyleUnavailable", "현재 스타일 {0}은(는) {1} 구역에서 사용할 수 없습니다. 기본 스타일 {2}을(를) 사용합니다" },
            { "Log_LoadingStyle", "{0} 구역 배경 로드 중 — 스타일: {1}" },
            { "Log_SectorAngle", "{0}개의 구역 아이콘 배치 계산 완료. 각 구역 각도: {1}°" },
            
            // 아이템 사용 관련
            { "Use_ExplosionArt", "폭발은 예술이다!" },
            { "Use_EatItem", "{0}을(를) 먹는다!" },
            { "Use_EquipItem", "{0} 장착!" },
            { "Use_UseItem", "{0} 사용!" },
            { "Use_HealthRecovered", "체력 회복 완료!" },
            { "Use_Ouch", "아야! 빨리 치료해야 해!" },
            { "Use_ReplaceAfterUse", "한 번 쓰면 새 것으로 교체됩니다" },
            { "Use_HealthRemaining", "남은 회복량: {0}" },
            { "Use_DrinkItem", "{0}을(를) 마신다!" },
            { "Use_ColaOverflow", "치익! 콜라가 넘쳤다!" },
            { "Use_ColaDrinking", "꿀꺽꿀꺽꿀꺽……" },
            { "Use_TasteItem", "{0}을(를) 살짝 맛본다~" },
            { "Use_ColaByeBye", "콜라 한 모금에 근심 안녕!" },
            { "Use_Cheers", "건배!" },
            { "Use_DrinkFirst", "먼저 건배하고 마시자!" },
            { "Use_DrinkForgetWorries", "이 한 잔이면 걱정 다 잊는다~" },
            { "Use_FoodTasty", "{0} 진짜 맛있다!" },
            { "Use_SoFragrant", "와—향이 끝내주네!" },
            { "Use_HealthFull", "체력이 이미 가득합니다" },
            { "Use_StrongDrink", "와, 독하다!" },
            { "Use_DrinkTasty", "정말 맛있다!" },
            { "Use_DrinkSecretly", "{0}을(를) 몰래 한 모금~" },
            { "Use_FoodCannotUse", "음식 {0}은(는) 현재 사용할 수 없습니다" },
            { "Log_FoodEaten", "음식을 섭취했습니다: {0}" },

            // 똥 관련 메시지
            { "Use_DuckRefusesPoop", "오리가 거부합니다! 이건 음식이 아니에요!" },
            { "Use_DontEatDuckPoop", "오리한테 똥 먹이지 마세요!" },
            { "Use_PoopDetected", "의심스러운 물체 감지: 고위험 생물 폐기물!" },
            { "Use_PoopGourmet", "오리는 미식가가 아닙니다—그런 취향은 사양합니다!" },
            { "Use_DuckCry", "오리가 슬프게 울었습니다: '내가 뭘 잘못했지?'" },
            { "Use_DuckQuestionLife", "오리가 인생을 되돌아보기 시작했습니다……그리고 당신도요." },
            { "Use_DuckReputation", "오리의 평판이 99점 하락했습니다!" },
            { "Use_DuckGag", "으윽—오리가 토할 것 같아요!" },
            { "Use_DuckCivilRights", "오리가 기본 식사권을 요구합니다!" },
            { "Use_SurvivalMode", "생존 본능 발동……대가로 영혼이 상했습니다." },
            { "Use_PoopCuisine", "신메뉴 해금: 프렌치 똥 플래터 with 오리" },
            { "Use_DuckBetrayed", "오리는 깊은 배신감을 느꼈습니다……" },

            // 똥 바인딩 관련
            { "FoodBind_AutoBindBurger", "라오바 특제 버거 자동 바인딩됨!" },
            { "FoodBind_FoundPoop", "가방에서 {0}을(를) 발견했습니다! 악취가 심해요" },
            { "FoodBind_PoopDetected", "고위험 생물 폐기물 감지!" },
            
            // 바인딩 관련
            { "Binding_Success", "바인딩 성공: {0}" },
            { "Binding_Success_Short", "{0} 바인딩 완료" },
            { "Binding_Failed", "바인딩 실패: {0}" },
            { "Item_RemainingCount", "남은 개수: {0}" },
            { "Item_NoMoreItems_1", "이 아이템은 더 이상 없습니다!" },
            { "Item_NoMoreItems_2", "이건 다 써버렸어요!" },
            { "Item_NoMoreItems_3", "더 이상 가지고 있지 않습니다!" },
            { "Item_NoMoreItems_4", "모두 사용되었습니다!" },
            { "Item_NoMoreItems_5", "없습니다!" },
            { "Item_NoMoreItems_6", "싹 다 써버렸어요!" },
            
            // 입력 처리 관련
            { "Input_SelectItemFirst", "먼저 아이템을 선택하세요" },
            { "Input_NoRadialHere", "여기서는 원형 메뉴를 열 수 없습니다!" },
            { "Input_NoLowValueItem", "버릴 만한 물건을 찾지 못했습니다" },
            { "Input_LowestValueItem", "가성비 최저는 {0}입니다!" },
            { "Input_SuggestDrop", "{0}은(는) 버리는 것을 권장합니다!" },
            { "Input_CannotCarry", "더 이상 들 수 없습니다 — {0}을(를) 버리세요!" },
            { "Input_NotWorthMoney", "{0}? 값어치가 별로 없네요" },
            { "Input_HeavyDrop", "너무 무거워요! {0}을(를) 내려놓으세요!" },
            { "Input_CannotCarryAlt", "가방이 찼어요! {0}을(를) 버리세요~" },
            { "Input_LeastWorth", "{0}이(가) 가장 쓸모없어요~" },
            { "Input_AuthorRequest", "원형 메뉴 모드가 마음에 드셨다면 좋아요 부탁드립니다!" },
            { "Menu_FocusLostClosed", "포커스를 잃어 메뉴가 자동으로 닫혔습니다" },
            { "Menu_PressToReopen", "{0} 키를 길게 눌러 다시 열기" },
            
            // 음식 바인딩 관련
            { "Food_AutoBindSector", "구역 {0}에 자동으로 바인딩됨: {1}" },
            { "Food_PreviousFoodEaten", "이전 음식이 다 소진되었습니다. 이제 {0}을(를) 먹습니다!" },
            { "Food_TiredOfOld", "예전 음식에 질렸습니다, 드디어 {0}(으)로 교체!" },
            { "Food_FoundBetter", "더 맛있는 {0}을(를) 찾았습니다! 교체 완료!" },
            { "Food_CheapDelicious", "{0}은(는) 싸고 맛있습니다!" },
        };

        Log.Info($"Loaded translations for {LocalizationData.Count} languages");
    }

    
    /// <summary>
    /// 应用指定语言的翻译
    /// </summary>
    private static void ApplyTranslations(SystemLanguage language)
    {
        // 如果没有该语言的翻译，使用英语作为后备
        if (!LocalizationData.ContainsKey(language))
        {
            Log.Warn($"No translations found for {language}, falling back to English");
            language = SystemLanguage.ChineseSimplified;
        }

        var translations = LocalizationData[language];
        foreach (var kvp in translations)
        {
            string fullKey = GetFullKey(kvp.Key);
            LocalizationManager.SetOverrideText(fullKey, kvp.Value);
        }

        Log.Info( $"Applied {translations.Count} translations for {language}");
    }

    /// <summary>
    /// 获取完整的本地化键名
    /// </summary>
    private static string GetFullKey(string shortKey)
    {
        return KeyPrefix + shortKey;
    }

    /// <summary>
    /// 获取本地化文本（便捷方法）
    /// </summary>
    public static string Get(string key)
    {
        return LocalizationManager.GetPlainText(GetFullKey(key));
    }

    /// <summary>
    /// 获取格式化的本地化文本
    /// </summary>
    public static string GetFormatted(string key, params object[] args)
    {
        string text = Get(key);
        try
        {
            return string.Format(text, args);
        }
        catch (System.Exception ex)
        {
            Log.Error($"Failed to format localization string '{key}': {ex.Message}");
            return text;
        }
    }

    /// <summary>
    /// 为 Text 组件自动更新本地化文本
    /// 使用方式: LocalizationHelper.SetLocalizedText(textComponent, "localization_key");
    /// </summary>
    public static void SetLocalizedText(UnityEngine.UI.Text textComponent, string localizationKey)
    {
        if (textComponent == null) return;

        textComponent.text = Get(localizationKey);

        // 订阅语言变更事件，当语言改变时自动更新文本
        OnLanguageChanged += (lang) =>
        {
            if (textComponent != null && !string.IsNullOrEmpty(localizationKey))
            {
                textComponent.text = Get(localizationKey);
            }
        };
    }

    /// <summary>
    /// 为 TextMeshProUGUI 组件自动更新本地化文本
    /// 使用方式: LocalizationHelper.SetLocalizedText(tmpText, "localization_key");
    /// </summary>
    public static void SetLocalizedText(TMPro.TextMeshProUGUI textComponent, string localizationKey)
    {
        if (textComponent == null) return;

        textComponent.text = Get(localizationKey);

        // 订阅语言变更事件，当语言改变时自动更新文本
        OnLanguageChanged += (lang) =>
        {
            if (textComponent != null && !string.IsNullOrEmpty(localizationKey))
            {
                textComponent.text = Get(localizationKey);
            }
        };
    }
}

