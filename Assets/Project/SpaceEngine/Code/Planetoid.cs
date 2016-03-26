﻿using UnityEngine;

using System.Collections.Generic;

public class Planetoid : MonoBehaviour
{
	public Atmosphere Atmosphere;

	public bool DrawWireframe = false;
	public bool DrawNormals = false;

	public bool Working = false;

	public Transform LODTarget = null;

	public float PlanetRadius = 1024;

	public bool DebugEnabled = false;

	public List<Quad> MainQuads = new List<Quad>();
	public List<Quad> Quads = new List<Quad>();

	public Shader ColorShader;
	public ComputeShader CoreShader;

	public int RenderQueue = 2000;

	public int DispatchSkipFramesCount = 8;

	public int LODDistanceMultiplier = 1;
	public int LODMaxLevel = 8;
	public int[] LODDistances = new int[11] { 2048, 1024, 512, 256, 128, 64, 32, 16, 8, 4, 2 };

	public Mesh TopPrototypeMesh;
	public Mesh BottomPrototypeMesh;
	public Mesh LeftPrototypeMesh;
	public Mesh RightPrototypeMesh;
	public Mesh FrontPrototypeMesh;
	public Mesh BackPrototypeMesh;

	public GameObject QuadsRoot = null;
	public QuadStorage Cache = null;
	public NoiseParametersSetter NPS = null;

	public bool UseUnityCulling = true;
	public bool UseLOD = true;
	public bool RenderPerUpdate = false;
	public bool OneSplittingQuad = true;
	public bool ExternalRendering = false;

	public float TerrainMaxHeight = 64.0f;

	public Vector3 Origin = Vector3.zero;
	public Quaternion OriginRotation = Quaternion.identity;

	public QuadDistanceToClosestCornerComparer qdtccc;

	public class QuadDistanceToClosestCornerComparer : IComparer<Quad>
	{
		public int Compare(Quad x, Quad y)
		{
			if (x.DistanceToClosestCorner > y.DistanceToClosestCorner)
				return 1;
			if (x.DistanceToClosestCorner < y.DistanceToClosestCorner)
				return -1;
			else
				return 0;
		}
	}

	private void Awake()
	{
		Origin = transform.position;
		OriginRotation = QuadsRoot.transform.rotation;

		if (Atmosphere != null) Atmosphere.Origin = Origin;
	}

	private void Start()
	{
		ThreadScheduler.Initialize();

		if (Cache == null)
			if (this.gameObject.GetComponentInChildren<QuadStorage>() != null)
				Cache = this.gameObject.GetComponentInChildren<QuadStorage>();

		if (NPS != null)
			NPS.LoadAndInit();

		foreach (Quad q in MainQuads)
		{
			Atmosphere.InitUniforms(q.QuadMaterial);
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F1))
		{
			DrawWireframe = !DrawWireframe;
		}

		if (Input.GetKeyDown(KeyCode.F2))
		{
			DrawNormals = !DrawNormals;
		}

		CheckCutoff();

		Origin = transform.position;
		OriginRotation = QuadsRoot.transform.rotation;

		if (Atmosphere != null)
		{
			Atmosphere.Origin = Origin;

			if (!DrawWireframe)
			{
				Atmosphere.Sun.UpdateNode();
				Atmosphere.UpdateNode();
				Atmosphere.Render(false);
			}
		}

