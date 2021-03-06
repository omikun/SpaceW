﻿#region License
// Procedural planet generator.
// 
// Copyright (C) 2015-2017 Denis Ovchinnikov [zameran] 
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions
// are met:
// 1. Redistributions of source code must retain the above copyright
//    notice, this list of conditions and the following disclaimer.
// 2. Redistributions in binary form must reproduce the above copyright
//    notice, this list of conditions and the following disclaimer in the
//    documentation and/or other materials provided with the distribution.
// 3. Neither the name of the copyright holders nor the names of its
//    contributors may be used to endorse or promote products derived from
//    this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
// INTERRUPTION)HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
// 
// Creation Date: 2016.05.18
// Creation Time: 16:45
// Creator: zameran
#endregion

using System;

using UnityEngine;

/// <summary>
/// Class - extensions holder for a <see cref="Material"/>.
/// </summary>
[Obsolete("Please, use direct methods. Material.Set[]/Shader.Set[]/MaterialPropertyBlock.Set[].")]
public static class MaterialUniformsHelper
{
    public static void SetProperty(this Material mat, string name, ComputeBuffer buffer)
    {
        if (mat.HasProperty(name))
        {
            mat.SetBuffer(name, buffer);
        }
    }

    public static void SetProperty(this Material mat, string name, Color color)
    {
        if (mat.HasProperty(name))
        {
            mat.SetColor(name, color);
        }
    }

    public static void SetProperty(this Material mat, string name, float value)
    {
        if (mat.HasProperty(name))
        {
            mat.SetFloat(name, value);
        }
    }

    public static void SetProperty(this Material mat, string name, int value)
    {
        if (mat.HasProperty(name))
        {
            mat.SetInt(name, value);
        }
    }

    public static void SetProperty(this Material mat, string name, Matrix4x4 value)
    {
        if (mat.HasProperty(name))
        {
            mat.SetMatrix(name, value);
        }
    }

    public static void SetProperty(this Material mat, string name, Vector4 value)
    {
        if (mat.HasProperty(name))
        {
            mat.SetVector(name, value);
        }
    }

    public static void SetProperty(this Material mat, string name, Texture value)
    {
        if (mat.HasProperty(name))
        {
            mat.SetTexture(name, value);
        }
    }
}