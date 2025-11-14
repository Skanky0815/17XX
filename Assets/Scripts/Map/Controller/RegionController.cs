using System.Collections.Generic;
using Assets.Scripts.Map.Objects;
using Ui.Map;
using UnityEngine;

namespace Map.Controller
{
    public class RegionController : MonoBehaviour, ISelectable
    {
        public Texture2D RegionIdMap;
        public Renderer MapRenderer;

        public RegionMenu RegionMenu;

        public GameTimeController gameTimeController;

        private Dictionary<Color32, Region.Id> _regionIDs;

        private void Start()
        {
            _regionIDs = RegionManager.GetIdMap();

            gameTimeController.CurrentTime.OnNewDay += RegionManager.OnNewDay;
        }

        private void OnDestroy()
        { 
            gameTimeController.CurrentTime.OnNewDay -= RegionManager.OnNewDay;
        }

        public void Select(Vector2 position)
        {
            var ray = Camera.main.ScreenPointToRay(position);

            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.collider.gameObject == MapRenderer.gameObject)
                {
                    var uv = hit.textureCoord;
                    var color = RegionIdMap.GetPixelBilinear(uv.x, uv.y);

                    if (_regionIDs.TryGetValue(color, out var regionId))
                    {
                        MapRenderer.material.SetColor("_HoverColor", color);
                        MapRenderer.material.SetColor("_GlowColor", Color.chartreuse);
                        MapRenderer.material.SetFloat("_GlowStrength", 1.8f);
                        MapRenderer.material.SetFloat("_Tolerance", 0.01f);

                        RegionMenu.Show(regionId);
                    }
                }
            }
        }

        public void Deselect()
        {
            MapRenderer.material.SetColor("_HoverColor", new Color32(0, 0, 0, 0));
            MapRenderer.material.SetColor("_GlowColor", new Color(0, 0, 0));

            RegionMenu.Hide();
        }
    }
}