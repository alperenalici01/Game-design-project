using UnityEngine;

// Ortak Goal.cs bu eski türü aradığı için ad ve GUID korunur.
// Hareket yapmaz; oyuncudaki tek respawn bileşeni MertcanRespawn davranışını miras alır.
[DefaultExecutionOrder(-100)]
[DisallowMultipleComponent]
[AddComponentMenu("Mertcan/Player Compatibility")]
public class PlayerMove_Mertcan : MertcanRespawn
{
}
