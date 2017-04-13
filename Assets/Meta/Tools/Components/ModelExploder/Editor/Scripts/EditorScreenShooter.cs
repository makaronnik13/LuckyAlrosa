using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Meta.Tools.Editor
{
	public class EditorScreenShooter
	{
		private static int _additionalObjectsLayer = 29;
		private static int _targetsLayer = 28;
		private static int _isolatedNeighboursLayer = 27;

		private GameObject torus;
		private Camera camera;

		public static Color _BackgroundColor = new Color(0.87058f, 0.87058f, 0.87058f);
		//public static Color _BackgroundColor = new Color(0.8941f, 0.0f, 0.0f);

		public EditorCameraBestViewFinder cameraViewFinder = new EditorCameraBestViewFinder();
		private Material _effectMaterial;
		public Color GridColor = new Color(0.5f, 1f, 0.5f, 1f);
		public Color GridBackgroundColor = new Color(0f, 0f, 0f, 1f);
		public ProjectionAplliedEffect ProjectionAppliedEffect = ProjectionAplliedEffect.WireFrame;
		public Color AppliedMaterialColor = new Color(0.68235f, 0.66474f, 0.66474f, 0.45882f);
		public float GridDimension = 16f;

		public static Vector2 Resolution = new Vector2(900, 600);

		public EditorScreenShooter()
		{
			_effectMaterial = (Material)Resources.Load("Materials/WireframeMaterial");
		}

		/*
         * Tking a picture consists bunch of steps.
         * 
         * 1. Preparing
         *  a. When we take a picture we may want to isolate target object first.
         *  b. Then we might want to apply some effect on it, so it will have some predictable look.
         *  c. At lasts we'll probably want to place some additional objects, that'll make picture complete. Most probably, they would be rendered on top of all other objects.
         * 
         * 2. Taking Picture
         *  We must be able to configure any feature of a camera. Currently they are:
         *   a. Position
         *   b. Rotation
         *   c. Projection Mode
         *   d. Field of View
         *   e. Background Color
         *   f. Clear Flags
         * 
         * 3. Restoring scene
         *  After all is done we must remove all what we've done, including Camera, materials, objects
         *  
         *  1.a. In order to isolate target object from another, we move it on a specific Layer. There must be no other objects on that layer, so we conserve all other objects 
         *  from that layer in the array, and temporarily move them to another layer.
         */

		public CameraAxisSide TargetCameraAxisSide = CameraAxisSide.MinusZ;

		public Texture TakeScreenShoot(ModelExploder target, Color effectColor, bool needGrid = true, Action<Camera> additionalActions = null,
			bool orthographic = true, float fieldOfView = 60f, CameraClearFlags clearFlags = CameraClearFlags.Color)
		{
			Texture result;
			isolate(target);
			applyEffect(target.gameObject, effectColor);

			Quaternion lastRotation = target.transform.rotation;
			target.transform.rotation = Quaternion.Euler(Vector3.zero);
			target.RecalculateAABB();

			result = takePicture(target.AABB, TargetCameraAxisSide, target.transform ,needGrid, orthographic, fieldOfView, clearFlags, additionalActions);

			target.transform.rotation = lastRotation;

			clearAllAdditions();
			removeAppliedEffects();
			returnIsolatedToTheirNests();

			return result;
		}

		#region 1.a. Isolating

		private GameObject[] _isolatedsNeighbours;
		private GameObject[] _additionsNeighbours;
		private List<int> intiialTargetsLayers;
		private List<GameObject> targetsGameObjects;

		private void isolate(ModelExploder target)
		{
			_isolatedsNeighbours = FindGameObjectsWithLayer(_targetsLayer);
			_additionsNeighbours = FindGameObjectsWithLayer(_additionalObjectsLayer);

			for (int i = 0; i < _isolatedsNeighbours.Length; i++)
			{
				_isolatedsNeighbours[i].layer = _isolatedNeighboursLayer;
			}
			for (int i = 0; i < _additionsNeighbours.Length; i++)
			{
				_additionsNeighbours[i].layer = _isolatedNeighboursLayer;
			}

			intiialTargetsLayers = new List<int>();
			targetsGameObjects = new List<GameObject>();
			for (int i = 0; i < target.Parts.Count; i++)
			{
				GameObject[] gos = target.GetGameObjectsFromPart(target.Parts[i]);
				if (gos != null)
				{
					for (int j = 0; j < gos.Length; j++)
					{
						intiialTargetsLayers.Add(gos[j].layer);
						targetsGameObjects.Add(gos[j]);
						gos[j].layer = _targetsLayer;
					}
				}
			}
		}

		private void returnIsolatedToTheirNests()
		{
			for (int i = 0; i < targetsGameObjects.Count; i++)
			{
				targetsGameObjects[i].layer = intiialTargetsLayers[i];
			}

			for (int i = 0; i < _isolatedsNeighbours.Length; i++)
			{
				_isolatedsNeighbours[i].layer = _targetsLayer;
			}
			for (int i = 0; i < _additionsNeighbours.Length; i++)
			{
				_additionsNeighbours[i].layer = _additionalObjectsLayer;
			}

			_isolatedsNeighbours = null;
			_additionsNeighbours = null;

			targetsGameObjects.Clear();
			targetsGameObjects = null;
			intiialTargetsLayers.Clear();
			intiialTargetsLayers = null;
		}

		private GameObject[] FindGameObjectsWithLayer(int layer)
		{
			GameObject[] goArray = UnityEngine.Object.FindObjectsOfType(typeof(GameObject)) as GameObject[];
			List<GameObject> goList = new List<GameObject>();
			for (int i = 0; i < goArray.Length; i++)
			{
				if (goArray[i].layer == layer)
				{
					goList.Add(goArray[i]);
				}
			}
			return goList.ToArray();
		}

		#endregion 1.a. Isolating

		#region 1.b. Applying effects

		GameObject[] _appliedEffectGameObjects;
		Material[][] _appliedEffectSharedMaterials;
		private void applyEffect(GameObject target, Color color)
		{
			_effectMaterial.color = color;
			_appliedEffectGameObjects = Utilities.GetMeshRenderedGameObjects(target);
			_appliedEffectSharedMaterials = new Material[_appliedEffectGameObjects.Length][];

			switch(ProjectionAppliedEffect)
			{
			case ProjectionAplliedEffect.WireFrame:
				_effectMaterial = (Material)UnityEngine.Object.Instantiate(Resources.Load("Materials/WireframeMaterial"));
				break;
				/*case ProjectionAplliedEffect.SelectionDefault:
                    _effectMaterial = (Material)UnityEngine.Object.Instantiate(Resources.Load("InEditorSelectionMaterial"));
                    break;*/
			case ProjectionAplliedEffect.TransparentColor:
				_effectMaterial = (Material)UnityEngine.Object.Instantiate(Resources.Load("Materials/DefaultEditorScreenshotEffectMaterial"));
				break;
			}

			_effectMaterial.color = AppliedMaterialColor;

            List<GameObject> selectedGroup = new List<GameObject>();

            if (target.GetComponent<ModelExploder>().CurrentlySelectedPart!=-1) {
                selectedGroup = target.GetComponent<ModelExploder>().GetGameObjectsFromPart(target.GetComponent<ModelExploder>().Parts[target.GetComponent<ModelExploder>().CurrentlySelectedPart]).ToList();
            }
            for (int i = 0; i < _appliedEffectGameObjects.Length; i++)
			{
				_appliedEffectSharedMaterials[i] = _appliedEffectGameObjects[i].GetComponent<Renderer>().sharedMaterials;
				Material[] materials = new Material[_appliedEffectSharedMaterials[i].Length];

				int j = 0;
				for (; j < materials.Length; j++)
				{
                    if (!selectedGroup.Contains(_appliedEffectGameObjects[i]))
                    {
                        materials[j] = _effectMaterial;
                    }
                    else
                    {
                        materials[j] = Utilities.Material(Utilities.SceneViewSelectionType.Variant2);
                    }
				}

				_appliedEffectGameObjects[i].GetComponent<Renderer>().sharedMaterials = materials;
			}
		}

		private void removeAppliedEffects()
		{
			for (int i = 0; i < _appliedEffectGameObjects.Length; i++)
			{
				_appliedEffectGameObjects[i].GetComponent<Renderer>().sharedMaterials = _appliedEffectSharedMaterials[i];
				/*Material[] materials = _appliedEffectGameObjects[i].GetComponent<MeshRenderer>().sharedMaterials;
                Material[] newMaterials = new Material[materials.Length - 1];
                int j = 0;
                for (; j < newMaterials.Length; j++)
                {
                    newMaterials[j] = materials[j];
                }
                _appliedEffectGameObjects[i].GetComponent<MeshRenderer>().sharedMaterials = newMaterials;*/
			}
		}

		#endregion 1.b. Applying effects

		#region 1.c. Placing Additions

		private List<GameObject> additionObjects = new List<GameObject>();
		public void PlaceTorus(Vector3 position, Quaternion rotation, Vector3 scale, Color color)
		{
			torus = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/EditorTorus"));
			//Debug.Log (torus);
			torus.transform.position = position;
			torus.transform.rotation = rotation;
			torus.transform.localScale = scale;
			torus.layer = _additionalObjectsLayer;
			torus.GetComponent<MeshRenderer>().sharedMaterial.color = color;
			additionObjects.Add(torus);
		}

        public void PlaceRomb(Vector3 position, Quaternion rotation, Vector3 scale, Color color)
        {
            torus = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/EditorRomb"));
            //Debug.Log (torus);
            torus.transform.position = position;
            torus.transform.rotation = rotation;
            torus.transform.localScale = scale;
            torus.layer = _additionalObjectsLayer;
            torus.GetComponent<MeshRenderer>().sharedMaterial.color = color;
            additionObjects.Add(torus);
        }

        private void clearAllAdditions()
		{
			for (int i = 0; i < additionObjects.Count; i++)
			{
				UnityEngine.Object.DestroyImmediate(additionObjects[i]);
			}

			additionObjects.Clear();
		}

		#endregion 1.c. Placing Additions

		#region 2. Taking Picture

		private Texture2D takePicture(Bounds targetBounds, CameraAxisSide targetAxisSide, Transform target ,bool needGrid = true,
			bool orthographic = true, float fieldOfView = 60f, CameraClearFlags clearFlags = CameraClearFlags.Color, Action<Camera> additionalActions = null)
		{
			
			//Debug.Log (torus);
			//Debug.Log ("take Pic");
			//rotate torus to camere
			if(torus){
			switch (targetAxisSide)
			{
			case CameraAxisSide.MinusX:
				torus.transform.rotation = Quaternion.Euler(Vector3.forward);
				break;
			case CameraAxisSide.PlusX:
					torus.transform.rotation = Quaternion.Euler(Vector3.back);
				break;
			case CameraAxisSide.MinusY:
				torus.transform.rotation = Quaternion.Euler(Vector3.up);
				break;
			case CameraAxisSide.PlusY:
				torus.transform.rotation = Quaternion.Euler(Vector3.down);
				break;
			case CameraAxisSide.MinusZ:
					torus.transform.rotation = Quaternion.Euler(Vector3.left);
				break;
			case CameraAxisSide.PlusZ:
					torus.transform.rotation = Quaternion.Euler(Vector3.right);
				break;
				}
			}

			GameObject cameraObject = new GameObject("ScreenshootCamera");

			camera = cameraObject.AddComponent<Camera>();

			cameraViewFinder.PlaceFittingAABB(camera, targetBounds, Resolution, targetAxisSide);
            cameraViewFinder.PlaceFittingAABB(Camera.main, targetBounds, Resolution, targetAxisSide);

			camera.orthographic = orthographic;
			camera.fieldOfView = fieldOfView;
			camera.clearFlags = clearFlags;
			camera.backgroundColor = _BackgroundColor;
			camera.cullingMask = 1 << _additionalObjectsLayer | 1 << _targetsLayer;

            Camera.main.orthographic = orthographic;
            Camera.main.fieldOfView = fieldOfView;
            Camera.main.clearFlags = clearFlags;
            Camera.main.backgroundColor = _BackgroundColor;
           

            GameObject grid = null;
			if (needGrid)
			{
				grid = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/EditorGrid"));
				grid.transform.position = targetBounds.center - (camera.transform.position - targetBounds.center);
				float gridDimesion = (Resolution.x / Resolution.y) * camera.orthographicSize * 2f;
				grid.transform.localScale = new Vector3(gridDimesion, gridDimesion, 1f);
				grid.transform.LookAt(targetBounds.center);
				grid.transform.Rotate(new Vector3(0f, 180f, 0f));
				grid.layer = _additionalObjectsLayer;

                GridDimension = (16f / 300f) * Resolution.x;
                
                float thicknessOfLines = (0.0029f / (300f / 256)) * (Resolution.x / (GridDimension * GridDimension));

                //grid.GetComponent<MeshRenderer>().sharedMaterial.SetColor("_LinesColor", GridColor);
				grid.GetComponent<MeshRenderer>().sharedMaterial.SetColor("_BackgroundColor", GridBackgroundColor);
				grid.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_NumOfLinesHorizontal", GridDimension);
                grid.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_NumOfLinesVertical", GridDimension);
                grid.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_LinesThickness", thicknessOfLines);
            }

            if (additionalActions != null)
            {
                additionalActions.Invoke(camera);
            }

            RenderTexture rt = new RenderTexture((int)Resolution.x, (int)Resolution.y, 24);
			rt.antiAliasing = 8;
			camera.targetTexture = rt;
			Texture2D screenShot = new Texture2D((int)Resolution.x, (int)Resolution.y, TextureFormat.RGB24, false);
			screenShot.anisoLevel = 9;
			screenShot.filterMode = FilterMode.Trilinear;
			camera.Render();
			RenderTexture.active = rt;
			screenShot.ReadPixels(new Rect(0, 0, (int)Resolution.x, (int)Resolution.y), 0, 0);
			screenShot.Apply();
			camera.targetTexture = null;
			RenderTexture.active = null; // JC: added to avoid errors


			if (needGrid)
			{
				UnityEngine.Object.DestroyImmediate(grid);
			}

			UnityEngine.Object.DestroyImmediate(rt);
			UnityEngine.Object.DestroyImmediate(camera);
			UnityEngine.Object.DestroyImmediate(cameraObject);

			return screenShot;
		}

		#endregion 2. Taking Picture
	}
}
