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
    private static Vector2Int[] directions = new Vector2Int[4] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

    public void FloodFill(Vector2Int startPos)
    {
        Debug.Log("FloodFillStart");
        Stack<Vector2Int> openStack1 = new();
        Stack<Vector2Int> openStack2 = new();

        HashSet<Vector2Int> closeHash1 = new();
        HashSet<Vector2Int> closeHash2 = new();

        bool left = PixelManager.Instance.GetPixel(startPos.x, startPos.y) == FillState.Past && PixelManager.Instance.GetPixel((startPos + Vector2Int.left).x, startPos.y) == FillState.Past;
        bool right = PixelManager.Instance.GetPixel(startPos.x, startPos.y) == FillState.Past && PixelManager.Instance.GetPixel((startPos + Vector2Int.right).x, startPos.y) == FillState.Past;

        if (left || right)
        {
            openStack1.Push(startPos + Vector2Int.up);
            closeHash1.Add(startPos + Vector2Int.up);//Close에 처음부터 넣을 것
            openStack2.Push(startPos + Vector2Int.down);
            closeHash2.Add(startPos + Vector2Int.down);//Close에 처음부터 넣을 것
        }
        else
        {
            openStack1.Push(startPos + Vector2Int.left);
            closeHash1.Add(startPos + Vector2Int.left);//Close에 처음부터 넣을 것
            openStack2.Push(startPos + Vector2Int.right);
            closeHash2.Add(startPos + Vector2Int.right);//Close에 처음부터 넣을 것
        }

        while (openStack1.Count != 0 && openStack2.Count != 0)
        {
            Vector2Int checkPos = openStack1.Pop();
            Vector2Int checkPos2 = openStack2.Pop();

            for (int i = 0; i < directions.Length; i++)
            {
                Vector2Int operationPos = checkPos + directions[i];

                if (IsVaild(closeHash1, operationPos))
                {
                    openStack1.Push(operationPos);
                    closeHash1.Add(operationPos);
                }

                operationPos = checkPos2 + directions[i];

                if (IsVaild(closeHash2, operationPos))
                {
                    openStack2.Push(operationPos);
                    closeHash2.Add(operationPos);
                }
            }
        }

        if (closeHash1.Count > closeHash2.Count)
        {
            SetChangePixel(closeHash2);
        }
        else
        {
            SetChangePixel(closeHash1);
        }
        PixelManager.Instance.SetApply();

    }

    private bool IsVaild(HashSet<Vector2Int> closeHash, Vector2Int operationPos)
    {
        if (PixelManager.Instance.nodeArray[operationPos.x, operationPos.y] == FillState.None && !closeHash.Contains(operationPos))
        {
            return true;
        }
        return false;
    }

    private void SetChangePixel(HashSet<Vector2Int> closeHash)
    {
        foreach (var close in closeHash)
        {
            PixelManager.Instance.SetColor(close.x, close.y, PixelManager.Instance.GetColor(close.x, close.y, TextureType.After));
            PixelManager.Instance.SetPixel(close.x, close.y, FillState.Fill);
            Debug.Log(close);
        }
    }

    #region 주석
    //Stack<Vector2Int> openStack1 = new();
    //Stack<Vector2Int> openStack2 = new();

    //HashSet<Vector2Int> closeHash1 = new();
    //HashSet<Vector2Int> closeHash2 = new();

    //StartCoroutine(FloodFillS(startPos));

    //IEnumerator FloodFillS(Vector2Int startPos)
    //{
    //    Vector2Int operationPosR = startPos + Vector2Int.right;
    //    Vector2Int operationPosL = startPos + Vector2Int.left;

    //    //Debug.Log($"operL{operationPosL}");
    //    //Debug.Log($"operR{operationPosR}");

    //    bool left = PixelManager.Instance.GetPixel(startPos.x, startPos.y) == FillState.Past && PixelManager.Instance.GetPixel(operationPosL.x, operationPosL.y) == FillState.Past;
    //    bool right = PixelManager.Instance.GetPixel(startPos.x, startPos.y) == FillState.Past && PixelManager.Instance.GetPixel(operationPosR.x, operationPosR.y) == FillState.Past;

    //    if (left || right)
    //    {
    //        openStack1.Push(startPos + Vector2Int.up);
    //        openStack2.Push(startPos + Vector2Int.down);
    //        //Debug.Log($"startPos+Up{startPos + Vector2Int.up}");
    //        //Debug.Log($"startPos+Down{startPos + Vector2Int.down}");
    //        yield return null;
    //    }
    //    else
    //    {
    //        openStack1.Push(startPos + Vector2Int.left);
    //        openStack2.Push(startPos + Vector2Int.right);
    //        //Debug.Log($"startPos+Left{startPos + Vector2Int.left}");
    //        //Debug.Log($"startPos+Right{startPos + Vector2Int.right}");
    //        yield return null;
    //    }
    //    float time = 0;
    //    while (openStack1.Count != 0 && openStack2.Count != 0)
    //    {
    //        time += Time.deltaTime;
    //        if (time > 100)
    //        {
    //            break;
    //        }
    //        Vector2Int checkPos1 = openStack1.Pop();
    //        Vector2Int checkPos2 = openStack2.Pop();

    //        for (int i = 0; i < 4; i++)
    //        {
    //            Vector2Int operPos = checkPos1 + directions[i];
    //            if (IsVaild(checkPos1, closeHash1))
    //            {
    //                Debug.Log(1);
    //                AddPixel(operPos, openStack1, closeHash1);
    //            }
    //            operPos = checkPos2 + directions[i];
    //            if (IsVaild(checkPos2, closeHash2))
    //            {
    //                Debug.Log(2);
    //                AddPixel(operPos, openStack2, closeHash2);
    //            }
    //        }
    //    }
    //    Debug.Log(closeHash1.Count);
    //    Debug.Log(closeHash2.Count);
    //    yield return null;
    //    DestroyPixel(closeHash1, closeHash2);
    //    Debug.Log("Stop");
    //}

    //void AddPixel(Vector2Int operPos, Stack<Vector2Int> openStack, HashSet<Vector2Int> closeHash)
    //{
    //    openStack.Push(operPos);
    //    closeHash.Add(operPos);
    //}
    #endregion
}