using UI.Map;
using UnityEngine;

namespace Map.Controller
{
    public class RegionController : MonoBehaviour, ISelectable
    {
        public Texture2D RegionIdMap;
        public Renderer MapRenderer;

        public RegionMenu RegionMenu;

        public GameTimeController gameTimeController;

        private void Start()
        {
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
                if (hit.collider.gameObject != MapRenderer.gameObject) return;
  
                var uv = hit.textureCoord;
                var color = RegionIdMap.GetPixelBilinear(uv.x, uv.y);

                if (RegionManager.RegionColorMapping.TryGetValue(color, out var regionId))
                {
                    MapRenderer.material.SetColor("_HoverColor", color);
                    MapRenderer.material.SetColor("_GlowColor", Color.lawnGreen);
                    MapRenderer.material.SetFloat("_GlowStrength", 1.8f);
                    MapRenderer.material.SetFloat("_Tolerance", 0.01f);

                    RegionMenu.Show(regionId);
                }
            }
        }

        public void Deselect()
        {
            MapRenderer.material.SetColor("_HoverColor", new Color32(0, 0, 0, 0));
            MapRenderer.material.SetColor("_GlowColor", new Color32(0, 0, 0, 0));

            RegionMenu.Hide();
        }
    }
}