		if (ExternalRendering && RenderPerUpdate)
		{
			Render();
		}
	}

	private void OnRenderObject()
	{
		if (ExternalRendering && !RenderPerUpdate)
		{
			Render();
		}
	}

	public void Render()
	{
		for (int i = 0; i < Quads.Count; i++)
		{
			Quads[i].Render();
		}
	}

	[ContextMenu("DestroyQuads")]
	public void DestroyQuads()
	{
		for (int i = 0; i < Quads.Count; i++)
		{
			if(Quads[i] != null)
				DestroyImmediate(Quads[i].gameObject);
		}

		Quads.Clear();
		MainQuads.Clear();

		if (QuadsRoot != null) DestroyImmediate(QuadsRoot);

		if (TopPrototypeMesh != null) DestroyImmediate(TopPrototypeMesh);
		if (BottomPrototypeMesh != null) DestroyImmediate(BottomPrototypeMesh);
		if (LeftPrototypeMesh != null) DestroyImmediate(LeftPrototypeMesh);
		if (RightPrototypeMesh != null) DestroyImmediate(RightPrototypeMesh);
		if (FrontPrototypeMesh != null) DestroyImmediate(FrontPrototypeMesh);
		if (BackPrototypeMesh != null) DestroyImmediate(BackPrototypeMesh);
	}

	[ContextMenu("SetupMeshes")]
	public void SetupMeshes()
	{
		if (TopPrototypeMesh != null) DestroyImmediate(TopPrototypeMesh);
		if (BottomPrototypeMesh != null) DestroyImmediate(BottomPrototypeMesh);
		if (LeftPrototypeMesh != null) DestroyImmediate(LeftPrototypeMesh);
		if (RightPrototypeMesh != null) DestroyImmediate(RightPrototypeMesh);
		if (FrontPrototypeMesh != null) DestroyImmediate(FrontPrototypeMesh);
		if (BackPrototypeMesh != null) DestroyImmediate(BackPrototypeMesh);

		//bool uv_01 = true;
		bool invert = false;

		TopPrototypeMesh = MeshFactory.SetupQuadMesh(QS.nVertsPerEdge, MeshFactory.PLANE.XZ, !invert); //MeshFactory.MakePlane(QS.nVertsPerEdge, QS.nVertsPerEdge, MeshFactory.PLANE.XZ, uv_01, false, !invert);

		BottomPrototypeMesh = MeshFactory.SetupQuadMesh(QS.nVertsPerEdge, MeshFactory.PLANE.XZ, invert); //MeshFactory.MakePlane(QS.nVertsPerEdge, QS.nVertsPerEdge, MeshFactory.PLANE.XZ, uv_01, false, invert);

		LeftPrototypeMesh = MeshFactory.SetupQuadMesh(QS.nVertsPerEdge, MeshFactory.PLANE.YZ, !invert); //MeshFactory.MakePlane(QS.nVertsPerEdge, QS.nVertsPerEdge, MeshFactory.PLANE.YZ, uv_01, false, !invert);

		RightPrototypeMesh = MeshFactory.SetupQuadMesh(QS.nVertsPerEdge, MeshFactory.PLANE.YZ, invert); //MeshFactory.MakePlane(QS.nVertsPerEdge, QS.nVertsPerEdge, MeshFactory.PLANE.YZ, uv_01, false, invert);

		FrontPrototypeMesh = MeshFactory.SetupQuadMesh(QS.nVertsPerEdge, MeshFactory.PLANE.XY, !invert); //MeshFactory.MakePlane(QS.nVertsPerEdge, QS.nVertsPerEdge, MeshFactory.PLANE.XY, uv_01, false, !invert);

		BackPrototypeMesh = MeshFactory.SetupQuadMesh(QS.nVertsPerEdge, MeshFactory.PLANE.XY, invert); //MeshFactory.MakePlane(QS.nVertsPerEdge, QS.nVertsPerEdge, MeshFactory.PLANE.XY, uv_01, false, invert);
	}

	public int GetCulledQuadsCount()
	{
		int count = 0;

		for (int i = 0; i < Quads.Count; i++)
		{
			if (!Quads[i].Visible)
				count++;
		}

		return count;
	}

	public Quad GetMainQuad(QuadPosition position)
	{
		foreach (Quad q in MainQuads)
		{
			if (q.Position == position)
				return q;
		}

		return null;
	}

	public Mesh GetMesh(QuadPosition position)
	{
		Mesh temp = null;

		switch(position)
		{
			case QuadPosition.Top:
				temp = TopPrototypeMesh;
				break;
			case QuadPosition.Bottom:
				temp = BottomPrototypeMesh;
				break;
			case QuadPosition.Left:
				temp = LeftPrototypeMesh;
				break;
			case QuadPosition.Right:
				temp = RightPrototypeMesh;
				break;
			case QuadPosition.Front:
				temp = FrontPrototypeMesh;
				break;
			case QuadPosition.Back:
				temp = BackPrototypeMesh;
				break;
		}

		return temp;
	}

	[ContextMenu("SetupQuads")]
	public void SetupQuads()
	{
		if (Quads.Count > 0)
			return;

		SetupMeshes();
		SetupRoot();

		SetupMainQuad(QuadPosition.Top);
		SetupMainQuad(QuadPosition.Bottom);
		SetupMainQuad(QuadPosition.Left);
		SetupMainQuad(QuadPosition.Right);
		SetupMainQuad(QuadPosition.Front);
		SetupMainQuad(QuadPosition.Back);

		if (NPS != null)
			NPS.LoadAndInit();
	}

	[ContextMenu("ReSetupQuads")]
	public void ReSetupQuads()
	{
		DestroyQuads();
		SetupQuads();
	}

	public void SetupRoot()
	{
		if (QuadsRoot == null)
		{
			QuadsRoot = new GameObject("Quads_Root");
			QuadsRoot.transform.position = transform.position;
			QuadsRoot.transform.rotation = transform.rotation;
			QuadsRoot.transform.parent = transform;
		}
		else
		{
			return;
		}
	}

	public void SetupMainQuad(QuadPosition quadPosition)
	{
		GameObject go = new GameObject("Quad" + "_" + quadPosition.ToString());
		go.transform.position = Vector3.zero;
		go.transform.rotation = Quaternion.identity;
		go.transform.parent = QuadsRoot.transform;

		Mesh mesh = GetMesh(quadPosition);
		mesh.bounds = new Bounds(Vector3.zero, new Vector3(PlanetRadius * 2, PlanetRadius * 2, PlanetRadius * 2));

		Material material = new Material(ColorShader);
		material.name += "_" + quadPosition.ToString() + "(Instance)" + "_" + Random.Range(float.MinValue, float.MaxValue);

		Quad quadComponent = go.AddComponent<Quad>();
		quadComponent.CoreShader = CoreShader;
		quadComponent.Planetoid = this;
		quadComponent.QuadMesh = mesh;
		quadComponent.QuadMaterial = material;

		QuadGenerationConstants gc = QuadGenerationConstants.Init(TerrainMaxHeight);
		gc.planetRadius = PlanetRadius;

		gc.cubeFaceEastDirection = quadComponent.GetCubeFaceEastDirection(quadPosition);
		gc.cubeFaceNorthDirection = quadComponent.GetCubeFaceNorthDirection(quadPosition);
		gc.patchCubeCenter = quadComponent.GetPatchCubeCenter(quadPosition);
		
		quadComponent.Position = quadPosition;
		quadComponent.ID = QuadID.One;
		quadComponent.generationConstants = gc;
		quadComponent.Planetoid = this;
		quadComponent.SetupCorners(quadPosition);
		quadComponent.ShouldDraw = true;
		quadComponent.ReadyForDispatch = true;

		Quads.Add(quadComponent);
		MainQuads.Add(quadComponent);
	}

	public Quad SetupSubQuad(QuadPosition quadPosition)
	{
		GameObject go = new GameObject("Quad" + "_" + quadPosition.ToString());
		go.transform.position = Vector3.zero;
		go.transform.rotation = Quaternion.identity;

		Mesh mesh = GetMesh(quadPosition);
		mesh.bounds = new Bounds(Vector3.zero, new Vector3(PlanetRadius * 2, PlanetRadius * 2, PlanetRadius * 2));

		Material material = new Material(ColorShader);
		material.name += "_" + quadPosition.ToString() + "(Instance)" + "_" + Random.Range(float.MinValue, float.MaxValue);

		Quad quadComponent = go.AddComponent<Quad>();
		quadComponent.CoreShader = CoreShader;
		quadComponent.Planetoid = this;
		quadComponent.QuadMesh = mesh;
		quadComponent.QuadMaterial = material;
		quadComponent.SetupCorners(quadPosition);
		Atmosphere.InitUniforms(quadComponent.QuadMaterial);

		QuadGenerationConstants gc = QuadGenerationConstants.Init(TerrainMaxHeight);
		gc.planetRadius = PlanetRadius;

		quadComponent.Position = quadPosition;
		quadComponent.generationConstants = gc;
		quadComponent.Planetoid = this;
		quadComponent.ShouldDraw = false;

		if (qdtccc == null)
			qdtccc = new QuadDistanceToClosestCornerComparer();

		Quads.Add(quadComponent);
		Quads.Sort(qdtccc);

		return quadComponent;
	}

	public void CheckCutoff()
	{
		//Prevent fast jumping of lod distances check and working state.
		if(Vector3.Distance(LODTarget.transform.position, this.transform.position) > this.PlanetRadius * 2 + LODDistances[0])
		{
			for (int i = 0; i < Quads.Count; i++)
			{
				Quads[i].StopAllCoroutines();
			}

			this.Working = false;
		}
	}
}