﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraRenderer
{
	ScriptableRenderContext context;

	Camera camera;

	readonly static string bufferName = "Render Camera";

	CommandBuffer buffer = new CommandBuffer
	{
		name = bufferName
	};

	CullingResults cullingResults;

	static ShaderTagId unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit");

	public void Render(ScriptableRenderContext context, Camera camera)
	{
		this.context = context;
		this.camera = camera;

		if (! Cull())
		{
			return;
		}

		Setup();
		DrawVisiableGeometry();
		Submit();
	}

	void DrawVisiableGeometry()
	{
		var sortingSettings = new SortingSettings(camera)
		{
			criteria = SortingCriteria.CommonOpaque
		};
		var drawingSettings = new DrawingSettings(unlitShaderTagId, sortingSettings);
		var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);

		context.DrawRenderers(
			cullingResults, ref drawingSettings, ref filteringSettings
		);
		context.DrawSkybox(camera);
		sortingSettings.criteria = SortingCriteria.CommonTransparent;
		drawingSettings.sortingSettings = sortingSettings;
		filteringSettings.renderQueueRange = RenderQueueRange.transparent;

		context.DrawRenderers(
			cullingResults, ref drawingSettings, ref filteringSettings
		);
	}

	void Setup() {
		context.SetupCameraProperties(camera);
		buffer.ClearRenderTarget(true, true, Color.clear);
		buffer.BeginSample(bufferName);
		ExecuteBuffer();
	}

	bool Cull()
	{
		if (camera.TryGetCullingParameters(out ScriptableCullingParameters p))
		{
			cullingResults = context.Cull(ref p);
			return true;
		}
		return false;
	}

	void Submit() {
		buffer.EndSample(bufferName);
		ExecuteBuffer();
		context.Submit();
	}


	void ExecuteBuffer()
	{
		context.ExecuteCommandBuffer(buffer);
		buffer.Clear();
	}
}
