using UnityEngine;

namespace Meta.Tools.Editor
{
	public class EditorCameraBestViewFinder
	{
		public float DistancePaddingMultiplier = 1f;
		public float MinCameraPaddingMultiplier = 3f;

		public void PlaceFittingAABB(Camera targetCamera, Bounds aabb, Vector2 dimensions, CameraAxisSide targetCameraAxisSide)
		{
			//Debug.Log("aabb.center = " + aabb.center);

			//DebugFeaturesExtensions.DrawCube(aabb.center, aabb.extents);

			Vector3 targetCameraPosition = aabb.center;
			Vector2 extents2D = Vector2.zero;
			switch (targetCameraAxisSide)
			{
			case CameraAxisSide.MinusX:
				targetCameraPosition.x -= targetCamera.nearClipPlane + aabb.extents.x + DistancePaddingMultiplier * aabb.extents.x;
				extents2D.x = aabb.extents.z;
				extents2D.y = aabb.extents.y;
				break;
			case CameraAxisSide.PlusX:
				targetCameraPosition.x += targetCamera.nearClipPlane + aabb.extents.x  + DistancePaddingMultiplier * aabb.extents.x;
				extents2D.x = aabb.extents.z;
				extents2D.y = aabb.extents.y;
				break;
			case CameraAxisSide.MinusY:
				targetCameraPosition.y -= targetCamera.nearClipPlane + aabb.extents.y + DistancePaddingMultiplier * aabb.extents.y;
				extents2D.x = aabb.extents.x;
				extents2D.y = aabb.extents.z;
				break;
			case CameraAxisSide.PlusY:
				targetCameraPosition.y += targetCamera.nearClipPlane + aabb.extents.y + DistancePaddingMultiplier * aabb.extents.y;
				extents2D.x = aabb.extents.x;
				extents2D.y = aabb.extents.z;
				break;
			case CameraAxisSide.MinusZ:
				targetCameraPosition.z -= targetCamera.nearClipPlane + aabb.extents.z + DistancePaddingMultiplier * aabb.extents.z;
				extents2D.x = aabb.extents.x;
				extents2D.y = aabb.extents.y;
				break;
			case CameraAxisSide.PlusZ:
				targetCameraPosition.z += targetCamera.nearClipPlane + aabb.extents.z + DistancePaddingMultiplier * aabb.extents.z;
				extents2D.x = aabb.extents.x;
				extents2D.y = aabb.extents.y;
				break;
			}

			targetCamera.transform.position = targetCameraPosition;
			targetCamera.transform.LookAt(aabb.center);

			float orthographicSize = 0f;
			if (extents2D.x / extents2D.y > dimensions.x / dimensions.y)
			{
				orthographicSize = extents2D.x * (dimensions.y / dimensions.x);
			}
			else
			{
				orthographicSize = extents2D.y;
			}

			targetCamera.orthographicSize = orthographicSize * (1f + MinCameraPaddingMultiplier);
		}
	}
}

