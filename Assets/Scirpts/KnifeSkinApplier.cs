// using UnityEngine;
// using UnityEngine.UI;

// /// <summary>
// /// Game wali knife par lagao (best: knife PREFAB par).
// /// Jo knife shop me equip hai wahi sprite is knife par lag jayegi,
// /// purani sprite replace ho jayegi. Equip badalte hi live update bhi hota hai.
// ///
// /// Setup:
// ///   - Is script ko game ki knife object / prefab par lagao.
// ///   - allKnifeSprites me saari knives ki sprites SHOP ke order me daalo:
// ///       Page 1 ki slot 1 se 9, phir Page 2 ki slot 1 se 9, phir Page 3 ... (global index = pageIndex * 9 + slot)
// ///   - Knife me SpriteRenderer ho ya UI Image, dono chalega (khud dhoondh leta hai).
// /// </summary>
// public class KnifeSkinApplier : MonoBehaviour
// {
//     [Header("Saari knives ki sprites (shop ke order me)")]
//     [Tooltip("Index 0 = Page 1 slot 1 ... Index 9 = Page 2 slot 1 ... Bilkul shop ke global order me.")]
//     public Sprite[] allKnifeSprites;

//     [Header("Kis par lagani hai (khali chhodo to khud dhoondh lega)")]
//     public SpriteRenderer spriteRenderer;
//     public Image uiImage;

//     void Awake()
//     {
//         if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
//         if (uiImage == null)        uiImage        = GetComponentInChildren<Image>(true);
//     }

//     void OnEnable()
//     {
//         Apply();
//         KnifeShop.OnEquippedChanged += Apply;   // shop me equip badle to live update
//     }

//     void OnDisable()
//     {
//         KnifeShop.OnEquippedChanged -= Apply;
//     }

//     public void Apply()
//     {
//         int index = KnifeShop.EquippedIndex;

//         // Galat index ya sprite nahi di -> knife ki default sprite waisi hi rahegi
//         if (allKnifeSprites == null || index < 0 || index >= allKnifeSprites.Length) return;

//         Sprite sprite = allKnifeSprites[index];
//         if (sprite == null) return;

//         if (spriteRenderer != null) spriteRenderer.sprite = sprite;
//         if (uiImage != null)        uiImage.sprite = sprite;
//     }
// }