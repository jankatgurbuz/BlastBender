using Global.View;

namespace Global.Controller
{
    public class LayersController
    {
        private readonly LayersProperties _layersProperties;
        public LayersController(LayersProperties layersProperties)
        {
            _layersProperties = layersProperties;
        }

        public LayersProperties.LayerProperties GetLayerInfo(string itemName)
        {
            return _layersProperties[itemName];
        }
    }
}
