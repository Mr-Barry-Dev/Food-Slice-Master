// using System;
// using UnityEngine;

// /// <summary>
// /// Game scene me EK baar lagao (kisi bhi empty object par, jaise "KnifeSelector").
// /// Knife prefabs me kuch lagane ki zaroorat nahi.
// ///
// /// Prefabs PAGE-WISE daalo: Pages ka array, har page ke andar uski knives ka array.
// ///   Page 1 -> slot 1 se 9
// ///   Page 2 -> slot 1 se 9 ...
// /// (Shop ki KnifeShop script me jo pageIndex aur slot order hai, bilkul wahi.)
// ///
// /// Do tareeke se use ho sakta hai:
// ///  A) Tumhara spawner knife Instantiate karta hai:
// ///       Instantiate(knifeSelector.EquippedPrefab, position, rotation);
// ///  B) Scene me pehle se ek knife rakhi hai:
// ///       "currentKnifeInScene" me drag karo. Start par wo hat kar equipped knife ka prefab
// ///       usi jagah lag jayega (shop me equip badle to live bhi badlega).
// /// </summary>
// public class KnifeSelector : MonoBehaviour
// {
//     [Serializable]
//     public class KnifePage
//     {
//         [Tooltip("Sirf pehchan ke liye naam, jaise Page 1")]
//         public string pageName = "Page";

//         [Tooltip("Is page ki knives ke prefabs, slot 1 se 9 ke order me")]
//         public GameObject[] knives = new GameObject[9];
//     }

//     [Header("Pages (har page ki knives alag)")]
//     [Tooltip("Element 0 = Page 1 (pageIndex 0), Element 1 = Page 2 (pageIndex 1) ...")]
//     public KnifePage[] pages;

//     [Header("Shop ke ek page me kitne slots hain (KnifeShop ke slots jitna)")]
//     public int slotsPerPage = 9;

//     [Header("Tareeka B: scene me rakhi hui knife (optional)")]
//     [Tooltip("Khali chhodo agar spawner se knife aati hai (tareeka A).")]
//     public GameObject currentKnifeInScene;
//     public bool replaceOnStart = true;
//     [Tooltip("ON: naya knife apne prefab wale position/rotation/size me aayega (purani knife ka size copy nahi hoga). " +
//              "OFF: purani knife ki jagah par aayega, par size prefab ka apna hi rahega.")]
//     public bool usePrefabOwnTransform = true;

//     // Tareeka A ke liye: spawner isse prefab le
//     public GameObject EquippedPrefab
//     {
//         get
//         {
//             int index = KnifeShop.EquippedIndex;    // global number = pageIndex * slotsPerPage + slot
//             if (index < 0) index = 0;

//             int page = index / slotsPerPage;
//             int slot = index % slotsPerPage;

//             GameObject prefab = GetPrefab(page, slot);

//             // Prefab na mile (khali slot / galat index) -> Page 1 ki pehli knife
//             if (prefab == null) prefab = GetPrefab(0, 0);
//             return prefab;
//         }
//     }

//     GameObject GetPrefab(int page, int slot)
//     {
//         if (pages == null || page < 0 || page >= pages.Length) return null;

//         KnifePage p = pages[page];
//         if (p == null || p.knives == null || slot < 0 || slot >= p.knives.Length) return null;

//         return p.knives[slot];
//     }

//     void Start()
//     {
//         if (replaceOnStart && currentKnifeInScene != null)
//             ReplaceKnife();
//     }

//     void OnEnable()
//     {
//         KnifeShop.OnEquippedChanged += OnEquipChanged;
//     }

//     void OnDisable()
//     {
//         KnifeShop.OnEquippedChanged -= OnEquipChanged;
//     }

//     void OnEquipChanged()
//     {
//         if (currentKnifeInScene != null)
//             ReplaceKnife();
//     }

//     // Purani knife hatao, usi jagah (position, rotation, parent) naya equipped prefab lagao
//     void ReplaceKnife()
//     {
//         GameObject prefab = EquippedPrefab;
//         if (prefab == null)
//         {
//             Debug.LogWarning("KnifeSelector: equipped knife ka prefab nahi mila. Pages/Knives array check karo.");
//             return;
//         }

//         Transform old = currentKnifeInScene.transform;
//         Transform parent = old.parent;
//         int siblingIndex = old.GetSiblingIndex();

//         GameObject newKnife;
//         if (usePrefabOwnTransform)
//         {
//             // Prefab ka apna local position / rotation / scale (purani knife ka size copy nahi hoga)
//             newKnife = Instantiate(prefab, parent, false);
//         }
//         else
//         {
//             // Purani knife ki jagah par, par size prefab ka apna hi
//             newKnife = Instantiate(prefab, old.position, old.rotation, parent);
//         }
//         newKnife.transform.SetSiblingIndex(siblingIndex);

//         Debug.Log("KnifeSelector: " + newKnife.name + " laga di (equipped index " + KnifeShop.EquippedIndex + ")");

//         Destroy(currentKnifeInScene);
//         currentKnifeInScene = newKnife;
//     }
// }

using System;
using UnityEngine;

/// <summary>
/// Saari knife prefabs ki list, ek ASSET file me (kisi scene me nahi).
/// Isliye shop scene aur game scene, dono se same list use ho sakti hai.
///
/// Banane ka tareeka: Project window me right-click ->
///   Create -> Knife -> Knife Library
///
/// Prefabs PAGE-WISE daalo (shop ke pageIndex / slot order me):
///   Pages[0] = shop ka Page 1 (pageIndex 0), slot 1 se 9
///   Pages[1] = shop ka Page 2 (pageIndex 1), slot 1 se 9 ...
/// </summary>
[CreateAssetMenu(fileName = "KnifeLibrary", menuName = "Knife/Knife Library")]
public class KnifeLibrary : ScriptableObject
{
    [Serializable]
    public class KnifePage
    {
        [Tooltip("Sirf pehchan ke liye naam, jaise Page 1")]
        public string pageName = "Page";

        [Tooltip("Is page ki knives ke prefabs, slot 1 se 9 ke order me")]
        public GameObject[] knives = new GameObject[9];
    }

    [Header("Pages (har page ki knives alag)")]
    [Tooltip("Element 0 = Page 1 (pageIndex 0), Element 1 = Page 2 (pageIndex 1) ...")]
    public KnifePage[] pages;

    [Header("Shop ke ek page me kitne slots hain (KnifeShop ke slots jitna)")]
    public int slotsPerPage = 9;

    // Shop me jo knife equip hai uska prefab (game scene se bhi chalega, PlayerPrefs se padhta hai)
    public GameObject EquippedPrefab
    {
        get
        {
            int index = KnifeShop.EquippedIndex;    // global number = pageIndex * slotsPerPage + slot
            if (index < 0) index = 0;

            int perPage = Mathf.Max(1, slotsPerPage);
            int page = index / perPage;
            int slot = index % perPage;

            GameObject prefab = GetPrefab(page, slot);

            // Prefab na mile (khali slot / galat index) -> Page 1 ki pehli knife
            if (prefab == null) prefab = GetPrefab(0, 0);
            return prefab;
        }
    }

    public GameObject GetPrefab(int page, int slot)
    {
        if (pages == null || page < 0 || page >= pages.Length) return null;

        KnifePage p = pages[page];
        if (p == null || p.knives == null || slot < 0 || slot >= p.knives.Length) return null;

        return p.knives[slot];
    }
}
