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
