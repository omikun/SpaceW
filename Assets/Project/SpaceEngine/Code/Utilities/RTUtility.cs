#region License
//
// Procedural planet renderer.
// Copyright (c) 2008-2011 INRIA
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// Proland is distributed under a dual-license scheme.
// You can obtain a specific license from Inria: proland-licensing@inria.fr.
//
// Authors: Justin Hawkins 2014.
// Modified by Denis Ovchinnikov 2015-2017
#endregion

using System;
using System.IO;

using UnityEngine;

using Object = UnityEngine.Object;

public static class RTUtility
{
    public static void Blit(RenderTexture src, RenderTexture des, bool clear = false)
    {
        var oldRT = RenderTexture.active;

        Graphics.SetRenderTarget(des);

        if (clear) GL.Clear(true, true, Color.clear);

        GL.PushMatrix();
        GL.LoadOrtho();

        GL.Begin(GL.QUADS);
        GL.TexCoord3(0.0f, 0.0f, 0.0f);
        GL.Vertex3(0.0f, 0.0f, 0.1f);
        GL.TexCoord3(1.0f, 0.0f, 0.0f);
        GL.Vertex3(1.0f, 0.0f, 0.1f);
        GL.TexCoord3(1.0f, 1.0f, 0.0f);
        GL.Vertex3(1.0f, 1.0f, 0.1f);
        GL.TexCoord3(0.0f, 1.0f, 0.0f);
        GL.Vertex3(0.0f, 1.0f, 0.1f);
        GL.End();

        GL.PopMatrix();

        RenderTexture.active = oldRT;
    }

    public static void Blit(RenderTexture src, RenderTexture des, Material mat, int pass = 0, bool clear = true)
    {
        mat.SetTexture("_MainTex", src);

        var oldRT = RenderTexture.active;

        Graphics.SetRenderTarget(des);

        if (clear) GL.Clear(true, true, Color.clear);

        GL.PushMatrix();
        GL.LoadOrtho();

        mat.SetPass(pass);

        GL.Begin(GL.QUADS);
        GL.TexCoord3(0.0f, 0.0f, 0.0f);
        GL.Vertex3(0.0f, 0.0f, 0.1f);
        GL.TexCoord3(1.0f, 0.0f, 0.0f);
        GL.Vertex3(1.0f, 0.0f, 0.1f);
        GL.TexCoord3(1.0f, 1.0f, 0.0f);
        GL.Vertex3(1.0f, 1.0f, 0.1f);
        GL.TexCoord3(0.0f, 1.0f, 0.0f);
        GL.Vertex3(0.0f, 1.0f, 0.1f);
        GL.End();

        GL.PopMatrix();

        RenderTexture.active = oldRT;
    }

    public static void Blit(RenderTexture src, RenderTexture des, Rect rect, bool clear = true)
    {
        //rect must have normalized coords, ie 0 - 1

        var oldRT = RenderTexture.active;

        Graphics.SetRenderTarget(des);

        if (clear) GL.Clear(true, true, Color.clear);

        GL.PushMatrix();
        GL.LoadOrtho();

        GL.Begin(GL.QUADS);
        GL.TexCoord2(rect.x, rect.y);
        GL.Vertex3(rect.x, rect.y, 0.1f);
        GL.TexCoord2(rect.x + rect.width, rect.y);
        GL.Vertex3(rect.x + rect.width, rect.y, 0.1f);
        GL.TexCoord2(rect.x + rect.width, rect.y + rect.height);
        GL.Vertex3(rect.x + rect.width, rect.y + rect.height, 0.1f);
        GL.TexCoord2(rect.x, rect.y + rect.height);
        GL.Vertex3(rect.x, rect.y + rect.height, 0.1f);
        GL.End();

        GL.PopMatrix();

        RenderTexture.active = oldRT;
    }

    public static void Blit(RenderTexture src, RenderTexture des, Material mat, Rect rect, int pass = 0, bool clear = true)
    {
        //rect must have normalized coords, ie 0 - 1

        mat.SetTexture("_MainTex", src);

        var oldRT = RenderTexture.active;

        Graphics.SetRenderTarget(des);

        if (clear) GL.Clear(true, true, Color.clear);

        GL.PushMatrix();
        GL.LoadOrtho();

        mat.SetPass(pass);

        GL.Begin(GL.QUADS);
        GL.TexCoord2(rect.x, rect.y);
        GL.Vertex3(rect.x, rect.y, 0.1f);
        GL.TexCoord2(rect.x + rect.width, rect.y);
        GL.Vertex3(rect.x + rect.width, rect.y, 0.1f);
        GL.TexCoord2(rect.x + rect.width, rect.y + rect.height);
        GL.Vertex3(rect.x + rect.width, rect.y + rect.height, 0.1f);
        GL.TexCoord2(rect.x, rect.y + rect.height);
        GL.Vertex3(rect.x, rect.y + rect.height, 0.1f);
        GL.End();

        GL.PopMatrix();

        RenderTexture.active = oldRT;
    }

