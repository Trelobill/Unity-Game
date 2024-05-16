using Ilumisoft.RadarSystem.UI;
using System.Collections.Generic;
using UnityEngine;

namespace Ilumisoft.RadarSystem
{
    [AddComponentMenu("Radar System/Radar")]
    [DefaultExecutionOrder(-10)]
    public class Radar : MonoBehaviour
    {
        readonly Dictionary<LocatableComponent, LocatableIconComponent> locatableIconDictionary = new();

        [SerializeField] RectTransform iconContainer;
        Camera cam;

        [SerializeField, Min(1)] float range = 20;
        public float Range { get => range; set => range = value; }

        private void Awake()
        {
            cam = Camera.main;
        }
        private void OnEnable()
        {
            LocatableManager.OnLocatableAdded += OnLocatableAdded;
            LocatableManager.OnLocatableRemoved += OnLocatableRemoved;
        }

        private void OnDisable()
        {
            LocatableManager.OnLocatableAdded -= OnLocatableAdded;
            LocatableManager.OnLocatableRemoved -= OnLocatableRemoved;
        }
        private void OnLocatableAdded(LocatableComponent locatable)
        {
            // Create the icon for the locatable and add a new entry to the dictionary
            if (locatable != null && !locatableIconDictionary.ContainsKey(locatable))
            {
                var icon = locatable.CreateIcon();

                icon.transform.SetParent(iconContainer.transform, false);

                locatableIconDictionary.Add(locatable, icon);
            }
        }
        private void OnLocatableRemoved(LocatableComponent locatable)
        {
            // Remove the locatable from the dictionary and destroy the icon
            if (locatable != null && locatableIconDictionary.TryGetValue(locatable, out LocatableIconComponent icon))
            {
                locatableIconDictionary.Remove(locatable);

                Destroy(icon.gameObject);
            }
        }

        private void LateUpdate()
        {
            if (cam != null)
            {
                UpdateLocatableIcons();
            }
        }
        private void UpdateLocatableIcons()
        {
            // Run through all locatables in the dictionary
            foreach (var locatable in locatableIconDictionary.Keys)
            {
                // Update the icon position and visibility for the locatable
                if (locatableIconDictionary.TryGetValue(locatable, out var icon))
                {
                    if (TryGetIconLocation(locatable, out var iconLocation))
                    {
                        icon.SetVisible(true);

                        var rectTransform = icon.GetComponent<RectTransform>();

                        rectTransform.anchoredPosition = iconLocation;
                    }
                    else
                    {
                        icon.SetVisible(false);
                    }
                }
            }
        }

        /// <summary>
        /// Computes the location of the icon on the radar. Returns true if the icon is visible, false otherwise
        /// </summary>
        /// <param name="locatable"></param>
        /// <param name="iconLocation"></param>
        /// <returns></returns>
        private bool TryGetIconLocation(LocatableComponent locatable, out Vector2 iconLocation)
        {
            iconLocation = GetDistanceToPlayer(locatable);

            float radarSize = GetRadarUISize();

            var scale = radarSize / Range;

            iconLocation *= scale;
            // Get the forward vector of the player projected on the xz plane
            var playerForwardDirectionXZ = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up);

            // Create a roation from the direction
            var rotation = Quaternion.LookRotation(playerForwardDirectionXZ);

            // Mirror y rotation
            var euler = rotation.eulerAngles;
            euler.y = -euler.y;
            rotation.eulerAngles = euler;

            // Rotate the icon location in 3D space
            var rotatedIconLocation = rotation * new Vector3(iconLocation.x, 0.0f, iconLocation.y);

            // Convert from 3D to 2D
            iconLocation = new Vector2(rotatedIconLocation.x, rotatedIconLocation.z);

            if (iconLocation.sqrMagnitude < radarSize * radarSize || locatable.ClampOnRadar)
            {
                // Make sure it is not shown outside the radar
                iconLocation = Vector2.ClampMagnitude(iconLocation, radarSize);

                return true;
            }

            return false;
        }
        private float GetRadarUISize()
        {
            return iconContainer.rect.width / 2;
        }
        private Vector2 GetDistanceToPlayer(LocatableComponent locatable)
        {
            Vector3 distanceToPlayer = locatable.transform.position - cam.transform.position;

            return new Vector2(distanceToPlayer.x, distanceToPlayer.z);
        }
    }
}