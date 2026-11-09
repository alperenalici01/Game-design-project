using UnityEngine;

// Sabit sayıdaki blokları havuzda tekrar kullanır; dalga sırası ve üretim aralığını yönetir.
[DefaultExecutionOrder(-100)]
[DisallowMultipleComponent]
public class MertcanBlockParty : MonoBehaviour
{
    [Header("Course 06 pooled Block Party waves")]
    [SerializeField] private Rigidbody partyPlayerBody = null;
    [SerializeField] private MertcanBlockPartyPiece[] partyPieces = null;
    [SerializeField, Min(0.1f)] private float partySpeed = 3f;
    [SerializeField, Min(5.5f)] private float partyWaveInterval = 6f;
    [SerializeField, Min(1.5f)] private float partyGapWidth = 3.6f;
    [SerializeField] private float partySpawnX = 154.5f;
    [SerializeField] private float partyDespawnX = 121.5f;
    [SerializeField] private float partyArenaStartX = 120f;
    [SerializeField] private float partyArenaEndX = 156f;
    [SerializeField] private float partyCenterZ = 21.75f;
    [SerializeField] private float partyWidth = 16.5f;
    [SerializeField] private float partyFloorY = 2.51f;
    [SerializeField, Min(0.5f)] private float partySideMargin = 0.75f;
    [SerializeField, Min(3.5f)] private float partyHighHeight = 3.6f;
    [SerializeField, Min(0.2f)] private float partyLowHeight = 0.75f;
    [SerializeField, Min(0.5f)] private float partyBlockDepth = 1.2f;
    [SerializeField, Min(2f)] private float partySpawnClearance = 14f;
    private static readonly int[] PartySequence = { 0, 1, 2, 3, 2, 1, 0, 3 };
    private bool[] partySlotActive;
    private float[] partySlotX;
    private float partyClock;
    private float partyNextSpawn;
    private int partyPatternIndex;
    internal float Speed => partySpeed;

    private void Start()
    {
        if (partyPlayerBody == null || partyPieces == null || partyPieces.Length < 2 || partyPieces.Length % 2 != 0)
        {
            Debug.LogError("BlockParty needs a player body and paired, prebuilt pool pieces.", this);
            enabled = false;
            return;
        }
        bool needsPhysicalPush = false;
        foreach (MertcanBlockPartyPiece piece in partyPieces)
        {
            if (piece == null || piece.GetComponent<Rigidbody>() == null)
            {
                Debug.LogError("BlockParty pool has an invalid solid kinematic piece.", this);
                enabled = false;
                return;
            }
            if (!piece.IsHazard) needsPhysicalPush = true;
            piece.Owner = this;
            piece.Body = piece.GetComponent<Rigidbody>();
            piece.gameObject.SetActive(false);
        }
        partySlotActive = new bool[partyPieces.Length / 2];
        partySlotX = new float[partySlotActive.Length];
        partyClock = 0f;
        partyNextSpawn = 0f;
        partyPatternIndex = 0;
        if (needsPhysicalPush && partyPlayerBody.GetComponent<MertcanBlockPartyPush>() == null)
            partyPlayerBody.gameObject.AddComponent<MertcanBlockPartyPush>();
    }

    private bool IsPartySpawnClear()
    {
        Vector3 p = partyPlayerBody.position;
        bool insideLane = p.x >= partyArenaStartX && p.x < partyArenaEndX &&
            Mathf.Abs(p.z - partyCenterZ) <= partyWidth * 0.5f + 1f &&
            p.y >= partyFloorY - 1f && p.y < partyFloorY + partyHighHeight + 3f;
        return !insideLane || Mathf.Abs(p.x - partySpawnX) > partySpawnClearance;
    }