    public static void MultiTargetBlit(RenderTexture[] des, Material mat, int pass = 0)
    {
        //RenderTexture oldRT = RenderTexture.active;

        var rb = new RenderBuffer[des.Length];

        for (int i = 0; i < des.Length; i++)
        {
            rb[i] = des[i].colorBuffer;
        }

        Graphics.SetRenderTarget(rb, des[0].depthBuffer);

        GL.Clear(true, true, Color.clear);

        GL.PushMatrix();
        GL.LoadOrtho();

        mat.SetPass(pass);

        GL.Begin(GL.QUADS);
        GL.TexCoord2(0.0f, 0.0f);
        GL.Vertex3(0.0f, 0.0f, 0.1f);
        GL.TexCoord2(1.0f, 0.0f);
        GL.Vertex3(1.0f, 0.0f, 0.1f);
        GL.TexCoord2(1.0f, 1.0f);
        GL.Vertex3(1.0f, 1.0f, 0.1f);
        GL.TexCoord2(0.0f, 1.0f);
        GL.Vertex3(0.0f, 1.0f, 0.1f);
        GL.End();

        GL.PopMatrix();

        //RenderTexture.active = oldRT;
    }

    public static void MultiTargetBlit(RenderBuffer[] des_rb, RenderBuffer des_db, Material mat, int pass = 0)
    {
        //RenderTexture oldRT = RenderTexture.active;

        Graphics.SetRenderTarget(des_rb, des_db);

        GL.Clear(true, true, Color.clear);

        GL.PushMatrix();
        GL.LoadOrtho();

        mat.SetPass(pass);

        GL.Begin(GL.QUADS);
        GL.TexCoord2(0.0f, 0.0f);
        GL.Vertex3(0.0f, 0.0f, 0.1f);
        GL.TexCoord2(1.0f, 0.0f);
        GL.Vertex3(1.0f, 0.0f, 0.1f);
        GL.TexCoord2(1.0f, 1.0f);
        GL.Vertex3(1.0f, 1.0f, 0.1f);
        GL.TexCoord2(0.0f, 1.0f);
        GL.Vertex3(0.0f, 1.0f, 0.1f);
        GL.End();

        GL.PopMatrix();

        //RenderTexture.active = oldRT;
    }

    public static void Swap(RenderTexture[] texs)
    {
        var temp = texs[0];

        texs[0] = texs[1];
        texs[1] = temp;
    }

    public static void Swap(ref RenderTexture tex0, ref RenderTexture tex1)
    {
        var temp = tex0;

        tex0 = tex1;
        tex1 = temp;
    }

    public static void ClearColor(RenderTexture tex)
    {
        Graphics.SetRenderTarget(tex);

        GL.Clear(false, true, Color.clear);
    }

    public static void ClearColor(RenderTexture[] texs)
    {
        for (int i = 0; i < texs.Length; i++)
        {
            Graphics.SetRenderTarget(texs[i]);
            GL.Clear(false, true, Color.clear);
        }
    }

    public static void SetToPoint(RenderTexture[] texs)
    {
        for (int i = 0; i < texs.Length; i++)
        {
            texs[i].filterMode = FilterMode.Point;
        }
    }

    public static void SetToBilinear(RenderTexture[] texs)
    {
        for (int i = 0; i < texs.Length; i++)
        {
            texs[i].filterMode = FilterMode.Bilinear;
        }
    }

    public static void Destroy(RenderTexture[] texs)
    {
        if (texs == null) return;

        for (int i = 0; i < texs.Length; i++)
        {
            Object.DestroyImmediate(texs[i]);
        }
    }

    public static void SaveAsRaw(int size, int channels, string fileName, string filePath, RenderTexture rtex, ComputeShader readDataComputeShader)
    {
        var buffer = new ComputeBuffer(size, sizeof(float) * channels);

        CBUtility.ReadFromRenderTexture(rtex, channels, buffer, readDataComputeShader);

        var data = new float[size * channels];

        buffer.GetData(data);

        var byteArray = new byte[size * 4 * channels];

        Buffer.BlockCopy(data, 0, byteArray, 0, byteArray.Length);
        File.WriteAllBytes(Application.dataPath + filePath + fileName + ".raw", byteArray);

        buffer.Release();
    }

    public static void SaveAs8bit(int width, int height, int channels, string fileName, string filePath, RenderTexture rtex, ComputeShader readDataComputeShader, float scale = 1.0f)
    {
        var buffer = new ComputeBuffer(width * height, sizeof(float) * channels);

        CBUtility.ReadFromRenderTexture(rtex, channels, buffer, readDataComputeShader);

        var data = new float[width * height * channels];

        buffer.GetData(data);

        var texture = new Texture2D(width, height);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var color = new Color(0, 0, 0, 1);

                color.r = data[(x + y * width) * channels + 0];

                if (channels > 1)
                    color.g = data[(x + y * width) * channels + 1];

                if (channels > 2)
                    color.b = data[(x + y * width) * channels + 2];

                texture.SetPixel(x, y, color * scale);
            }
        }

        texture.Apply();

        var bytes = texture.EncodeToPNG();

        File.WriteAllBytes(Application.dataPath + filePath + fileName + ".png", bytes);

        buffer.Release();
    }
}