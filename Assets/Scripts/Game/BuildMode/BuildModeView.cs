using System.Collections.Generic;
using Simulation.Data;
using UnityEngine;

namespace Game.BuildMode
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
	public class BuildModeView : MonoBehaviour
	{
		// Defines the amount of rays casted against the terrain per 1 unit.
		private const float RayCastResolution = 1;
		private const float RayLength = 0.5f;

		[SerializeField] private MeshRenderer _footprintRenderer;
		[SerializeField] private Vector2 _footprintSize = Vector2.one;

		// We want those to be serialized, but don't sow them in te inspector
		[SerializeField, HideInInspector] private BoxCollider _footprintCollider;
		[SerializeField, HideInInspector] private Collider[] _colliders;
		[SerializeField, HideInInspector] private MeshRenderer[] _renderers;

		private ushort _collisionCounter;
		private bool _wasLastPositionValid;
		private SimulationVector3 _lastPosition;
		private Vector3[] _basePoints;

		// A temporary list which gets re-used to avoid garbage
		private readonly List<Vector3> _tempPointsList = new List<Vector3>(9);

		private void Awake()
		{
			// Awake is also called in Editor mode since we use [ExecuteAlways] attribute
			if (Application.isPlaying) _footprintRenderer.enabled = false;
		}

		/// <summary>
		/// OnValidate is only called in the UnityEditor
		/// </summary>
		private void OnValidate()
		{
			// Don't execute the code below when we hit play
			if (Application.isPlaying) return;

			// Disable functionality to edit the transform in the inspector and set 
			// the scale of the transform according to the footprint size
			transform.hideFlags = HideFlags.NotEditable;
			transform.localScale = new Vector3(_footprintSize.x, _footprintSize.y, 1);

			_footprintCollider = GetComponent<BoxCollider>();
			_footprintCollider.isTrigger = true;

			var parentObject = transform.parent;
			_renderers = parentObject.GetComponentsInChildren<MeshRenderer>();
			_colliders = parentObject.GetComponentsInChildren<Collider>();
		}

		public void ShowPreview()
		{
			_footprintRenderer.enabled = true;
			ActivateOtherCollidersThanSelf(false);
		}

		public void ConfirmPlacement()
		{
			_wasLastPositionValid = true;
			_footprintRenderer.enabled = false;
			_basePoints = null;

			SetMaterialTint();
			ActivateOtherCollidersThanSelf(true);
			_tempPointsList.Clear();
			_tempPointsList.TrimExcess();
		}

		public void CheckPlacement()
		{
			_wasLastPositionValid = IsPlacementValid();
			SetMaterialTint();
		}

		public bool IsPlacementValid()
		{
			if (_collisionCounter > 0) return false;

			_basePoints = GetBaseBoundPoints(_footprintCollider);
			foreach (var point in _basePoints)
			{
				// Cast a ray from each calculated point downwards. If one ray misses the terrain, the placement is invalid
				if (!TryGetHit(out var hit, point, RayLength)) return false;
			}
			return true;
		}

		private void SetMaterialTint()
		{
			foreach (var meshRenderer in _renderers)
			{
				// We assume here that the default material was not tinted, so we can set the tint to white if the placement is valid
				// set the tint to red if the placement is invalid;
				meshRenderer.material.color = _wasLastPositionValid ? Color.white : Color.red;
			}
			_footprintRenderer.material.color = _wasLastPositionValid ? Color.white : Color.red;
		}

		private void OnDrawGizmos()
		{
			if(_footprintCollider == null || _basePoints == null) return;
			// store the current gizmo color
			var color = Gizmos.color;
			var offset = Vector3.up * RayLength * 0.5f;
			foreach (var point in _basePoints)
			{
				// Do the raycasts again only for visualization to color rays that hit green and miss red
				Gizmos.color = TryGetHit(out var hit, point, RayLength) ? Color.green : Color.red;
				Gizmos.DrawRay(point + offset, Vector3.down * RayLength);
			}
			Gizmos.color = color;
		}
		
		private void Reset()
		{
			transform.hideFlags = HideFlags.NotEditable;
		}

		private void OnTriggerEnter(Collider other)
		{
			// Ignore collisions with the Terrain
			if(other.gameObject.layer == BuildModeService.TerrainLayerId) return;
			_collisionCounter++;
		}

		private void OnTriggerExit(Collider other)
		{
			// Ignore collisions with the Terrain
			if(other.gameObject.layer == BuildModeService.TerrainLayerId) return;
			_collisionCounter--;
		}

		private Vector3[] GetBaseBoundPoints(Collider meshCollider)
		{
			// clear the list first
			_tempPointsList.Clear();

			var colliderBounds = meshCollider.bounds;
			// Calculate the amount of rays in X and Z direction, round up when there is a fraction and add 1 so that
			// we always cast at least 2 rays
			var raysInXDirection = Mathf.CeilToInt(colliderBounds.size.x * RayCastResolution) + 1;
			var raysInZDirection = Mathf.CeilToInt(colliderBounds.size.z * RayCastResolution) + 1;

			// Calculate the distance between the rays in X and Z direction, subtract 1 because we want to divide into (n-1) segments
			var xDirectionStepSize = colliderBounds.size.x / (raysInXDirection - 1);
			var zDirectionStepSize = colliderBounds.size.z / (raysInZDirection - 1);

			// Calculate a grid of points
			for (var x = 0; x < raysInXDirection; x++)
			{
				for (var z = 0; z < raysInZDirection; z++)
				{
					var offset = new Vector3(x * xDirectionStepSize, 0, z * zDirectionStepSize);
					_tempPointsList.Add(colliderBounds.min + offset);
				}
			}
			return _tempPointsList.ToArray();
		}

		private void ActivateOtherCollidersThanSelf(bool activate)
		{
			foreach (var col in _colliders)
			{
				if(col == _footprintCollider) continue;
				col.enabled = activate;
			}
		}

		private static bool TryGetHit(out RaycastHit hit, Vector3 origin, float distance = 1f)
		{
			hit = default;
			var ray = new Ray(origin + Vector3.up * distance * 0.5f, Vector3.down * distance);
			// Use bit-shifting to get the Terrain LayerMask, which has layer index 6
			return Physics.Raycast(ray, out hit, distance, 1 << 6);
		}
	}
}