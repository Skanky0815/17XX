using UI.Map;
using UnityEngine;

namespace Map.Controller
{
    public class RegionController : MonoBehaviour, ISelectable
    {
        private static readonly int HoverColor = Shader.PropertyToID("_HoverColor");
        private static readonly int GlowColor = Shader.PropertyToID("_GlowColor");
        private static readonly int GlowStrength = Shader.PropertyToID("_GlowStrength");
        private static readonly int Tolerance = Shader.PropertyToID("_Tolerance");
        
        public Texture2D regionIdMap;
        public Renderer mapRenderer;
        public RegionMenu regionMenu;
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

            if (!Physics.Raycast(ray, out var hit)) return;
            if (hit.collider.gameObject != mapRenderer.gameObject) return;
  
            var uv = hit.textureCoord;
            var color = regionIdMap.GetPixelBilinear(uv.x, uv.y);

            if (!RegionManager.RegionColorMapping.TryGetValue(color, out var regionId)) return;
            
            mapRenderer.material.SetColor(HoverColor, color);
            mapRenderer.material.SetColor(GlowColor, Color.lawnGreen);
            mapRenderer.material.SetFloat(GlowStrength, 1.8f);
            mapRenderer.material.SetFloat(Tolerance, 0.01f);

            regionMenu.Show(regionId);
        }

        public void Deselect()
        {
            mapRenderer.material.SetColor(HoverColor, new Color32(0, 0, 0, 0));
            mapRenderer.material.SetColor(GlowColor, new Color32(0, 0, 0, 0));

            regionMenu.Hide();
        }
    }
}