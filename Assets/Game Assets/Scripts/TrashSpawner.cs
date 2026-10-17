using UnityEngine;

public enum TrashCategory { Biodegradable, NonBio, Recyclable, Hazardous, Collectible }

[System.Serializable]
public class TrashEntry
{
    public string name;
    public Sprite sprite;
    public TrashCategory category;
    [Tooltip("0 = use the default weight for its category")]
    public float weight = 0f;
    [Tooltip("0 = normal item. 1000 means a 1 in 1000 chance per spawn")]
    public int oneIn = 0;
}

public class TrashSpawner : MonoBehaviour
{
    [Header("Map")]
    [SerializeField] MapSlider mapSlider;

    [Header("Timing")]
    [SerializeField] float minInterval = 1f;
    [SerializeField] float maxInterval = 8f;
    [SerializeField] int maxPerMap = 12;

    [Header("Look")]
    [SerializeField] TrashPickup trashPrefab;
    [SerializeField] float trashSize = 0.6f;   // longest side in world units
    [SerializeField] int sortingOrder = 10;

    [Header("Variety")]
    [Range(0f, 1f)]
    [SerializeField] float mixChance = 0.3f;   // 0 = each map uses only its own list, 1 = everything everywhere

    [Header("Where trash can appear (world units; select this object to see boxes)")]
    [SerializeField] Rect forestArea = new Rect(-1.5f, -2.5f, 3f, 4f);
    [SerializeField] Rect beachArea  = new Rect(-1.5f, -2.5f, 3f, 4f);
    [SerializeField] Rect cityArea   = new Rect(-1.5f, -2.5f, 3f, 4f);

    [Header("Trash per location")]
    [SerializeField] TrashEntry[] forest;
    [SerializeField] TrashEntry[] beach;
    [SerializeField] TrashEntry[] city;

    [Header("Testing")]
    [SerializeField] float rareBoost = 1f;   // 500 makes rare items 500x more likely

    float timer;

    void Start() { timer = Random.Range(minInterval, maxInterval); }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer > 0f) return;

        bool handled = TrySpawn();
        timer = handled ? Random.Range(minInterval, maxInterval) : 0.25f;
    }

    bool TrySpawn()
    {
        if (mapSlider.IsSliding) return false;

        Transform map = mapSlider.CurrentMap;
        if (map.GetComponentsInChildren<TrashPickup>().Length >= maxPerMap) return true;

        int i = mapSlider.Index;
        TrashEntry[] home = i == 0 ? forest : i == 1 ? beach : city;
        Rect area = i == 0 ? forestArea : i == 1 ? beachArea : cityArea;

        bool mix = Random.value < mixChance;
        TrashEntry pick = Roll(home, mix);
        if (pick == null || pick.sprite == null) return true;

        Vector3 pos = new Vector3(
            Random.Range(area.xMin, area.xMax),
            Random.Range(area.yMin, area.yMax), 0f);

        var t = Instantiate(trashPrefab, pos, Quaternion.identity);

        // Same on-screen size for every item, whatever the sprite's pixel size
        float longest = Mathf.Max(pick.sprite.bounds.size.x, pick.sprite.bounds.size.y);
        t.transform.localScale = Vector3.one * (trashSize / longest);

        t.transform.SetParent(map, true);          // moves with the map when it slides
        t.Setup(pick, sortingOrder);
        return true;
    }

    TrashEntry Roll(TrashEntry[] home, bool mix)
    {
        // 1) Rare items stay on their own map: each rolls its own 1-in-N chance
        if (home != null)
            foreach (var e in home)
                if (e.oneIn > 0 && Random.value < rareBoost / e.oneIn) return e;

        // 2) Normal items: weighted pick from this map, or from all maps when mixing
        TrashEntry[][] pools = mix ? new[] { forest, beach, city } : new[] { home };

        float total = 0f;
        foreach (var pool in pools)
        {
            if (pool == null) continue;
            foreach (var e in pool)
                if (e.oneIn <= 0) total += WeightOf(e);
        }
        if (total <= 0f) return null;

        float r = Random.value * total;
        TrashEntry last = null;
        foreach (var pool in pools)
        {
            if (pool == null) continue;
            foreach (var e in pool)
            {
                if (e.oneIn > 0) continue;
                last = e;
                r -= WeightOf(e);
                if (r <= 0f) return e;
            }
        }
        return last;
    }

    float WeightOf(TrashEntry e)
    {
        if (e.weight > 0f) return e.weight;
        switch (e.category)
        {
            case TrashCategory.Hazardous:  return 3f;    // rarer
            case TrashCategory.Recyclable: return 8f;
            default:                       return 10f;   // bio and non-bio are common
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;  Gizmos.DrawWireCube(forestArea.center, forestArea.size);
        Gizmos.color = Color.cyan;   Gizmos.DrawWireCube(beachArea.center,  beachArea.size);
        Gizmos.color = Color.yellow; Gizmos.DrawWireCube(cityArea.center,   cityArea.size);
    }
}