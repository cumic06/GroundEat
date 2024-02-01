using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TextureType
{
    Before,
    After
}

public class PixelManager : MonoSingleton<PixelManager>
{
    #region º¯¼ö
    [SerializeField] private SpriteRenderer backGroundSpriteRenderer;

    [SerializeField] private Texture2D beforeTexture;
    [SerializeField] private Texture2D afterTexture;
    private Texture2D cloneTexture;

    public FillState[,] nodeArray;
    #endregion

    private void Start()
    {
        nodeArray = new FillState[GetResolution().x, GetResolution().y];
        //Debug.Log(GetResolution().x);
        //Debug.Log(GetResolution().y);
        CloneBackGround();
        CreateWall();
    }

    private void CloneBackGround()
    {
        cloneTexture = Instantiate(beforeTexture);
        if (cloneTexture.isReadable)
        {
            Rect rect = new(0, 0, cloneTexture.width, cloneTexture.height);
            backGroundSpriteRenderer.sprite = Sprite.Create(cloneTexture, rect, new Vector2(0.5f, 0.5f));
        }
    }

    private void CreateWall()
    {
        for (int x = 0; x < GetResolution().x - 1; x++)
        {
            SetPixel(x, 0, FillState.Wall);
            SetPixel(x, GetResolution().y - 1, FillState.Wall);
        }
        for (int y = 0; y < GetResolution().y - 1; y++)
        {
            SetPixel(0, y, FillState.Wall);
            SetPixel(GetResolution().x - 1, y, FillState.Wall);
        }
    }

    #region GetSetPixelColor
    public void SetPixel(int x, int y, FillState state)
    {
        nodeArray[x, y] = state;
    }

    public FillState GetPixel(int x, int y)
    {
        return nodeArray[x, y];
    }

    public Color GetColor(int x, int y, TextureType textureType)
    {
        if (textureType == TextureType.Before)
        {
            return beforeTexture.GetPixel(x, y);
        }
        return afterTexture.GetPixel(x, y);
    }

    public void SetColor(int x, int y, Color color)
    {
        cloneTexture.SetPixel(x, y, color);
    }

    public void SetApply()
    {
        cloneTexture.Apply();
    }

    public void ResetColor(int x, int y, TextureType resetTexture)
    {
        Color setColor = GetColor(x, y, resetTexture);
        SetColor(x, y, setColor);
    }
    #endregion

    public Vector2Int GetResolution()
    {
        int width = Camera.main.pixelWidth;
        int height = Camera.main.pixelHeight;
        Vector2Int resolution = new(width, height);
        return resolution;
    }
}
