using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum FillState
{
    None,
    Fill,
    Wall,
    Past
}

public class FloodFillSystem : MonoSingleton<FloodFillSystem>
{
    public void FloodFill(Vector2Int startPos)
    {
        Stack<Vector2Int> openStack1 = new();
        Stack<Vector2Int> openStack2 = new();

        HashSet<Vector2Int> closeHash1 = new();
        HashSet<Vector2Int> closeHash2 = new();

        StartCoroutine(FloodFillS(startPos));

        IEnumerator FloodFillS(Vector2Int startPos)
        {
            Vector2Int operationPosR = startPos + Vector2Int.right;
            Vector2Int operationPosL = startPos + Vector2Int.left;

            //Debug.Log($"operL{operationPosL}");
            //Debug.Log($"operR{operationPosR}");

            bool left = PixelManager.Instance.GetPixel(startPos.x, startPos.y) == FillState.Past && PixelManager.Instance.GetPixel(operationPosL.x, operationPosL.y) == FillState.Past;
            bool right = PixelManager.Instance.GetPixel(startPos.x, startPos.y) == FillState.Past && PixelManager.Instance.GetPixel(operationPosR.x, operationPosR.y) == FillState.Past;

            if (left || right)
            {
                openStack1.Push(startPos + Vector2Int.up);
                openStack2.Push(startPos + Vector2Int.down);
                //Debug.Log($"startPos+Up{startPos + Vector2Int.up}");
                //Debug.Log($"startPos+Down{startPos + Vector2Int.down}");
                yield return null;
            }
            else
            {
                openStack1.Push(startPos + Vector2Int.left);
                openStack2.Push(startPos + Vector2Int.right);
                //Debug.Log($"startPos+Left{startPos + Vector2Int.left}");
                //Debug.Log($"startPos+Right{startPos + Vector2Int.right}");
                yield return null;
            }

            while (openStack1.Count != 0 || openStack2.Count != 0)
            {
                Vector2Int checkPos1 = openStack1.Pop();
                Vector2Int checkPos2 = openStack2.Pop();

                Vector2Int[] directions = new Vector2Int[4] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

                for (int i = 0; i < 4; i++)
                {
                    Debug.Log($"i {i}");
                    Vector2Int operPos = checkPos1 + directions[i];
                    if (IsVaild(checkPos1, closeHash1))
                    {
                        Debug.Log("Add1");
                        AddPixel1(operPos);
                    }
                    operPos = checkPos2 + directions[i];
                    if (IsVaild(checkPos2, closeHash2))
                    {
                        Debug.Log("Add2");
                        AddPixel2(operPos);
                    }
                }
                yield return null;
            }
            DestroyPixel(closeHash1, closeHash2);
            Debug.Log("Stop");
        }

        void AddPixel1(Vector2Int operPos)
        {
            openStack1.Push(operPos);
            closeHash1.Add(operPos);
        }

        void AddPixel2(Vector2Int operPos)
        {
            openStack2.Push(operPos);
            closeHash2.Add(operPos);
        }

        void DestroyPixel(HashSet<Vector2Int> closeHash1, HashSet<Vector2Int> closeHash2)
        {
            if (closeHash1.Count > closeHash2.Count)
            {
                closeHash1.Clear();
                Debug.Log("DestroyHash1");
                foreach (var close in closeHash2)
                {
                    Debug.Log($"hash2 {close}");
                    PixelManager.Instance.SetColor(close.x, close.y, Color.blue/*PixelManager.Instance.GetColor(close.x, close.y, TextureType.After)*/);
                    PixelManager.Instance.SetApply();
                    PixelManager.Instance.SetPixel(close.x, close.y, FillState.Fill);
                }
            }
            else
            {
                closeHash2.Clear();
                Debug.Log("DestroyHash2");
                foreach (var close in closeHash1)
                {
                    Debug.Log($"hash1 {close}");
                    PixelManager.Instance.SetColor(close.x, close.y, Color.yellow/*PixelManager.Instance.GetColor(close.x, close.y, TextureType.After)*/);
                    PixelManager.Instance.SetApply();
                    PixelManager.Instance.SetPixel(close.x, close.y, FillState.Fill);
                }
            }
        }
    }

    private bool IsVaild(Vector2Int checkPos, HashSet<Vector2Int> closeHashPos)
    {
        FillState checkArray = PixelManager.Instance.nodeArray[checkPos.x, checkPos.y];

        if (checkArray == FillState.None && !closeHashPos.Contains(checkPos))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}