    private void FixedUpdate()
    {
        if (partySlotActive == null) return;
        float speed = Mathf.Max(0.1f, partySpeed);
        partyClock += Time.fixedDeltaTime;
        for (int slot = 0; slot < partySlotActive.Length; slot++)
        {
            if (!partySlotActive[slot]) continue;
            float x = partySlotX[slot] - speed * Time.fixedDeltaTime;
            if (x <= partyDespawnX)
            {
                partyPieces[slot * 2].gameObject.SetActive(false);
                partyPieces[slot * 2 + 1].gameObject.SetActive(false);
                partySlotActive[slot] = false;
                continue;
            }
            partySlotX[slot] = x;
            for (int part = 0; part < 2; part++)
            {
                MertcanBlockPartyPiece piece = partyPieces[slot * 2 + part];
                if (!piece.gameObject.activeSelf) continue;
                Vector3 position = piece.Body.position;
                position.x = x;
                piece.Body.MovePosition(position);
            }
        }
        // A pending spawn is deferred near the player; active waves keep moving and the sequence never ends.
        if (partyClock < partyNextSpawn || !IsPartySpawnClear()) return;
        for (int slot = 0; slot < partySlotActive.Length; slot++)
        {
            if (partySlotActive[slot]) continue;
            SpawnPartySlot(slot, PartySequence[partyPatternIndex]);
            partyPatternIndex = (partyPatternIndex + 1) % PartySequence.Length;
            partyNextSpawn = partyClock + Mathf.Max(5.5f, partyWaveInterval);
            break;
        }
    }

    private void SpawnPartySlot(int slot, int pattern)
    {
        float width = Mathf.Max(4f, partyWidth);
        float gap = Mathf.Clamp(partyGapWidth, 1.5f, width - 2f * partySideMargin);
        float minZ = partyCenterZ - width * 0.5f;
        float maxZ = partyCenterZ + width * 0.5f;
        MertcanBlockPartyPiece first = partyPieces[slot * 2];
        MertcanBlockPartyPiece second = partyPieces[slot * 2 + 1];
        if (pattern == 3)
        {
            ConfigurePartyPiece(first, partySpawnX, partyCenterZ, width,
                Mathf.Max(0.2f, partyLowHeight));
            second.gameObject.SetActive(false);
        }
        else
        {
            float side = width * 0.5f - partySideMargin - gap * 0.5f;
            float center = partyCenterZ + (pattern == 0 ? side : (pattern == 2 ? -side : 0f));
            float leftEnd = center - gap * 0.5f;
            float rightStart = center + gap * 0.5f;
            ConfigurePartyPiece(first, partySpawnX, (minZ + leftEnd) * 0.5f,
                leftEnd - minZ, Mathf.Max(3.5f, partyHighHeight));
            ConfigurePartyPiece(second, partySpawnX, (rightStart + maxZ) * 0.5f,
                maxZ - rightStart, Mathf.Max(3.5f, partyHighHeight));
        }
        partySlotX[slot] = partySpawnX;
        partySlotActive[slot] = true;
    }

    private void ConfigurePartyPiece(MertcanBlockPartyPiece piece, float x, float z, float width,
        float height)
    {
        piece.gameObject.SetActive(false);
        if (width <= 0.01f) return;
        piece.GetComponent<Collider>().isTrigger = piece.IsHazard;
        piece.transform.localScale = new Vector3(Mathf.Max(0.5f, partyBlockDepth), height / 0.4f, width);
        Vector3 position = new Vector3(x, partyFloorY + height * 0.5f, z);
        piece.transform.SetPositionAndRotation(position, Quaternion.identity);
        piece.Body.position = position;
        piece.Body.rotation = Quaternion.identity;
        // Use the scene's shared M_Hazard material unchanged, including after pool reuse.
        piece.GetComponent<Renderer>().SetPropertyBlock(null);
        piece.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        if (partyPieces != null)
            foreach (MertcanBlockPartyPiece piece in partyPieces)
                if (piece != null) piece.gameObject.SetActive(false);
        if (partySlotActive != null)
            for (int i = 0; i < partySlotActive.Length; i++) partySlotActive[i] = false;
        partyNextSpawn = partyClock;
    }
}
