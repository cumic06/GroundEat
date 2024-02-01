using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveMent : MonoSingleton<PlayerMoveMent>
{
    #region º¯¼ö
    [SerializeField] private Vector2Int pixelPos = new();

    private Vector2Int[] movePosArray = new Vector2Int[4] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
    private List<Vector2Int> moveVecList = new();
    [SerializeField] private List<Vector2Int> pastMoveLineList = new();

    private float moveX;
    private float moveY;

    private bool isBack;
    #endregion

    private void Start()
    {
        //pixelPos = Vector2Int.one;
        moveX = Camera.main.pixelWidth;
        moveY = Camera.main.pixelHeight;
        StartCoroutine(nameof(BackMove));
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if (!isBack)
        {
            for (int i = 0; i < InputSystem.Instance.moveKeyArray.Length; i++)
            {
                if (Input.GetKey(InputSystem.Instance.moveKeyArray[i]))
                {
                    if (!moveVecList.Contains(movePosArray[i]))
                    {
                        moveVecList.Add(movePosArray[i]);
                    }
                }
                else
                {
                    moveVecList.Remove(movePosArray[i]);
                }
            }

            if (moveVecList.Count > 0)
            {
                Debug.Log(PixelManager.Instance.nodeArray[pixelPos.x, pixelPos.y]);
                if (pixelPos.x + moveVecList[^1].x >= moveX || pixelPos.y + moveVecList[^1].y >= moveY || pixelPos.x + moveVecList[^1].x <= -1 || pixelPos.y + moveVecList[^1].y <= -1) return;

                FillState currentPixel = PixelManager.Instance.GetPixel(pixelPos.x, pixelPos.y);
                FillState nextPixel = PixelManager.Instance.GetPixel(pixelPos.x + moveVecList[^1].x, pixelPos.y + moveVecList[^1].y);

                pastMoveLineList.Add(pixelPos);
                PixelManager.Instance.SetPixel(pixelPos.x, pixelPos.y, FillState.Past);
                if (nextPixel != FillState.Fill && nextPixel != FillState.Past)
                {
                    pixelPos += moveVecList[^1];
                    PixelManager.Instance.SetColor(pixelPos.x, pixelPos.y, Color.red);
                }

                if (currentPixel == FillState.None)
                {
                    if (nextPixel == FillState.Wall || nextPixel == FillState.Fill)
                    {
                        FloodFillSystem.Instance.FloodFill(pastMoveLineList[^1]);
                        for (int i = 0; i < pastMoveLineList.Count; i++)
                        {
                            PixelManager.Instance.SetColor(pastMoveLineList[i].x, pastMoveLineList[i].y, Color.black);
                        }
                        RemoveAllPastMoveList();
                    }
                }
            }
            PixelManager.Instance.SetApply();
        }
        WorldPos2PixelPos();
    }

    private void WorldPos2PixelPos()
    {
        Vector3 pixelVec = Camera.main.ViewportToWorldPoint(new Vector3(pixelPos.x / moveX, pixelPos.y / moveY, 10));
        transform.position = pixelVec;
    }

    private IEnumerator BackMove()
    {
        while (true)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                isBack = true;

                PixelManager.Instance.ResetColor(pixelPos.x, pixelPos.y, TextureType.Before);

                if (pastMoveLineList.Count > 0)
                {
                    pixelPos = pastMoveLineList[^1];
                    pastMoveLineList.Remove(pastMoveLineList[^1]);
                }
                PixelManager.Instance.SetPixel(pixelPos.x, pixelPos.y, FillState.None);
                PixelManager.Instance.SetApply();
                yield return null;
            }
            else
            {
                isBack = false;
                yield return null;
            }
            yield return null;
        }
    }

    public void RemoveAllPastMoveList()
    {
        pastMoveLineList.Clear();
    }
}
