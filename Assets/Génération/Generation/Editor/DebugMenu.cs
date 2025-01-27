using UnityEngine;
using UnityEditor;

public static class DebugMenu
{
    [MenuItem("Debug/Print Global Position")]
    public static void PrintGlobalPosition()
    {
        if (Selection.activeGameObject != null)
        {
            Debug.Log(Selection.activeGameObject.name + " is at " + Selection.activeGameObject.transform.position);
        } else Debug.LogWarning("No Object Selected");
    }


    [MenuItem("Debug/BoxCaster")]
    public static void BoxCaster()
    {
        if (Selection.activeGameObject != null)
        {
            Collider2D[] cols = new Collider2D[99];
            int n = Physics2D.OverlapBoxNonAlloc(Selection.activeGameObject.transform.position, Selection.activeGameObject.transform.localScale, 0, cols);
            cols = new Collider2D[n];
            Physics2D.OverlapBoxNonAlloc(Selection.activeGameObject.transform.position, Selection.activeGameObject.transform.localScale, 0, cols);
            if (n > 0)
            {
                foreach(Collider2D col in cols) { Debug.Log(col.name); }
            }
        }
    }

        [MenuItem("Debug/Is Overlapping ?")]
    public static bool IsOverlapping()
    {
        if (Selection.activeGameObject != null) 
        {
            Collider2D[] cols = Physics2D.OverlapBoxAll(
                    Selection.activeGameObject.transform.position,
                    Selection.activeGameObject.GetComponent<Room>()._boxCollider.size,
                    0
                    );
            int n = 0;
            if(cols.Length > 0)
            {
                foreach(Collider2D col in cols) { n++; Debug.Log(col.name); }
                Debug.Log(n);
                return true;
            }else { Debug.Log("No Collider"); }
            return false;

        }
        else Debug.LogWarning("No Object Selected");
        return false;        
    }

    [MenuItem("Debug/Add Exits")]
    public static void AddExits()
    {
        if (Selection.activeGameObject != null)
        {
            foreach(GameObject go in Selection.gameObjects)
            {
                go.AddComponent<Exit>();
            }
        }
        else Debug.LogWarning("No Object Selected");
    }
    [MenuItem("Debug/Check Exits")]
    public static void CheckExits()
    {
        Room room = null;
        if (Selection.activeGameObject != null && Selection.activeGameObject.TryGetComponent<Room>(out room))
        {
            foreach (Exit ex in room.GetExits())
            {
                ex.CanPlaceARoom();
            }
        }
        else Debug.Log("Object not a Room : " + Selection.activeGameObject.name );
    }
}