using Meta.XR.MRUtilityKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    internal class CustomPrefabSpawn : AnchorPrefabSpawner
    {
        public override GameObject CustomPrefabSelection(MRUKAnchor anchor, List<GameObject> prefabs)
        {
            GameObject sizeMatchingPrefab = null;
            if (!anchor.VolumeBounds.HasValue)
            {
                throw new InvalidOperationException(
                    "Cannot match a prefab with the closest size to this anchor as the latter has no volume");
            }

            var anchorBaseRatio = MathF.Min(anchor.VolumeBounds.Value.size.x / anchor.VolumeBounds.Value.size.y,
                anchor.VolumeBounds.Value.size.y / anchor.VolumeBounds.Value.size.x);
            var anchorHeightToBase = anchor.VolumeBounds.Value.size.z /
                MathF.Sqrt(anchor.VolumeBounds.Value.size.x * anchor.VolumeBounds.Value.size.y);
            var closestSizeDifference = Mathf.Infinity;
            foreach (var prefab in prefabs)
            {
                var bounds = Utilities.GetPrefabBounds(prefab);
                if (!bounds.HasValue)
                {
                    continue;
                }

                var prefabBaseRatio = MathF.Min(bounds.Value.size.x / bounds.Value.size.z,
                    bounds.Value.size.z / bounds.Value.size.x);
                var prefabHeightToBase = bounds.Value.size.y /
                    MathF.Sqrt(bounds.Value.size.x * bounds.Value.size.z);
                var sizeDifference = Mathf.Abs(anchorBaseRatio - prefabBaseRatio) + Mathf.Abs(anchorHeightToBase - prefabHeightToBase);
                if (sizeDifference >= closestSizeDifference)
                {
                    continue;
                }
                closestSizeDifference = sizeDifference;
                sizeMatchingPrefab = prefab;
            }

            return sizeMatchingPrefab;
        }
    }
}
